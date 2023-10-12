namespace Another_Mirai_Native.Native
{
    public static class CQPImplementation
    {
        public static int CQ_sendPrivateMsg(int authCode, long qqId, string msg)
        {
            return 0;
        }

        public static int CQ_sendGroupMsg(int authCode, long groupId, string msg)
        {
            return 0;
        }

        public static int CQ_sendDiscussMsg(int authCode, long discussId, string msg)
        {
            return 0;
        }

        public static int CQ_deleteMsg(int authCode, long msgId)
        {
            return 0;
        }

        public static int CQ_sendLikeV2(int authCode, long qqId, int count)
        {
            return 0;
        }

        public static string CQ_getCookiesV2(int authCode, string domain)
        {
            return "";
        }

        public static string CQ_getRecordV2(int authCode, string file, string format)
        {
            return "";
        }

        public static int CQ_getCsrfToken(int authCode)
        {
            return 0;
        }

        public static string CQ_getAppDirectory(int authCode)
        {
            return "";
        }

        public static long CQ_getLoginQQ(int authCode)
        {
            return 0;
        }

        public static string CQ_getLoginNick(int authCode)
        {
            return "";
        }

        public static int CQ_setGroupKick(int authCode, long groupId, long qqId, bool refuses)
        {
            return 0;
        }

        public static int CQ_setGroupBan(int authCode, long groupId, long qqId, long time)
        {
            return 0;
        }

        public static int CQ_setGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            return 0;
        }

        public static int CQ_setGroupSpecialTitle(int authCode, long groupId, long qqId, string title, long durationTime)
        {
            return 0;
        }

        public static int CQ_setGroupWholeBan(int authCode, long groupId, bool isOpen)
        {
            return 0;
        }

        public static int CQ_setGroupAnonymousBan(int authCode, long groupId, string anonymous, long banTime)
        {
            return 0;
        }

        public static int CQ_setGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            return 0;
        }

        public static int CQ_setGroupCard(int authCode, long groupId, long qqId, string newCard)
        {
            return 0;
        }

        public static int CQ_setGroupLeave(int authCode, long groupId, bool isDisband)
        {
            return 0;
        }

        public static int CQ_setDiscussLeave(int authCode, long discussId)
        {
            return 0;
        }

        public static int CQ_setFriendAddRequest(int authCode, string identifying, int requestType, string appendMsg)
        {
            return 0;
        }

        public static int CQ_setGroupAddRequestV2(int authCode, string identifying, int requestType, int responseType, string appendMsg)
        {
            return 0;
        }

        public static int CQ_addLog(int authCode, int priority, string type, string msg)
        {
            return 0;
        }

        public static int CQ_setFatal(int authCode, string errorMsg)
        {
            return 0;
        }

        public static string CQ_getGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache)
        {
            return "";
        }

        public static string CQ_getGroupMemberList(int authCode, long groupId)
        {
            return "";
        }

        public static string CQ_getGroupList(int authCode)
        {
            return "";
        }

        public static string CQ_getStrangerInfo(int authCode, long qqId, bool notCache)
        {
            return "";
        }

        public static string CQ_getGroupInfo(int authCode, long groupId, bool notCache)
        {
            return "";
        }

        public static string CQ_getFriendList(int authCode, bool reserved)
        {
            return "";
        }

        public static int CQ_canSendImage(int authCode)
        {
            return 0;
        }

        public static int CQ_canSendRecord(int authCode)
        {
            return 0;
        }

        public static string CQ_getImage(int authCode, string file)
        {
            return "";
        }
    }
}