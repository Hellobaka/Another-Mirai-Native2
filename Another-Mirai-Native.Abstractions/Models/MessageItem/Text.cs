using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 纯文本消息片段。
    /// </summary>
    /// <param name="text">文本内容。</param>
    public class Text(string text) : MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Text;

        /// <summary>
        /// 文本内容。
        /// </summary>
        public string Content { get; set; } = text;

        /// <inheritdoc/>
        public override string ToString()
        {
            return Content;
        }
    }
}
