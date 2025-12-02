using Another_Mirai_Native.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.Models
{
    public static class Caches
    {
        /// <summary>
        /// 好友信息列表缓存
        /// </summary>
        public static ConcurrentDictionary<long, FriendInfo> FriendInfoCache { get; set; } = [];

        /// <summary>
        /// 群信息列表缓存
        /// </summary>
        public static ConcurrentDictionary<long, GroupInfo> GroupInfoCache { get; set; } = [];

        /// <summary>
        /// 群成员信息列表缓存
        /// </summary>
        public static ConcurrentDictionary<long, Dictionary<long, GroupMemberInfo>> GroupMemberCache { get; set; } = [];

        public static void ClearAllCaches()
        {
            FriendInfoCache.Clear();
            GroupInfoCache.Clear();
            GroupMemberCache.Clear();
        }

        public static void LoadFriendCaches()
        {
            var rawList = ProtocolManager.Instance.CurrentProtocol.GetRawFriendList(false);
            foreach(var item in rawList)
            {
                FriendInfoCache[item.QQ] = item;
            }
        }

        public static void LoadGroupInfoCaches(long groupId)
        {
            var rawGroupInfo = ProtocolManager.Instance.CurrentProtocol.GetRawGroupInfo(groupId, false);
            GroupInfoCache[groupId] = rawGroupInfo;
        }

        public static void LoadGroupMemberCaches(long groupId)
        {
            var rawList = ProtocolManager.Instance.CurrentProtocol.GetRawGroupMemberList(groupId);
            GroupMemberCache[groupId] = [];
            foreach(var item in rawList)
            {
                if (item == null)
                {
                    continue;
                }
                GroupMemberCache[groupId][item.QQ] = item;
            }
        }

        /// <summary>
        /// 从 <see cref="FriendInfoCache"/> 中获取好友昵称
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="qq">好友ID</param>
        /// <returns>昵称, 失败时返回QQ号</returns>
        public static async Task<string> GetFriendNick(long qq, bool retry = false)
        {
            if (FriendInfoCache.TryGetValue(qq, out var info))
            {
                if (string.IsNullOrEmpty(info.Postscript))
                {
                    return string.IsNullOrEmpty(info.Nick) ? qq.ToString() : info.Nick;
                }
                else
                {
                    return info.Postscript;
                }
            }
            else if (!retry)
            {
                LoadFriendCaches();
                return await GetFriendNick(qq, true);
            }
            return qq.ToString();
        }

        /// <summary>
        /// 从 <see cref="GroupMemberCache"/> 中获取群员名片
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="group">群来源</param>
        /// <param name="qq">群员QQ</param>
        /// <returns>群员名片, 若不存在则返回昵称, 若调用失败则返回QQ号</returns>
        public static async Task<string> GetGroupMemberNick(long groupId, long qq, bool retry = false)
        {
            if (GroupMemberCache.TryGetValue(groupId, out var member))
            {
                if (member.TryGetValue(qq, out var info))
                {
                    if (string.IsNullOrEmpty(info.Card))
                    {
                        return string.IsNullOrEmpty(info.Nick) ? qq.ToString() : info.Nick;
                    }
                    else
                    {
                        return info.Card;
                    }
                }
                else
                {
                    return qq.ToString();
                }
            }
            else if (!retry)
            {
                LoadGroupMemberCaches(groupId);
                return await GetGroupMemberNick(groupId, qq, true);
            }
            return qq.ToString();
        }

        /// <summary>
        /// 从 <see cref="GroupInfoCache"/> 中获取群名称
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <returns>群名称, 若不存在则返回群号</returns>
        public static async Task<string> GetGroupName(long groupId, bool retry = false)
        {
            if (GroupInfoCache.TryGetValue(groupId, out var info))
            {
                return string.IsNullOrEmpty(info.Name) ? groupId.ToString() : info.Name;
            }
            else if (!retry)
            {
                LoadGroupInfoCaches(groupId);
                return await GetGroupName(groupId, true);
            }
            return groupId.ToString();
        }

        public static void RemoveGroupMember(long group, long qq)
        {
            if (GroupMemberCache.TryGetValue(group, out var member))
            {
                member.Remove(qq);
            }
        }
    }
}
