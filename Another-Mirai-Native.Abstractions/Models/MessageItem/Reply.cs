using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class Reply(int id) : MessageItemBase
    {
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Reply;

        public int Id { get; set; } = id;

        public override string ToString()
        {
            return $"[CQ:reply,id={id}]";
        }
    }
}
