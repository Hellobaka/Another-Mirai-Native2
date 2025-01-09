﻿using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.Protocol.OneBot.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace Another_Mirai_Native.Protocol.OneBot
{
    public partial class OneBotAPI : ConfigBase, IProtocol
    {
        public OneBotAPI()
            : base(@"conf\OneBot_v11.json")
        {
            LoadConfig();
        }

        public bool IsConnected
        {
            get
            {
                return APIClient != null && APIClient.ReadyState == WebSocketState.Open &&
                         EventClient != null && EventClient.ReadyState == WebSocketState.Open;
            }

            set
            {
                _ = value;
            }
        }

        public string Name { get; set; } = "OneBot v11";

        private bool DiscardOfflineMessage { get; set; } = true;

        private List<FriendInfo> FriendInfoList { get; set; } = new();

        private List<GroupInfo> GroupInfoList { get; set; } = new();

        private ConcurrentDictionary<long, ConcurrentDictionary<long, GroupMemberInfo>> GroupMemberInfoDict { get; set; } = new();

        public int CanSendImage()
        {
            var r = CallOneBotAPI(APIType.can_send_image, new Dictionary<string, object>
            {
            });
            var token = r?["yes"];
            if (token == null)
            {
                return 0;
            }
            return token.ToObject<bool>() ? 1 : 0;
        }

        public int CanSendRecord()
        {
            var r = CallOneBotAPI(APIType.can_send_record, new Dictionary<string, object>
            {
            });
            var token = r?["yes"];
            if (token == null)
            {
                return 0;
            }
            return token.ToObject<bool>() ? 1 : 0;
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
            }) != null ? 0 : 1;
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
            LoadConfig();
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
            return r != null ? r["cookies"]?.ToString() ?? "" : "";
        }

        public string GetCsrfToken()
        {
            var r = CallOneBotAPI(APIType.get_csrf_token, new Dictionary<string, object>
            {
            });
            var token = r?["token"];
            if (token == null)
            {
                return "";
            }
            return token.ToString();
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
            var r = CallOneBotAPI(APIType.get_login_info, new Dictionary<string, object>
            {
            });
            var token = r?["nickname"];
            if (token == null)
            {
                return "";
            }
            return token.ToString();
        }

        public long GetLoginQQ()
        {
            var r = CallOneBotAPI(APIType.get_login_info, new Dictionary<string, object>
            {
            });
            var token = r?["user_id"];
            if (token == null)
            {
                return 0;
            }
            return token.ToObject<long>();
        }

        public List<FriendInfo> GetRawFriendList(bool reserved)
        {
            var r = CallOneBotAPI(APIType.get_friend_list, new Dictionary<string, object>
            {
            });
            if (r is JArray array)
            {
                var arr = reserved ? array.Reverse() : array;
                List<FriendInfo> friendInfos = new();
                foreach (var item in arr)
                {
                    friendInfos.Add(new FriendInfo
                    {
                        Nick = item["nickname"]?.ToString() ?? "",
                        Postscript = item["remark"]?.ToString() ?? "",
                        QQ = (long?)item["user_id"] ?? 0
                    });
                }
                return friendInfos;
            }
            return new List<FriendInfo>();
        }

        public GroupInfo GetRawGroupInfo(long groupId, bool notCache)
        {
            var r = CallOneBotAPI(APIType.get_group_info, new Dictionary<string, object>
            {
                {"group_id", groupId},
                {"no_cache", notCache},
            });
            if (r != null)
            {
                return new GroupInfo
                {
                    Group = (long?)r["group_id"] ?? 0,
                    Name = r["group_name"]?.ToString() ?? "",
                    CurrentMemberCount = (int?)r["member_count"] ?? 0,
                    MaxMemberCount = (int?)r["max_member_count"] ?? 0,
                };
            }
            return new GroupInfo();
        }

        public List<GroupInfo> GetRawGroupList()
        {
            var r = CallOneBotAPI(APIType.get_group_list, new Dictionary<string, object>
            {
            });
            if (r is JArray arr)
            {
                List<GroupInfo> groupInfos = new();
                foreach (var item in arr)
                {
                    groupInfos.Add(new GroupInfo
                    {
                        Group = (long?)item["group_id"] ?? 0,
                        Name = item["group_name"]?.ToString() ?? "",
                        CurrentMemberCount = (int?)item["member_count"] ?? 0,
                        MaxMemberCount = (int?)item["max_member_count"] ?? 0,
                    });
                }
                return groupInfos;
            }
            return new List<GroupInfo>();
        }

        public GroupMemberInfo GetRawGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            var r = CallOneBotAPI(APIType.get_group_member_info, new Dictionary<string, object>
            {
                {"group_id", groupId},
                {"user_id", qqId},
                {"no_cache", isCache},
            });
            return r != null ? ParseResult2GroupMemberInfo(r as JObject) : new GroupMemberInfo();
        }

        public List<GroupMemberInfo> GetRawGroupMemberList(long groupId)
        {
            var r = CallOneBotAPI(APIType.get_group_member_list, new Dictionary<string, object>
            {
                {"group_id", groupId},
            });
            if (r is JArray arr)
            {
                List<GroupMemberInfo> groupInfos = new();
                foreach (var item in arr)
                {
                    groupInfos.Add(ParseResult2GroupMemberInfo(item as JObject));
                }
                return groupInfos;
            }
            return new();
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
                    Age = (int?)r["age"] ?? 0,
                    Nick = r["nickname"]?.ToString() ?? "",
                    QQ = (long?)r["user_id"] ?? 0,
                    Sex = ParseString2QQSex(r["sex"]?.ToString())
                }.ToNativeBase64()
                : new StrangerInfo().ToNativeBase64();
        }

        public void LoadConfig()
        {
            WsURL = GetConfig("WebSocketURL", "");
            AuthKey = GetConfig("AuthKey", "");
            MessageType = GetConfig("MessageType", "Array");
            DiscardOfflineMessage = GetConfig("DiscardOfflineMessage", true);
        }

        public int SendDiscussMsg(long discussId, string msg)
        {
            return 1;
        }

        public int SendGroupMessage(long groupId, string msg, int msgId = 0)
        {
            if (msgId != 0)
            {
                msg = $"[CQ:reply,id={msgId}]" + msg;
            }
            msg = RepackCQCode(msg);
            if (string.IsNullOrEmpty(msg))
            {
                return 0;
            }
            if (MessageType == "Array")
            {
                var arrMessage = PackCQCodeMessage(msg);
                var r = CallOneBotAPI(APIType.send_group_msg, new Dictionary<string, object>
                {
                    {"group_id", groupId },
                    {"message", arrMessage },
                    {"auto_escape", false },
                });
                return r != null ? (int?)r["message_id"] ?? 0 : 0;
            }
            else
            {
                var r = CallOneBotAPI(APIType.send_group_msg, new Dictionary<string, object>
                {
                    {"group_id", groupId },
                    {"message", msg },
                    {"auto_escape", false },
                });
                return r != null ? (int?)r["message_id"] ?? 0 : 0;
            }
        }

        private object PackCQCodeMessage(string msg)
        {
            Regex regex = new("(\\[CQ:.*?,.*?\\])");
            var cqCodeCaptures = regex.Matches(msg).Cast<Match>().Select(m => m.Value).ToList();

            var ls = CQCode.Parse(msg);
            var s = regex.Split(msg).ToList();
            s.RemoveAll(string.IsNullOrEmpty);
            List<JObject> result = new();
            foreach (var item in s)
            {
                if (cqCodeCaptures.Contains(item))
                {
                    var cqcode = CQCode.Parse(item).FirstOrDefault();
                    if (cqcode == null)
                    {
                        continue;
                    }
                    JObject json_data = new();
                    JObject json = new()
                    {
                        new JProperty("type", cqcode.Function.GetDescription()),
                        new JProperty("data", json_data)
                    };
                    foreach (var data in cqcode.Items)
                    {
                        json_data.Add(new JProperty(data.Key, data.Value));
                    }
                    result.Add(json);
                }
                else
                {
                    result.Add(new JObject
                    {
                        new JProperty("type", "text"),
                        new JProperty("data", new JObject()
                        {
                            new JProperty("text", item),
                        })
                    });
                }
            }
            return result;
        }

        public int SendLike(long qqId, int count)
        {
            return CallOneBotAPI(APIType.send_like, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"times", count },
            }) != null ? 0 : 1;
        }

        public int SendPrivateMessage(long qqId, string msg)
        {
            msg = RepackCQCode(msg);
            if (string.IsNullOrEmpty(msg))
            {
                return -1;
            }
            if (MessageType == "Array")
            {
                var arrMessage = PackCQCodeMessage(msg);
                var r = CallOneBotAPI(APIType.send_private_msg, new Dictionary<string, object>
                {
                    {"user_id", qqId },
                    {"message", arrMessage },
                    {"auto_escape", false },
                });
                return r != null ? (int?)r["message_id"] ?? 0 : 0;
            }
            else
            {
                var r = CallOneBotAPI(APIType.send_private_msg, new Dictionary<string, object>
                {
                    {"user_id", qqId },
                    {"message", msg },
                    {"auto_escape", false },
                });
                return r != null ? (int?)r["message_id"] ?? 0 : 0;
            }
        }

        public bool SetConnectionConfig(Dictionary<string, string> config)
        {
            if (config == null
                || !config.ContainsKey("Ws")
                || !config.ContainsKey("AuthKey")
                || string.IsNullOrEmpty(config["Ws"]))
            {
                return false;
            }

            WsURL = config["Ws"];
            if (WsURL.EndsWith("/"))
            {
                WsURL = WsURL.Substring(0, WsURL.Length - 1);
            }
            AuthKey = config["AuthKey"];
            SetConfig("WebSocketURL", WsURL);
            SetConfig("AuthKey", AuthKey);
            return true;
        }

        public int SetDiscussLeave(long discussId)
        {
            return 1;
        }

        public int SetFriendAddRequest(string identifying, int responseType, string appendMsg)
        {
            RequestCache.FriendRequest.Remove(identifying);

            var r = CallOneBotAPI(APIType.set_friend_add_request, new Dictionary<string, object>
            {
                {"flag", identifying },
                {"approve", responseType == 1 },
                {"remark", appendMsg },
            });
            return r != null ? 0 : 1;
        }

        public int SetGroupAddRequest(string identifying, int requestType, int responseType, string appendMsg)
        {
            RequestCache.FriendRequest.Remove(identifying);
            var r = CallOneBotAPI(APIType.set_group_add_request, new Dictionary<string, object>
            {
                {"flag", identifying },
                {"sub_type", requestType == 1 ? "add" : "invite" },
                {"approve", responseType == 1 },
                {"remark", appendMsg },
            });
            return r != null ? 0 : 1;
        }

        public int SetGroupAdmin(long groupId, long qqId, bool isSet)
        {
            var r = CallOneBotAPI(APIType.set_group_admin, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"group_id", groupId },
                {"enable", isSet },
            });
            return r != null ? 0 : 1;
        }

        public int SetGroupAnonymous(long groupId, bool isOpen)
        {
            var r = CallOneBotAPI(APIType.set_group_anonymous, new Dictionary<string, object>
            {
                {"group_id", groupId },
                {"enable", isOpen },
            });
            return r != null ? 0 : 1;
        }

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime)
        {
            var r = CallOneBotAPI(APIType.set_group_anonymous_ban, new Dictionary<string, object>
            {
                {"group_id", groupId },
                {"anonymous_flag", anonymous },
                {"duration", banTime },
            });
            return r != null ? 0 : 1;
        }

        public int SetGroupBan(long groupId, long qqId, long banTime)
        {
            var r = CallOneBotAPI(APIType.set_group_ban, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"group_id", groupId },
                {"duration", banTime },
            });
            return r != null ? 0 : 1;
        }

        public int SetGroupCard(long groupId, long qqId, string newCard)
        {
            var r = CallOneBotAPI(APIType.set_group_card, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"group_id", groupId },
                {"card", newCard },
            });
            return r != null ? 0 : 1;
        }

        public int SetGroupKick(long groupId, long qqId, bool refuses)
        {
            var r = CallOneBotAPI(APIType.set_group_kick, new Dictionary<string, object>
            {
                {"user_id", qqId },
                {"group_id", groupId },
                {"reject_add_request", refuses },
            });
            return r != null ? 0 : 1;
        }

        public int SetGroupLeave(long groupId, bool isDisband)
        {
            var r = CallOneBotAPI(APIType.set_group_leave, new Dictionary<string, object>
            {
                {"group_id", groupId },
                {"is_dismiss", isDisband }
            });
            return r != null ? 0 : 1;
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
            return r != null ? 0 : 1;
        }

        public int SetGroupWholeBan(long groupId, bool isOpen)
        {
            var r = CallOneBotAPI(APIType.set_group_whole_ban, new Dictionary<string, object>
            {
                {"group_id", groupId },
                {"enable", isOpen },
            });
            return r != null ? 0 : 1;
        }

        private string? GetGroupName(long groupId, bool withBracket = false)
        {
            var groupInfo = GroupInfoList.FirstOrDefault(x => x.Group == groupId);
            if (groupInfo == null)
            {
                groupInfo = GetRawGroupInfo(groupId, true);
                if (groupInfo != null && groupInfo.Group > 0)
                {
                    GroupInfoList.Add(groupInfo);
                }
            }
            return groupInfo == null ? "" : withBracket ? $"({groupInfo.Name})" : groupInfo.Name;
        }

        private string GetNickFromGroupMemberInfo(GroupMemberInfo info)
        {
            if (info == null)
            {
                return "";
            }
            return string.IsNullOrEmpty(info.Card) ? info.Nick : info.Card;
        }

        private string? GetGroupMemberNick(long groupId, long qq, bool withBracket = false)
        {
            if (GroupMemberInfoDict.TryGetValue(groupId, out var groupInfo))
            {
                if (groupInfo.TryGetValue(qq, out var memberNick))
                {
                    string nick = GetNickFromGroupMemberInfo(memberNick);
                    return withBracket ? $"({nick})" : nick;
                }
                else
                {
                    var info = GetRawGroupMemberInfo(groupId, qq, false);
                    if (info != null && info.Group > 0)
                    {
                        groupInfo.TryAdd(qq, info);
                        string nick = GetNickFromGroupMemberInfo(info);
                        return withBracket ? $"({nick})" : nick;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            else
            {
                groupInfo = new();
                GroupMemberInfoDict.TryAdd(groupId, groupInfo);
                var info = GetRawGroupMemberInfo(groupId, qq, false);
                if (info != null && info.Group > 0)
                {
                    groupInfo.TryAdd(qq, info);
                    string nick = GetNickFromGroupMemberInfo(info);
                    return withBracket ? $"({nick})" : nick;
                }
                else
                {
                    return "";
                }
            }
        }

        private string? GetFriendNick(long qq, bool withBracket = false)
        {
            var friendInfo = FriendInfoList.FirstOrDefault(x => x.QQ == qq);
            if (friendInfo == null)
            {
                FriendInfoList = GetRawFriendList(false);
                var info = FriendInfoList.FirstOrDefault(x => x.QQ == qq);
                return info == null ? "" : withBracket ? $"({info.Nick})" : info.Nick;
            }
            else
            {
                return friendInfo == null ? "" : withBracket ? $"({friendInfo.Nick})" : friendInfo.Nick;
            }
        }

        private void UpdateGroupMemberNick(long groupId, long qq, string nick)
        {
            if (GroupMemberInfoDict.TryGetValue(groupId, out var dict) && dict.TryGetValue(qq, out var info))
            {
                info.Card = nick;
            }
        }

        private void UpdateMemberLeave(long groupId, long qq)
        {
            if (GroupMemberInfoDict.TryGetValue(groupId, out var dict) && dict.TryGetValue(qq, out _))
            {
                dict.TryRemove(qq, out _);
            }
        }

        private void UpdateGroupLeave(long groupId)
        {
            var groupInfo = GroupInfoList.FirstOrDefault(x => x.Group == groupId);
            if (groupInfo != null)
            {
                GroupInfoList.Remove(groupInfo);
            }

            if (GroupMemberInfoDict.ContainsKey(groupId))
            {
                GroupMemberInfoDict.TryRemove(groupId, out _);
            }
        }

        private GroupMemberInfo ParseResult2GroupMemberInfo(JObject? r)
        {
            if (r == null)
            {
                return new();
            }
            return new GroupMemberInfo
            {
                Age = (int?)r["age"] ?? 0,
                Area = r["area"]?.ToString() ?? "",
                Card = r["card"]?.ToString() ?? "",
                ExclusiveTitle = r["title"]?.ToString() ?? "",
                ExclusiveTitleExpirationTime = Helper.TimeStamp2DateTime((int?)r["title_expire_time"] ?? 0),
                Group = (long?)r["group_id"] ?? 0,
                IsAllowEditorCard = (bool?)r["card_changeable"] ?? false,
                IsBadRecord = (bool?)r["unfriendly"] ?? false,
                JoinGroupDateTime = Helper.TimeStamp2DateTime((int?)r["join_time"] ?? 0),
                LastSpeakDateTime = Helper.TimeStamp2DateTime((int?)r["last_sent_time"] ?? 0),
                Level = r["level"]?.ToString() ?? "",
                Nick = r["nickname"]?.ToString() ?? "",
                QQ = (long?)r["user_id"] ?? 0,
                Sex = ParseString2QQSex(r["sex"]?.ToString()),
                MemberType = ParseString2MemberType(r["role"]?.ToString() ?? "")
            };
        }

        private QQGroupMemberType ParseString2MemberType(string memberType)
        {
            return memberType == "owner" ? QQGroupMemberType.Creator : memberType == "admin"
                ? QQGroupMemberType.Manage : QQGroupMemberType.Member;
        }

        private QQSex ParseString2QQSex(string? sex)
        {
            if (string.IsNullOrEmpty(sex))
            {
                return QQSex.Unknown;
            }
            return sex == "male" ? QQSex.Man : sex == "female"
                ? QQSex.Woman : QQSex.Unknown;
        }

        private string RepackCQCode(string msg)
        {
            foreach (var item in CQCode.Parse(msg))
            {
                CQCode newCQcode;
                if (item.IsImageCQCode)
                {
                    string picPath = item.Items["file"];
                    // 以下为两个纠错路径, 防止拼接路径时出现以下两种情况
                    // basePath + "\foo.jpg"
                    // basePath + "foo.jpg"
                    string picPathA = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image") + picPath;
                    string picPathB = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", picPath);
                    if (File.Exists(picPathA))
                    {
                        picPath = picPathA;
                    }
                    else if (File.Exists(picPathB))
                    {
                        picPath = picPathB;
                    }
                    else
                    {
                        // 若以上两个路径均不存在, 判断对应的 cqimg 文件是否存在
                        picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", picPath + ".cqimg");
                        if (!File.Exists(picPath))
                        {
                            LogHelper.WriteLog(LogLevel.Warning, "发送图片", "文件不存在", "");
                            continue;
                        }
                        string picTmp = File.ReadAllText(picPath);
                        // 分离 cqimg 文件中的 url
                        picTmp = picTmp.Split('\n').Last().Replace("url=", "");
                        newCQcode = new(CQCodeType.Image, new KeyValuePair<string, string>("file", picTmp));
                        if (item.Items.ContainsKey("flash"))
                        {
                            newCQcode.Items.Add("type", "flash");
                        }
                        msg = msg.Replace(item.ToSendString(), newCQcode.ToSendString());
                        continue;
                    }
                    // 将图片转换为 base64
                    string picBase64 = Helper.ParsePic2Base64(picPath);
                    if (string.IsNullOrEmpty(picBase64))
                    {
                        continue;
                    }
                    newCQcode = new(CQCodeType.Image, new KeyValuePair<string, string>("file", "base64://" + picBase64));
                    if (item.Items.ContainsKey("flash"))
                    {
                        newCQcode.Items.Add("type", "flash");
                    }
                    msg = msg.Replace(item.ToSendString(), newCQcode.ToSendString());
                }
                else if (item.IsRecordCQCode)
                {
                    newCQcode = new CQCode(CQCodeType.Record);
                    string recordPath = item.Items["file"];
                    recordPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\record", recordPath);
                    if (File.Exists(recordPath))
                    {
                        string extension = new FileInfo(recordPath).Extension;
                        if (extension != ".silk")
                        {
                            if (SilkConverter.SilkEncode(recordPath, extension))
                            {
                                recordPath = recordPath.Replace(extension, ".silk");
                            }
                            else
                            {
                                continue;
                            }
                        }
                        recordPath = new FileInfo(recordPath).FullName;
                        newCQcode.Items.Add("file", $"base64://{Helper.ParsePic2Base64(recordPath)}");
                        msg = msg.Replace(item.ToSendString(), newCQcode.ToSendString());
                    }
                    else if (File.Exists(recordPath + ".cqrecord"))
                    {
                        string recordUrl = File.ReadAllText(recordPath + ".cqrecord").Replace("[record]\nurl=", "");
                        newCQcode.Items.Add("file", recordUrl);
                        msg = msg.Replace(item.ToSendString(), newCQcode.ToSendString());
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return msg;
        }

        public event Action<string, byte[]> QRCodeDisplayAction;

        public event Action QRCodeFinishedAction;
    }
}