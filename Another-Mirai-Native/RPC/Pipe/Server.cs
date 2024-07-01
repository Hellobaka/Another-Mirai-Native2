using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC.Interface;
using Fleck;
using Newtonsoft.Json.Linq;
using System.IO.Pipes;
using System.Text;

namespace Another_Mirai_Native.RPC.Pipe
{
    public class NamedPipe : IDisposable
    {
        public string GUID => Guid.NewGuid().ToString();

        public NamedPipeServerStream ServerInstance { get; set; }

        public int PID { get; set; }

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
        private static string PipeName => "Another_Mirai_Native2_NamedPipe";

        private bool IsRunning { get; set; }

        public event Action<NamedPipe> ClientConnected;
        public event Action<NamedPipe> ClientDisconnected;

        private List<NamedPipe> Pipes { get; set; } = [];

        public override bool Start()
        {
            try
            {
                IsRunning = true;
                Task.Run(() => ListenForClients());
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
            foreach (NamedPipe pipe in Pipes)
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

        private async void ListenForClients()
        {
            while (IsRunning)
            {
                var pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, -1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                await pipeServer.WaitForConnectionAsync();

                var connection = new NamedPipe
                {
                    ServerInstance = pipeServer
                };
                Pipes.Add(connection);

                ClientConnected?.Invoke(connection);
                _ = Task.Run(() => HandleClient(connection));
            }
        }

        private async Task HandleClient(NamedPipe pipe)
        {
            var buffer = new byte[1024];
            var client = pipe.ServerInstance;
            while (IsRunning && client.IsConnected)
            {
                try
                {
#if NET5_0_OR_GREATER
                    int bytesRead = await client.ReadAsync(new Memory<byte>(buffer));
#else
                    int bytesRead = await client.ReadAsync(buffer, 0, buffer.Length);
#endif
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        LogHelper.Debug("收到客户端消息", message);
                        Task.Run(() => HandleClientMessage(message, pipe));
                    }
                    else
                    {
                        ClientDisconnected?.Invoke(pipe);
                        pipe.Disconnect();
                        break;
                    }
                }
                catch (Exception)
                {
                    ClientDisconnected?.Invoke(pipe);
                    pipe.Disconnect();
                    break;
                }
            }

            pipe.Dispose();
            Pipes.Remove(pipe);
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
                SendMessage(client: pipe.ServerInstance, message).Wait();
            }
        }

        public override bool SetConnectionConfig()
        {
            return true;
        }

        public async Task SendMessage(NamedPipeServerStream client, string message)
        {
            if (client.IsConnected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
#if NET5_0_OR_GREATER
                await client.WriteAsync(new Memory<byte>(buffer));
#else
                await client.WriteAsync(buffer, 0, buffer.Length);
#endif
            }
        }
    }
}
