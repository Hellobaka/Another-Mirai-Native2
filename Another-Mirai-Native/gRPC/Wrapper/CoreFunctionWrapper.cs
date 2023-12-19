using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Collections.Concurrent;
using System.Reflection;

namespace Another_Mirai_Native.gRPC
{
    public class CoreFunctionWrapper : CoreFunctions.CoreFunctionsBase
    {
        public static ConcurrentDictionary<int, IAsyncStreamReader<StreamRequest>> ClientStreams { get; set; } = new();

        public static ConcurrentDictionary<int, IServerStreamWriter<StreamResponse>> ServerStreams { get; set; } = new();

        public static ConcurrentDictionary<int, HeartBeatInfo> ConnectionAliveStatus { get; set; } = new();

        public static ConcurrentDictionary<int, int> WaitingResults { get; set; } = new();

        private static int SyncID { get; set; } = 1;

        private static System.Timers.Timer HeartBeatTimer { get; set; }

        public override async Task InvokeCQPEvents(IAsyncStreamReader<StreamRequest> requestStream,
            IServerStreamWriter<StreamResponse> responseStream, ServerCallContext context)
        {
            if (HeartBeatTimer == null)
            {
                HeartBeatTimer.Interval = AppConfig.HeartBeatInterval;
                HeartBeatTimer.Elapsed += HeartBeatTimer_Elapsed;
                HeartBeatTimer.Enabled = true;
                HeartBeatTimer.Start();
            }
            var authCodeHeader = context.RequestHeaders.FirstOrDefault(x => x.Key == "authCode");
            if (authCodeHeader != null && int.TryParse(authCodeHeader.Value, out int authCode))
            {
                ClientStreams.AddOrUpdate(authCode, requestStream, (key, oldValue) => requestStream);
                ServerStreams.AddOrUpdate(authCode, responseStream, (key, oldValue) => responseStream);
                ConnectionAliveStatus.AddOrUpdate(authCode, new HeartBeatInfo
                {
                    Key = authCode,
                    Alive = true,
                    CurrentErrorCount = 0
                }, (key, oldValue) => new HeartBeatInfo
                {
                    Key = authCode,
                    Alive = true,
                    CurrentErrorCount = 0
                });
            }
            else// 未包含Header的请求 忽略
            {
                return;
            }
            try
            {
                while (await requestStream.MoveNext())
                {
                    var request = requestStream.Current;
                    if (request.WaitID == 0)
                    {
                        Send(authCode, new StreamResponse
                        {
                            WaitID = 0,
                            Response = Any.Pack(new HeartBeatRequest { Timestamp = Helper.TimeStamp })
                        });
                        if (ConnectionAliveStatus.TryGetValue(authCode, out var status))
                        {
                            status.CurrentErrorCount = 0;
                            status.Alive = true;
                        }
                    }
                    else
                    {
                        WaitingResults.AddOrUpdate(request.WaitID, request.Request, (key, oldValue) => request.Request);
                        RequestWaiter.TriggerByKey(request.WaitID);
                    }
                }
            }
            catch (Exception e)
            {
                // TODO: log
                // 考虑是否需要重置插件来源等待
            }
            finally
            {
            }
        }

        private void HeartBeatTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var item in ConnectionAliveStatus)
            {
                item.Value.CurrentErrorCount++;
                if (item.Value.CurrentErrorCount > 5)
                {
                    item.Value.Alive = false;

                }
            }
        }

        public static int Invoke(CQPluginProxy target, PluginEventType eventType, object[] args)
        {
            StreamResponse response = new()
            {
                WaitID = ++SyncID
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

                MethodInfo unpackMethodInfo = typeof(Any).GetMethod("Pack");
                MethodInfo genericUnpackMethod = unpackMethodInfo.MakeGenericMethod(argumentType);

                Any any = (Any)genericUnpackMethod.Invoke(null, new object[] { instance }); // 调用Pack
                response.Response = any;
                if (Send(target.AppInfo.AuthCode, response)
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

        public static bool Send(int authCode, StreamResponse response)
        {
            if (!ServerStreams.TryGetValue(authCode, out var serverStream))
            {
                // log
                return false;
            }
            try
            {
                serverStream.WriteAsync(response).Wait();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override Task<Empty> ClientStartup(ClientStartupRequest request, ServerCallContext context)
        {
            var authCodeHeader = context.RequestHeaders.FirstOrDefault(x => x.Key == "authCode");
            if (authCodeHeader != null && int.TryParse(authCodeHeader.Value, out int authCode))
            {
                // 实现
            }
            return Task.FromResult(new Empty());
        }

        public override Task<Int32Value> AddLog(AddLogRequest request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value()
            {
                Value = LogHelper.WriteLog(new LogModel
                {
                    id = request.Id,
                    detail = request.Detail,
                    name = request.Name,
                    priority = request.Priority,
                    source = request.Source,
                    status = request.Status,
                    time = request.Time
                })
            });
        }

        public override Task<StringValue> GetCoreVersion(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new StringValue() { Value = GetType().Assembly.GetName().Version.ToString() });
        }

        public override Task<Int32Value> DisablePlugin(Int32Value request, ServerCallContext context)
        {
            return base.DisablePlugin(request, context);
        }

        public override Task<Int32Value> EnablePlugin(Int32Value request, ServerCallContext context)
        {
            return base.EnablePlugin(request, context);
        }

        public override Task<Int32Value> Restart(Int32Value request, ServerCallContext context)
        {
            return base.Restart(request, context);
        }

        public override Task<PluginResponse> GetAllPlugins(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new PluginResponse()
            {
                
            });
        }
    }
}
