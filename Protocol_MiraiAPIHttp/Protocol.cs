namespace Another_Mirai_Native.Protocol.MiraiAPIHttp
{
    public class Protocol : IProtocol
    {
        public string Name { get; set; } = "MiraiAPIHttp";

        public bool IsConnected { get; set; }

        public void AddLog(int authCode, int priority, string type, string msg)
        {
            throw new System.NotImplementedException();
        }

        public int CanSendImage(int authCode)
        {
            throw new System.NotImplementedException();
        }

        public int CanSendRecord(int authCode)
        {
            throw new System.NotImplementedException();
        }

        public bool Connect()
        {
            return true;
        }

        public int DeleteMsg(int authCode, long msgId)
        {
            throw new System.NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new System.NotImplementedException();
        }

        public string GetAppDirectory(int authCode)
        {
            throw new System.NotImplementedException();
        }

        public string GetCookiesV2(int authCode, string domain)
        {
            throw new System.NotImplementedException();
        }

        public string GetCsrfToken(int authCode)
        {
            throw new System.NotImplementedException();
        }

        public string GetFriendList(int authCode, bool reserved)
        {
            throw new System.NotImplementedException();
        }

        public string GetGroupInfo(int authCode, long groupId, bool notCache)
        {
            throw new System.NotImplementedException();
        }

        public string GetGroupList(int authCode)
        {
            throw new System.NotImplementedException();
        }

        public string GetGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache)
        {
            throw new System.NotImplementedException();
        }

        public string GetGroupMemberList(int authCode, long groupId)
        {
            throw new System.NotImplementedException();
        }

        public string GetImage(int authCode, string file)
        {
            throw new System.NotImplementedException();
        }

        public string GetLoginNick(int authCode)
        {
            throw new System.NotImplementedException();
        }

        public long GetLoginQQ(int authCode)
        {
            throw new System.NotImplementedException();
        }

        public string GetRecordV2(int authCode, string file, string format)
        {
            throw new System.NotImplementedException();
        }

        public string GetStrangerInfo(int authCode, long qqId, bool notCache)
        {
            throw new System.NotImplementedException();
        }

        public int SendGroupMessage(int authCode, long groupId, string msg, int msgId = 0)
        {
            throw new System.NotImplementedException();
        }

        public int SendLikeV2(int authCode, long qqId, int count)
        {
            throw new System.NotImplementedException();
        }

        public int SendPrivateMessage(int authCode, long qqId, string msg)
        {
            throw new System.NotImplementedException();
        }

        public int SetDiscussLeave(int authCode, long discussId)
        {
            throw new System.NotImplementedException();
        }

        public int SetFriendAddRequest(int authCode, long identifying, int requestType, string appendMsg)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupAddRequestV2(int authCode, long identifying, int requestType, int responseType, string appendMsg)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupAnonymousBan(int authCode, long groupId, string anonymous, long banTime)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupBan(int authCode, long groupId, long qqId, long time)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupCard(int authCode, long groupId, long qqId, string newCard)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupKick(int authCode, long groupId, long qqId, bool refuses)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupLeave(int authCode, long groupId, bool isDisband)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupSpecialTitle(int authCode, long groupId, long qqId, string title)
        {
            throw new System.NotImplementedException();
        }

        public int SetGroupWholeBan(int authCode, long groupId, bool isOpen)
        {
            throw new System.NotImplementedException();
        }
    }
}