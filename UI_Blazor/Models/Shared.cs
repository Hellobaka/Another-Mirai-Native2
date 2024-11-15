using MudBlazor;

namespace Another_Mirai_Native.BlazorUI.Models
{
    public class Shared
    {
        public bool IsDarkMode { get; set; } = true;

        public bool AutoScroll { get; set; } = true;

        public bool TestGroup { get; set; } = true;

        public long SelectChatHistoryId { get; set; }

        public List<ChatItemModel> TestMessages { get; set; } = [];

        public Breakpoint CurrentBreakPoint { get; set; }

        public event Action<Breakpoint> CurrentBreakPointChanged;

        public void TriggerBreakPointChanged(Breakpoint breakpoint)
        {
            CurrentBreakPointChanged?.Invoke(breakpoint);
        }
    }
}
