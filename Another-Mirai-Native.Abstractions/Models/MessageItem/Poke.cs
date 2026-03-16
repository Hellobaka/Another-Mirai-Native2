using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class Poke(string action) : MessageItemBase
    {
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Poke;

        public string Action { get; set; } = action;

        public override string ToString()
        {
            return $"[CQ:poke,name={Action}]";
        }
    }
}
