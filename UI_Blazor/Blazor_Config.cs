using Another_Mirai_Native.Config;
using System.Net;

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

        public string ListenIP { get; set; } = "127.0.0.1";

        public int ListenPort { get; set; } = 5000;

        public string Password { get; set; }

        public void LoadConfig()
        {
            Password = GetConfig("Password", Guid.NewGuid().ToString());
            ListenIP = GetConfig("ListenIP", "127.0.0.1");
            ListenPort = GetConfig("ListenPort", 5000);
            if (IPAddress.TryParse(ListenIP, out _) is false)
            {
                Console.Error.WriteLine($"ListenIP {ListenIP} is invalid ip, now changed to 127.0.0.1");
                ListenIP = "127.0.0.1";
            }
            if (ListenPort < 0 || ListenPort > 65535)
            {
                Console.Error.WriteLine($"ListenPort {ListenPort} is invalid port, now changed to 5000");
                ListenPort = 5000;
            }
        }
    }
}
