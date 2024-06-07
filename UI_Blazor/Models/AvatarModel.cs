namespace Another_Mirai_Native.BlazorUI.Models
{
    public class AvatarModel
    {
        public AvatarTypes AvatarType { get; set; } = AvatarTypes.Fallback;

        public string FallbackName { get; set; } = "";

        public long Id { get; set; }

        public bool IsRound { get; set; }

        public string BackgroundColor { get; set; }
    }
}
