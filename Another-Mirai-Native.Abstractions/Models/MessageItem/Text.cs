using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class Text(string text) : MessageItemBase
    {
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Text;

        public string Content { get; set; } = text;

        public override string ToString()
        {
            return Content;
        }
    }
}
