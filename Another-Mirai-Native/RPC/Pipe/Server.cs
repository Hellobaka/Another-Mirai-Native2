using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC.Interface;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;

namespace Another_Mirai_Native.RPC.Pipe
{
    public class NamedPipe : IDisposable
    {
        private readonly object _sendLock = new();

        public string GUID => Guid.NewGuid().ToString();

        public NamedPipeServerStream ServerInstance { get; set; }

        public int PID { get; set; }

        /// <summary>
        /// 同一连接上的并发写入会让两个帧交错，必须串行化。
        /// </summary>
        public void Send(string message)
        {
            lock (_sendLock)
            {
                NamedPipeServerStream? stream = ServerInstance;
                if (stream == null || !stream.IsConnected)
                {
                    return;
                }
                PipeFraming.WriteFrame(stream, Encoding.UTF8.GetBytes(message));
            }
        }

        public void Dispose()
        {
            ServerInstance?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Disconnect()
        {
            try
            {
                if (ServerInstance != null && ServerInstance.IsConnected)
                {
                    ServerInstance.Disconnect();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("", ex);
            }
        }
    }

    public class Server : ServerBase
    {
        private static string PipeName => $"Another_Mirai_Native2_NamedPipe_{Process.GetCurrentProcess().Id}";

        private bool IsRunning { get; set; }

        private readonly CancellationTokenSource _cts = new();

        /// <summary>正在等待连接的那个实例；Stop() 通过 Dispose 它来打断等待。</summary>
        private NamedPipeServerStream? _pendingAccept;

        public event Action<NamedPipe> ClientConnected;
        public event Action<NamedPipe> ClientDisconnected;

        private ConcurrentDictionary<NamedPipe, byte> Pipes { get; } = new();

        public override bool Start()
        {
            try
            {
                IsRunning = true;
                _ = Task.Run(ListenForClientsAsync);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool Stop()
        {
            IsRunning = false;
            TryDispose(_pendingAccept);
            _pendingAccept = null;
            try
            {
                _cts.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }
            foreach (NamedPipe pipe in Pipes.Keys)
            {
                try
                {
                    pipe.Dispose();
                }
                catch { }
            }
            Pipes.Clear();
            return true;
        }

        private async Task ListenForClientsAsync()
        {
            while (IsRunning)
            {
                // 自己做了 4 字节长度头分帧，所以必须用字节模式：
                // Message 模式的读取语义不同，会让分帧更容易出错。
                NamedPipeServerStream pipeServer = new(PipeName, PipeDirection.InOut, -1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                _pendingAccept = pipeServer;
                try
                {
                    await pipeServer.WaitForConnectionAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    LogHelper.Debug("Pipe_Accept", $"接受连接结束: {e.Message}");
                    TryDispose(pipeServer);
                    break;
                }
                _pendingAccept = null;

                if (!IsRunning)
                {
                    TryDispose(pipeServer);
                    break;
                }

                var connection = new NamedPipe
                {
                    ServerInstance = pipeServer
                };
                Pipes.TryAdd(connection, 0);

                ClientConnected?.Invoke(connection);
                _ = Task.Run(() => HandleClient(connection));
            }
        }

        private static void TryDispose(NamedPipeServerStream? stream)
        {
            if (stream == null)
            {
                return;
            }
            try
            {
                stream.Dispose();
            }
            catch
            {
            }
        }

        private async Task HandleClient(NamedPipe pipe)
        {
            NamedPipeServerStream client = pipe.ServerInstance;
            while (IsRunning)
            {
                byte[]? payload;
                try
                {
                    payload = await PipeFraming.ReadFrameAsync(client, _cts.Token).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    break;
                }

                if (payload == null)
                {
                    break;
                }

                string message = Encoding.UTF8.GetString(payload);
                LogHelper.Debug("收到客户端消息", message);
                _ = Task.Run(() => HandleClientMessage(message, pipe));
            }

            try
            {
                ClientDisconnected?.Invoke(pipe);
                LogPipeDisconnect(pipe);
                pipe.Disconnect();
            }
            finally
            {
                // 清理必须放在 finally：否则上面任何一处抛出都会把管道永久留在 Pipes 里
                Pipes.TryRemove(pipe, out _);
                pipe.Dispose();
            }
        }

        private void LogPipeDisconnect(NamedPipe pipe)
        {
            var pid = Connections.FirstOrDefault(kvp => kvp.Value == pipe).Key;
            if (pid == 0)
            {
                return;
            }

            try
            {
                if (!Process.GetProcessById(pid).HasExited)
                {
                    LogHelper.Error("Pipe_Disconnect", $"与插件连接断开");
                }
            }
            catch
            {
                // 进程已退出
            }
            finally
            {
                var proxy = PluginManagerProxy.Proxies.FirstOrDefault(x => x.PluginProcess?.Id == pid);
                if (proxy != null)
                {
                    RequestWaiter.ResetSignalByPluginProxy(proxy);
                }
                RequestWaiter.ResetSignalByProcess(pid);
            }
        }

        private void HandleClientMessage(string message, NamedPipe server)
        {
            try
            {
                JObject json = JObject.Parse(message);
                if (json.ContainsKey("Args"))
                {
                    InvokeBody? caller = json.ToObject<InvokeBody>();
                    HandleInvokeBody(caller, server);
                }
                else
                {
                    InvokeResult? result = json.ToObject<InvokeResult>();
                    HandleInvokeResult(result, server);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理插件消息", ex);
            }
        }

        public override void SendMessage(object connection, string message)
        {
            if (connection is NamedPipe pipe && !string.IsNullOrEmpty(message))
            {
                pipe.Send(message);
            }
        }

        public override bool SetConnectionConfig()
        {
            return true;
        }

        public override void DropConnection(object connection)
        {
            if (connection is NamedPipe pipe)
            {
                try
                {
                    pipe.Disconnect();
                    pipe.Dispose();
                }
                catch { }
            }
        }
    }
}
