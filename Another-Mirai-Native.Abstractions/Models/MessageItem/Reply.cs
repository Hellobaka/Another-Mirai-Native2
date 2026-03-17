using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 引用消息片段。
    /// </summary>
    /// <param name="id">被引用消息 ID。</param>
    public class Reply(int id) : MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Reply;

        /// <summary>
        /// 被引用消息 ID。
        /// </summary>
        public int Id { get; set; } = id;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[CQ:reply,id={Id}]";
        }
    }
}
