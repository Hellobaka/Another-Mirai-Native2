using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Protocol.MiraiAPIHttp.MiraiAPIResponse;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace Another_Mirai_Native.Protocol.MiraiAPIHttp
{
    public partial class Protocol : IProtocol
    {
        public bool IsConnected
        {
            get
            {
                return MessageConnection != null && MessageConnection.ReadyState == WebSocketState.Open &&
                        EventConnection != null && EventConnection.ReadyState == WebSocketState.Open;
            }
            set
            {
                _ = value;
            }
        }

        public string Name { get; set; } = "MiraiAPIHttp";

        public int CanSendImage()
        {
            return 1;
        }

        public int CanSendRecord()
        {
            return 1;
        }

        public bool Connect()
        {
            ExitFlag = false;
            GetConnectionConfig();
            return ConnectEventServer() && ConnectMessageServer();
        }

        public int DeleteMsg(long msgId)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = 0, // 似乎没问题
                messageId = msgId
            };
            JObject json = CallMiraiAPI(MiraiApiType.recall, request);
            return json == null ? 0 : ((int)json["code"]) == 0 ? 0 : 1;
        }

        public bool Disconnect()
        {
            ExitFlag = true;
            SessionKey_Event = "";
            SessionKey_Message = "";
            EventConnection.Close();
            MessageConnection.Close();
            return IsConnected == false;
        }

        public Dictionary<string, string> GetConnectionConfig()
        {
            MAHConfig.Instance = new MAHConfig();
            WsURL = MAHConfig.Instance.WebSocketURL;
            AuthKey = MAHConfig.Instance.AuthKey;
            QQ = MAHConfig.Instance.QQ;
            return new Dictionary<string, string>
            {
                { "Ws", WsURL },
                { "AuthKey", AuthKey },
                { "QQ", QQ.ToString() },
            };
        }

        public string GetCookies(string domain)
        {
            return "";
        }

        public string GetCsrfToken()
        {
            return "";
        }

        public string GetFriendList(bool reserved)
        {
            return FriendInfo.CollectionToList(GetRawFriendList(reserved));
        }

        public string GetGroupInfo(long groupId, bool notCache)
        {
            return GetRawGroupInfo(groupId, notCache).ToNativeBase64(false);
        }

        public string GetGroupList()
        {
            return GroupInfo.CollectionToList(GetRawGroupList());
        }

        public string GetGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            return GetRawGroupMemberInfo(groupId, qqId, isCache).ToNativeBase64();
        }

        public string GetGroupMemberList(long groupId)
        {
            return GroupMemberInfo.CollectionToList(GetRawGroupMemberList(groupId));
        }

        public string GetLoginNick()
        {
            var profiler = GetBotProfilerInternal();
            return profiler == null ? "" : profiler.nickname;
        }

        public long GetLoginQQ()
        {
            return QQ;
        }

        public List<FriendInfo> GetRawFriendList(bool reserved)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
            };
            JObject json = CallMiraiAPI(MiraiApiType.friendList, request);
            if (json == null)
            {
                return new List<FriendInfo>();
            }
            if (((int)json["code"]) == 0)
            {
                var arr = reserved ? (json["data"] as JArray).Reverse() : json["data"] as JArray;
                List<FriendInfo> friendInfos = new();
                foreach (var item in arr)
                {
                    friendInfos.Add(new FriendInfo
                    {
                        Nick = item["nickname"].ToString(),
                        Postscript = item["remark"].ToString(),
                        QQ = (long)item["id"]
                    });
                }
                return friendInfos;
            }
            else
            {
                return new List<FriendInfo>();
            }
        }

        public GroupInfo GetRawGroupInfo(long groupId, bool notCache)
        {
            var list = GetGroupListInternal();
            if (list == null && !list.Any(x => x.id == groupId))
            {
                return new GroupInfo();
            }
            var result = list.First(x => x.id == groupId);
            return new GroupInfo
            {
                Group = result.id,
                Name = result.name,
                // 若需要则实现成员数量 需再拉取群成员列表
            };
        }

        public List<GroupInfo> GetRawGroupList()
        {
            var list = GetGroupListInternal();
            if (list == null)
            {
                return new List<GroupInfo>();
            }
            List<GroupInfo> result = new();
            foreach (var item in list)
            {
                result.Add(new GroupInfo
                {
                    Group = item.id,
                    Name = item.name,
                    // 若需要则实现成员数量 需再拉取群成员列表
                });
            }
            return result;
        }

        public GroupMemberInfo GetRawGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            var list = GetGroupMemberListInternal(groupId);
            return list == null && !list.Any(x => x.group.id == groupId && x.id == qqId)
                ? new GroupMemberInfo()
                : ParseGroupMemberInfoResponse2GroupMemberInfo(list.First(x => x.group.id == groupId && x.id == qqId));
        }

        public List<GroupMemberInfo> GetRawGroupMemberList(long groupId)
        {
            var list = GetGroupMemberListInternal(groupId);
            if (list == null)
            {
                return new List<GroupMemberInfo>();
            }
            List<GroupMemberInfo> members = new();
            foreach (var item in list)
            {
                members.Add(ParseGroupMemberInfoResponse2GroupMemberInfo(item));
            }
            return members;
        }

        public string GetStrangerInfo(long qqId, bool notCache)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = qqId
            };
            JObject json = CallMiraiAPI(MiraiApiType.userProfile, request);
            var profiler = json["data"].ToObject<ProfilerResponse>();
            return profiler == null
                ? new StrangerInfo().ToNativeBase64()
                : new StrangerInfo()
                {
                    Age = profiler.age,
                    Nick = profiler.nickname,
                    QQ = qqId,
                    Sex = ParseMiraiSex2QQSex(profiler.sex)
                }.ToNativeBase64();
        }

        public int SendDiscussMsg(long discussId, string msg)
        {
            return 1;
        }

        public int SendGroupMessage(long groupId, string msg, int msgId = 0)
        {
            IMiraiMessageBase[] msgChains = CQCodeBuilder.BuildMessageChains(msg, out int quoteId).ToArray();
            if (msgId == 0)
            {
                msgId = quoteId;
            }
            if (msgChains.Length <= 0)
            {
                return 0;
            }
            object request;
            if (msgId > 0)
            {
                request = new
                {
                    sessionKey = SessionKey_Message,
                    target = groupId,
                    quote = msgId,
                    messageChain = msgChains
                };
            }
            else
            {
                request = new
                {
                    sessionKey = SessionKey_Message,
                    target = groupId,
                    messageChain = msgChains
                };
            }
            JObject json = CallMiraiAPI(MiraiApiType.sendGroupMessage, request);
            return json == null || ((int)json["code"]) != 0 ? 0 : (int)json["messageId"];
        }

        public int SendLike(long qqId, int count)
        {
            return 1;
        }

        public int SendPrivateMessage(long qqId, string msg)
        {
            IMiraiMessageBase[] msgChains = CQCodeBuilder.BuildMessageChains(msg, out int quoteId).ToArray();
            if (msgChains.Length <= 0)
            {
                return 0;
            }
            object request;
            if (quoteId > 0)
            {
                request = new
                {
                    sessionKey = SessionKey_Message,
                    target = qqId,
                    quote = quoteId,
                    messageChain = msgChains
                };
            }
            else
            {
                request = new
                {
                    sessionKey = SessionKey_Message,
                    target = qqId,
                    messageChain = msgChains
                };
            }
            JObject json = CallMiraiAPI(MiraiApiType.sendFriendMessage, request);
            return json == null || ((int)json["code"]) != 0 ? 0 : (int)json["messageId"];
        }

        public bool SetConnectionConfig(Dictionary<string, string> config)
        {
            bool success = config != null && config.ContainsKey("Ws") && config.ContainsKey("AuthKey") && config.ContainsKey("QQ");
            success = success && !string.IsNullOrEmpty(config["Ws"]);
            success = success && !string.IsNullOrEmpty(config["AuthKey"]);
            long value = 0;
            success = success && long.TryParse(config["QQ"], out value);
            if (success)
            {
                WsURL = config["Ws"];
                if (WsURL.EndsWith("/"))
                {
                    WsURL = WsURL.Substring(0, WsURL.Length - 1);
                }
                AuthKey = config["AuthKey"];
                QQ = value;
                MAHConfig.Instance.SetConfig("WebSocketURL", WsURL);
                MAHConfig.Instance.SetConfig("AuthKey", AuthKey);
                MAHConfig.Instance.SetConfig("QQ", QQ);
            }
            return success;
        }

        public int SetDiscussLeave(long discussId)
        {
            return 1;
        }

        public int SetFriendAddRequest(string identifying, int responseType, string appendMsg)
        {
            long qqId = 0;
            if (RequestCache.FriendRequest.TryGetValue(identifying, out (long, string) value))
            {
                qqId = value.Item1;
                RequestCache.FriendRequest.Remove(identifying);
            }
            if (qqId == 0)
            {
                LogHelper.Error("处理好友添加请求", "无法从缓存获取请求来源");
                return 1;
            }

            object request = new
            {
                sessionKey = SessionKey_Message,
                eventId = Convert.ToInt64(identifying),
                operate = responseType == 1 ? 0 : 1,
                fromId = qqId,
                groupId = 0,
                message = appendMsg
            };
            JObject json = CallMiraiAPI(MiraiApiType.resp_newFriendRequestEvent, request);
            return json == null ? 1 : 0;
        }

        public int SetGroupAddRequest(string identifying, int requestType, int responseType, string appendMsg)
        {
            long groupId = 0, qqId = 0;
            if (RequestCache.GroupRequest.TryGetValue(identifying, out (long, string, long, string) value))
            {
                qqId = value.Item1;
                groupId = value.Item3;
                RequestCache.GroupRequest.Remove(identifying);
            }
            if (groupId == 0 || qqId == 0)
            {
                LogHelper.Error("处理群添加请求", "无法从缓存获取请求来源");
                return 1;
            }
            object request = new
            {
                sessionKey = SessionKey_Message,
                eventId = Convert.ToInt64(identifying),
                fromId = qqId,
                groupId = groupId,
                operate = responseType == 1 ? 0 : 1,
                message = appendMsg
            };
            JObject json = null;
            if (requestType == 1)
            {
                json = CallMiraiAPI(MiraiApiType.resp_memberJoinRequestEvent, request);
            }
            else if (requestType == 2)
            {
                json = CallMiraiAPI(MiraiApiType.resp_botInvitedJoinGroupRequestEvent, request);
            }
            return json == null ? 1 : 0;
        }

        public int SetGroupAdmin(long groupId, long qqId, bool isSet)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = groupId,
                memberId = qqId,
                assign = isSet
            };
            JObject json = CallMiraiAPI(MiraiApiType.memberAdmin, request);
            return json == null ? 0 : (int)json["code"] == 0 ? 0 : 1;
        }

        public int SetGroupAnonymous(long groupId, bool isOpen)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = groupId,
                config = new
                {
                    anonymousChat = isOpen
                }
            };
            JObject json = CallMiraiAPI(MiraiApiType.SubgroupConfig_update, request);
            return json == null ? 0 : (int)json["code"] == 0 ? 0 : 1;
        }

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime)
        {
            return 1;
        }

        public int SetGroupBan(long groupId, long qqId, long banTime)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = groupId,
                memberId = qqId,
                time = banTime
            };
            JObject json = CallMiraiAPI(banTime > 0 ? MiraiApiType.mute : MiraiApiType.unmute, request);
            return json == null ? 0 : (int)json["code"] == 0 ? 0 : 1;
        }

        public int SetGroupCard(long groupId, long qqId, string newCard)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = groupId,
                memberId = qqId,
                info = new
                {
                    name = newCard
                }
            };
            JObject json = CallMiraiAPI(MiraiApiType.SubmemberInfo_update, request);
            return json == null ? 0 : (int)json["code"] == 0 ? 0 : 1;
        }

        public int SetGroupKick(long groupId, long qqId, bool refuses)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = groupId,
                memberId = qqId,
                block = refuses
            };
            JObject json = CallMiraiAPI(MiraiApiType.kick, request);
            return json == null ? 0 : (int)json["code"] == 0 ? 0 : 1;
        }

        public int SetGroupLeave(long groupId, bool isDisband)
        {
            // Mirai不支持解散群聊
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = groupId
            };
            JObject json = CallMiraiAPI(MiraiApiType.quit, request);
            return json == null ? 0 : (int)json["code"] == 0 ? 0 : 1;
        }

        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = groupId,
                memberId = qqId,
                info = new
                {
                    specialTitle = title
                }
            };
            JObject json = CallMiraiAPI(MiraiApiType.SubmemberInfo_update, request);
            return json == null ? 0 : (int)json["code"] == 0 ? 0 : 1;
        }

        public int SetGroupWholeBan(long groupId, bool isOpen)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = groupId,
            };
            JObject json = CallMiraiAPI(isOpen ? MiraiApiType.unmuteAll : MiraiApiType.muteAll, request);
            return json == null ? 0 : (int)json["code"] == 0 ? 0 : 1;
        }

        private ProfilerResponse GetBotProfilerInternal()
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
            };
            JObject json = CallMiraiAPI(MiraiApiType.botProfile, request);
            return json.ToObject<ProfilerResponse>();
        }

        private List<GroupListResponse> GetGroupListInternal()
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
            };
            JObject json = CallMiraiAPI(MiraiApiType.groupList, request);
            if (json == null)
            {
                return null;
            }
            return ((int)json["code"]) == 0 ? json["data"].ToObject<List<GroupListResponse>>() : null;
        }

        private List<GroupMemberInfoResponse> GetGroupMemberListInternal(long target)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target
            };
            JObject json = CallMiraiAPI(MiraiApiType.memberList, request);
            return json["data"].ToObject<List<GroupMemberInfoResponse>>();
        }

        private ProfilerResponse GetMemberProfilerInternal(long groupId, long memberId)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                target = groupId,
                memberId = memberId
            };
            JObject json = CallMiraiAPI(MiraiApiType.memberProfile, request);
            return json != null ? json.ToObject<ProfilerResponse>() : null;
        }

        private string GetMessageByMsgId(int messageId, long authorId)
        {
            object request = new
            {
                sessionKey = SessionKey_Message,
                messageId,
                target = authorId
            };
            JObject json = CallMiraiAPI(MiraiApiType.messageFromId, request);
            if (json == null)
            {
                return "";
            }
            return ((int)json["code"]) == 0
                ? CQCodeBuilder.Parse(CQCodeBuilder.ParseJArray2MiraiMessageBaseList(json["data"]["messageChain"] as JArray))
                : string.Empty;
        }

        private GroupMemberInfo ParseGroupMemberInfoResponse2GroupMemberInfo(GroupMemberInfoResponse response)
        {
            ProfilerResponse appendInfo = null;
            if (MAHConfig.Instance.FullMemberInfo)
            {
                appendInfo = GetMemberProfilerInternal(response.group.id, response.id);
            }
            int userPermission = 0;
            switch (response.permission)
            {
                case "MEMBER":
                    userPermission = 1;
                    break;
                case "ADMINISTRATOR":
                    userPermission = 2;
                    break;
                case "OWNER":
                    userPermission = 3;
                    break;
                default:
                    break;
            }
            return new GroupMemberInfo
            {
                Age = appendInfo == null ? 0 : appendInfo.age,
                Area = "",
                Card = response.memberName,
                ExclusiveTitle = response.specialTitle,
                ExclusiveTitleExpirationTime = null,
                Group = response.group.id,
                IsAllowEditorCard = true,
                IsBadRecord = false,
                JoinGroupDateTime = Helper.TimeStamp2DateTime(response.joinTimestamp),
                LastSpeakDateTime = Helper.TimeStamp2DateTime(response.lastSpeakTimestamp),
                Level = appendInfo == null ? "0" : appendInfo.level.ToString(),
                MemberType = (QQGroupMemberType)userPermission,
                Nick = response.memberName,
                QQ = response.id,
                Sex = ParseMiraiSex2QQSex(appendInfo?.sex)
            };
        }

        private QQSex ParseMiraiSex2QQSex(string sex)
        {
            return sex == null ? QQSex.Unknown : sex == "MALE" ? QQSex.Man : sex == "FEMALE" ? QQSex.Woman : QQSex.Unknown;
        }
    }
}