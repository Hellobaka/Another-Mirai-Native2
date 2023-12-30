using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SqlSugar;
using System.Reflection;

namespace Another_Mirai_Native.gRPC
{
    public class Client : RPC.Interface.ClientBase
    {
        private object writeLock = new object();

        public static Metadata Headers => new()
        {
            { "authCode", AppConfig.Core_AuthCode.ToString() }
        };

        private string ConnectIP { get; set; } = AppConfig.gRPCListenIP;

        private int ConnectPort { get; set; } = AppConfig.gRPCListenPort;

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
            // 由于调用参数并非简单排列，需重新实现CQP实现类构造
            MethodInfo method = CQPClient.GetType().GetMethod(function, BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
            {
                return null;
            }
            try
            {
                var result = method.Invoke(CQPClient, args);
                if (result != null && result is Int32Value int32Value)
                {
                    return int32Value.Value;
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
                catch
                {
                }
                finally
                {
                    ResponseStream = null;
                    RequestStream = null;
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
                // log
                return;
            }
            Task.Run(() =>
            {
                try
                {
                    MethodInfo unpackMethodInfo = typeof(Any).GetMethod("Unpack", System.Type.EmptyTypes);
                    MethodInfo genericUnpackMethod = unpackMethodInfo.MakeGenericMethod(argumentType);
                    
                    object argument = genericUnpackMethod.Invoke(response.Response, null); // 调用UnPack
                    object[] args = argument.GetType().GetProperties().Where(x => x.CanWrite).Select(x => x.GetValue(argument)).ToArray();

                    if(typeName == "HeartBeatRequest")
                    {
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
                catch(Exception e)
                {
                    LogHelper.Error("InvokeEvents", e);
                    lock (writeLock)
                    {
                        RequestStream.WriteAsync(new StreamRequest
                        {
                            WaitID = response.WaitID,
                            Request = 0
                        }).Wait();
                    }
                }
            });
        }
    }
}