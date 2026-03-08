using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Model;
using System.Collections.Generic;

namespace Another_Mirai_Native.Abstractions.Services
{
    /// <summary>
    /// 提供用于管理群组操作的抽象，包括成员管理、群组信息检索以及在群组上下文中的管理操作。
    /// </summary>
    public interface IGroupApi
    {
        /// <summary>
        /// 全群禁言/解除全群禁言；要求当前账号必须是群主或管理员，且不能对群主或自己进行此操作
        /// </summary>
        /// <param name="groupId">操作的群号</param>
        /// <param name="enable"><see langword="true"/> 表示开启全群禁言；<see langword="false"/> 表示解除全群禁言</param>
        /// <returns>操作是否成功</returns>
        bool BanGroup(long groupId, bool enable);

        /// <summary>
        /// 禁言/解除禁言群成员；要求当前账号必须是群主或管理员，且不能对群主或自己进行此操作
        /// </summary>
        /// <param name="groupId">操作的群号</param>
        /// <param name="qq">操作的目标群成员</param>
        /// <param name="duration">禁言时长（秒）；当值为 <see langword="0"/> 时，表示解除此人的禁言</param>
        /// <returns>操作是否成功</returns>
        bool BanMember(long groupId, long qq, long duration);

        /// <summary>
        /// 通过群号获取群信息
        /// </summary>
        /// <param name="groupId">期望获取群消息的群号</param>
        /// <returns>获取成功则返回 <see cref="GroupInfo"/> 对象，否则会返回 <see langword="null"/></returns>
        GroupInfo? GetGroupInfo(long groupId);

        /// <summary>
        /// 获取当前登录账号已加入的群列表
        /// </summary>
        /// <returns>获取成功则返回 <see cref="GroupMemberInfo"/> 数组</returns>
        List<GroupInfo> GetGroupList();

        /// <summary>
        /// 获取群成员信息
        /// </summary>
        /// <param name="groupId">目标成员所在群</param>
        /// <param name="qq">目标成员QQ</param>
        /// <returns>获取成功则返回 <see cref="GroupMemberInfo"/> 信息对象，否则会返回 <see langword="null"/></returns>
        GroupMemberInfo? GetGroupMemberInfo(long groupId, long qq);

        /// <summary>
        /// 获取某个群的成员列表，成员较多时会耗时较长
        /// </summary>
        /// <param name="groupId">查询群的群号</param>
        /// <returns>获取成功则返回 <see cref="GroupMemberInfo"/> 数组</returns>
        List<GroupMemberInfo> GetGroupMembers(long groupId);

        /// <summary>
        /// 将群成员从某个群移除；要求当前账号必须是群主或管理员，且不能对群主或自己进行此操作
        /// </summary>
        /// <param name="groupId">操作的群号</param>
        /// <param name="qq">操作的群成员QQ</param>
        /// <param name="rejectAddRequest">拒绝后续入群</param>
        /// <returns>操作是否成功</returns>
        bool Kick(long groupId, long qq, bool rejectAddRequest = false);

        /// <summary>
        /// 使当前账号退出某个群；当账号为群主时，此操作将解散群；
        /// </summary>
        /// <param name="groupId">将要离开的群号</param>
        /// <returns>操作是否成功</returns>
        bool Leave(long groupId);

        /// <summary>
        /// 将群成员设置为管理员或取消管理员；要求当前账号必须是群主，且不能对群主或自己进行此操作
        /// </summary>
        /// <param name="groupId">操作的群号</param>
        /// <param name="qq">操作的群成员QQ</param>
        /// <param name="isAdmin"><see langword="true"/> 表示设置为管理员；<see langword="false"/> 表示解除管理员</param>
        /// <returns>操作是否成功</returns>
        bool SetAdmin(long groupId, long qq, bool isAdmin);

        /// <summary>
        /// 设置某个群成员显示的名片；要求当前账号必须是群主或管理员
        /// </summary>
        /// <param name="groupId">操作的群号</param>
        /// <param name="qq">操作的群成员QQ</param>
        /// <param name="card">将要设置的名片，不可为空</param>
        /// <returns>操作是否成功</returns>
        bool SetMemberCard(long groupId, long qq, string card);

        /// <summary>
        /// 设置某个群成员显示的头衔；要求当前账号必须是群主
        /// </summary>
        /// <param name="groupId">操作的群号</param>
        /// <param name="qq">操作的群成员QQ</param>
        /// <param name="title">将要设置的头衔，不可为空</param>
        /// <returns>操作是否成功</returns>
        bool SetMemberTitle(long groupId, long qq, string title);
    }
}
