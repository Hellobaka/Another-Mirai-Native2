using System.Collections.Concurrent;
using System.Diagnostics;

namespace Another_Mirai_Native
{
    public class RequestWaiter
    {
        public static ConcurrentDictionary<object, WaiterInfo> CommonWaiter { get; set; } = new();
    }

    public class WaiterInfo
    {
        public Process CurrentProcess { get; set; }

        public string WebSocketConnectionID { get; set; }

        public ManualResetEvent WaitSignal { get; set; }
    }
}