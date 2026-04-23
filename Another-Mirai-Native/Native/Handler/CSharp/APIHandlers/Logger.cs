using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.RPC;

namespace Another_Mirai_Native.Native.Handler.CSharp.APIHandlers
{
    public class Logger(PluginInfo pluginInfo) : ILogger
    {
        private PluginInfo PluginInfo { get; set; } = pluginInfo;

        private int AuthCode => PluginInfo.AuthCode;

        public void Debug(string type, string message)
        {
            // int authCode, int priority, string type, string msg
            ClientManager.Client.InvokeCQPFunction("CQ_addLog", true, AuthCode, (int)LogLevel.Debug, type, message);
        }

        public void Error(string type, string message)
        {
            ClientManager.Client.InvokeCQPFunction("CQ_addLog", true, AuthCode, (int)LogLevel.Error, type, message);
        }

        public void Fatal(string message)
        {
            ClientManager.Client.InvokeCQPFunction("CQ_setFatal", true, AuthCode, message);
        }

        public void Info(string type, string message)
        {
            ClientManager.Client.InvokeCQPFunction("CQ_addLog", true, AuthCode, (int)LogLevel.Info, type, message);
        }

        public void Warn(string type, string message)
        {
            ClientManager.Client.InvokeCQPFunction("CQ_addLog", true, AuthCode, (int)LogLevel.Warning, type, message);
        }
    }
}
