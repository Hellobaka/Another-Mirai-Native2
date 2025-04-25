using Another_Mirai_Native;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Protocol_NoConnection
{
    public class Protocol : ConfigBase, IProtocol
    {
        private object msgIdLock = new ();

        public Protocol()
            : base(@"conf\NoConnection_ProtocolConfig.json")
        {
            LoadConfig();
            Instance = this;
        }

        public static Protocol Instance { get; set; }

        public bool IsConnected { get; set; } = false;

        public string Name { get; set; } = "NoConnection";

        public bool ShowTestDialog { get; set; }

        public bool BuildTestPicServer { get; set; }

        public string PicServerListenIP { get; set; } = "127.0.0.1";

        public ushort PicServerListenPort { get; set; } = 45000;

        public long TestLoginQQ { get; set; } = 100000;

        public string TestNickName { get; set; } = "";

        public PicServer PicServer { get; set; }

        private List<FriendInfo> FriendInfos { get; set; } = new();

        private List<GroupInfo> GroupInfos { get; set; } = new();

        private List<GroupMemberInfo> GroupMemberInfos { get; set; } = new();

        private Tester TesterForm { get; set; }

        private int MsgId { get; set; } = 10;

        public void BuildMockData()
        {
            FriendInfos.Clear();
            GroupInfos.Clear();
            GroupMemberInfos.Clear();
            FriendInfos.Add(new FriendInfo
            {
                Nick = "琪露诺",
                Postscript = "Baka",
                QQ = 1145141919
            });
            for (int i = 0; i < Helper.RandomNext(2, 30); i++)
            {
                FriendInfos.Add(new FriendInfo
                {
                    Nick = $"Nick{i + 1}",
                    Postscript = $"Remark{i + 1}",
                    QQ = Helper.RandomNext(20000, int.MaxValue)
                });
            }
            GroupInfos.Add(new GroupInfo
            {
                CurrentMemberCount = 1,
                Group = 1919810,
                MaxMemberCount = 30,
                Name = "幻♂想乡"
            });
            GroupMemberInfos.Add(new GroupMemberInfo
            {
                Age = 18,
                Area = "霓虹",
                Card = "头脑很好",
                ExclusiveTitle = "BA☆KA",
                ExclusiveTitleExpirationTime = null,
                Group = 1919810,
                IsAllowEditorCard = true,
                IsBadRecord = false,
                JoinGroupDateTime = new DateTime(2018, 9, 9),
                LastSpeakDateTime = DateTime.Now - new TimeSpan(Helper.RandomNext(0, 24), Helper.RandomNext(0, 60), Helper.RandomNext(0, 60)),
                Level = "九十九",
                MemberType = QQGroupMemberType.Manage,
                Nick = "琪露诺",
                QQ = 1145141919,
                Sex = QQSex.Woman
            });
            GroupMemberInfos.Add(BuildSelfMockData(1919810));
            for (int i = 0; i < Helper.RandomNext(2, 25); i++)
            {
                GroupInfos.Add(new GroupInfo
                {
                    CurrentMemberCount = Helper.RandomNext(5, 100),
                    Group = Helper.RandomNext(20000, int.MaxValue),
                    Name = $"Group{i + 1}"
                });
                GroupInfos.Last().MaxMemberCount = GroupInfos.Last().CurrentMemberCount * 2;
                for (int j = 0; j < GroupInfos.Last().CurrentMemberCount; j++)
                {
                    GroupMemberInfos.Add(new GroupMemberInfo
                    {
                        Age = Helper.RandomNext(0, 99),
                        Area = $"Area{Helper.RandomNext()}",
                        Card = $"Card{Helper.RandomNext()}",
                        ExclusiveTitle = $"ExclusiveTitle{Helper.RandomNext()}",
                        ExclusiveTitleExpirationTime = null,
                        Group = GroupInfos.Last().Group,
                        IsAllowEditorCard = true,
                        IsBadRecord = Helper.RandomNextDouble() > 0.5,
                        JoinGroupDateTime = new DateTime(Helper.RandomNext(2000, 2023), Helper.RandomNext(1, 12), Helper.RandomNext(1, 25)),
                        LastSpeakDateTime = DateTime.Now - new TimeSpan(Helper.RandomNext(0, 24), Helper.RandomNext(0, 60), Helper.RandomNext(0, 60)),
                        Level = $"Level{Helper.RandomNext()}",
                        MemberType = QQGroupMemberType.Member,
                        Nick = $"Nick{Helper.RandomNext()}",
                        QQ = Helper.RandomNext(20000, int.MaxValue),
                        Sex = Helper.RandomNextDouble() > 0.3 ? QQSex.Man : Helper.RandomNextDouble() > 0.1 ? QQSex.Woman : QQSex.Unknown
                    });
                }
                GroupMemberInfos.Add(BuildSelfMockData(GroupInfos.Last().Group));
                GroupMemberInfos.Where(x => x.Group == GroupInfos.Last().Group).First().MemberType = QQGroupMemberType.Creator;
                GroupMemberInfos.Where(x => x.Group == GroupInfos.Last().Group).Skip(1).First().MemberType = QQGroupMemberType.Manage;
            }
            if (ShowTestDialog)
            {
                if (TesterForm == null)
                {
                    Thread uiThread = new(() =>
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        TesterForm = new Tester();
                        TesterForm.Protocol = this;
                        Application.Run(TesterForm);
                    });
                    uiThread.SetApartmentState(ApartmentState.STA);
                    uiThread.Start();
                }
            }
            if (BuildTestPicServer)
            {
                PicServer = new PicServer(@"data\image", PicServerListenIP, PicServerListenPort);
                PicServer.Start();
            }
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
            IsConnected = true;
            BuildMockData();
            return true;
        }

        public int DeleteMsg(long msgId)
        {
            return 1;
        }

        public bool Disconnect()
        {
            IsConnected = false;
            PicServer?.Stop();
            TesterForm?.Invoke(() =>
            {
                TesterForm.Close();
                TesterForm.Dispose();
            });
            TesterForm = null;
            return true;
        }

        public Dictionary<string, string> GetConnectionConfig()
        {
            return new();
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
            return GetRawGroupInfo(groupId, notCache)?.ToNativeBase64(false) ?? "";
        }

        public string GetGroupList()
        {
            return GroupInfo.CollectionToList(GetRawGroupList());
        }

        public string GetGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            return GetRawGroupMemberInfo(groupId, qqId, isCache)?.ToNativeBase64() ?? "";
        }

        public string GetGroupMemberList(long groupId)
        {
            return GroupMemberInfo.CollectionToList(GetRawGroupMemberList(groupId));
        }

        public string GetLoginNick()
        {
            return TestNickName;
        }

        public long GetLoginQQ()
        {
            return TestLoginQQ;
        }

        public List<FriendInfo> GetRawFriendList(bool reserved)
        {
            return reserved ? FriendInfos.OrderBy(x => x.QQ).ToList()
                : FriendInfos.OrderByDescending(x => x.QQ).ToList();
        }

        public GroupInfo GetRawGroupInfo(long groupId, bool notCache)
        {
            return GroupInfos.FirstOrDefault(x => x.Group == groupId);
        }

        public List<GroupInfo> GetRawGroupList()
        {
            return GroupInfos;
        }

        public GroupMemberInfo GetRawGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            return GroupMemberInfos.FirstOrDefault(x => x.Group == groupId && x.QQ == qqId);
        }

        public List<GroupMemberInfo> GetRawGroupMemberList(long groupId)
        {
            return GroupMemberInfos.Any(x => x.Group == groupId) ? GroupMemberInfos.Where(x => x.Group == groupId).ToList()
                 : new();
        }

        public string GetStrangerInfo(long qqId, bool notCache)
        {
            return new StrangerInfo
            {
                Age = Helper.RandomNext(0, 99),
                Nick = $"Stranger{Helper.RandomNext()}",
                QQ = qqId,
                Sex = Helper.RandomNextDouble() > 0.5 ? QQSex.Man : QQSex.Woman
            }.ToNativeBase64();
        }

        public void LoadConfig()
        {
            TestNickName = GetConfig("NoConnection_Nick", "测试账号9");
            TestLoginQQ = GetConfig("NoConnection_QQ", (long)999999999);
            ShowTestDialog = GetConfig("ShowTestDialog", true);
            BuildTestPicServer = GetConfig("BuildTestPicServer", false);
            PicServerListenIP = GetConfig("PicServerListenIP", "127.0.0.1");
            PicServerListenPort = GetConfig("PicServerListenPort", (ushort)45000);
        }

        public int SendDiscussMsg(long discussId, string msg)
        {
            lock (msgIdLock)
            {
                return MsgId++;
            }
        }

        public int SendGroupMessage(long groupId, string msg, int msgId = 0)
        {
            lock (msgIdLock)
            {
                return MsgId++;
            }
        }

        public int SendLike(long qqId, int count)
        {
            return 0;
        }

        public int SendPrivateMessage(long qqId, string msg)
        {
            lock (msgIdLock)
            {
                return MsgId++;
            }
        }

        public bool SetConnectionConfig(Dictionary<string, string> config)
        {
            return true;
        }

        public int SetDiscussLeave(long discussId)
        {
            return 0;
        }

        public int SetFriendAddRequest(string identifying, int requestType, string appendMsg)
        {
            return 0;
        }

        public int SetGroupAddRequest(string identifying, int requestType, int responseType, string appendMsg)
        {
            return 0;
        }

        public int SetGroupAdmin(long groupId, long qqId, bool isSet)
        {
            return 0;
        }

        public int SetGroupAnonymous(long groupId, bool isOpen)
        {
            return 0;
        }

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime)
        {
            return 0;
        }

        public int SetGroupBan(long groupId, long qqId, long time)
        {
            return 0;
        }

        public int SetGroupCard(long groupId, long qqId, string newCard)
        {
            return 0;
        }

        public int SetGroupKick(long groupId, long qqId, bool refuses)
        {
            return 0;
        }

        public int SetGroupLeave(long groupId, bool isDisband)
        {
            return 0;
        }

        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime)
        {
            return 0;
        }

        public int SetGroupWholeBan(long groupId, bool isOpen)
        {
            return 0;
        }

        private GroupMemberInfo BuildSelfMockData(long groupId)
        {
            return new GroupMemberInfo
            {
                Age = Helper.RandomNext(0, 99),
                Area = $"Area{Helper.RandomNext()}",
                Card = $"Card{Helper.RandomNext()}",
                ExclusiveTitle = $"ExclusiveTitle{Helper.RandomNext()}",
                ExclusiveTitleExpirationTime = null,
                Group = groupId,
                IsAllowEditorCard = true,
                IsBadRecord = Helper.RandomNextDouble() > 0.5,
                JoinGroupDateTime = new DateTime(Helper.RandomNext(2000, 2023), Helper.RandomNext(1, 12), Helper.RandomNext(1, 25)),
                LastSpeakDateTime = DateTime.Now - new TimeSpan(Helper.RandomNext(0, 24), Helper.RandomNext(0, 60), Helper.RandomNext(0, 60)),
                Level = $"Level{Helper.RandomNext()}",
                MemberType = QQGroupMemberType.Member,
                Nick = $"Nick{Helper.RandomNext()}",
                QQ = GetLoginQQ(),
                Sex = QQSex.Man
            };
        }

        public event Action<string, byte[]> QRCodeDisplayAction;

        public event Action QRCodeFinishedAction;

        public event Action OnProtocolOnline;

        public event Action OnProtocolOffline;

        public void ShowQRCode(string path, byte[] buffer)
        {
            QRCodeDisplayAction?.Invoke(path, buffer);
        }

        public void HideQRCode()
        {
            QRCodeFinishedAction?.Invoke();
        }
        public void SetProtocolOnline()
        {
            OnProtocolOnline?.Invoke();
        }
        public void SetProtocolOffline()
        {
            OnProtocolOffline?.Invoke();
        }
    }
}