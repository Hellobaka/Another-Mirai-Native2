using Another_Mirai_Native;
using Another_Mirai_Native.Model;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    public partial class LagrangeCoreAPI : IProtocol
    {
        public string Name { get; set; } = $"Langrange.Core";
       
        public bool IsConnected { get; set ; }

        public int CanSendImage()
        {
            return 0;
        }

        public int CanSendRecord()
        {
            return 0;
        }

        public bool Connect()
        {
            return true;
        }

        public int DeleteMsg(long msgId)
        {
            return 0;
        }

        public bool Disconnect()
        {
            return true;
        }

        public Dictionary<string, string> GetConnectionConfig()
        {
            return [];
        }

        public string GetCookies(string domain)
        {
            return "";
        }

        public string GetCsrfToken()
        {
            return "";
        }

        public string GetFriendList(bool reserved)
        {
            return "";
        }

        public string GetGroupInfo(long groupId, bool notCache)
        {
            return "";
        }

        public string GetGroupList()
        {
            return "";
        }

        public string GetGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            return "";
        }

        public string GetGroupMemberList(long groupId)
        {
            return "";
        }

        public string GetLoginNick()
        {
            return "";
        }

        public long GetLoginQQ()
        {
            return 0;
        }

        public List<FriendInfo> GetRawFriendList(bool reserved)
        {
            return [];
        }

        public GroupInfo GetRawGroupInfo(long groupId, bool notCache)
        {
            return new();
        }

        public List<GroupInfo> GetRawGroupList()
        {
            return [];
        }

        public GroupMemberInfo GetRawGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            return new();
        }

        public List<GroupMemberInfo> GetRawGroupMemberList(long groupId)
        {
            return [];
        }

        public string GetStrangerInfo(long qqId, bool notCache)
        {
            return "";
        }

        public int SendDiscussMsg(long discussId, string msg)
        {
            return 0;
        }

        public int SendGroupMessage(long groupId, string msg, int msgId = 0)
        {
            return 0;
        }

        public int SendLike(long qqId, int count)
        {
            return 0;
        }

        public int SendPrivateMessage(long qqId, string msg)
        {
            return 0;
        }

        public bool SetConnectionConfig(Dictionary<string, string> config)
        {
            return true;
        }

        public int SetDiscussLeave(long discussId)
        {
            return 0;
        }

        public int SetFriendAddRequest(string identifying, int responseType, string appendMsg)
        {
            return 0;
        }

        public int SetGroupAddRequest(string identifying, int requestType, int responseType, string appendMsg)
        {
            return 0;
        }

        public int SetGroupAdmin(long groupId, long qqId, bool isSet)
        {
            return 0;
        }

        public int SetGroupAnonymous(long groupId, bool isOpen)
        {
            return 0;
        }

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime)
        {
            return 0;
        }

        public int SetGroupBan(long groupId, long qqId, long banTime)
        {
            return 0;
        }

        public int SetGroupCard(long groupId, long qqId, string newCard)
        {
            return 0;
        }

        public int SetGroupKick(long groupId, long qqId, bool refuses)
        {
            return 0;
        }

        public int SetGroupLeave(long groupId, bool isDisband)
        {
            return 0;
        }

        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime)
        {
            return 0;
        }

        public int SetGroupWholeBan(long groupId, bool isOpen)
        {
            return 0;
        }
    }
}
