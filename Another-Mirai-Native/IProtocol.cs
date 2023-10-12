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

        public void AddLog(int priority, string type, string msg);

        public int CanSendImage();

        public int CanSendRecord();

        public int DeleteMsg(long msgId);

        public string GetAppDirectory();

        public string GetCookiesV2(string domain);

        public string GetCsrfToken();

        public string GetFriendList(bool reserved);

        public string GetGroupInfo(long groupId, bool notCache);

        public string GetGroupList();

        public string GetGroupMemberInfoV2(long groupId, long qqId, bool isCache);

        public string GetGroupMemberList(long groupId);

        public string GetImage(string file);

        public string GetLoginNick();

        public long GetLoginQQ();

        public string GetRecordV2(string file, string format);

        public string GetStrangerInfo(long qqId, bool notCache);

        public int SendGroupMessage(long groupId, string msg, int msgId = 0);

        public int SendLikeV2(long qqId, int count);

        public int SendPrivateMessage(long qqId, string msg);

        public int SetDiscussLeave(long discussId);

        public int SetFriendAddRequest(long identifying, int requestType, string appendMsg);

        public int SetGroupAddRequestV2(long identifying, int requestType, int responseType, string appendMsg);

        public int SetGroupAdmin(long groupId, long qqId, bool isSet);

        public int SetGroupAnonymous(long groupId, bool isOpen);

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime);

        public int SetGroupBan(long groupId, long qqId, long time);

        public int SetGroupCard(long groupId, long qqId, string newCard);

        public int SetGroupKick(long groupId, long qqId, bool refuses);

        public int SetGroupLeave(long groupId, bool isDisband);

        public int SetGroupSpecialTitle(long groupId, long qqId, string title);

        public int SetGroupWholeBan(long groupId, bool isOpen);
    }
}