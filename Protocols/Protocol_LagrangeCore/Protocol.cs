using Another_Mirai_Native;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    public partial class LagrangeCoreAPI : IProtocol
    {
        public string Name { get; set; } = $"Lagrange.Core";

        public bool IsConnected { get; set; }

        public bool ForceQRLogin { get; set; } = false;

        public bool ClearDeviceInfo { get; set; } = false;

        public event Action<string, byte[]> QRCodeDisplayAction;

        public event Action QRCodeFinishedAction;

        public event Action OnProtocolOnline;

        public event Action OnProtocolOffline;

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
            _ = new LagrangeConfig();
            if (!InitBotContext())
            {
                LogHelper.Error("登录", "Bot 实例创建失败");
                return false;
            }
            return Login();
        }

        public int DeleteMsg(long msgId)
        {
            MessageChain? chain = MessageCacher.GetMessageById((int)msgId);
            if (chain == null)
            {
                return 0;
            }
            bool result;
            if (chain.GroupUin.HasValue)
            {
                result = BotContext.RecallGroupMessage(chain).Result;
            }
            else
            {
                result = BotContext.RecallFriendMessage(chain).Result;
            }
            return result ? 1 : 0;
        }

        public bool Disconnect()
        {
            return DisposeBotContext();
        }

        public Dictionary<string, string> GetConnectionConfig()
        {
            ForceQRLogin = false;
            return new Dictionary<string, string> 
            {
                { "bool_强制扫码登录", "false" },
                { "bool_重建设备信息", "false" },
            };
        }

        public string GetCookies(string domain)
        {
            return BotContext.FetchCookies([domain]).Result.FirstOrDefault() ?? "";
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
            return BotContext.BotName ?? "";
        }

        public long GetLoginQQ()
        {
            return BotContext.BotUin;
        }

        public List<FriendInfo> GetRawFriendList(bool reserved)
        {
            List<FriendInfo> friendInfos = [];
            var list = BotContext.FetchFriends().Result;
            if (reserved)
            {
                list.Reverse();
            }
            foreach (var item in list)
            {
                friendInfos.Add(new FriendInfo
                {
                    QQ = item.Uin,
                    Nick = item.Nickname,
                    Postscript = item.Remarks
                });
            }
            return friendInfos;
        }

        public GroupInfo GetRawGroupInfo(long groupId, bool notCache)
        {
            var info = BotContext.FetchGroupInfo((ulong)groupId).Result;

            return new()
            {
                CurrentMemberCount = (int)info.info.MemberCount,
                Group = groupId,
                MaxMemberCount = (int)info.info.MaxMemberCount,
                Name = info.info.Name
            };
        }

        public List<GroupInfo> GetRawGroupList()
        {
            List<GroupInfo> infos = [];
            foreach (var item in BotContext.FetchGroups().Result)
            {
                infos.Add(new()
                {
                    CurrentMemberCount = (int)item.MemberCount,
                    Group = item.GroupUin,
                    MaxMemberCount = (int)item.MaxMember,
                    Name = item.GroupName
                });
            }
            return infos;
        }

        public GroupMemberInfo GetRawGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            var list = BotContext.FetchMembers((uint)groupId, isCache).Result;
            var info = list.FirstOrDefault(x => x.Uin == qqId);
            if (info == null)
            {
                return new();
            }
            return new()
            {
                Age = 0,
                Area = "",
                Card = info.MemberCard ?? "",
                ExclusiveTitle = info.SpecialTitle ?? "",
                ExclusiveTitleExpirationTime = null,
                Group = groupId,
                IsAllowEditorCard = true,
                IsBadRecord = false,
                JoinGroupDateTime = info.JoinTime,
                LastSpeakDateTime = info.LastMsgTime,
                Level = info.GroupLevel.ToString(),
                Nick = info.MemberName,
                QQ = info.Uin,
                Sex = QQSex.Unknown,
                MemberType = info.Permission switch
                {
                    GroupMemberPermission.Admin => QQGroupMemberType.Manage,
                    GroupMemberPermission.Owner => QQGroupMemberType.Creator,
                    _ => QQGroupMemberType.Member
                }
            };
        }

        public List<GroupMemberInfo> GetRawGroupMemberList(long groupId)
        {
            List<GroupMemberInfo> result = [];
            foreach (var item in BotContext.FetchMembers((uint)groupId).Result)
            {
                result.Add(new()
                {
                    Age = 0,
                    Area = "",
                    Card = item.MemberCard ?? "",
                    ExclusiveTitle = item.SpecialTitle ?? "",
                    ExclusiveTitleExpirationTime = null,
                    Group = groupId,
                    IsAllowEditorCard = true,
                    IsBadRecord = false,
                    JoinGroupDateTime = item.JoinTime,
                    LastSpeakDateTime = item.LastMsgTime,
                    Level = item.GroupLevel.ToString(),
                    Nick = item.MemberName,
                    QQ = item.Uin,
                    Sex = QQSex.Unknown,
                    MemberType = item.Permission switch
                    {
                        GroupMemberPermission.Admin => QQGroupMemberType.Manage,
                        GroupMemberPermission.Owner => QQGroupMemberType.Creator,
                        _ => QQGroupMemberType.Member
                    }
                });
            }
            return result;
        }

        public string GetStrangerInfo(long qqId, bool notCache)
        {
            var info = BotContext.FetchUserInfo((uint)qqId, !notCache).Result;
            if (info == null)
            {
                return new StrangerInfo().ToNativeBase64();
            }
            StrangerInfo strangerInfo = new()
            {
                Age = (int)info.Age,
                Nick = info.Nickname,
                QQ = info.Uin,
                Sex = info.Gender switch
                {
                    BotUserInfo.GenderInfo.Male => QQSex.Man,
                    BotUserInfo.GenderInfo.Female => QQSex.Woman,
                    _ => QQSex.Unknown
                }
            };
            return strangerInfo.ToNativeBase64();
        }

        public int SendDiscussMsg(long discussId, string msg)
        {
            return 0;
        }

        public int SendGroupMessage(long groupId, string msg, int msgId = 0)
        {
            if (msgId != 0)
            {
                msg = $"[CQ:reply,id={msgId}]" + msg;
            }
            MessageBuilder builder = MessageBuilder.Group((uint)groupId);
            MessageChainParser.ParseCQCodeToMessageChain(builder, msg);
            var chain = builder.Build();
            var result = BotContext.SendMessage(chain).Result;
            if (result.Result != 0 || result.Sequence == null || result.Sequence == 0)
            {
                return 0;
            }
            return MessageCacher.RecordMessage(chain, result.MessageId, result.Sequence.Value);
        }

        public int SendLike(long qqId, int count)
        {
            var r = BotContext.Like((uint)qqId, (uint)count).Result;
            return r ? 0 : 1;
        }

        public int SendPrivateMessage(long qqId, string msg)
        {
            MessageBuilder builder = MessageBuilder.Friend((uint)qqId);
            MessageChainParser.ParseCQCodeToMessageChain(builder, msg);
            var chain = builder.Build();
            var result = BotContext.SendMessage(chain).Result;
            if (result.Result != 0 || result.Sequence == null || result.Sequence == 0)
            {
                return 0;
            }
            return MessageCacher.RecordMessage(chain, result.MessageId, result.Sequence.Value);
        }

        public bool SetConnectionConfig(Dictionary<string, string> config)
        {
            ForceQRLogin = config["bool_强制扫码登录"] == "true";
            ClearDeviceInfo = config["bool_重建设备信息"] == "true";
            return true;
        }

        public int SetDiscussLeave(long discussId)
        {
            return 0;
        }

        public int SetFriendAddRequest(string identifying, int responseType, string appendMsg)
        {
            var requests = BotContext.FetchFriendRequests().Result;
            if (requests == null)
            {
                return 1;
            }
            var request = requests.FirstOrDefault(x => x.SourceUin.ToString() == identifying);
            if (request == null)
            {
                return 1;
            }
            return BotContext.SetFriendRequest(request, responseType == 1).Result ? 0 : 1;
        }

        public int SetGroupAddRequest(string identifying, int requestType, int responseType, string appendMsg)
        {
            var requests = BotContext.FetchGroupRequests().Result;
            if (requests == null)
            {
                return 1;
            }
            var request = requests.FirstOrDefault(x => $"{x.GroupUin}_{x.TargetMemberUin}" == identifying);
            if (request == null)
            {
                return 1;
            }
            return BotContext.SetGroupRequest(request, responseType == 1).Result ? 0 : 1;
        }

        public int SetGroupAdmin(long groupId, long qqId, bool isSet)
        {
            return BotContext.SetGroupAdmin((uint)groupId, (uint)qqId, isSet).Result ? 0 : 1;
        }

        public int SetGroupAnonymous(long groupId, bool isOpen)
        {
            return 1;
        }

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime)
        {
            return 1;
        }

        public int SetGroupBan(long groupId, long qqId, long banTime)
        {
            return BotContext.MuteGroupMember((uint)groupId, (uint)qqId, (uint)banTime).Result ? 0 : 1;
        }

        public int SetGroupCard(long groupId, long qqId, string newCard)
        {
            return BotContext.RenameGroupMember(BotContext.BotUin, (uint)qqId, newCard).Result ? 0 : 1;
        }

        public int SetGroupKick(long groupId, long qqId, bool refuses)
        {
            return BotContext.KickGroupMember((uint)groupId, (uint)qqId, refuses).Result ? 0 : 1;
        }

        public int SetGroupLeave(long groupId, bool isDisband)
        {
            return BotContext.LeaveGroup((uint)groupId).Result ? 0 : 1;
        }

        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime)
        {
            return BotContext.GroupSetSpecialTitle((uint)groupId, (uint)qqId, title).Result ? 0 : 1;
        }

        public int SetGroupWholeBan(long groupId, bool isOpen)
        {
            return BotContext.MuteGroupGlobal((uint)groupId, isOpen).Result ? 0 : 1;
        }
    }
}