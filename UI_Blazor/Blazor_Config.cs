using Another_Mirai_Native.Config;

namespace Another_Mirai_Native.BlazorUI
{
    public class Blazor_Config : ConfigBase
    {
        public Blazor_Config() : base("conf\\Blazor_Config.json")
        {
            ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigPath);
            LoadConfig();
        }

        public static Blazor_Config Instance { get; private set; } = new();

        public bool FocusDarkMode { get; set; }

        public string Password { get; set; }

        private void LoadConfig()
        {
            FocusDarkMode = GetConfig("FocusDarkMode", false);
            Password = GetConfig("Password", Guid.NewGuid().ToString());
        }
    }
}
