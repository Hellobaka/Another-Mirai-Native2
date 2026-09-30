// Generated from CQP/DllEntry.cs.
extern "C" int __stdcall CQ_sendGroupMsg(int authCode, int64_t groupId, const char* msg)
{
    return static_cast<int>(result_number(call("CQ_sendGroupMsg", authCode, groupId, msg)));
}
extern "C" int __stdcall CQ_sendGroupQuoteMsg(int authCode, int64_t groupId, int msgId, const char* msg)
{
    return static_cast<int>(result_number(call("CQ_sendGroupQuoteMsg", authCode, groupId, msgId, msg)));
}
extern "C" int __stdcall CQ_sendPrivateMsg(int authCode, int64_t qqId, const char* msg)
{
    return static_cast<int>(result_number(call("CQ_sendPrivateMsg", authCode, qqId, msg)));
}
extern "C" int __stdcall CQ_sendDiscussMsg(int authCode, int64_t discussId, const char* msg)
{
    return static_cast<int>(result_number(call("CQ_sendDiscussMsg", authCode, discussId, msg)));
}
extern "C" int __stdcall CQ_deleteMsg(int authCode, int64_t msgId)
{
    return static_cast<int>(result_number(call("CQ_deleteMsg", authCode, msgId)));
}
extern "C" int __stdcall CQ_sendLikeV2(int authCode, int64_t qqId, int count)
{
    return static_cast<int>(result_number(call("CQ_sendLikeV2", authCode, qqId, count)));
}
extern "C" const char* __stdcall CQ_getCookiesV2(int authCode, const char* domain)
{
    return result_text(call("CQ_getCookiesV2", authCode, domain));
}
extern "C" const char* __stdcall CQ_getRecordV2(int authCode, const char* file, const char* format)
{
    return result_text(call("CQ_getRecordV2", authCode, file, format));
}
extern "C" const char* __stdcall CQ_getCsrfToken(int authCode)
{
    return result_text(call("CQ_getCsrfToken", authCode));
}
extern "C" const char* __stdcall CQ_getAppDirectory(int authCode)
{
    return result_text(call("CQ_getAppDirectory", authCode));
}
extern "C" int64_t __stdcall CQ_getLoginQQ(int authCode)
{
    return static_cast<int64_t>(result_number(call("CQ_getLoginQQ", authCode)));
}
extern "C" const char* __stdcall CQ_getLoginNick(int authCode)
{
    return result_text(call("CQ_getLoginNick", authCode));
}
extern "C" int __stdcall CQ_setGroupKick(int authCode, int64_t groupId, int64_t qqId, bool refuses)
{
    return static_cast<int>(result_number(call("CQ_setGroupKick", authCode, groupId, qqId, refuses)));
}
extern "C" int __stdcall CQ_setGroupBan(int authCode, int64_t groupId, int64_t qqId, int64_t time)
{
    return static_cast<int>(result_number(call("CQ_setGroupBan", authCode, groupId, qqId, time)));
}
extern "C" int __stdcall CQ_setGroupAdmin(int authCode, int64_t groupId, int64_t qqId, bool isSet)
{
    return static_cast<int>(result_number(call("CQ_setGroupAdmin", authCode, groupId, qqId, isSet)));
}
extern "C" int __stdcall CQ_setGroupSpecialTitle(int authCode, int64_t groupId, int64_t qqId, const char* title, int64_t durationTime)
{
    return static_cast<int>(result_number(call("CQ_setGroupSpecialTitle", authCode, groupId, qqId, title, durationTime)));
}
extern "C" int __stdcall CQ_setGroupWholeBan(int authCode, int64_t groupId, bool isOpen)
{
    return static_cast<int>(result_number(call("CQ_setGroupWholeBan", authCode, groupId, isOpen)));
}
extern "C" int __stdcall CQ_setGroupAnonymousBan(int authCode, int64_t groupId, const char* anonymous, int64_t banTime)
{
    return static_cast<int>(result_number(call("CQ_setGroupAnonymousBan", authCode, groupId, anonymous, banTime)));
}
extern "C" int __stdcall CQ_setGroupAnonymous(int authCode, int64_t groupId, bool isOpen)
{
    return static_cast<int>(result_number(call("CQ_setGroupAnonymous", authCode, groupId, isOpen)));
}
extern "C" int __stdcall CQ_setGroupCard(int authCode, int64_t groupId, int64_t qqId, const char* newCard)
{
    return static_cast<int>(result_number(call("CQ_setGroupCard", authCode, groupId, qqId, newCard)));
}
extern "C" int __stdcall CQ_setGroupLeave(int authCode, int64_t groupId, bool isDisband)
{
    return static_cast<int>(result_number(call("CQ_setGroupLeave", authCode, groupId, isDisband)));
}
extern "C" int __stdcall CQ_setDiscussLeave(int authCode, int64_t discussId)
{
    return static_cast<int>(result_number(call("CQ_setDiscussLeave", authCode, discussId)));
}
extern "C" int __stdcall CQ_setFriendAddRequest(int authCode, const char* identifying, int requestType, const char* appendMsg)
{
    return static_cast<int>(result_number(call("CQ_setFriendAddRequest", authCode, identifying, requestType, appendMsg)));
}
extern "C" int __stdcall CQ_setGroupAddRequestV2(int authCode, const char* identifying, int requestType, int responseType, const char* appendMsg)
{
    return static_cast<int>(result_number(call("CQ_setGroupAddRequestV2", authCode, identifying, requestType, responseType, appendMsg)));
}
extern "C" int __stdcall CQ_addLog(int authCode, int priority, const char* type, const char* msg)
{
    return static_cast<int>(result_number(call("CQ_addLog", authCode, priority, type, msg)));
}
extern "C" int __stdcall CQ_setFatal(int authCode, const char* errorMsg)
{
    return static_cast<int>(result_number(call("CQ_setFatal", authCode, errorMsg)));
}
extern "C" const char* __stdcall CQ_getGroupMemberInfoV2(int authCode, int64_t groupId, int64_t qqId, bool isCache)
{
    return result_text(call("CQ_getGroupMemberInfoV2", authCode, groupId, qqId, isCache));
}
extern "C" const char* __stdcall CQ_getGroupMemberList(int authCode, int64_t groupId)
{
    return result_text(call("CQ_getGroupMemberList", authCode, groupId));
}
extern "C" const char* __stdcall CQ_getGroupList(int authCode)
{
    return result_text(call("CQ_getGroupList", authCode));
}
extern "C" const char* __stdcall CQ_getStrangerInfo(int authCode, int64_t qqId, bool notCache)
{
    return result_text(call("CQ_getStrangerInfo", authCode, qqId, notCache));
}
extern "C" int __stdcall CQ_canSendImage(int authCode)
{
    return static_cast<int>(result_number(call("CQ_canSendImage", authCode)));
}
extern "C" int __stdcall CQ_canSendRecord(int authCode)
{
    return static_cast<int>(result_number(call("CQ_canSendRecord", authCode)));
}
extern "C" const char* __stdcall CQ_getImage(int authCode, const char* file)
{
    return result_text(call("CQ_getImage", authCode, file));
}
extern "C" const char* __stdcall CQ_getGroupInfo(int authCode, int64_t groupId, bool notCache)
{
    return result_text(call("CQ_getGroupInfo", authCode, groupId, notCache));
}
extern "C" const char* __stdcall CQ_getFriendList(int authCode, bool reserved)
{
    return result_text(call("CQ_getFriendList", authCode, reserved));
}
extern "C" bool __stdcall cq_start(const char* path, int authCode)
{
    return true;
}
extern "C" const char* __stdcall CQ_getChatHistory(int authCode, int64_t groupId, int64_t qq, int count)
{
    return result_text(call("CQ_getChatHistory", authCode, groupId, qq, count));
}
extern "C" const char* __stdcall CQ_getChatHistoryById(int authCode, int64_t parentId, bool isGroup, int messageId)
{
    return result_text(call("CQ_getChatHistoryById", authCode, parentId, isGroup, messageId));
}
