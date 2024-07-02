using Another_Mirai_Native.Model.Enums.Other.XiaoLiZi;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Model.Other.XiaoLiZi
{
    public class AppEnableEvent
    {
        public EventProcessEnum eventProcessEnum;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ServiceInfo
    {
        public ServiceInformation ServiceList;

        public int ServiceLevel;
    }

    /// <summary>
    /// 滑块识别
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SliderVerificationEvent
    {
        /// <summary>
        /// 来源QQ
        /// </summary>
        public long sourceQQ;

        /// <summary>
        /// url
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string url;
    }

    /// <summary>
    /// 取短信验证码
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SMSVerificationEvent
    {
        /// <summary>
        /// 来源QQ
        /// </summary>
        public long sourceQQ;

        /// <summary>
        /// 手机号
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string phone;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PrivateMessageEvent
    {
        /// <summary>
        /// 发送人QQ
        /// </summary>
        public long SenderQQ;

        /// <summary>
        /// 框架QQ
        /// </summary>
        public long ThisQQ;

        /// <summary>
        /// 消息Req
        /// </summary>
        public uint MessageReq;

        /// <summary>
        /// 消息Seq
        /// </summary>
        public long MessageSeq;

        /// <summary>
        /// 消息接收时间
        /// </summary>
        public uint MessageReceiveTime;

        /// <summary>
        /// 消息群号 当为群临时会话时可取
        /// </summary>
        public long MessageGroupQQ;

        /// <summary>
        /// 消息发送时间
        /// </summary>
        public uint MessageSendTime;

        /// <summary>
        /// 消息Random
        /// </summary>
        public long MessageRandom;

        /// <summary>
        /// 消息分片序列
        /// </summary>
        public uint MessageClip;

        /// <summary>
        /// 消息分片数量
        /// </summary>
        public uint MessageClipCount;

        /// <summary>
        /// 消息分片标识
        /// </summary>
        public long MessageClipID;

        /// <summary>
        /// 消息内容
        /// </summary>
        //public string MessageContent;
        [MarshalAs(UnmanagedType.LPStr)]
        public string MessageContent;

        /// <summary>
        /// 气泡Id
        /// </summary>
        public uint BubbleID;

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageTypeEnum MessageType;

        /// <summary>
        /// 消息子类型
        /// </summary>
        public MessageSubTypeEnum MessageSubType;

        /// <summary>
        /// 消息子临时类型 0 群 1 讨论组 129 腾讯公众号 201 QQ咨询
        /// </summary>
        public MessageSubTypeEnum MessageSubTemporaryType;

        /// <summary>
        /// 红包类型
        /// </summary>
        public uint RedEnvelopeType;

        /// <summary>
        /// 会话token
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string SessionToken;

        /// <summary>
        /// 来源事件QQ
        /// </summary>
        public long SourceEventQQ;

        /// <summary>
        /// 来源事件QQ昵称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string SourceEventQQName;

        /// <summary>
        /// 文件Id
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string FileID;

        /// <summary>
        /// 文件Md5
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string FileMD5;

        /// <summary>
        /// 文件名
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string FileName;

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MessageEvent
    {
        /// <summary>
        /// 框架QQ
        /// </summary>
        public long ThisQQ;

        /// <summary>
        /// 消息群号
        /// </summary>
        public long SourceMessageGroupQQ;

        /// <summary>
        /// 操作QQ
        /// </summary>
        public long OperationEventQQ;

        /// <summary>
        /// 触发QQ
        /// </summary>
        public long SourceEventQQ;

        /// <summary>
        /// 消息Seq
        /// </summary>
        public long MessageSeq;

        /// <summary>
        /// 消息时间戳
        /// </summary>
        public uint MessageSendTime;

        /// <summary>
        /// 来源群名
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string SourceGroupName;

        /// <summary>
        /// 操作QQ昵称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string OperationQQName;

        /// <summary>
        /// 触发QQ昵称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string SourceQQName;

        /// <summary>
        /// 消息内容
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string MessageContent;

        /// <summary>
        /// 消息类型
        /// </summary>
        public EventTypeEnum MessageType;

        /// <summary>
        /// 消息类型
        /// </summary>
        public uint MessageSubType;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EventTypeBase
    {
        /// <summary>
        /// 框架QQ
        /// </summary>
        public long ThisQQ;

        /// <summary>
        /// 来源群号
        /// </summary>
        public long SourceGroupQQ;

        public long OperateQQ;

        /// <summary>
        /// 触发QQ
        /// </summary>
        public long TriggerQQ;

        /// <summary>
        /// 消息Seq
        /// </summary>
        public long MessageSeq;

        // 消息时间戳
        /// <summary>
        /// 消息时间戳
        /// </summary>
        public int MessageTimestamp;

        /// <summary>
        /// 来源群名
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string SourceGroupName;

        /// <summary>
        /// 操作QQ昵称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string OperateQQName;

        /// <summary>
        /// 触发QQ昵称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string TriggerQQName;

        /// <summary>
        /// 事件内容
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string MessageContent;

        /// <summary>
        /// 事件类型
        /// </summary>
        public EventTypeEnum EventType;

        /// <summary>
        /// 事件子类型
        /// </summary>
        public int EventSubType;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GroupMessageEvent
    {
        /// <summary>
        /// 发送人QQ
        /// </summary>
        public long SenderQQ;

        /// <summary>
        /// 框架QQ
        /// </summary>
        public long ThisQQ;

        /// <summary>
        /// 消息Req
        /// </summary>
        public int MessageReq;

        /// <summary>
        /// 消息接收时间
        /// </summary>
        public int MessageReceiveTime;

        /// <summary>
        /// 消息群号
        /// </summary>
        public long MessageGroupQQ;

        /// <summary>
        /// 消息来源群名（貌似失效了）etext SourceGroupName = nullptr;
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string SourceGroupName;

        /// <summary>
        /// 发送人群名片 没有名片则为空白
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string SenderNickname;

        /// <summary>
        /// 消息发送时间
        /// </summary>
        public int MessageSendTime;

        /// <summary>
        /// 消息Random
        /// </summary>
        public long MessageRandom;

        /// <summary>
        /// 消息分片序列
        /// </summary>
        public int MessageClip;

        /// <summary>
        /// 消息分片数量
        /// </summary>
        public int MessageClipCount;

        /// <summary>
        /// 消息分片标识
        /// </summary>
        public long MessageClipID;

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageTypeEnum MessageType;

        /// <summary>
        /// 发送人群头衔
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string SenderTitle;

        /// <summary>
        /// 消息内容
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string MessageContent;

        /// <summary>
        /// 回复对象消息内容 如果是回复消息
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string ReplyMessageContent;

        /// <summary>
        /// 发送者气泡ID
        /// </summary>
        public int BubbleID;

        /// <summary>
        /// 群聊等级
        /// </summary>
        public int GroupChatLevel;

        /// <summary>
        /// 挂件Id
        /// </summary>
        public int PendantID;

        /// <summary>
        /// 匿名昵称：消息是匿名消息时,此为对方的匿名昵称,否则为空
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string AnonymousNickname;

        /// <summary>
        /// 匿名标识：可用于禁言等<para>此字段需要开发者自行调用API处理返回byte[]<see cref=""/></para>
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray,SizeConst =1024*100)]
        public nint AnonymousFalg;

        /// <summary>
        /// 保留参数
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string ReservedParameters;

        /// <summary>
        /// 框架QQ匿名Id：用于判断框架开启匿名时,收到的消息是否为自身的消息
        /// </summary>
        public long AnonymousId;

        /// <summary>
        /// 字体Id
        /// </summary>
        public int FontId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FriendDataList
    {
        public int index;//数组索引

        public int Amount;//数组元素数量

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 * 10)]//5000人群 5000/4+8 =1258
        public byte[] pAddrList;//每个元素的指针
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GetFriendDataList
    {
        //public int index;
        public FriendInfo friendInfo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FriendInfo
    {
        // 邮箱
        /// <summary>
        /// 邮箱
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Email;

        // 账号
        /// <summary>
        /// 账号
        /// </summary>
        public long QQNumber;

        // 昵称
        /// <summary>
        /// 昵称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;

        // 备注
        /// <summary>
        /// 备注
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Note;

        // 在线状态 只能使用[取好友列表]获取
        /// <summary>
        /// 在线状态 只能使用[取好友列表]获取
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Status;

        // 赞数量 只能使用[查询好友信息]获取
        /// <summary>
        /// 赞数量 只能使用[查询好友信息]获取
        /// </summary>
        public uint Likes;

        // 签名 只能使用[查询好友信息]获取
        /// <summary>
        /// 签名 只能使用[查询好友信息]获取
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Signature;

        // 性别 255: 隐藏, 0: 男, 1: 女
        /// <summary>
        /// 性别 255: 隐藏, 0: 男, 1: 女
        /// </summary>
        public uint Gender;

        // Q等级 只能使用[查询好友信息]获取
        /// <summary>
        /// Q等级 只能使用[查询好友信息]获取
        /// </summary>
        public uint Level;

        // 年龄 只能使用[查询好友信息]获取
        /// <summary>
        /// 年龄 只能使用[查询好友信息]获取
        /// </summary>
        public uint Age;

        // 国家 只能使用[查询好友信息]获取
        /// <summary>
        /// 国家 只能使用[查询好友信息]获取
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Nation;

        // 省份 只能使用[查询好友信息]获取
        /// <summary>
        /// 省份 只能使用[查询好友信息]获取
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Province;

        // 城市 只能使用[查询好友信息]获取
        /// <summary>
        /// 城市 只能使用[查询好友信息]获取
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string City;

        // 服务列表 只能使用[查询好友信息]获取
        /// <summary>
        /// 服务列表 只能使用[查询好友信息]获取
        /// </summary>
        public ServiceInfo serviceInfo;

        // 连续在线天数 只能使用[查询好友信息]获取
        /// <summary>
        /// 连续在线天数 只能使用[查询好友信息]获取
        /// </summary>
        public uint ContinuousOnlineTime;

        // QQ达人 只能使用[查询好友信息]获取
        /// <summary>
        /// QQ达人 只能使用[查询好友信息]获取
        /// <para>2.7.1RC9 SDK前 是QQ达人</para>
        /// <para>2.7.1RC9 SDK后含2.7.1RC9 是所属分组名</para>
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string QQTalent;

        // 今日已赞 只能使用[查询好友信息]获取
        /// <summary>
        /// 今日已赞 只能使用[查询好友信息]获取
        /// </summary>
        public uint LikesToday;

        // 今日可赞数 只能使用[查询好友信息]获取
        /// <summary>
        /// 今日可赞数 只能使用[查询好友信息]获取
        /// </summary>
        public uint LikesAvailableToday;
    }/// <summary>

     /// 群卡片信息
     /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GroupCardInfo
    {
        // 群名称
        /// <summary>
        /// 群名称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string GroupName;

        // 群地点
        /// <summary>
        /// 群地点
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string GroupLocation;

        // 群分类
        /// <summary>
        /// 群分类
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string GroupClassification;

        // 群标签 以|分割
        /// <summary>
        /// 群标签 以|分割
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string GroupTags;

        // 群介绍
        /// <summary>
        /// 群介绍
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string GroupDescription;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GroupCardInfoDatList
    {
        public GroupCardInfo groupCardInfo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GroupDataList
    {
        public int index;//数组索引

        public int Amount;//数组元素数量

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 * 10)]//5000人群 5000/4+8 =1258
        public byte[] pAddrList;//每个元素的指针
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GetGroupData
    {
        public GroupInfo groupInfo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GroupInfo
    {
        // 群ID
        /// <summary>
        /// 群ID
        /// </summary>
        public long GroupID;

        // 群号
        /// <summary>
        /// 群号
        /// </summary>
        public long GroupQQ;

        // cFlag
        public long CFlag;

        // dwGroupInfoSeq
        public long GroupInfoSeq;

        // dwGroupFlagExt
        public long GroupFlagExt;

        // dwGroupRankSeq
        /// <summary>
        /// 群等级查询Seq
        /// </summary>
        public long GroupRankSeq;

        // dwCertificationType
        public long CertificationType;

        // 禁言时间戳
        /// <summary>
        /// 全员禁言解除时间戳
        /// </summary>
        public long ShutUpTimestamp;

        // 解除禁言时间戳
        /// <summary>
        /// 解除禁言时间戳
        /// </summary>
        public long ThisShutUpTimestamp;

        // dwCmdUinUinFlag
        public long CmdUinUinFlag;

        // dwAdditionalFlag
        public long AdditionalFlag;

        // dwGroupTypeFlag
        public long GroupTypeFlag;

        // dwGroupSecType
        public long GroupSecType;

        // dwGroupSecTypeInfo
        public long GroupSecTypeInfo;

        // dwGroupClassExt
        public long GroupClassExt;

        // dwAppPrivilegeFlag
        public long AppPrivilegeFlag;

        // dwSubscriptionUin
        public long SubscriptionUin;

        // 群成员数量
        /// <summary>
        /// 群成员数量
        /// </summary>
        public long GroupMemberCount;

        // dwMemberNumSeq
        /// <summary>
        /// 群成员名片查询Seq
        /// </summary>
        public long MemberNumSeq;

        // dwMemberCardSeq
        public long MemberCardSeq;

        // dwGroupFlagExt3
        /// <summary>
        /// 群主QQ
        /// </summary>
        public long GroupFlagExt3;

        // dwGroupOwnerUin
        public long GroupOwnerUin;

        // cIsConfGroup
        public long IsConfGroup;

        // cIsModifyConfGroupFace
        public long IsModifyConfGroupFace;

        // cIsModifyConfGroupName
        public long IsModifyConfGroupName;

        // dwCmduinJoinTime
        /// <summary>
        /// 入群时间戳
        /// </summary>
        public long CmduinJoinTime;

        // 群名称
        /// <summary>
        /// 群名称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string GroupName;

        // strGroupMemo
        /// <summary>
        /// 新人公告
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string GroupMemo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GMBriefDataList
    {
        public GMBriefInfo groupMemberBriefInfo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GMBriefInfo
    {
        /// <summary>
        /// 群上限
        /// </summary>
        public uint GroupMAax;

        /// <summary>
        /// 群人数
        /// </summary>
        public uint GruoupNum;

        /// <summary>
        /// 群主
        /// </summary>
        public long GroupOwner;

        /// <summary>
        /// 群管理员列表
        /// </summary>
        public IntPtr AdminiList;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AdminListDataList1
    {
        public AdminListDataList Admin;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AdminListDataList
    {
        public int index;//数组索引

        public int Amount;//数组元素数量

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public long[] pdatalist;
    }

    /// <summary>
    /// 群成员状况简略信息
    /// </summary>
    public class GroupMemberBriefInfo
    {
        /// <summary>
        /// 群上限
        /// </summary>
        public uint GroupMAax;

        /// <summary>
        /// 群人数
        /// </summary>
        public uint GruoupNum;

        /// <summary>
        /// 群主
        /// </summary>
        public long GroupOwner;

        /// <summary>
        /// 群管理员列表
        /// </summary>
        public long[] AdminiList;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GroupMemberDataList
    {
        public int index;//数组索引

        public int Amount;//数组元素数量

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 * 100)]//5000人群 5000/4+8 =1258
        public byte[] pAddrList;//每个元素的指针
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GroupMemberInfo
    {
        // 账号
        [MarshalAs(UnmanagedType.LPStr)]
        public string QQNumber;

        // 年龄
        public uint Age;

        // 性别
        public uint Gender;

        // 昵称
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;

        // 邮箱
        [MarshalAs(UnmanagedType.LPStr)]
        public string Email;

        // 名片
        [MarshalAs(UnmanagedType.LPStr)]
        public string Nickname;

        // 备注
        public string Note;

        // 头衔
        [MarshalAs(UnmanagedType.LPStr)]
        public string Title;

        // 手机号
        [MarshalAs(UnmanagedType.LPStr)]
        public string Phone;

        // 头衔到期时间
        public long TitleTimeout;

        // 禁言时间戳
        public long ShutUpTimestamp;

        // 加群时间
        public long JoinTime;

        // 发言时间
        public long ChatTime;

        // 群等级
        public long Level;
    }/// <summary>

     /// 群员信息
     /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OneGroupMemberInfo
    {
        /// <summary>
        /// 群名片
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string GroupCardName;

        /// <summary>
        /// 昵称
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string NickName;

        /// <summary>
        /// 群聊等级<para>文本型等级,取决于群等级设置,如：冒泡</para>
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string GroupChatLevel;

        /// <summary>
        /// 入群时间戳
        /// </summary>
        public long JoinTime;

        /// <summary>
        /// 最后发言时间戳
        /// </summary>
        public long LastSpeackTime;

        /// <summary>
        /// 管理权限<para>表示群成员对于所在群的群成员类型</para>
        /// </summary>
        public GroupPosition groupPosition;

        /// <summary>
        /// 头衔
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string GroupTitle;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OneGroupMemberDataList
    {
        public OneGroupMemberInfo oneGroupMemberInfo;
    }
}