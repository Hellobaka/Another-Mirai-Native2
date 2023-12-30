using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC.Interface;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Concurrent;
using System.Reflection;

namespace Another_Mirai_Native.gRPC
{
    public class Server : ServerBase
    {
        public static Grpc.Core.Server ServerInstance { get; set; }

        private string ListenIP { get; set; } = AppConfig.gRPCListenIP;

        private ushort ListenPort { get; set; } = AppConfig.gRPCListenPort;

        private int SyncId { get; set; } = 1;

        public static ConcurrentDictionary<int, int> WaitingResults { get; set; } = new();

        public override bool Start()
        {
            try
            {
                ServerInstance = new()
                {
                    Ports = { new Grpc.Core.ServerPort(ListenIP, ListenPort, Grpc.Core.ServerCredentials.Insecure) }
                };
                ServerInstance.Services.Add(CQPFunctions.BindService(new CQPFunctionWrapper()));
                ServerInstance.Services.Add(CoreFunctions.BindService(new CoreFunctionWrapper()));
                ServerInstance.Start();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public override bool Stop()
        {
            try
            {
                ServerInstance?.ShutdownAsync().Wait();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override bool SetConnectionConfig()
        {
            return !(string.IsNullOrEmpty(ListenIP) || ListenPort <= 0 || ListenPort >= 65535);
        }

        public override int? InvokeEvents(CQPluginProxy target, PluginEventType eventType, params object[] args)
        {
            StreamResponse response = new()
            {
                WaitID = ++SyncId
            };
            var argumentType = typeof(Event_OnAdminChange_Parameters).Assembly.GetType($"Another_Mirai_Native.gRPC.Event_On{eventType}_Parameters");
            if (argumentType == null)
            {
                // log
                return 0;
            }
            try
            {
                var instance = Activator.CreateInstance(argumentType);
                int index = 0;
                foreach (var item in instance.GetType().GetProperties().Where(x => x.CanWrite))
                {
                    item.SetValue(instance, args[index]);
                    index++;
                }

                MethodInfo packMethodInfo = typeof(Any).GetMethod("Pack", new System.Type[] { typeof(IMessage) });

                Any any = (Any)packMethodInfo.Invoke(null, new object[] { instance }); // 调用Pack
                response.Response = any;
                if (CoreFunctionWrapper.Send(target.AppInfo.AuthCode, response)
                    && RequestWaiter.Wait(response.WaitID, target, AppConfig.PluginInvokeTimeout)
                    && WaitingResults.TryRemove(response.WaitID, out int result))
                {
                    return result;
                }
                else
                {
                    // log
                    return 0;
                }
            }
            catch
            {
                // log
                return 0;
            }
        }

        public static CQPImplementation GetCQPImplementation(object gRPCParameters)
        {
            string typeName = gRPCParameters.GetType().Name;
            if (typeName.Contains("_Parameters"))
            {
                var plugin = PluginManagerProxy.GetProxyByAuthCode((int)gRPCParameters.GetType().GetProperty("AuthCode").GetValue(gRPCParameters))
                    ?? throw new Exception("无法获取调用插件实例");
                return new CQPImplementation(plugin);
            }
            else
            {
                throw new Exception("无法从参数获取调用信息");
            }
        }

        public static string GetFunctionName(object gRPCParameters)
        {
            string typeName = gRPCParameters.GetType().Name;
            if (typeName.Contains("_Parameters"))
            {
                return typeName.Replace("_Parameters", "");
            }
            else
            {
                throw new Exception("无法从参数获取调用信息");
            }
        }
    }
}
