using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// @消息片段。
    /// </summary>
    /// <param name="target">目标 QQ。</param>
    /// <param name="allTarget">是否 @全体成员。</param>
    public class At(long target, bool allTarget) : MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.At;

        /// <summary>
        /// 目标 QQ。
        /// </summary>
        public long Target { get; set; } = target;

        /// <summary>
        /// 是否 @全体成员。
        /// </summary>
        public bool AllTarget { get; set; } = allTarget;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[CQ:at,qq={(AllTarget ? "all" : Target.ToString())}]";
        }
    }
}
