using Another_Mirai_Native.Native;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Another_Mirai_Native.gRPC
{
    public class CQPFunctionWrapper : CQPFunctions.CQPFunctionsBase
    {
        public override Task<Int32Value> CQ_sendPrivateMsg(CQ_sendPrivateMsg_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.QqId, request.Msg) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_sendGroupMsg(CQ_sendGroupMsg_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.Msg) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_sendGroupQuoteMsg(CQ_sendGroupQuoteMsg_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.MsgId, request.Msg) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_sendDiscussMsg(CQ_sendDiscussMsg_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.DiscussId, request.Msg) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_deleteMsg(CQ_deleteMsg_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.MsgId) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_sendLikeV2(CQ_sendLikeV2_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.QqId, request.Count) ?? 0)
            });
        }

        public override Task<StringValue> CQ_getCookiesV2(CQ_getCookiesV2_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.Domain)?.ToString()
            });
        }

        public override Task<StringValue> CQ_getImage(CQ_getImage_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.File)?.ToString()
            });
        }

        public override Task<StringValue> CQ_getRecordV2(CQ_getRecordV2_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.File, request.Format)?.ToString()
            });
        }

        public override Task<StringValue> CQ_getCsrfToken(CQ_getCsrfToken_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode)?.ToString()
            });
        }

        public override Task<StringValue> CQ_getAppDirectory(CQ_getAppDirectory_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode)?.ToString()
            });
        }

        public override Task<Int32Value> CQ_getLoginQQ(CQ_getLoginQQ_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode) ?? 0)
            });
        }

        public override Task<StringValue> CQ_getLoginNick(CQ_getLoginNick_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode)?.ToString()
            });
        }

        public override Task<Int32Value> CQ_setGroupKick(CQ_setGroupKick_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.QqId, request.Refuses) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setGroupBan(CQ_setGroupBan_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.QqId, request.Time) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setGroupAdmin(CQ_setGroupAdmin_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.QqId, request.IsSet) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setGroupSpecialTitle(CQ_setGroupSpecialTitle_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.QqId, request.Title, request.DurationTime) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setGroupWholeBan(CQ_setGroupWholeBan_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.IsOpen) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setGroupAnonymousBan(CQ_setGroupAnonymousBan_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.Anonymous, request.BanTime) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setGroupAnonymous(CQ_setGroupAnonymous_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.IsOpen) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setGroupCard(CQ_setGroupCard_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.QqId, request.NewCard) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setGroupLeave(CQ_setGroupLeave_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.IsDisband) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setDiscussLeave(CQ_setDiscussLeave_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.DiscussId) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setFriendAddRequest(CQ_setFriendAddRequest_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.Identifying, request.RequestType, request.AppendMsg) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setGroupAddRequestV2(CQ_setGroupAddRequestV2_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.Identifying, request.RequestType, request.ResponseType, request.AppendMsg) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_addLog(CQ_addLog_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.Priority, request.Type, request.Msg) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_setFatal(CQ_setFatal_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.ErrorMsg) ?? 0)
            });
        }

        public override Task<StringValue> CQ_getGroupMemberInfoV2(CQ_getGroupMemberInfoV2_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.QqId, request.IsCache)?.ToString()
            });
        }

        public override Task<StringValue> CQ_getGroupMemberList(CQ_getGroupMemberList_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId)?.ToString()
            });
        }

        public override Task<StringValue> CQ_getGroupList(CQ_getGroupList_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode)?.ToString()
            });
        }

        public override Task<StringValue> CQ_getStrangerInfo(CQ_getStrangerInfo_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.QqId, request.NotCache)?.ToString()
            });
        }

        public override Task<StringValue> CQ_getGroupInfo(CQ_getGroupInfo_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.GroupId, request.NotCache)?.ToString()
            });
        }

        public override Task<StringValue> CQ_getFriendList(CQ_getFriendList_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue
            {
                Value = Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode, request.Reserved)?.ToString()
            });
        }

        public override Task<Int32Value> CQ_canSendImage(CQ_canSendImage_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode) ?? 0)
            });
        }

        public override Task<Int32Value> CQ_canSendRecord(CQ_canSendRecord_Parameters request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value
            {
                Value = (int)(Server.GetCQPImplementation(request).Invoke(Server.GetFunctionName(request), request.AuthCode) ?? 0)
            });
        }
    }
}