using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC.Interface;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Concurrent;
using System.Diagnostics;
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
                LogHelper.WriteLog(LogLevel.Error, PluginManager.LoadedPlugin.Name, "服务端调用事件", messages: $"事件名称: {argumentType}, WaitID: {response.WaitID}, 反射参数类型失败");
                return 0;
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                var instance = Activator.CreateInstance(argumentType);

                var ls = argumentType.GetProperties().Where(x => x.CanWrite);
                var outList = ls.Where(x => argumentType.GetField($"{x.Name}FieldNumber") != null)
                    .OrderBy(x => (int)(argumentType.GetField($"{x.Name}FieldNumber").GetValue(instance))).ToList();
                if (args.Length != outList.Count)
                {
                    LogHelper.Error("服务端调用事件", $"校验反射参数 {eventType} 参数数量失败。目标数量: {args.Length}，实际数量: {outList.Count}");
                    return 0;
                }

                for (int i = 0; i < outList.Count; i++)
                {
                    outList[i].SetValue(instance, args[i]);
                }

                MethodInfo packMethodInfo = typeof(Any).GetMethod("Pack", new System.Type[] { typeof(IMessage) });

                Any any = (Any)packMethodInfo.Invoke(null, new object[] { instance }); // 调用Pack
                response.Response = any;
                ManualResetEvent signal = new(false);
                RequestWaiter.CommonWaiter.TryAdd($"EventID_{response.WaitID}", new WaiterInfo()
                {
                    CurrentPluginProxy = target,
                    WaitSignal = signal,
                });
                Task.Run(() =>
                {
                    if (CoreFunctionWrapper.Send(target.AppInfo.AuthCode, response) is false)
                    {
                        WaitingResults.TryAdd(response.WaitID, -1);
                        RequestWaiter.TriggerByKey($"EventID_{response.WaitID}");
                    }
                });
                ;
                if (signal.WaitOne(AppConfig.PluginInvokeTimeout)
                    && WaitingResults.TryRemove(response.WaitID, out int result))
                {
                    return result;
                }
                else
                {
                    LogHelper.Error("服务端调用事件", $"插件名称: {target.PluginName}, 事件名称: {eventType}, WaitID: {response.WaitID}, 写入流失败");
                    return 0;
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("服务端调用事件", $"插件名称: {target.PluginName}, 事件名称: {eventType}, WaitID: {response.WaitID}, {e.Message} {e.StackTrace}");
                return 0;
            }
            finally
            {
                stopwatch.Stop();
                LogHelper.WriteLog(LogLevel.Debug, target.PluginName, "服务器调用事件", messages: $"事件名称: {eventType}, WaitID: {response.WaitID}, 耗时: {stopwatch.ElapsedMilliseconds}ms");
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
