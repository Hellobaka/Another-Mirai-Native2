using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class Dice(int point) : MessageItemBase
    {
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Dice;

        public int Point { get; set; } = point;

        public override string ToString()
        {
            return $"[CQ:dice,type={Point}]";
        }
    }
}
