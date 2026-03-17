using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 屏幕抖动消息片段。
    /// </summary>
    public class Shake : MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Shake;

        /// <inheritdoc/>
        public override string ToString()
        {
            return "[CQ:shake]";
        }
    }
}
