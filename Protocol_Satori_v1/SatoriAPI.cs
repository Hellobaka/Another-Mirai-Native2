using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Protocol.Satori.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Xml.Linq;

namespace Another_Mirai_Native.Protocol.Satori
{
    public partial class Protocol : ConfigBase, IProtocol
    {
        public Protocol()
            : base(@"conf\Satori_v1.json")
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            WsURL = GetConfig("WebSocketURL", "");
            Token = GetConfig("Token", "");
        }

        public string Name { get; set; } = "Satori v1";

        public bool IsConnected
        {
            get
            {
                return EventClient != null && EventClient.ReadyState == WebSocketState.Open;
            }

            set
            {
                _ = value;
            }
        }

        private JObject SendRequest(string api, string payload)
        {
            using var http = new HttpClient();
            int failCode = 400;
            try
            {
                if (string.IsNullOrEmpty(WsURL))
                {
                    LogHelper.Error("发送API请求", "未指定连接参数");
                    throw new ArgumentNullException("BaseUrl");
                }

                http.BaseAddress = new Uri($"{WsURL.Replace("ws", "http")}/v1/");
                StringContent content = new(payload);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                if (!string.IsNullOrEmpty(Token))
                {
                    http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
                }
                content.Headers.Add("X-Self-ID", AppConfig.Instance.CurrentQQ.ToString());
                content.Headers.Add("X-Platform", CurrentPlatform);

                var request = http.PostAsync(api, content).Result;
                failCode = (int)request.StatusCode;
                request.EnsureSuccessStatusCode();
                string response = request.Content.ReadAsStringAsync().Result;
                LogHelper.Debug("Request", response);
                var json = JToken.Parse(response);
                if (json is JObject jobject)
                {
                    return jobject;
                }
                else if (json is JArray jArray)
                {
                    return jArray.FirstOrDefault() as JObject;
                }
                else
                {
                    throw new InvalidCastException("无法转换响应为可接收的json");
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("发送API请求", e);
                return BuildFailResponse(failCode);
            }
        }

        private static JObject BuildFailResponse(int retCode)
        {
            return new JObject
            {
                { "ret", retCode }
            };
        }

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
            return ConnectEventServer();
        }

