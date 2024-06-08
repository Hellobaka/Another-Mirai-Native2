namespace Another_Mirai_Native.BlazorUI.Models
{
    public class Shared
    {
        public bool IsDarkMode { get; set; } = true;

        public bool AutoScroll { get; set; } = true;

        public List<ChatItemModel> TestMessages { get; set; } = [];
    }
}
