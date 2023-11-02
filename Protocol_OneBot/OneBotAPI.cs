using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Protocol.OneBot.Enums;
using Newtonsoft.Json.Linq;

namespace Another_Mirai_Native.Protocol.OneBot
{
    public partial class OneBotAPI : IProtocol
    {
        public bool IsConnected { get; set; }

        public string Name { get; set; } = "OneBot v11";

        public int CanSendImage()
        {
            var r = CallOneBotAPI(APIType.can_send_image, new Dictionary<string, object>
            {
            });
            return r != null ? r["yes"].ToObject<bool>() ? 1 : 0 : 0;
        }

        public int CanSendRecord()
        {
            var r = CallOneBotAPI(APIType.can_send_record, new Dictionary<string, object>
            {
            });
            return r != null ? r["yes"].ToObject<bool>() ? 1 : 0 : 0;
        }

        public bool Connect()
        {
            ExitFlag = false;
            GetConnectionConfig();
            return ConnectAPIServer() && ConnectEventServer();
        }

        public int DeleteMsg(long msgId)
        {
            return CallOneBotAPI(APIType.delete_msg, new Dictionary<string, object>
            {
                { "message_id", msgId },
            }) != null ? 1 : 0;
        }

        public bool Disconnect()
        {
            ExitFlag = true;
            APIClient.Close();
            EventClient.Close();
            return IsConnected == false;
        }

        public Dictionary<string, string> GetConnectionConfig()
        {
            WsURL = ConfigHelper.GetConfig("WebSocketURL", @"conf\OneBot_v11.json", "");
            AuthKey = ConfigHelper.GetConfig("AuthKey", @"conf\OneBot_v11.json", "");
            return new Dictionary<string, string>
            {
                { "Ws", WsURL },
                { "AuthKey", AuthKey },
            };
        }

        public string GetCookies(string domain)
        {
            var r = CallOneBotAPI(APIType.get_cookies, new Dictionary<string, object>
            {
                {"domain", domain},
            });
            return r != null ? r["cookies"].ToString() : "";
        }

        public string GetCsrfToken()
        {
            var r = CallOneBotAPI(APIType.get_csrf_token, new Dictionary<string, object>
            {
            });
            return r != null ? r["token"].ToString() : "";
        }

        public string GetFriendList(bool reserved)
        {
            var r = CallOneBotAPI(APIType.get_friend_list, new Dictionary<string, object>
            {
            });
            if (r != null)
            {
                var arr = reserved ? (r as JArray).Reverse() : r as JArray;
                List<FriendInfo> friendInfos = new();
                foreach (var item in arr)
                {
                    friendInfos.Add(new FriendInfo
                    {
                        Nick = item["nickname"].ToString(),
                        Postscript = item["remark"].ToString(),
                        QQ = (long)item["user_id"]
                    });
                }
                return FriendInfo.CollectionToList(friendInfos);
            }
            return FriendInfo.CollectionToList(new List<FriendInfo>());
        }

        public string GetGroupInfo(long groupId, bool notCache)
        {
            var r = CallOneBotAPI(APIType.get_group_info, new Dictionary<string, object>
            {
                {"group_id", groupId},
                {"no_cache", notCache},
            });
            if (r != null)
            {
                new GroupInfo
                {
                    Group = (long)r["group_id"],
                    Name = r["group_name"].ToString(),
                    CurrentMemberCount = (int)r["member_count"],
                    MaxMemberCount = (int)r["max_member_count"],
                }.ToNativeBase64(false);
            }
            return new GroupInfo().ToNativeBase64(false);
        }

        public string GetGroupList()
        {
            var r = CallOneBotAPI(APIType.get_group_list, new Dictionary<string, object>
            {
            });
            if (r != null)
            {
                var arr = r as JArray;
                List<GroupInfo> groupInfos = new();
                foreach (var item in arr)
                {
                    groupInfos.Add(new GroupInfo
                    {
                        Group = (long)item["group_id"],
                        Name = item["group_name"].ToString(),
                        CurrentMemberCount = (int)item["member_count"],
                        MaxMemberCount = (int)item["max_member_count"],
                    });
                }
                return GroupInfo.CollectionToList(groupInfos);
            }
            return GroupInfo.CollectionToList(new List<GroupInfo>());
        }

