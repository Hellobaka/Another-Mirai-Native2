using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System.Reflection;

namespace Another_Mirai_Native.gRPC
{
    public class Client
    {
        public Client()
        {
            Instance = this;
        }
        public static Client Instance { get; set; }
        public Channel GrpcChannel { get; set; }

        public CQPFunctions.CQPFunctionsClient CQPClient { get; set; }

        public CoreFunctions.CoreFunctionsClient CoreClient { get; set; }

        public IClientStreamWriter<StreamRequest> RequestStream { get; set; }

        public IAsyncStreamReader<StreamResponse> ResponseStream { get; set; }

        public static Metadata Headers => new()
        {
            { "authCode", AppConfig.Core_AuthCode.ToString() }
        };

        private static System.Timers.Timer HeartBeatTimer { get; set; }

        // TODO: 抽离通用Server接口
        public bool Connect(string ip, int port)
        {
            try
            {
                GrpcChannel = new Channel($"{ip}:{port}", ChannelCredentials.Insecure);
                CQPClient = new CQPFunctions.CQPFunctionsClient(GrpcChannel);
                CoreClient = new CoreFunctions.CoreFunctionsClient(GrpcChannel);
                HeartBeatTimer = new();
                HeartBeatTimer.Interval = AppConfig.HeartBeatInterval;
                HeartBeatTimer.Elapsed += HeartBeatTimer_Elapsed;
                HeartBeatTimer.Enabled = true;
                HeartBeatTimer.Start();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        private void HeartBeatTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            RequestStream?.WriteAsync(new StreamRequest
            {
                WaitID = 0
            }).Wait();
        }

        public bool Disconnect()
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
            return true;
        }

        public void StartStream()
        {
            using var call = CoreClient.InvokeCQPEvents(headers: Headers);
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
            Task.Run(async () =>
            {
                try
                {
                    MethodInfo unpackMethodInfo = typeof(Any).GetMethod("Unpack");
                    MethodInfo genericUnpackMethod = unpackMethodInfo.MakeGenericMethod(argumentType);

                    object argument = genericUnpackMethod.Invoke(response.Response, null); // 调用UnPack
                    object[] args = argument.GetType().GetProperties().Where(x => x.CanWrite).Select(x => x.GetValue(argument)).ToArray();

                    int result = PluginManager.Instance.CallEvent(
                        (PluginEventType)System.Enum.Parse(typeof(PluginEventType), typeName.Replace("_Parameters", "").Replace("Event_On", ""))
                        , args);

                    await RequestStream.WriteAsync(new StreamRequest
                    {
                        WaitID = response.WaitID,
                        Request = result
                    });
                }
                catch
                {
                    // log
                    await RequestStream.WriteAsync(new StreamRequest
                    {
                        WaitID = response.WaitID,
                        Request = 0
                    });
                }
            });
        }
    }
}