using Newtonsoft.Json.Linq;
using System;

namespace Another_Mirai_Native.Protocol.MiraiAPIHttp.MiraiAPIResponse
{
    public class WaitingMessage
    {
        public bool Finished { get; set; }

        public JObject Result { get; set; }
    }
}