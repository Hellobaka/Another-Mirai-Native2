using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.RPC.Interface;
using System.IO.Pipes;
using System.Text;

namespace Another_Mirai_Native.RPC.Pipe
{
    public class Client : ClientBase
    {
        private object _lock = new object();

        public int ReconnectCount { get; private set; }

        public bool Reconnecting { get; private set; }

        private static string PipeName => $"Another_Mirai_Native2_NamedPipe_{AppConfig.Instance.Core_PID}";

        private NamedPipeClientStream PipeClient { get; set; }

        public override void Close()
        {
            try
            {
                PipeClient?.Close();
                PipeClient?.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public override bool Connect()
        {
            try
            {
                PipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                PipeClient.Connect(5000);
                LogHelper.Debug("连接服务端", "连接成功");
                LogHelper.LocalDebug("Pipe_Connect", "Connection Ok.");
                Task.Run(ListenForMessages);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override void Send(string message)
        {
            lock (_lock) 
            {
                SendMessage(message);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                if (PipeClient.IsConnected)
                {
                    LogHelper.LocalDebug("Pipe_Send", message);

                    byte[] buffer = Encoding.UTF8.GetBytes(message).Concat(Delimiter).ToArray();
#if NET5_0_OR_GREATER
                    PipeClient.Write(new ReadOnlySpan<byte>(buffer));
#else
                    PipeClient.Write(buffer, 0, buffer.Length);
#endif
                    LogHelper.LocalDebug("Pipe_Send", "Send Ok.");
                }
            }
            catch(Exception e)
            {
                LogHelper.LocalDebug("Pipe_Send_Error", e.Message);
            }
        }

        public override bool SetConnectionConfig()
        {
            return true;
        }

        private void HandleClose()
        {
            bool r = false;
            lock (_lock)
            {
                LogHelper.LocalDebug("Websocket_Close", "Connection Lost...");

                ReconnectCount++;
                LogHelper.Error("与服务器连接断开", $"{AppConfig.Instance.ReconnectTime} ms后重新连接...");
                RequestWaiter.ResetSignalByConnection(PipeClient);
                Thread.Sleep(AppConfig.Instance.ReconnectTime);

                r = Connect();
            }
            if (r)
            {
                Reconnecting = false;
                UpdateConnection();
            }
            else
            {
                HandleClose();
            }
        }

        private async Task ListenForMessages()
        {
            var buffer = new byte[1024];
            var messageBuffer = new List<byte>();

            while (PipeClient.IsConnected)
            {
                try
                {
#if NET5_0_OR_GREATER
                    int bytesRead = await PipeClient.ReadAsync(new Memory<byte>(buffer));
#else
                    int bytesRead = await PipeClient.ReadAsync(buffer, 0, buffer.Length);
#endif
                    LogHelper.LocalDebug("Pipe_Receive", "");
                    if (bytesRead > 0)
                    {
                        messageBuffer.AddRange(buffer.Take(bytesRead));

                        int delimiterIndex;
                        while ((delimiterIndex = messageBuffer.IndexOf(Delimiter)) != -1)
                        {
                            // 提取完整消息
                            var messageBytes = messageBuffer.Take(delimiterIndex).ToArray();
                            string message = Encoding.UTF8.GetString(messageBytes);

                            // 移除已处理的消息部分
                            messageBuffer.RemoveRange(0, delimiterIndex + Delimiter.Length);

                            // 处理消息
                            new Thread(() => HandleMessage(message)).Start();
                        }
                    }
                    else
                    {
                        HandleClose();
                        Close();
                        break;
                    }
                }
                catch (Exception)
                {
                    HandleClose();
                    Close();
                    break;
                }
            }
        }
    }
}