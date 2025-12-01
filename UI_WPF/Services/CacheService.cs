using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.Services
{
    /// <summary>
    /// 缓存服务实现，使用线程安全的并发字典管理缓存
    /// 支持批量查询和缓存预热
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly ConcurrentDictionary<long, FriendInfo> _friendInfoCache = new();
        private readonly ConcurrentDictionary<long, GroupInfo> _groupInfoCache = new();
        private readonly ConcurrentDictionary<long, ConcurrentDictionary<long, GroupMemberInfo>> _groupMemberCache = new();
        private readonly SemaphoreSlim _apiLock = new(1, 1);

        /// <summary>
        /// 获取好友昵称
        /// </summary>
        public async Task<string> GetFriendNickAsync(long qq)
        {
            try
            {
                await _apiLock.WaitAsync();

                // 如果是当前用户，直接返回当前昵称
                if (qq == AppConfig.Instance.CurrentQQ)
                {
                    return AppConfig.Instance.CurrentNickName;
                }

                // 尝试从缓存获取
                if (_friendInfoCache.TryGetValue(qq, out var info))
                {
                    return info?.Nick ?? qq.ToString();
                }

                // 缓存中不存在，调用API获取
                string result = qq.ToString();
                await Task.Run(() =>
                {
                    var friendList = ProtocolManager.Instance.CurrentProtocol.GetRawFriendList(false);
                    foreach (var friend in friendList)
                    {
                        _friendInfoCache.AddOrUpdate(friend.QQ, friend, (key, oldValue) => friend);
                        if (friend.QQ == qq)
                        {
                            result = friend.Nick;
                        }
                    }
                });
                return result;
            }
            catch
            {
                return qq.ToString();
            }
            finally
            {
                _apiLock.Release();
            }
        }

        /// <summary>
        /// 获取群名称
        /// </summary>
        public async Task<string> GetGroupNameAsync(long groupId)
        {
            try
            {
                await _apiLock.WaitAsync();

                // 尝试从缓存获取
                if (_groupInfoCache.TryGetValue(groupId, out var info))
                {
                    return info?.Name ?? groupId.ToString();
                }

                // 缓存中不存在，调用API获取
                string result = groupId.ToString();
                await Task.Run(() =>
                {
                    var groupInfo = ProtocolManager.Instance.CurrentProtocol.GetRawGroupInfo(groupId, false);
                    _groupInfoCache.AddOrUpdate(groupId, groupInfo, (key, oldValue) => groupInfo);
                    result = groupInfo?.Name ?? groupId.ToString();
                });
                return result;
            }
            catch
            {
                return groupId.ToString();
            }
            finally
            {
                _apiLock.Release();
            }
        }

        /// <summary>
        /// 获取群成员昵称（名片优先）
        /// </summary>
        public async Task<string> GetGroupMemberNickAsync(long groupId, long qq)
        {
            try
            {
                await _apiLock.WaitAsync();

                // 如果是当前用户，直接返回当前昵称
                if (qq == AppConfig.Instance.CurrentQQ)
                {
                    return AppConfig.Instance.CurrentNickName;
                }

                // 尝试从缓存获取
                if (_groupMemberCache.TryGetValue(groupId, out var memberDict) 
                    && memberDict.TryGetValue(qq, out var memberInfo))
                {
                    if (memberInfo == null)
                    {
                        return qq.ToString();
                    }
                    return string.IsNullOrEmpty(memberInfo.Card) ? memberInfo.Nick : memberInfo.Card;
                }

                // 缓存中不存在，确保群成员字典存在
                if (!_groupMemberCache.ContainsKey(groupId))
                {
                    _groupMemberCache.TryAdd(groupId, new ConcurrentDictionary<long, GroupMemberInfo>());
                }

                // 调用API获取
                string result = qq.ToString();
                await Task.Run(() =>
                {
                    var memberInfo = ProtocolManager.Instance.CurrentProtocol.GetRawGroupMemberInfo(groupId, qq, false);
                    _groupMemberCache[groupId].AddOrUpdate(qq, memberInfo, (key, oldValue) => memberInfo);
                    
                    if (memberInfo != null)
                    {
                        result = string.IsNullOrEmpty(memberInfo.Card) ? memberInfo.Nick : memberInfo.Card;
                    }
                });
                return result;
            }
            catch
            {
                return qq.ToString();
            }
            finally
            {
                _apiLock.Release();
            }
        }

        /// <summary>
        /// 批量获取好友昵称
        /// </summary>
        public async Task<Dictionary<long, string>> GetFriendNicksBatchAsync(IEnumerable<long> qqList)
        {
            var result = new Dictionary<long, string>();
            var uncachedQQs = new List<long>();

            // 先从缓存获取
            foreach (var qq in qqList)
            {
                if (qq == AppConfig.Instance.CurrentQQ)
                {
                    result[qq] = AppConfig.Instance.CurrentNickName;
                }
                else if (_friendInfoCache.TryGetValue(qq, out var info))
                {
                    result[qq] = info?.Nick ?? qq.ToString();
                }
                else
                {
                    uncachedQQs.Add(qq);
                }
            }

            // 如果有未缓存的，批量获取
            if (uncachedQQs.Count > 0)
            {
                try
                {
                    await _apiLock.WaitAsync();
                    await Task.Run(() =>
                    {
                        var friendList = ProtocolManager.Instance.CurrentProtocol.GetRawFriendList(false);
                        foreach (var friend in friendList)
                        {
                            _friendInfoCache.AddOrUpdate(friend.QQ, friend, (key, oldValue) => friend);
                        }
                    });

                    // 从更新后的缓存获取结果
                    foreach (var qq in uncachedQQs)
                    {
                        if (_friendInfoCache.TryGetValue(qq, out var info))
                        {
                            result[qq] = info?.Nick ?? qq.ToString();
                        }
                        else
                        {
                            result[qq] = qq.ToString();
                        }
                    }
                }
                catch
                {
                    // 失败时使用QQ号作为昵称
                    foreach (var qq in uncachedQQs)
                    {
                        result[qq] = qq.ToString();
                    }
                }
                finally
                {
                    _apiLock.Release();
                }
            }

            return result;
        }

        /// <summary>
        /// 批量获取群名称
        /// </summary>
        public async Task<Dictionary<long, string>> GetGroupNamesBatchAsync(IEnumerable<long> groupIds)
        {
            var result = new Dictionary<long, string>();
            var uncachedGroups = new List<long>();

            // 先从缓存获取
            foreach (var groupId in groupIds)
            {
                if (_groupInfoCache.TryGetValue(groupId, out var info))
                {
                    result[groupId] = info?.Name ?? groupId.ToString();
                }
                else
                {
                    uncachedGroups.Add(groupId);
                }
            }

            // 如果有未缓存的，使用单个锁批量获取
            if (uncachedGroups.Count > 0)
            {
                try
                {
                    await _apiLock.WaitAsync();
                    foreach (var groupId in uncachedGroups)
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                var groupInfo = ProtocolManager.Instance.CurrentProtocol.GetRawGroupInfo(groupId, false);
                                _groupInfoCache.AddOrUpdate(groupId, groupInfo, (key, oldValue) => groupInfo);
                                result[groupId] = groupInfo?.Name ?? groupId.ToString();
                            });
                        }
                        catch
                        {
                            result[groupId] = groupId.ToString();
                        }
                    }
                }
                finally
                {
                    _apiLock.Release();
                }
            }

            return result;
        }

        /// <summary>
        /// 批量获取群成员昵称
        /// </summary>
        public async Task<Dictionary<long, string>> GetGroupMemberNicksBatchAsync(long groupId, IEnumerable<long> qqList)
        {
            var result = new Dictionary<long, string>();
            var uncachedQQs = new List<long>();

            // 确保群成员字典存在
            _groupMemberCache.TryAdd(groupId, new ConcurrentDictionary<long, GroupMemberInfo>());

            // 先从缓存获取
            foreach (var qq in qqList)
            {
                if (qq == AppConfig.Instance.CurrentQQ)
                {
                    result[qq] = AppConfig.Instance.CurrentNickName;
                }
                else if (_groupMemberCache[groupId].TryGetValue(qq, out var info) && info != null)
                {
                    result[qq] = string.IsNullOrEmpty(info.Card) ? info.Nick : info.Card;
                }
                else
                {
                    uncachedQQs.Add(qq);
                }
            }

            // 如果未缓存的数量超过阈值，预加载整个群成员列表；否则逐个获取
            const int PreloadThreshold = 5;
            if (uncachedQQs.Count > PreloadThreshold)
            {
                // 预加载整个群成员列表
                await PreloadGroupMemberCacheAsync(groupId);

                // 从更新后的缓存获取结果
                foreach (var qq in uncachedQQs)
                {
                    if (_groupMemberCache[groupId].TryGetValue(qq, out var info) && info != null)
                    {
                        result[qq] = string.IsNullOrEmpty(info.Card) ? info.Nick : info.Card;
                    }
                    else
                    {
                        result[qq] = qq.ToString();
                    }
                }
            }
            else if (uncachedQQs.Count > 0)
            {
                // 逐个获取单个成员信息
                try
                {
                    await _apiLock.WaitAsync();
                    foreach (var qq in uncachedQQs)
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                var memberInfo = ProtocolManager.Instance.CurrentProtocol.GetRawGroupMemberInfo(groupId, qq, false);
                                _groupMemberCache[groupId].AddOrUpdate(qq, memberInfo, (key, oldValue) => memberInfo);
                                if (memberInfo != null)
                                {
                                    result[qq] = string.IsNullOrEmpty(memberInfo.Card) ? memberInfo.Nick : memberInfo.Card;
                                }
                                else
                                {
                                    result[qq] = qq.ToString();
                                }
                            });
                        }
                        catch
                        {
                            result[qq] = qq.ToString();
                        }
                    }
                }
                finally
                {
                    _apiLock.Release();
                }
            }

            return result;
        }

        /// <summary>
        /// 预热好友列表缓存
        /// </summary>
        public async Task PreloadFriendCacheAsync()
        {
            try
            {
                await _apiLock.WaitAsync();
                await Task.Run(() =>
                {
                    var friendList = ProtocolManager.Instance.CurrentProtocol.GetRawFriendList(false);
                    foreach (var friend in friendList)
                    {
                        _friendInfoCache.AddOrUpdate(friend.QQ, friend, (key, oldValue) => friend);
                    }
                });
            }
            catch
            {
                // 忽略预热失败
            }
            finally
            {
                _apiLock.Release();
            }
        }

        /// <summary>
        /// 预热群列表缓存
        /// </summary>
        public async Task PreloadGroupCacheAsync()
        {
            try
            {
                await _apiLock.WaitAsync();
                await Task.Run(() =>
                {
                    var groupList = ProtocolManager.Instance.CurrentProtocol.GetRawGroupList();
                    foreach (var group in groupList)
                    {
                        var groupInfo = new GroupInfo { Group = group.Group, Name = group.Name };
                        _groupInfoCache.AddOrUpdate(group.Group, groupInfo, (key, oldValue) => groupInfo);
                    }
                });
            }
            catch
            {
                // 忽略预热失败
            }
            finally
            {
                _apiLock.Release();
            }
        }

        /// <summary>
        /// 预热指定群的成员缓存
        /// </summary>
        public async Task PreloadGroupMemberCacheAsync(long groupId)
        {
            try
            {
                await _apiLock.WaitAsync();

                // 确保群成员字典存在
                _groupMemberCache.TryAdd(groupId, new ConcurrentDictionary<long, GroupMemberInfo>());

                await Task.Run(() =>
                {
                    var memberList = ProtocolManager.Instance.CurrentProtocol.GetRawGroupMemberList(groupId);
                    if (memberList != null)
                    {
                        foreach (var member in memberList)
                        {
                            if (member != null)
                            {
                                _groupMemberCache[groupId].AddOrUpdate(member.QQ, member, (key, oldValue) => member);
                            }
                        }
                    }
                });
            }
            catch
            {
                // 忽略预热失败
            }
            finally
            {
                _apiLock.Release();
            }
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void ClearCache()
        {
            _friendInfoCache.Clear();
            _groupInfoCache.Clear();
            _groupMemberCache.Clear();
        }

        /// <summary>
        /// 清空好友缓存
        /// </summary>
        public void ClearFriendCache()
        {
            _friendInfoCache.Clear();
        }

        /// <summary>
        /// 清空群缓存
        /// </summary>
        public void ClearGroupCache()
        {
            _groupInfoCache.Clear();
        }

        /// <summary>
        /// 清空指定群的成员缓存
        /// </summary>
        public void ClearGroupMemberCache(long groupId)
        {
            _groupMemberCache.TryRemove(groupId, out _);
        }
    }
}
