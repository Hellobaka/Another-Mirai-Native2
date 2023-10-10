using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native
{
    public class WebSocketServer
    {
        public static WebSocketServer Instance { get; set; }

        public static void Start()
        {
            WebSocketServer server = new();
            Instance = server;
        }
    }
}