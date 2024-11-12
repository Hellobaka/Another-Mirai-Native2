using Microsoft.AspNetCore.Components;

namespace Another_Mirai_Native.BlazorUI.Models
{
    public enum RenderType
    {
        Image,
        At,
        Reply,
        Text,
        Face,
        Url,
        Other
    }

    public class RenderChatItem
    {
        public RenderType RenderType { get; set; } = RenderType.Text;

        public string Text { get; set; }

        public int FaceId { get; set; }

        public string? ImageUrl { get; set; }

        public long AtTarget { get; set; }

        public string AtNick { get; set; }

        public int ReplyId { get; set; }
    }
}
