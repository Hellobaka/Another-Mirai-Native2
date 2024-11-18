using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.RPC.Interface;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
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
        private static string PipeName => $"Another_Mirai_Native2_NamedPipe_{Process.GetCurrentProcess().Id}";

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
            var stringBuilder = new StringBuilder();
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
                        string messagePart = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        stringBuilder.Append(messagePart);

                        // 查找完整消息的定界符
                        string completeMessage = stringBuilder.ToString();
                        int nullCharIndex;
                        while ((nullCharIndex = completeMessage.IndexOf('\0')) != -1)
                        {
                            // 提取完整消息
                            string message = completeMessage.Substring(0, nullCharIndex);
                            // 移除已处理的消息部分
                            completeMessage = completeMessage.Substring(nullCharIndex + 1);

                            LogHelper.Debug("收到客户端消息", message);
                            // 处理消息
                            new Thread(() => HandleClientMessage(message, pipe)).Start();
                        }

                        // 将剩余的部分放回 StringBuilder
                        stringBuilder.Clear();
                        stringBuilder.Append(completeMessage);
                    }
                    else
                    {
                        ClientDisconnected?.Invoke(pipe);
                        pipe.Disconnect();
                        LogHelper.Error("Pipe_Disconnect", "与插件连接断开");
                        break;
                    }
                }
                catch (Exception)
                {
                    ClientDisconnected?.Invoke(pipe);
                    pipe.Disconnect();
                    LogHelper.Error("Pipe_Disconnect", "与插件连接断开");
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
                SendMessage(client: pipe.ServerInstance, message);
            }
        }

        public override bool SetConnectionConfig()
        {
            return true;
        }

        public void SendMessage(NamedPipeServerStream client, string message)
        {
            if (client.IsConnected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message + "\0");
#if NET5_0_OR_GREATER
                client.Write(new ReadOnlySpan<byte>(buffer));
#else
                client.Write(buffer, 0, buffer.Length);
#endif
            }
        }
    }
}
