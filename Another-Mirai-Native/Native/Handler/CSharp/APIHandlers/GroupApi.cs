using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.RPC;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Native.Handler.CSharp.APIHandlers
{
    public class GroupApi(PluginInfo pluginInfo) : IGroupApi
    {
        private PluginInfo PluginInfo { get; set; } = pluginInfo;

        private int AuthCode => PluginInfo.AuthCode;

        public bool BanGroup(long groupId, bool enable)
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_setGroupWholeBan", true, AuthCode, groupId, enable);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"BanGroup 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> BanGroupAsync(long groupId, bool enable)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_setGroupWholeBan", true, AuthCode, groupId, enable);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"BanGroupAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public bool BanMember(long groupId, long qq, long duration)
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_setGroupBan", true, AuthCode, groupId, qq, duration);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"BanMember 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> BanMemberAsync(long groupId, long qq, long duration)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_setGroupBan", true, AuthCode, groupId, qq, duration);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"BanMemberAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public GroupInfo? GetGroupInfo(long groupId)
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_getGroupInfo", true, AuthCode, groupId, false);
            if (ret is string r)
            {
                var groupInfo = Model.GroupInfo.FromNative(r);
                return new GroupInfo(groupInfo.Group, groupInfo.Name, groupInfo.CurrentMemberCount, groupInfo.MaxMemberCount, groupInfo.LastUpdateTime);
            }
            throw new InvalidCastException($"GetGroupInfo 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public async Task<GroupInfo?> GetGroupInfoAsync(long groupId)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_getGroupInfo", true, AuthCode, groupId, false);
            if (ret is string r)
            {
                var groupInfo = Model.GroupInfo.FromNative(r);
                return new GroupInfo(groupInfo.Group, groupInfo.Name, groupInfo.CurrentMemberCount, groupInfo.MaxMemberCount, groupInfo.LastUpdateTime);
            }
            throw new InvalidCastException($"GetGroupInfoAsync 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public List<GroupInfo> GetGroupList()
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_getGroupList", true, AuthCode);
            if (ret is string r)
            {
                return Model.GroupInfo.RawToList(r).Select(x=>new GroupInfo(x.Group, x.Name, x.CurrentMemberCount, x.MaxMemberCount, x.LastUpdateTime)).ToList();
            }
            throw new InvalidCastException($"GetGroupList 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public async Task<List<GroupInfo>> GetGroupListAsync()
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_getGroupList", true, AuthCode);
            if (ret is string r)
            {
                return Model.GroupInfo.RawToList(r).Select(x => new GroupInfo(x.Group, x.Name, x.CurrentMemberCount, x.MaxMemberCount, x.LastUpdateTime)).ToList();
            }
            throw new InvalidCastException($"GetGroupListAsync 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public GroupMemberInfo? GetGroupMemberInfo(long groupId, long qq)
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_getGroupMemberInfoV2", true, AuthCode, groupId, qq, true);
            if (ret is string r)
            {
                var info = Model.GroupMemberInfo.FromNative(r);
                return new GroupMemberInfo(info.Group,
                                           info.QQ,
                                           info.Nick,
                                           info.Card,
                                           info.Sex,
                                           info.Age,
                                           info.Area,
                                           info.JoinGroupDateTime,
                                           info.LastSpeakDateTime,
                                           info.Level,
                                           info.MemberType,
                                           info.IsBadRecord,
                                           info.ExclusiveTitle,
                                           info.ExclusiveTitleExpirationTime,
                                           info.IsAllowEditorCard,
                                           info.LastUpdateTime);
            }
            throw new InvalidCastException($"GetGroupMemberInfo 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public async Task<GroupMemberInfo?> GetGroupMemberInfoAsync(long groupId, long qq)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_getGroupMemberInfoV2", true, AuthCode, groupId, qq, true);
            if (ret is string r)
            {
                var info = Model.GroupMemberInfo.FromNative(r);
                return new GroupMemberInfo(info.Group,
                                           info.QQ,
                                           info.Nick,
                                           info.Card,
                                           info.Sex,
                                           info.Age,
                                           info.Area,
                                           info.JoinGroupDateTime,
                                           info.LastSpeakDateTime,
                                           info.Level,
                                           info.MemberType,
                                           info.IsBadRecord,
                                           info.ExclusiveTitle,
                                           info.ExclusiveTitleExpirationTime,
                                           info.IsAllowEditorCard,
                                           info.LastUpdateTime);
            }
            throw new InvalidCastException($"GetGroupMemberInfoAsync 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public List<GroupMemberInfo> GetGroupMembers(long groupId)
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_getGroupMemberList", true, AuthCode, groupId);
            if (ret is string r)
            {
                return Model.GroupMemberInfo.RawToList(r).Select(info => new GroupMemberInfo(info.Group,
                                           info.QQ,
                                           info.Nick,
                                           info.Card,
                                           info.Sex,
                                           info.Age,
                                           info.Area,
                                           info.JoinGroupDateTime,
                                           info.LastSpeakDateTime,
                                           info.Level,
                                           info.MemberType,
                                           info.IsBadRecord,
                                           info.ExclusiveTitle,
                                           info.ExclusiveTitleExpirationTime,
                                           info.IsAllowEditorCard,
                                           info.LastUpdateTime)).ToList();
            }
            throw new InvalidCastException($"GetGroupMembers 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public async Task<List<GroupMemberInfo>> GetGroupMembersAsync(long groupId)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_getGroupMemberList", true, AuthCode, groupId);
            if (ret is string r)
            {
                return Model.GroupMemberInfo.RawToList(r).Select(info => new GroupMemberInfo(info.Group,
                                           info.QQ,
                                           info.Nick,
                                           info.Card,
                                           info.Sex,
                                           info.Age,
                                           info.Area,
                                           info.JoinGroupDateTime,
                                           info.LastSpeakDateTime,
                                           info.Level,
                                           info.MemberType,
                                           info.IsBadRecord,
                                           info.ExclusiveTitle,
                                           info.ExclusiveTitleExpirationTime,
                                           info.IsAllowEditorCard,
                                           info.LastUpdateTime)).ToList();
            }
            throw new InvalidCastException($"GetGroupMembersAsync 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public bool Kick(long groupId, long qq, bool rejectAddRequest = false)
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_setGroupKick", true, AuthCode, groupId, qq, rejectAddRequest);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"Kick 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> KickAsync(long groupId, long qq, bool rejectAddRequest = false)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_setGroupKick", true, AuthCode, groupId, qq, rejectAddRequest);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"KickAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public bool Leave(long groupId)
        {
            // TODO: 似乎与CQP定义不符，isDisband未转换
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_setGroupLeave", true, AuthCode, groupId, false);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"Leave 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> LeaveAsync(long groupId)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_setGroupLeave", true, AuthCode, groupId, false);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"LeaveAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public bool SetAdmin(long groupId, long qq, bool isAdmin)
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_setGroupAdmin", true, AuthCode, groupId, qq, isAdmin);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetAdmin 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> SetAdminAsync(long groupId, long qq, bool isAdmin)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_setGroupAdmin", true, AuthCode, groupId, qq, isAdmin);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetAdminAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public bool SetMemberCard(long groupId, long qq, string card)
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_setGroupCard", true, AuthCode, groupId, qq, card);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetMemberCard 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> SetMemberCardAsync(long groupId, long qq, string card)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_setGroupCard", true, AuthCode, groupId, qq, card);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetMemberCardAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public bool SetMemberTitle(long groupId, long qq, string title)
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_setGroupSpecialTitle", true, AuthCode, groupId, qq, title, -1);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetMemberTitle 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> SetMemberTitleAsync(long groupId, long qq, string title)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_setGroupSpecialTitle", true, AuthCode, groupId, qq, title, -1);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetMemberTitleAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public bool SetGroupAddRequest(string flag, bool accept, string refuseReason = "")
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_setGroupAddRequestV2", true, AuthCode, flag, 1, accept ? 1 : 2, refuseReason);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetGroupAddRequest 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> SetGroupAddRequestAsync(string flag, bool accept, string refuseReason = "")
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_setGroupAddRequestV2", true, AuthCode, flag, 1, accept ? 1 : 2, refuseReason);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetGroupAddRequestAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public bool SetGroupInviteRequest(string flag, bool accept, string refuseReason = "")
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_setGroupAddRequestV2", true, AuthCode, flag, 2, accept ? 1 : 2, refuseReason);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetGroupInviteRequest 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> SetGroupInviteRequestAsync(string flag, bool accept, string refuseReason = "")
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_setGroupAddRequestV2", true, AuthCode, flag, 2, accept ? 1 : 2, refuseReason);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetGroupInviteRequestAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }
    }
}
