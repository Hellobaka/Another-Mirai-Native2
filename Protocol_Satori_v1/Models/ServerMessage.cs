using Another_Mirai_Native.Protocol.Satori.Enums;
using Newtonsoft.Json.Linq;

namespace Another_Mirai_Native.Protocol.Satori.Models
{
    public class ServerMessage
    {
        public EventOp op { get; set; }

        public JToken body { get; set; }
    }
}
