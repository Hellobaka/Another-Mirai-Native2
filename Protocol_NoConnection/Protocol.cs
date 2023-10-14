using Another_Mirai_Native;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using System;
using System.Collections.Generic;

namespace Protocol_NoConnection
{
    public class Protocol : IProtocol
    {
        public string Name { get; set; } = "NoConnection";

        public bool IsConnected { get; set; } = false;

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
            return true;
        }

        public int DeleteMsg(long msgId)
        {
            return 1;
        }

        public bool Disconnect()
        {
            IsConnected = false;
            return true;
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
            return FriendInfo.CollectionToList(new List<FriendInfo>()
            {
                new FriendInfo
                {
                    Nick = "琪露诺",
                    Postscript = "Baka",
                    QQ = 1145141919
                }
            });
        }

        public string GetGroupInfo(long groupId, bool notCache)
        {
            return new GroupInfo
            {
                CurrentMemberCount = 15,
                Group = 1919810,
                MaxMemberCount = 30,
                Name = "幻♂想乡"
            }.ToNativeBase64(false);
        }

        public string GetGroupList()
        {
            return GroupInfo.CollectionToList(new List<GroupInfo>
            {
                new GroupInfo
                {
                    CurrentMemberCount = 3,
                    Group = 1919810,
                    MaxMemberCount = 30,
                    Name = "幻♂想乡"
                }
            });
        }

        public string GetGroupMemberInfo(long groupId, long qqId, bool isCache)
        {
            return new GroupMemberInfo()
            {
                Age = 18,
                Area = "霓虹",
                Card = ""
            }.ToNativeBase64();
        }

        public string GetGroupMemberList(long groupId)
        {
            throw new NotImplementedException();
        }

        public string GetLoginNick()
        {
            return ConfigHelper.GetConfig("NoConnection_Nick", @"conf\NoConnection_ProtocolConfig.json", "测试账号9");
        }

        public long GetLoginQQ()
        {
            return ConfigHelper.GetConfig("NoConnection_QQ", @"conf\NoConnection_ProtocolConfig.json", (long)999999999);
        }

        public string GetStrangerInfo(long qqId, bool notCache)
        {
            throw new NotImplementedException();
        }

        public int SendDiscussMsg(long discussId, string msg)
        {
            throw new NotImplementedException();
        }

        public int SendGroupMessage(long groupId, string msg, int msgId = 0)
        {
            throw new NotImplementedException();
        }

        public int SendLike(long qqId, int count)
        {
            throw new NotImplementedException();
        }

        public int SendPrivateMessage(long qqId, string msg)
        {
            throw new NotImplementedException();
        }

        public int SetDiscussLeave(long discussId)
        {
            throw new NotImplementedException();
        }

        public int SetFriendAddRequest(long identifying, int requestType, string appendMsg)
        {
            throw new NotImplementedException();
        }

        public int SetGroupAddRequest(long identifying, int requestType, int responseType, string appendMsg)
        {
            throw new NotImplementedException();
        }

        public int SetGroupAdmin(long groupId, long qqId, bool isSet)
        {
            throw new NotImplementedException();
        }

        public int SetGroupAnonymous(long groupId, bool isOpen)
        {
            throw new NotImplementedException();
        }

        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime)
        {
            throw new NotImplementedException();
        }

        public int SetGroupBan(long groupId, long qqId, long time)
        {
            throw new NotImplementedException();
        }

        public int SetGroupCard(long groupId, long qqId, string newCard)
        {
            throw new NotImplementedException();
        }

        public int SetGroupKick(long groupId, long qqId, bool refuses)
        {
            throw new NotImplementedException();
        }

        public int SetGroupLeave(long groupId, bool isDisband)
        {
            throw new NotImplementedException();
        }

        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime)
        {
            throw new NotImplementedException();
        }

        public int SetGroupWholeBan(long groupId, bool isOpen)
        {
            throw new NotImplementedException();
        }
    }
}