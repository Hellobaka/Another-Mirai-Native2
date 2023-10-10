using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native
{
    public class WebSocketClient
    {
        public static WebSocketClient Instance { get; private set; }

        public static bool ExitFlag { get; private set; }

        public WebSocketClient()
        {
            Instance = this;
        }

        public bool Connect(string url)
        {
            return true;
        }
    }
}