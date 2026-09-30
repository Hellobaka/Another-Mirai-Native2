#pragma once
// Generated from XiaoLiZi/Structs.cs.
#include <cstdint>
namespace amn::xlz {
#pragma pack(push, 1)
struct ServiceInfo {
    int32_t ServiceList{};
    int32_t ServiceLevel{};
};
struct PrivateMessageEvent {
    int64_t SenderQQ{};
    int64_t ThisQQ{};
    int32_t MessageReq{};
    int64_t MessageSeq{};
    int32_t MessageReceiveTime{};
    int64_t MessageGroupQQ{};
    int32_t MessageSendTime{};
    int64_t MessageRandom{};
    uint32_t MessageClip{};
    uint32_t MessageClipCount{};
    int64_t MessageClipID{};
    const char* MessageContent = nullptr;
    uint32_t BubbleID{};
    int32_t MessageType{};
    int32_t MessageSubType{};
    int32_t MessageSubTemporaryType{};
    uint32_t RedEnvelopeType{};
    void* SessionToken{};
    int64_t SourceEventQQ{};
    const char* SourceEventQQName = nullptr;
    const char* FileID = nullptr;
    void* FileMD5{};
    const char* FileName = nullptr;
    int64_t FileSize{};
};
struct EventTypeBase {
    int64_t ThisQQ{};
    int64_t SourceGroupQQ{};
    int64_t OperateQQ{};
    int64_t TriggerQQ{};
    int64_t MessageSeq{};
    int32_t MessageTimestamp{};
    const char* SourceGroupName = nullptr;
    const char* OperateQQName = nullptr;
    const char* TriggerQQName = nullptr;
    const char* MessageContent = nullptr;
    int32_t EventType{};
    int32_t EventSubType{};
};
struct GroupMessageEvent {
    int64_t SenderQQ{};
    int64_t ThisQQ{};
    int32_t MessageReq{};
    int32_t MessageReceiveTime{};
    int64_t MessageGroupQQ{};
    const char* SourceGroupName = nullptr;
    const char* SenderNickname = nullptr;
    int32_t MessageSendTime{};
    int64_t MessageRandom{};
    int32_t MessageClip{};
    int32_t MessageClipCount{};
    int64_t MessageClipID{};
    int32_t MessageType{};
    const char* SenderTitle = nullptr;
    const char* MessageContent = nullptr;
    const char* ReplyMessageContent = nullptr;
    int32_t BubbleID{};
    int32_t GroupChatLevel{};
    int32_t PendantID{};
    const char* AnonymousNickname = nullptr;
    void* AnonymousFalg{};
    const char* ReservedParameters = nullptr;
    int64_t AnonymousId{};
    int32_t FontId{};
};
struct FriendInfo {
    const char* Email = nullptr;
    int64_t QQNumber{};
    const char* Name = nullptr;
    const char* Note = nullptr;
    const char* Status = nullptr;
    uint32_t Likes{};
    const char* Signature = nullptr;
    uint32_t Gender{};
    uint32_t Level{};
    uint32_t Age{};
    const char* Nation = nullptr;
    const char* Province = nullptr;
    const char* City = nullptr;
    ServiceInfo serviceInfo{};
    uint32_t ContinuousOnlineTime{};
    const char* QQTalent = nullptr;
    uint32_t LikesToday{};
    uint32_t LikesAvailableToday{};
};
struct GroupInfo {
    int64_t GroupID{};
    int64_t GroupQQ{};
    int64_t CFlag{};
    int64_t GroupInfoSeq{};
    int64_t GroupFlagExt{};
    int64_t GroupRankSeq{};
    int64_t CertificationType{};
    int64_t ShutUpTimestamp{};
    int64_t ThisShutUpTimestamp{};
    int64_t CmdUinUinFlag{};
    int64_t AdditionalFlag{};
    int64_t GroupTypeFlag{};
    int64_t GroupSecType{};
    int64_t GroupSecTypeInfo{};
    int64_t GroupClassExt{};
    int64_t AppPrivilegeFlag{};
    int64_t SubscriptionUin{};
    int64_t GroupMemberCount{};
    int64_t MemberNumSeq{};
    int64_t MemberCardSeq{};
    int64_t GroupFlagExt3{};
    int64_t GroupOwnerUin{};
    int64_t IsConfGroup{};
    int64_t IsModifyConfGroupFace{};
    int64_t IsModifyConfGroupName{};
    int64_t CmduinJoinTime{};
    const char* GroupName = nullptr;
    const char* GroupMemo = nullptr;
};
struct GroupMemberInfo {
    const char* QQNumber = nullptr;
    uint32_t Age{};
    uint32_t Gender{};
    const char* Name = nullptr;
    const char* Email = nullptr;
    const char* Nickname = nullptr;
    const char* Note = nullptr;
    const char* Title = nullptr;
    const char* Phone = nullptr;
    int64_t TitleTimeout{};
    int64_t ShutUpTimestamp{};
    int64_t JoinTime{};
    int64_t ChatTime{};
    int64_t Level{};
};
#pragma pack(pop)
}
