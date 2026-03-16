using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public abstract class MessageItemBase
    {
        public abstract MessageItemType MessageItemType { get; set; }

        public string Raw { get; }

        public abstract override string ToString();
    }
}
