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

        public int CanSendImage();

        public int CanSendRecord();

        public int DeleteMsg(long msgId);

        public string GetCookies(string domain);

        public string GetCsrfToken();

        public string GetFriendList(bool reserved);

        public string GetGroupInfo(long groupId, bool notCache);

        public string GetGroupList();

        public string GetGroupMemberInfo(long groupId, long qqId, bool isCache);

        public string GetGroupMemberList(long groupId);

        public string GetLoginNick();

        public long GetLoginQQ();

        public string GetStrangerInfo(long qqId, bool notCache);

        public int SendGroupMessage(long groupId, string msg, int msgId = 0);

        public int SendLike(long qqId, int count);

        public int SendPrivateMessage(long qqId, string msg);

        public int SendDiscussMsg(long discussId, string msg);

        public int SetDiscussLeave(long discussId);

        public int SetFriendAddRequest(long identifying, int requestType, string appendMsg);

        public int SetGroupAddRequest(long identifying, int requestType, int responseType, string appendMsg);

        public int SetGroupAdmin(long groupId, long qqId, bool isSet);

        public int SetGroupAnonymous(long groupId, bool isOpen);

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime);

        public int SetGroupBan(long groupId, long qqId, long time);

        public int SetGroupCard(long groupId, long qqId, string newCard);

        public int SetGroupKick(long groupId, long qqId, bool refuses);

        public int SetGroupLeave(long groupId, bool isDisband);

        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime);

        public int SetGroupWholeBan(long groupId, bool isOpen);
    }
}