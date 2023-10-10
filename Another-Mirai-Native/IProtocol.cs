namespace Another_Mirai_Native
{
    /// <summary>
    /// 协议需实现的接口
    /// </summary>
    public interface IProtocol
    {
        public string Name { get; set; }

        public bool Connect();

        public bool IsConnected { get; set; }

        public bool Disconnect();

        public void AddLog(int authCode, int priority, string type, string msg);

        public int CanSendImage(int authCode);

        public int CanSendRecord(int authCode);

        public int DeleteMsg(int authCode, long msgId);

        public string GetAppDirectory(int authCode);

        public string GetCookiesV2(int authCode, string domain);

        public string GetCsrfToken(int authCode);

        public string GetFriendList(int authCode, bool reserved);

        public string GetGroupInfo(int authCode, long groupId, bool notCache);

        public string GetGroupList(int authCode);

        public string GetGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache);

        public string GetGroupMemberList(int authCode, long groupId);

        public string GetImage(int authCode, string file);

        public string GetLoginNick(int authCode);

        public long GetLoginQQ(int authCode);

        public string GetRecordV2(int authCode, string file, string format);

        public string GetStrangerInfo(int authCode, long qqId, bool notCache);

        public int SendGroupMessage(int authCode, long groupId, string msg, int msgId = 0);

        public int SendLikeV2(int authCode, long qqId, int count);

        public int SendPrivateMessage(int authCode, long qqId, string msg);

        public int SetDiscussLeave(int authCode, long discussId);

        public int SetFriendAddRequest(int authCode, long identifying, int requestType, string appendMsg);

        public int SetGroupAddRequestV2(int authCode, long identifying, int requestType, int responseType, string appendMsg);

        public int SetGroupAdmin(int authCode, long groupId, long qqId, bool isSet);

        public int SetGroupAnonymous(int authCode, long groupId, bool isOpen);

        public int SetGroupAnonymousBan(int authCode, long groupId, string anonymous, long banTime);

        public int SetGroupBan(int authCode, long groupId, long qqId, long time);

        public int SetGroupCard(int authCode, long groupId, long qqId, string newCard);

        public int SetGroupKick(int authCode, long groupId, long qqId, bool refuses);

        public int SetGroupLeave(int authCode, long groupId, bool isDisband);

        public int SetGroupSpecialTitle(int authCode, long groupId, long qqId, string title);

        public int SetGroupWholeBan(int authCode, long groupId, bool isOpen);
    }
}