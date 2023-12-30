using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using System.Diagnostics;

namespace Another_Mirai_Native.RPC.Interface
{
    public class ClientBase : IClient
    {
        public event Action<bool> OnReceiveHeartBeat;
      
        public Dictionary<string, InvokeResult> WaitingMessage { get; set; } = new();

        public static int PID => Process.GetCurrentProcess().Id;

        public int HeartBeatLostCount { get; set; }

        public virtual void ClientStartUp()
        {
            throw new NotImplementedException();
        }

        public virtual void Close()
        {
        }

        public virtual bool Connect()
        {
            return false;
        }

        public virtual object InvokeCQPFuntcion(string function, bool waiting, params object[] args)
        {
            throw new NotImplementedException();
        }

        public int InvokeEvent(PluginEventType eventType, object[] args)
        {
            return PluginManager.Instance.CallEvent(eventType, args);
        }

        public void KillProcess()
        {
            Environment.Exit(0);
        }

        public virtual void SendHeartBeat()
        {
        }

        public virtual bool SetConnectionConfig()
        {
            return false;
        }

        public virtual void ShowErrorDialog(string guid, string title, string content, bool canIgnore)
        {
            throw new NotImplementedException();
        }

        public virtual void AddLog(LogModel model)
        {
            throw new NotImplementedException();
        }
    }
}
