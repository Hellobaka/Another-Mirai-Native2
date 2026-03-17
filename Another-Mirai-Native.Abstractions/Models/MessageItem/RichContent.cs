using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 卡片消息片段。
    /// </summary>
    /// <param name="richContentType">卡片内容类型。</param>
    /// <param name="content">卡片内容。</param>
    public class RichContent(RichContentType richContentType, string content) : MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Rich;

        /// <summary>
        /// 卡片内容类型。
        /// </summary>
        public RichContentType RichContentType { get; set; } = richContentType;

        /// <summary>
        /// 卡片内容。
        /// </summary>
        public string Content { get; set; } = content;

        /// <inheritdoc/>
        public override string ToString()
        {
            string type = RichContentType.ToString().ToLower();
            return $"[CQ:rich,type={type},content={Content}]";
        }
    }
}
