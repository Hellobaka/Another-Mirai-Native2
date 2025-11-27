using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.Services
{
    /// <summary>
    /// 缓存服务实现，使用线程安全的并发字典管理缓存
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
