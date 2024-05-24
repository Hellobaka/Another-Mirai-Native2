namespace UI_Blazor.Models
{
    public class Shared
    {
        public bool IsDarkMode { get; set; } = true;

        public string SessionId { get; set; } = Guid.NewGuid().ToString();
    }
}
