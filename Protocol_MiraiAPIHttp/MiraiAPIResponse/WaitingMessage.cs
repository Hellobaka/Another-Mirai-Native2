using System;

namespace Another_Mirai_Native.Protocol.MiraiAPIHttp.MiraiAPIResponse
{
    public class WaitingMessage
    {
        public Type Type { get; set; }

        public bool Finished { get; set; }

        public object Result { get; set; }
    }
}