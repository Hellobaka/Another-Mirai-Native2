namespace Another_Mirai_Native.BlazorUI.Models
{
    public class ChatHistoryItem
    {
        public AvatarTypes AvatarType { get; set; } = AvatarTypes.Fallback;

        public string Detail { get; set; } = "";

        public string Name { get; set; } = "";

        public long Id { get; set; }

        public DateTime Time { get; set; }

        public int UnreadCount { get; set; }

        public bool IsRound { get; set; } = true;
    }
}
