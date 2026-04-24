using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Enums
{
    /// <summary>
    /// 指令的消息来源作用域。
    /// </summary>
    public enum MessageScope
    {
        /// <summary>
        /// 仅响应群聊消息。
        /// </summary>
        Group,

        /// <summary>
        /// 仅响应私聊消息。
        /// </summary>
        Private,

        /// <summary>
        /// 同时响应群聊和私聊消息。
        /// </summary>
        All,
    }
}
