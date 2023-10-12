using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public bool Start(IProtocol protocol)
        {
            if (protocol == null)
            {
                return false;
            }
            bool flag = protocol.Connect();
            LogHelper.Info("Connect", $"加载 {protocol.Name} 协议{(flag ? "成功" : "失败")}");
            if (flag)
            {
                CurrentProtocol = protocol;
            }
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
                LogHelper.Error("Connect", $"未找到 {protocolName} 协议");
            }
            return false;
        }

        public void LoadProtocol()
        {
            foreach (var item in Directory.GetFiles("protocols", "*.dll"))
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
        }
    }
}