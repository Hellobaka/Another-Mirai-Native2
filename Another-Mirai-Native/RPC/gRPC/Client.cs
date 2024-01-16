using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SqlSugar;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;

namespace Another_Mirai_Native.gRPC
{
    public class Client : RPC.Interface.ClientBase
    {
        private object writeLock = new object();

        public static Metadata Headers => new()
        {
            { "authCode", AppConfig.Instance.Core_AuthCode.ToString() }
        };

        private string ConnectIP { get; set; } = AppConfig.Instance.gRPCListenIP;

        private int ConnectPort { get; set; } = AppConfig.Instance.gRPCListenPort;

        private CoreFunctions.CoreFunctionsClient CoreClient { get; set; }

        private CQPFunctions.CQPFunctionsClient CQPClient { get; set; }

        private Channel GrpcChannel { get; set; }

        private IClientStreamWriter<StreamRequest> RequestStream { get; set; }

        private IAsyncStreamReader<StreamResponse> ResponseStream { get; set; }

        public override void AddLog(LogModel model)
        {
            CoreClient.AddLog(new AddLogRequest
            {
                Detail = model.detail,
                Id = model.id,
                Name = model.name,
                Priority = model.priority,
                Source = model.source,
                Status = model.status,
                Time = model.time
            });
        }

        public override void ClientStartUp()
        {
            CoreClient.ClientStartup(new ClientStartupRequest
            {
                AppId = PluginManager.LoadedPlugin.AppInfo.AppId,
                Pid = PID
            }, headers: Headers);
        }

        public override void Close()
        {
            try
            {
                GrpcChannel.ShutdownAsync().Wait();
            }
            catch
            {
            }
            GrpcChannel = null;
            CQPClient = null;
        }

        public override bool Connect()
        {
            try
            {
                GrpcChannel = new Channel($"{ConnectIP}:{ConnectPort}", ChannelCredentials.Insecure);
                CQPClient = new CQPFunctions.CQPFunctionsClient(GrpcChannel);
                CoreClient = new CoreFunctions.CoreFunctionsClient(GrpcChannel);
                HeartBeatLostCount = 0;

                StartStream();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public override object InvokeCQPFuntcion(string function, bool waiting, params object[] args)
        {
            var argumentType = typeof(CQ_addLog_Parameters).Assembly.GetType($"Another_Mirai_Native.gRPC.{function}_Parameters");
            if (argumentType == null)
            {
                LogHelper.Error("InvokeCQPFuntcion", $"反射参数 {function} 类型失败");
                return 0;
            }
            var instance = Activator.CreateInstance(argumentType);
            var ls = argumentType.GetProperties().Where(x => x.CanWrite);
            var outList = ls.Where(x => argumentType.GetField($"{x.Name}FieldNumber") != null)
                .OrderBy(x => (int)(argumentType.GetField($"{x.Name}FieldNumber").GetValue(instance))).ToList();
            if(args.Length != outList.Count)
            {
                LogHelper.Error("InvokeCQPFuntcion", $"校验反射参数 {function} 参数数量失败。目标数量: {args.Length}，实际数量: {outList.Count}");
                return 0;
            }
            for (int i = 0; i < outList.Count; i++)
            {
                outList[i].SetValue(instance, args[i]);
            }

            MethodInfo method = CQPClient.GetType().GetMethod(function, new System.Type[]
            {
                argumentType,
                typeof(Metadata),
                typeof(DateTime?),
                typeof(CancellationToken)
            });
            if (method == null)
            {
                LogHelper.Error("InvokeCQPFuntcion", $"获取方法 {function} 类型失败");
                return 0;
            }
            try
            {
                var result = method.Invoke(CQPClient, new object[] { instance, Headers, null, null });
                if (result != null && result is Int32Value int32Value)
                {
                    return int32Value.Value;
                }
                if (result != null && result is Int64Value int64Value)
                {
                    return int64Value.Value;
                }
                else if (result != null && result is StringValue stringValue)
                {
                    return stringValue.Value;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override void SendHeartBeat()
        {
            RequestStream?.WriteAsync(new StreamRequest
            {
                WaitID = 0
            }).Wait();
        }

        public override bool SetConnectionConfig()
        {
            return string.IsNullOrEmpty(ConnectIP) is false && ConnectPort > 0 && ConnectPort < 65535;
        }

        public override void ShowErrorDialog(string guid, string title, string content, bool canIgnore)
        {
            CoreClient.ShowErrorDialog(new ErrorDialog
            {
                Guid = guid,
                Title = title,
                Content = content,
                CanIgnore = canIgnore
            }, headers: Headers);
        }

        public void StartStream()
        {
            var call = CoreClient.InvokeCQPEvents(headers: Headers);
            ResponseStream = call.ResponseStream;
            RequestStream = call.RequestStream;
            Task.Run(async () =>
            {
                try
                {
                    while (await ResponseStream.MoveNext())// 来自Server的消息
                    {
                        var response = ResponseStream.Current;
                        InvokeEvents(response);
                    }
                }
                catch (Exception e)
                {
                    StartStream();
                    LogHelper.Debug("事件流异常", $"{e.Message} {e.StackTrace}", PluginManager.LoadedPlugin.Name);
                }
            });
        }

        private void InvokeEvents(StreamResponse response)
        {
            string typeName = Any.GetTypeName(response.Response.TypeUrl);
            typeName = typeName.Substring(typeName.IndexOf('.') + 1);
            var argumentType = typeof(Event_OnAdminChange_Parameters).Assembly.GetType($"Another_Mirai_Native.gRPC.{typeName}");
            if (argumentType == null)
            {
                LogHelper.WriteLog(LogLevel.Error, PluginManager.LoadedPlugin.Name, "调用事件", messages: $"事件名称: {typeName}, WaitID: {response.WaitID}, 反射参数类型失败");
                return;
            }
            Task.Run(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    MethodInfo unpackMethodInfo = typeof(Any).GetMethod("Unpack", System.Type.EmptyTypes);
                    MethodInfo genericUnpackMethod = unpackMethodInfo.MakeGenericMethod(argumentType);

                    object argument = genericUnpackMethod.Invoke(response.Response, null); // 调用UnPack
                    object[] args = argument.GetType().GetProperties().Where(x => x.CanWrite).Select(x => x.GetValue(argument)).ToArray();

                    if (typeName == "HeartBeatRequest")
                    {
                        HeartBeatLostCount = 0;
                        return;
                    }

                    PluginEventType eventType = (PluginEventType)System.Enum.Parse(typeof(PluginEventType), typeName.Replace("_Parameters", "").Replace("Event_On", ""));
                    int result = InvokeEvent(eventType, args);

                    lock (writeLock)
                    {
                        RequestStream.WriteAsync(new StreamRequest
                        {
                            WaitID = response.WaitID,
                            Request = result
                        }).Wait();
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog(LogLevel.Error, PluginManager.LoadedPlugin.Name, "调用事件", messages: $"事件名称: {typeName}, WaitID: {response.WaitID}, {e.Message} {e.StackTrace}");
                    lock (writeLock)
                    {
                        RequestStream.WriteAsync(new StreamRequest
                        {
                            WaitID = response.WaitID,
                            Request = 0
                        }).Wait();
                    }
                }
                finally
                {
                    stopwatch.Stop();
                }
            });
        }
    }
}