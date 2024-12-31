using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.RPC;
using System.IO;
using System.Reflection;

namespace Another_Mirai_Native
{
    public class ProtocolManager
    {
        public static ProtocolManager Instance { get; set; }

        public static List<IProtocol> Protocols { get; set; } = new();

        public IProtocol CurrentProtocol { get; set; }

        public ProtocolManager()
        {
            if (Protocols.Count == 0)
            {
                LoadProtocol();
            }
            Instance = this;
        }

        public void SetQrCodeAction(Action<string, byte[]> displayAction, Action finishedAction)
        {
            foreach (var protocol in Protocols)
            {
                protocol.QRCodeDisplayAction += displayAction;
                protocol.QRCodeFinishedAction += finishedAction;
            }
        }

        public bool Start(IProtocol protocol)
        {
            if (protocol == null)
            {
                return false;
            }
            bool flag = protocol.Connect();
            RefreshBotInfo(protocol);
            flag = flag && !string.IsNullOrEmpty(AppConfig.Instance.CurrentNickName);
            if (flag)
            {
                CurrentProtocol = protocol;
                ServerManager.Server.NotifyCurrentQQChanged(AppConfig.Instance.CurrentQQ, AppConfig.Instance.CurrentNickName);
            }
            LogHelper.Info("加载协议", $"加载 {protocol.Name} 协议{(flag ? "成功" : "失败")}");
            return flag;
        }

        public bool Start(string protocolName)
        {
            if (!string.IsNullOrEmpty(protocolName) && Protocols.Any(x => x.Name == protocolName))
            {
                return Start(Protocols.First(x => x.Name == protocolName));
            }
            else
            {
                LogHelper.Error("加载协议", $"未找到 {protocolName} 协议");
            }
            return false;
        }

        public void LoadProtocol()
        {
            foreach (var item in Directory.GetFiles("protocols", "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(item);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (typeof(IProtocol).IsAssignableFrom(type) && !type.IsInterface)
                        {
                            if (Activator.CreateInstance(type) is IProtocol protocol)
                            {
                                Protocols.Add(protocol);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("加载协议", $"无法加载协议: {ex.Message} {ex.StackTrace}");
                }
            }
        }

        public void RefreshBotInfo(IProtocol protocol)
        {
            if (protocol != null && protocol.IsConnected)
            {
                AppConfig.Instance.CurrentNickName = protocol.GetLoginNick();
                AppConfig.Instance.CurrentQQ = protocol.GetLoginQQ();
            }
        }
    }
}