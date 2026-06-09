using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 无法解析的消息片段
    /// </summary>
    /// <param name="raw">原始消息内容</param>
    public class Unknown(string raw) : MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Unknown;

        /// <summary>
        /// 原始消息内容
        /// </summary>
        public string Raw { get; } = raw;

        /// <inheritdoc/>
        public override string ToString() => Raw;
    }
}
