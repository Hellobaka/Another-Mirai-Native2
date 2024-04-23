using Another_Mirai_Native.Protocol.Satori.Enums;

namespace Another_Mirai_Native.Protocol.Satori.Models
{
    public class Login
    {
        public User user { get; set; }

        public string self_id { get; set; }

        public string platform { get; set; }

        public LoginStatus status { get; set; }
    }
}
