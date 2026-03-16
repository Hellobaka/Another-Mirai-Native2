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
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_setGroupWholeBan", true, AuthCode, groupId, enable);
            if (ret is int r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"BanGroup 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public Task<bool> BanGroupAsync(long groupId, bool enable)
        {
            return Task.FromResult(BanGroup(groupId, enable));
        }

        public bool BanMember(long groupId, long qq, long duration)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_setGroupBan", true, AuthCode, groupId, qq, duration);
            if (ret is int r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"BanMember 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public Task<bool> BanMemberAsync(long groupId, long qq, long duration)
        {
            return Task.FromResult(BanMember(groupId, qq, duration));
        }

        public GroupInfo? GetGroupInfo(long groupId)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_getGroupInfo", true, AuthCode, groupId);
            if (ret is string r)
            {
                var groupInfo = Model.GroupInfo.FromNative(r);
                return new GroupInfo(groupInfo.Group, groupInfo.Name, groupInfo.CurrentMemberCount, groupInfo.MaxMemberCount, groupInfo.LastUpdateTime);
            }
            throw new InvalidCastException($"GetGroupInfo 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public Task<GroupInfo?> GetGroupInfoAsync(long groupId)
        {
            return Task.FromResult(GetGroupInfo(groupId));
        }

        public List<GroupInfo> GetGroupList()
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_getGroupList", true, AuthCode);
            if (ret is string r)
            {
                return Model.GroupInfo.RawToList(r).Select(x=>new GroupInfo(x.Group, x.Name, x.CurrentMemberCount, x.MaxMemberCount, x.LastUpdateTime)).ToList();
            }
            throw new InvalidCastException($"GetGroupList 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public Task<List<GroupInfo>> GetGroupListAsync()
        {
            return Task.FromResult(GetGroupList());
        }

        public GroupMemberInfo? GetGroupMemberInfo(long groupId, long qq)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_getGroupMemberInfoV2", true, AuthCode, groupId, qq);
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

        public Task<GroupMemberInfo?> GetGroupMemberInfoAsync(long groupId, long qq)
        {
            return Task.FromResult(GetGroupMemberInfo(groupId, qq));
        }

        public List<GroupMemberInfo> GetGroupMembers(long groupId)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_getGroupMemberList", true, AuthCode, groupId);
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

        public Task<List<GroupMemberInfo>> GetGroupMembersAsync(long groupId)
        {
            return Task.FromResult(GetGroupMembers(groupId));
        }

        public bool Kick(long groupId, long qq, bool rejectAddRequest = false)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_setGroupKick", true, AuthCode, groupId, qq, rejectAddRequest);
            if (ret is int r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"Kick 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public Task<bool> KickAsync(long groupId, long qq, bool rejectAddRequest = false)
        {
            return Task.FromResult(Kick(groupId, qq, rejectAddRequest));
        }

        public bool Leave(long groupId)
        {
            // TODO: 似乎与CQP定义不符，isDisband未转换
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_setGroupLeave", true, AuthCode, groupId, false);
            if (ret is int r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"Leave 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public Task<bool> LeaveAsync(long groupId)
        {
            return Task.FromResult(Leave(groupId));
        }

        public bool SetAdmin(long groupId, long qq, bool isAdmin)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_setGroupAdmin", true, AuthCode, groupId, qq, isAdmin);
            if (ret is int r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetAdmin 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public Task<bool> SetAdminAsync(long groupId, long qq, bool isAdmin)
        {
            return Task.FromResult(SetAdmin(groupId, qq, isAdmin));
        }

        public bool SetMemberCard(long groupId, long qq, string card)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_setGroupCard", true, AuthCode, groupId, qq, card);
            if (ret is int r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetMemberCard 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public Task<bool> SetMemberCardAsync(long groupId, long qq, string card)
        {
            return Task.FromResult(SetMemberCard(groupId, qq, card));
        }

        public bool SetMemberTitle(long groupId, long qq, string title)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_setGroupSpecialTitle", true, AuthCode, groupId, qq, title, -1);
            if (ret is int r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetMemberTitle 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public Task<bool> SetMemberTitleAsync(long groupId, long qq, string title)
        {
            return Task.FromResult(SetMemberTitle(groupId, qq, title));
        }
    }
}
