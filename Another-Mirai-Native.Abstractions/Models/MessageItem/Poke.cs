using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 戳一戳消息片段。
    /// </summary>
    /// <param name="action">戳一戳动作名称。</param>
    public class Poke(string action) : MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Poke;

        /// <summary>
        /// 戳一戳动作名称。
        /// </summary>
        public string Action { get; set; } = action;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[CQ:poke,name={Action}]";
        }
    }
}
