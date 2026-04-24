using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 骰子消息片段。
    /// </summary>
    /// <param name="point">骰子点数。</param>
    public class Dice(int point) : MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Dice;

        /// <summary>
        /// 骰子点数。
        /// </summary>
        public int Point { get; set; } = point;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[CQ:dice,type={Point}]";
        }
    }
}