        public string GetGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            var r = CallOneBotAPI(APIType.get_group_info, new Dictionary<string, object>
            {
                {"group_id", groupId},
                {"user_id", qqId},
                {"no_cache", isCache},
            });
            return r != null ? ParseResult2GroupMemberInfo(r as JObject).ToNativeBase64() : new GroupMemberInfo().ToNativeBase64();
        }

        public string GetGroupMemberList(long groupId)
        {
            var r = CallOneBotAPI(APIType.get_group_member_info, new Dictionary<string, object>
            {
                {"group_id", groupId},
            });
            if (r != null)
            {
                var arr = r as JArray;
                List<GroupMemberInfo> groupInfos = new();
                foreach (var item in arr)
                {
                    groupInfos.Add(ParseResult2GroupMemberInfo(item as JObject));
                }
                return GroupMemberInfo.CollectionToList(groupInfos);
            }
            return new GroupMemberInfo().ToNativeBase64();
        }

        public string GetLoginNick()
        {
            var r = CallOneBotAPI(APIType.get_login_info, new Dictionary<string, object>
            {
            });
            return r != null ? r["nickname"].ToString() : "";
        }

        public long GetLoginQQ()
        {
            var r = CallOneBotAPI(APIType.get_login_info, new Dictionary<string, object>
            {
            });
            return r != null ? (long)r["user_id"] : 10001;
        }

