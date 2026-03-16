using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class RPS(RpsType rpsType) : MessageItemBase
    {
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Rps;

        public RpsType RpsType { get; set; } = rpsType;

        public override string ToString()
        {
            return $"[CQ:rps,type={(int)RpsType}]";
        }
    }
}
