using Another_Mirai_Native.Config;
using System.Net;

namespace Another_Mirai_Native.WebAPI
{
    public class WebUIConfig : ConfigBase
    {
        public WebUIConfig() : base("conf\\Blazor_Config.json")
        {
            ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigPath);
            LoadConfig();
        }

        public static WebUIConfig Instance { get; private set; } = new();

        public string ListenIP { get; set; } = "127.0.0.1";

        public int ListenPort { get; set; } = 5000;

        public bool EnableHTTPS { get; set; }

        public string Password { get; set; }

        public string CertificatePath { get; set; }

        public string CertificateKeyPath { get; set; }

        public void LoadConfig()
        {
            Password = GetConfig("Password", Guid.NewGuid().ToString().Replace("-", "")[..16]);
            ListenIP = GetConfig("ListenIP", "127.0.0.1");
            ListenPort = GetConfig("ListenPort", 5000);
            if (IPAddress.TryParse(ListenIP, out _) is false && ListenIP != "*")
            {
                Console.Error.WriteLine($"ListenIP {ListenIP} is invalid ip, now changed to 127.0.0.1");
                ListenIP = "127.0.0.1";
            }
            if (ListenPort < 0 || ListenPort > 65535)
            {
                Console.Error.WriteLine($"ListenPort {ListenPort} is invalid port, now changed to 5000");
                ListenPort = 5000;
            }
            CertificatePath = GetConfig("CertificatePath", "");
            CertificateKeyPath = GetConfig("CertificateKeyPath", "");
            EnableHTTPS = GetConfig("EnableHTTPS", false);
        }
    }
}
