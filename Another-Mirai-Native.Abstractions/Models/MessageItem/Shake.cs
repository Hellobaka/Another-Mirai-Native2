using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class Shake : MessageItemBase
    {
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Shake;

        public override string ToString()
        {
            return "[CQ:shake]";
        }
    }
}