        public string GetStrangerInfo(long qqId, bool notCache)
        {
            var r = CallOneBotAPI(APIType.get_stranger_info, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"no_cache", notCache },
            });
            return r != null
                ? new StrangerInfo
                {
                    Age = (int)r["age"],
                    Nick = r[""].ToString(),
                    QQ = (long)r[""],
                    Sex = ParseString2QQSex(r["sex"].ToString())
                }.ToNativeBase64()
                : new StrangerInfo().ToNativeBase64();
        }

        public int SendDiscussMsg(long discussId, string msg)
        {
            return 0;
        }

        public int SendGroupMessage(long groupId, string msg, int msgId = 0)
        {
            // TODO: quote
            var r = CallOneBotAPI(APIType.send_group_msg, new Dictionary<string, object>
            {
                {"group_id", groupId },
                {"message", msg },
                {"auto_escape", false },
            });
            return r != null ? (int)r["message_id"] : 0;
        }

        public int SendLike(long qqId, int count)
        {
            return CallOneBotAPI(APIType.send_like, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"times", count },
            }) != null ? 1 : 0;
        }

        public int SendPrivateMessage(long qqId, string msg)
        {
            var r = CallOneBotAPI(APIType.send_private_msg, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"message", msg },
                {"auto_escape", false },
            });
            return r != null ? (int)r["message_id"] : 0;
        }

        public bool SetConnectionConfig(Dictionary<string, string> config)
        {
            bool success = config != null && config.ContainsKey("Ws") && config.ContainsKey("AuthKey") && config.ContainsKey("QQ");
            success = success && !string.IsNullOrEmpty(config["Ws"]);
            success = success && !string.IsNullOrEmpty(config["AuthKey"]);
            if (success)
            {
                WsURL = config["Ws"];
                if (WsURL.EndsWith("/"))
                {
                    WsURL = WsURL.Substring(0, WsURL.Length - 1);
                    ConfigHelper.SetConfig("WebSocketURL", WsURL, @"conf\MiraiAPIHttp.json");
                }
                AuthKey = config["AuthKey"];
            }
            return success;
        }

        public int SetDiscussLeave(long discussId)
        {
            return 0;
        }

        public int SetFriendAddRequest(string identifying, int responseType, string appendMsg)
        {
            var r = CallOneBotAPI(APIType.set_friend_add_request, new Dictionary<string, object>
            {
                {"flag", identifying },
                {"approve", responseType == 1 },
                {"remark", appendMsg },
            });
            return r != null ? 1 : 0;
        }

        public int SetGroupAddRequest(string identifying, int requestType, int responseType, string appendMsg)
        {
            var r = CallOneBotAPI(APIType.set_group_add_request, new Dictionary<string, object>
            {
                {"flag", identifying },
                {"sub_type", requestType == 1 ? "add" : "invite" },
                {"approve", responseType == 1 },
                {"remark", appendMsg },
            });
            return r != null ? 1 : 0;
        }

        public int SetGroupAdmin(long groupId, long qqId, bool isSet)
        {
            var r = CallOneBotAPI(APIType.set_group_admin, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"group_id", groupId },
                {"enable", isSet },
            });
            return r != null ? 1 : 0;
        }

        public int SetGroupAnonymous(long groupId, bool isOpen)
        {
            var r = CallOneBotAPI(APIType.set_group_anonymous, new Dictionary<string, object>
            {
                {"group_id", groupId },
                {"enable", isOpen },
            });
            return r != null ? 1 : 0;
        }

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime)
        {
            var r = CallOneBotAPI(APIType.set_group_anonymous_ban, new Dictionary<string, object>
            {
                {"group_id", groupId },
                {"anonymous_flag", anonymous },
                {"duration", banTime },
            });
            return r != null ? 1 : 0;
        }

        public int SetGroupBan(long groupId, long qqId, long banTime)
        {
            var r = CallOneBotAPI(APIType.set_group_ban, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"group_id", groupId },
                {"duration", banTime },
            });
            return r != null ? 1 : 0;
        }

        public int SetGroupCard(long groupId, long qqId, string newCard)
        {
            var r = CallOneBotAPI(APIType.set_group_card, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"group_id", groupId },
                {"card", newCard },
            });
            return r != null ? 1 : 0;
        }

        public int SetGroupKick(long groupId, long qqId, bool refuses)
        {
            var r = CallOneBotAPI(APIType.set_group_kick, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"group_id", groupId },
                {"reject_add_request", refuses },
            });
            return r != null ? 1 : 0;
        }

        public int SetGroupLeave(long groupId, bool isDisband)
        {
            var r = CallOneBotAPI(APIType.set_group_leave, new Dictionary<string, object>
            {
                {"group_id", groupId },
                {"is_dismiss", isDisband }
            });
            return r != null ? 1 : 0;
        }

        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime)
        {
            var r = CallOneBotAPI(APIType.set_group_special_title, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"group_id", groupId },
                {"special_title", title },
                {"duration", durationTime },
            });
            return r != null ? 1 : 0;
        }

        public int SetGroupWholeBan(long groupId, bool isOpen)
        {
            var r = CallOneBotAPI(APIType.set_group_whole_ban, new Dictionary<string, object>
            {
                {"group_id", groupId },
                {"enable", isOpen },
            });
            return r != null ? 1 : 0;
        }

        private GroupMemberInfo ParseResult2GroupMemberInfo(JObject r)
        {
            return new GroupMemberInfo
            {
                Age = (int)r["age"],
                Area = r["area"].ToString(),
                Card = r["card"].ToString(),
                ExclusiveTitle = r["title"].ToString(),
                ExclusiveTitleExpirationTime = Helper.TimeStamp2DateTime((int)r["title_expire_time"]),
                Group = (long)r["group_id"],
                IsAllowEditorCard = (bool)r["card_changeable"],
                IsBadRecord = (bool)r["unfriendly"],
                JoinGroupDateTime = Helper.TimeStamp2DateTime((int)r["join_time"]),
                LastSpeakDateTime = Helper.TimeStamp2DateTime((int)r["last_sent_time"]),
                Level = r["level"].ToString(),
                Nick = r["nickname"].ToString(),
                QQ = (long)r["user_id"],
                Sex = ParseString2QQSex(r["sex"].ToString()),
                MemberType = ParseString2MemberType(r["role"].ToString())
            };
        }

        private QQGroupMemberType ParseString2MemberType(string memberType)
        {
            return memberType == "owner" ? QQGroupMemberType.Creator : memberType == "admin"
                ? QQGroupMemberType.Manage : QQGroupMemberType.Member;
        }

        private QQSex ParseString2QQSex(string sex)
        {
            return sex == "male" ? QQSex.Man : sex == "female"
                ? QQSex.Woman : QQSex.Unknown;
        }
    }
}