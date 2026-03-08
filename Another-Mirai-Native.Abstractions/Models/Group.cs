using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.Model;
using System;
using System.Collections.Generic;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 表示群的类，可进行与这个群相关快捷操作
    /// </summary>
    public class Group(IPluginApi pluginApi, long groupId)
    {
        internal IPluginApi PluginApi => pluginApi;

        /// <summary>
        /// 群号
        /// </summary>
        public long Id { get; private set; } = groupId;

        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>若发送成功则返回消息ID（根据不同的框架实现，可能会有负数），若发送失败则返回 0</returns>
        public int SendGroupMessage(string message)
        {
            return PluginApi.MessageApi.SendGroupMessage(Id, message);
        }

        /// <summary>
        /// 获取群信息
        /// </summary>
        /// <returns>获取成功返回 <see cref="GroupInfo"/> 对象，否则会返回 <see langword="null"/></returns>
        public GroupInfo? GetGroupInfo()
        {
            return PluginApi.GroupApi.GetGroupInfo(Id);
        }

        /// <summary>
        /// 获取群成员列表
        /// </summary>
        /// <returns>获取成功返回 <see cref="GroupMemberInfo"/> 数组</returns>
        public List<GroupMemberInfo> GetGroupMemberList()
        {
            return PluginApi.GroupApi.GetGroupMembers(Id);
        }

        /// <summary>
        /// 获取群成员信息
        /// </summary>
        /// <param name="qqId">目标帐号</param>
        /// <returns>获取成功返回 <see cref="GroupMemberInfo"/></returns>
        public GroupMemberInfo? GetGroupMemberInfo(long qqId)
        {
            return PluginApi.GroupApi.GetGroupMemberInfo(Id, qqId);
        }

        /// <summary>
        /// 禁言群成员；要求当前账号必须是群主或管理员，且不能对群主或自己进行此操作
        /// </summary>
        /// <param name="qqId">目标QQ</param>
        /// <param name="time">禁言时长 (范围: 1秒 ~ 30天)</param>
        /// <returns>操作成功返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
        public bool SetMemberBan(long qqId, TimeSpan time)
        {
            return PluginApi.GroupApi.BanMember(Id, qqId, (long)time.TotalSeconds);
        }

        /// <summary>
        /// 解除群成员禁言；要求当前账号必须是群主或管理员，且不能对群主或自己进行此操作
        /// </summary>
        /// <param name="qqId">目标QQ</param>
        /// <returns>操作成功返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
        public bool RemoveMemberBan(long qqId)
        {
            return PluginApi.GroupApi.BanMember(Id, qqId, 0);
        }

        /// <summary>
        /// 全群禁言；要求当前账号必须是群主或管理员，且不能对群主或自己进行此操作
        /// </summary>
        /// <returns>操作成功返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
        public bool SetGroupBanSpeak()
        {
            return PluginApi.GroupApi.BanGroup(Id, true);
        }

        /// <summary>
        /// 解除全群禁言；要求当前账号必须是群主或管理员，且不能对群主或自己进行此操作
        /// </summary>
        /// <returns>操作成功返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
        public bool RemoveGroupBanSpeak()
        {
            return PluginApi.GroupApi.BanGroup(Id, false);
        }

        /// <summary>
        /// 设置某个群成员显示的名片；要求当前账号必须是群主或管理员
        /// </summary>
        /// <param name="qqId">目标QQ</param>
        /// <param name="newName">新名称</param>
        /// <returns>操作成功返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
        public bool SetMemberCard(long qqId, string newName)
        {
            return PluginApi.GroupApi.SetMemberCard(Id, qqId, newName);
        }

        /// <summary>
        /// 设置某个群成员显示的头衔；要求当前账号必须是群主
        /// </summary>
        /// <param name="qqId">目标QQ</param>
        /// <param name="newTitle">新头衔</param>
        public bool SetMemberTitle(long qqId, string newTitle)
        {
            return PluginApi.GroupApi.SetMemberTitle(Id, qqId, newTitle);
        }

        /// <summary>
        /// 将群成员设置为管理员；要求当前账号必须是群主，且不能对群主或自己进行此操作
        /// </summary>
        /// <param name="qqId">目标QQ</param>
        /// <returns>操作成功返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
        public bool SetAdmin(long qqId)
        {
            return PluginApi.GroupApi.SetAdmin(Id, qqId, true);
        }

        /// <summary>
        /// 将群成员取消管理员；要求当前账号必须是群主，且不能对群主或自己进行此操作
        /// </summary>
        /// <param name="qqId">目标QQ</param>
        /// <returns>操作成功返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
        public bool RemoveAdmin(long qqId)
        {
            return PluginApi.GroupApi.SetAdmin(Id, qqId, false);
        }

        /// <summary>
        /// 使当前账号退出某个群；当账号为群主时，此操作将解散群；
        /// </summary>
        /// <returns>操作成功返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
        public bool ExitGroup()
        {
            return PluginApi.GroupApi.Leave(Id);
        }
    }
}