using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Protocol.MiraiAPIHttp
{
    public class OtherClientOfflineEvent
    {
        public string type { get; set; }
        public Client client { get; set; }
        public class Client
        {
            public long id { get; set; }
            public string platform { get; set; }
        }
    }
}
