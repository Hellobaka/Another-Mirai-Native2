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
}