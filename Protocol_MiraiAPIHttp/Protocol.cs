namespace Another_Mirai_Native.Protocol.MiraiAPIHttp
{
    public class Protocol : IProtocol
    {
        public string Name { get; set; } = "MiraiAPIHttp";

        public bool IsConnected { get; set; }

        public void AddLog(int priority, string type, string msg)
        {
            throw new System.NotImplementedException();
        }

        public int CanSendImage()
        {
            throw new System.NotImplementedException();
        }

        public int CanSendRecord()
        {
            throw new System.NotImplementedException();
        }

        public bool Connect()
        {
            return true;
        }

        public int DeleteMsg(long msgId)
        {
            throw new System.NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new System.NotImplementedException();
        }

        public string GetAppDirectory()
        {
            throw new System.NotImplementedException();
        }

        public string GetCookies(string domain)
        {
            throw new System.NotImplementedException();
        }

        public string GetCsrfToken()
        {
            throw new System.NotImplementedException();
        }

        public string GetFriendList(bool reserved)
        {
            throw new System.NotImplementedException();
        }

        public string GetGroupInfo(long groupId, bool notCache)
        {
            throw new System.NotImplementedException();
        }

        public string GetGroupList()
        {
            throw new System.NotImplementedException();
        }

        public string GetGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            throw new System.NotImplementedException();
        }

        public string GetGroupMemberList(long groupId)
        {
            throw new System.NotImplementedException();
        }

        public string GetImage(string file)
        {
            throw new System.NotImplementedException();
        }

        public string GetLoginNick()
        {
            throw new System.NotImplementedException();
        }

        public long GetLoginQQ()
        {
            throw new System.NotImplementedException();
        }

        public string GetRecord(string file, string format)
        {
            throw new System.NotImplementedException();
        }

        public string GetStrangerInfo(long qqId, bool notCache)
        {
            throw new System.NotImplementedException();
        }

        public int SendDiscussMsg(long discussId, string msg)
        {
            throw new System.NotImplementedException();
        }

        public int SendGroupMessage(long groupId, string msg, int msgId = 0)
        {
            throw new System.NotImplementedException();
        }

        public int SendLike(long qqId, int count)
        {
            throw new System.NotImplementedException();
        }

        public int SendPrivateMessage(long qqId, string msg)
        {
            throw new System.NotImplementedException();
        }

        public int SetDiscussLeave(long discussId)
        {
            throw new System.NotImplementedException();
        }

        public int SetFriendAddRequest(long identifying, int requestType, string appendMsg)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupAddRequest(long identifying, int requestType, int responseType, string appendMsg)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupAdmin(long groupId, long qqId, bool isSet)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupAnonymous(long groupId, bool isOpen)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupBan(long groupId, long qqId, long time)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupCard(long groupId, long qqId, string newCard)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupKick(long groupId, long qqId, bool refuses)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupLeave(long groupId, bool isDisband)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupWholeBan(long groupId, bool isOpen)
        {
            throw new System.NotImplementedException();
        }
    }
}