namespace Another_Mirai_Native.Protocol.OneBot
{
    public partial class OneBotAPI : IProtocol
    {
        public string Name { get; set; } = "OneBot v11";

        public bool IsConnected { get; set; }

        public int CanSendImage()
        {
            throw new NotImplementedException();
        }

        public int CanSendRecord()
        {
            throw new NotImplementedException();
        }

        public bool Connect()
        {
            throw new NotImplementedException();
        }

        public int DeleteMsg(long msgId)
        {
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetConnectionConfig()
        {
            throw new NotImplementedException();
        }

        public string GetCookies(string domain)
        {
            throw new NotImplementedException();
        }

        public string GetCsrfToken()
        {
            throw new NotImplementedException();
        }

        public string GetFriendList(bool reserved)
        {
            throw new NotImplementedException();
        }

        public string GetGroupInfo(long groupId, bool notCache)
        {
            throw new NotImplementedException();
        }

        public string GetGroupList()
        {
            throw new NotImplementedException();
        }

        public string GetGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            throw new NotImplementedException();
        }

        public string GetGroupMemberList(long groupId)
        {
            throw new NotImplementedException();
        }

        public string GetLoginNick()
        {
            throw new NotImplementedException();
        }

        public long GetLoginQQ()
        {
            throw new NotImplementedException();
        }

        public string GetStrangerInfo(long qqId, bool notCache)
        {
            throw new NotImplementedException();
        }

        public int SendDiscussMsg(long discussId, string msg)
        {
            throw new NotImplementedException();
        }

        public int SendGroupMessage(long groupId, string msg, int msgId = 0)
        {
            throw new NotImplementedException();
        }

        public int SendLike(long qqId, int count)
        {
            throw new NotImplementedException();
        }

        public int SendPrivateMessage(long qqId, string msg)
        {
            throw new NotImplementedException();
        }

        public bool SetConnectionConfig(Dictionary<string, string> config)
        {
            throw new NotImplementedException();
        }

        public int SetDiscussLeave(long discussId)
        {
            throw new NotImplementedException();
        }

        public int SetFriendAddRequest(long identifying, int responseType, string appendMsg)
        {
            throw new NotImplementedException();
        }

        public int SetGroupAddRequest(long identifying, int requestType, int responseType, string appendMsg)
        {
            throw new NotImplementedException();
        }

        public int SetGroupAdmin(long groupId, long qqId, bool isSet)
        {
            throw new NotImplementedException();
        }

        public int SetGroupAnonymous(long groupId, bool isOpen)
        {
            throw new NotImplementedException();
        }

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime)
        {
            throw new NotImplementedException();
        }

        public int SetGroupBan(long groupId, long qqId, long banTime)
        {
            throw new NotImplementedException();
        }

        public int SetGroupCard(long groupId, long qqId, string newCard)
        {
            throw new NotImplementedException();
        }

        public int SetGroupKick(long groupId, long qqId, bool refuses)
        {
            throw new NotImplementedException();
        }

        public int SetGroupLeave(long groupId, bool isDisband)
        {
            throw new NotImplementedException();
        }

        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime)
        {
            throw new NotImplementedException();
        }

        public int SetGroupWholeBan(long groupId, bool isOpen)
        {
            throw new NotImplementedException();
        }
    }
}