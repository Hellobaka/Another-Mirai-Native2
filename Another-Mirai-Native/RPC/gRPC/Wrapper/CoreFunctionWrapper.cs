using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Media;

namespace Another_Mirai_Native.gRPC
{
    public class CoreFunctionWrapper : CoreFunctions.CoreFunctionsBase
    {
        private static object writeLock = new object();

        public static ConcurrentDictionary<int, IAsyncStreamReader<StreamRequest>> ClientStreams { get; set; } = new();

        public static ConcurrentDictionary<int, IServerStreamWriter<StreamResponse>> ServerStreams { get; set; } = new();

        public static ConcurrentDictionary<int, HeartBeatInfo> ConnectionAliveStatus { get; set; } = new();

        private static System.Timers.Timer HeartBeatTimer { get; set; }

        public override async Task InvokeCQPEvents(IAsyncStreamReader<StreamRequest> requestStream,
            IServerStreamWriter<StreamResponse> responseStream, ServerCallContext context)
        {
            if (HeartBeatTimer == null)
            {
                HeartBeatTimer = new System.Timers.Timer();
                HeartBeatTimer.Interval = AppConfig.HeartBeatInterval;
                HeartBeatTimer.Elapsed += HeartBeatTimer_Elapsed;
                HeartBeatTimer.Enabled = true;
                HeartBeatTimer.Start();
            }
            var authCodeHeader = context.RequestHeaders.FirstOrDefault(x => x.Key == "authcode");
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
                LogHelper.WriteLog(LogLevel.Debug, "AMN框架", "调用事件", messages: "Header为空", "");
                return;
            }
            try
            {
                while (await requestStream.MoveNext())
                {
                    var request = requestStream.Current;
                    if (request.WaitID == 0) // 心跳信息
                    {
                        // 发送心跳反馈
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
                        Server.WaitingResults.AddOrUpdate(request.WaitID, request.Request, (key, oldValue) => request.Request);
                        RequestWaiter.TriggerByKey($"EventID_{request.WaitID}");
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("服务端流异常", e);
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
                    var proxy = PluginManagerProxy.GetProxyByAuthCode(item.Key);
                    if (proxy != null)
                    {
                        ServerManager.Server.ClientDisconnect(proxy);
                    }
                }
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
                lock (writeLock)
                {
                    serverStream.WriteAsync(response).Wait();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override Task<Empty> ClientStartup(ClientStartupRequest request, ServerCallContext context)
        {
            var authCodeHeader = context.RequestHeaders.FirstOrDefault(x => x.Key == "authcode");
            if (authCodeHeader != null && int.TryParse(authCodeHeader.Value, out int authCode))
            {
                ServerManager.Server.ClientStartup(request.Pid, request.AppId);
            }
            return Task.FromResult(new Empty());
        }

        public override Task<Int32Value> AddLog(AddLogRequest request, ServerCallContext context)
        {
            var result = ServerManager.Server.AddLog(new LogModel
            {
                id = request.Id,
                detail = request.Detail,
                name = request.Name,
                priority = request.Priority,
                source = request.Source,
                status = request.Status,
                time = request.Time
            });
            return Task.FromResult(new Int32Value()
            {
                Value = result ?? 0
            });
        }

        public override Task<StringValue> GetCoreVersion(Empty request, ServerCallContext context)
        {
            var result = ServerManager.Server.GetCoreVersion();
            if (string.IsNullOrEmpty(result))
            {
                result = "";
            }
            return Task.FromResult(new StringValue() { Value = result });
        }

        public override Task<Int32Value> DisablePlugin(Int32Value request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value() { Value = ServerManager.Server.DisablePlugin(request.Value) ? 1 : 0 });
        }

        public override Task<Int32Value> EnablePlugin(Int32Value request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value() { Value = ServerManager.Server.EnablePlugin(request.Value) ? 1 : 0 });
        }

        public override Task<Int32Value> Restart(Int32Value request, ServerCallContext context)
        {
            return Task.FromResult(new Int32Value() { Value = ServerManager.Server.RestartPlugin(request.Value) ? 1 : 0 });
        }

        public override Task<PluginResponse> GetAllPlugins(Empty request, ServerCallContext context)
        {
            var r = new PluginResponse();
            foreach (var item in ServerManager.Server.GetAllPlugins())
            {
                var plugin = new PluginResponse_AppInfo
                {
                    Apiver = item.AppInfo.apiver,
                    AppId = item.AppInfo.AppId,
                    AuthCode = item.AppInfo.AuthCode,
                    Author = item.AppInfo.author,
                    Description = item.AppInfo.description,
                    Name = item.AppInfo.name,
                    Ret = item.AppInfo.ret,
                    Version = item.AppInfo.version,
                    VersionId = item.AppInfo.version_id,
                };
                foreach (var auth in item.AppInfo.auth)
                {
                    plugin.Auth.Add(auth);
                }
                foreach (var menu in item.AppInfo.menu)
                {
                    plugin.Menu.Add(new PluginResponse_Menu
                    {
                        Function = menu.function,
                        Name = menu.name
                    });
                }
                foreach (var @event in item.AppInfo._event)
                {
                    plugin.Event.Add(new PluginResponse_Event
                    {
                        Name = @event.name,
                        Function = @event.function,
                        Id = @event.id,
                        Priority = @event.priority,
                        Type = @event.type
                    });
                }
            }
            return Task.FromResult(r);
        }

        public override Task<AppConfigResponse> GetAppConfig(StringValue request, ServerCallContext context)
        {
            var config = ServerManager.Server.GetAppConfig(request.Value);
            return Task.FromResult(new AppConfigResponse
            {
                ConfigType = config.GetType().Name,
                ConfigValue = config.ToString()
            });
        }

        public override Task<Empty> ShowErrorDialog(ErrorDialog request, ServerCallContext context)
        {
            ServerManager.Server.ShowErrorDialog(request.Guid, request.AuthCode, request.Title, request.Content, request.CanIgnore);
            return Task.FromResult(new Empty());
        }
    }
}
