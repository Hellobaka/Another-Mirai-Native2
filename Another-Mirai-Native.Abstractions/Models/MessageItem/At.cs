using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class At(long target, bool allTarget) : MessageItemBase
    {
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.At;

        public long Target { get; set; } = target;

        public bool AllTarget { get; set; } = allTarget;

        public override string ToString()
        {
            return $"[CQ:at,qq={(AllTarget ? "all" : Target.ToString())}]";
        }
    }
}
