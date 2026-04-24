using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 猜拳消息片段。
    /// </summary>
    /// <param name="rpsType">猜拳类型。</param>
    public class RPS(RpsType rpsType) : MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Rps;

        /// <summary>
        /// 猜拳类型。
        /// </summary>
        public RpsType RpsType { get; set; } = rpsType;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[CQ:rps,type={(int)RpsType}]";
        }
    }
}
