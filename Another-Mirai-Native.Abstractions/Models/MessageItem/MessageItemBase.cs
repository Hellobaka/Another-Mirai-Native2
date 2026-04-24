using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 消息片段基类。
    /// </summary>
    public abstract class MessageItemBase
    {
        /// <summary>
        /// 获取或设置消息片段类型。
        /// </summary>
        public abstract MessageItemType MessageItemType { get; set; }

        /// <summary>
        /// 返回当前消息片段的 CQ 码字符串表示。
        /// </summary>
        /// <returns>CQ 码文本。</returns>
        public abstract override string ToString();
    }
}
