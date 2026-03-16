using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class RichContent(RichContentType richContentType, string content) : MessageItemBase
    {
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Rich;

        public RichContentType RichContentType { get; set; } = richContentType;

        public string Content { get; set; } = content;

        public override string ToString()
        {
            string type = RichContentType.ToString().ToLower();
            return $"[CQ:rich,type={type},content={Content}]";
        }
    }
}