        public int DeleteMsg(long msgId)
        {
            var msg = MessageDict.GetMessageByDictId((int)msgId);
            if (msg != null)
            {
                var info = SendRequest("message.delete", new { channel_id = msg.ParentId, message_id = msg.RawMessageId }.ToJson());
                if (info.ContainsKey("ret"))
                {
                    return 0;
                }
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public bool Disconnect()
        {
            ExitFlag = true;
            EventClient.Close();
            return IsConnected == false;
        }

        public Dictionary<string, string> GetConnectionConfig()
        {
            LoadConfig();
            return new Dictionary<string, string>
            {
                { "WebSocketURL", WsURL },
                { "Token", Token },
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
            var info = GetRawFriendList(false).FirstOrDefault(x => x.QQ == GetLoginQQ());
            if (info == null)
            {
                AppConfig.Instance.CurrentNickName = "";
            }
            else
            {
                AppConfig.Instance.CurrentNickName = info.Nick;
            }
            return AppConfig.Instance.CurrentNickName;
        }

        public long GetLoginQQ()
        {
            return AppConfig.Instance.CurrentQQ;
        }

        public List<FriendInfo> GetRawFriendList(bool reserved)
        {
            List<User>? user = SendRequest("friend.list", new { }.ToJson())?["data"]?.ToObject<List<User>?>();
            List<FriendInfo> friendInfos = new();
            if (user == null)
            {
                return friendInfos;
            }
            foreach (var item in user)
            {
                friendInfos.Add(new FriendInfo
                {
                    Nick = item.name,
                    Postscript = "",
                    QQ = long.Parse(item.id)
                });
            }
            if (reserved)
            {
                friendInfos.Reverse();
            }
            return friendInfos;
        }

        public GroupInfo GetRawGroupInfo(long groupId, bool notCache)
        {
            Guild guild = SendRequest("guild.get", new { guild_id = groupId.ToString() }.ToJson())?.ToObject<Guild>();
            if (guild == null)
            {
                return new GroupInfo();
            }
            else
            {
                return new GroupInfo
                {
                    Name = guild.name,
                    Group = long.Parse(guild.id)
                };
            }
        }

        public List<GroupInfo> GetRawGroupList()
        {
            List<Guild>? guilds = SendRequest("guild.list", new { }.ToJson())?["data"]?.ToObject<List<Guild>?>();
            List<GroupInfo> groupInfos = new();
            if (guilds == null)
            {
                return groupInfos;
            }
            foreach (var guild in guilds)
            {
                groupInfos.Add(new GroupInfo
                {
                    Name = guild.name,
                    Group = long.Parse(guild.id)
                });
            }
            return groupInfos;
        }

        public GroupMemberInfo GetRawGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            var list = GetRawGroupMemberList(groupId);
            if (list == null)
            {
                return new GroupMemberInfo();
            }
            var item = list.FirstOrDefault(x => x.QQ == qqId && x.Group == groupId);
            return item ?? new GroupMemberInfo();
        }

        public List<GroupMemberInfo> GetRawGroupMemberList(long groupId)
        {
            List<GuildMember>? guildMembers = SendRequest("guild.member.list", new { guild_id = groupId.ToString() }.ToJson())?["data"]?.ToObject<List<GuildMember>?>();
            List<GroupMemberInfo> groupMemberInfos = new();
            if (guildMembers == null)
            {
                return groupMemberInfos;
            }
            foreach (var member in guildMembers)
            {
                groupMemberInfos.Add(new GroupMemberInfo
                {
                    Card = member.nick,
                    Group = groupId,
                    Nick = member.nick,
                    JoinGroupDateTime = Helper.TimeStamp2DateTime(member.join_at),
                    QQ = 0
                });
            }
            return groupMemberInfos;
        }

        public string GetStrangerInfo(long qqId, bool notCache)
        {
            return new StrangerInfo().ToNativeBase64();
        }

        public int SendDiscussMsg(long discussId, string msg)
        {
            return 0;
        }

        public int SendGroupMessage(long groupId, string msg, int msgId = 0)
        {
            string parsedMessage = CQCodeBuilder.CQCodeParseToRaw(msg, CurrentPlatform);
            if (string.IsNullOrEmpty(parsedMessage))
            {
                return 0;
            }
            string quoteId = "";
            if (msgId != 0)
            {
                quoteId = MessageDict.GetMessageByDictId(msgId)?.RawMessageId;
            }
            if (!string.IsNullOrEmpty(quoteId))
            {
                // parsedMessage = $"<quote id={quoteId}>" + parsedMessage;
            }
            var info = SendRequest("message.create", new { channel_id = groupId.ToString(), content = parsedMessage }.ToJson());
            if (info.ContainsKey("ret"))
            {
                return 0;
            }
            return MessageDict.InsertMessage(new MessageDict
            {
                ParentId = groupId.ToString(),
                RawMessageId = info["id"].ToString()
            });
        }

        public int SendLike(long qqId, int count)
        {
            return 0;
        }

        public int SendPrivateMessage(long qqId, string msg)
        {
            string parsedMessage = CQCodeBuilder.CQCodeParseToRaw(msg, CurrentPlatform);
            if (string.IsNullOrEmpty(parsedMessage))
            {
                return 0;
            }

            var channel = SendRequest("user.channel.create", new { user_id = qqId.ToString() }.ToJson()).ToObject<Channel>();
            if (channel == null || channel.type == 0)
            {
                return 0;
            }

            var info = SendRequest("message.create", new { channel_id = channel.id.ToString(), content = parsedMessage }.ToJson());
            if (info.ContainsKey("ret"))
            {
                return 0;
            }
            return MessageDict.InsertMessage(new MessageDict
            {
                ParentId = channel.id.ToString(),
                RawMessageId = info["id"].ToString()
            });
        }

        public bool SetConnectionConfig(Dictionary<string, string> config)
        {
            bool success = config != null && config.ContainsKey("WebSocketURL") && config.ContainsKey("Token");
            success = success && !string.IsNullOrEmpty(config["WebSocketURL"]);
            if (success)
            {
                WsURL = config["WebSocketURL"];
                if (WsURL.EndsWith("/"))
                {
                    WsURL = WsURL.Substring(0, WsURL.Length - 1);
                }
                Token = config["Token"];
                SetConfig("WebSocketURL", WsURL);
                SetConfig("Token", Token);
            }
            return success;
        }

        public int SetDiscussLeave(long discussId)
        {
            return 0;
        }

        public int SetFriendAddRequest(string identifying, int responseType, string appendMsg)
        {
            var info = SendRequest("friend.approve", new { message_id = identifying, approve = responseType == 1, comment = appendMsg }.ToJson());
            if (info.ContainsKey("ret"))
            {
                return 0;
            }
            return 1;
        }

        public int SetGroupAddRequest(string identifying, int requestType, int responseType, string appendMsg)
        {
            var info = SendRequest("guild.member.approve", new { message_id = identifying, approve = responseType == 1, comment = appendMsg }.ToJson());
            if (info.ContainsKey("ret"))
            {
                return 0;
            }
            return 1;
        }

        public int SetGroupAdmin(long groupId, long qqId, bool isSet)
        {
            return 0;
            // 暂无对应文档
            List<GuildRole> roles = SendRequest("guild.role.list", new { guild_id = groupId.ToString() }.ToJson()).ToObject<List<GuildRole>>();
            string adminRoleId = "0";
            if (roles.Count > 0)
            {
                // 获取所有角色
            }

            var info = SendRequest(isSet ? "guild.member.role.set" : "guild.member.role.unset", new { guild_id = groupId.ToString(), user_id = qqId.ToString(), role_id = adminRoleId }.ToJson());
            if (info.ContainsKey("ret"))
            {
                return 0;
            }
            return 1;
        }

        public int SetGroupAnonymous(long groupId, bool isOpen)
        {
            return 0;
        }

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime)
        {
            return 0;
        }

        public int SetGroupBan(long groupId, long qqId, long banTime)
        {
            var info = SendRequest("guild.member.mute", new { channel_id = groupId.ToString(), user_id = qqId.ToString(), duration = banTime }.ToJson());
            if (info.ContainsKey("ret"))
            {
                return 0;
            }
            return 1;
        }

        public int SetGroupCard(long groupId, long qqId, string newCard)
        {
            return 0;
        }

        public int SetGroupKick(long groupId, long qqId, bool refuses)
        {
            var info = SendRequest("guild.member.kick", new { guild_id = groupId.ToString(), user_id = qqId.ToString(), permanent = refuses }.ToJson());
            if (info.ContainsKey("ret"))
            {
                return 0;
            }
            return 1;
        }

        public int SetGroupLeave(long groupId, bool isDisband)
        {
            if (CurrentPlatform == "chronocat")
            {
                var info = SendRequest("unsafe.guild.remove", new { guild_id = groupId.ToString() }.ToJson());
                if (info.ContainsKey("ret"))
                {
                    return 0;
                }
                return 1;
            }
            return 0;
        }

        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime)
        {
            return 0;
        }

        public int SetGroupWholeBan(long groupId, bool isOpen)
        {
            if (CurrentPlatform == "chronocat")
            {
                var info = SendRequest("unsafe.channel.mute", new { channel_id = groupId.ToString(), enable = !isOpen }.ToJson());
                if (info.ContainsKey("ret"))
                {
                    return 0;
                }
                return 1;
            }
            return 0;
        }
    }
}
