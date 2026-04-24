using Another_Mirai_Native.Abstractions.Enums;
using System;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 表示框架持久化记录的聊天记录的类。该类包含了聊天记录的基本属性，如消息内容、发送者信息、消息类型等。
    /// </summary>
    public class ChatHistory
    {
        /// <summary>
        /// 持久化ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 记录发生的时间
        /// </summary>
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        /// 聊天记录类别
        /// </summary>
        public ChatHistoryType Type { get; set; } = ChatHistoryType.Private;

        /// <summary>
        /// 群号或QQ
        /// </summary>
        public long ParentID { get; set; }

        /// <summary>
        /// 发送者ID，如果是群聊则为QQ，如果是私聊则与 ParentID 相同，如果是通知则为0
        /// </summary>
        public long SenderID { get; set; }

        /// <summary>
        /// 消息内容，包含未经处理的原始消息文本，可能包含CQ码等特殊格式。
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 消息ID，用于撤回、引用等操作
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// 是否已撤回
        /// </summary>
        public bool Recalled { get; set; }

        /// <summary>
        /// 由哪个插件发送的消息，如果是用户发送的消息则为 string.Empty
        /// </summary>
        public string PluginName { get; set; } = string.Empty;
    }
}
