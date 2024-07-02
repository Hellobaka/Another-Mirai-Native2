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

        private static string PipeName => "Another_Mirai_Native2_NamedPipe";

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
                SendMessage(message).Wait();
            }
        }

        public async Task SendMessage(string message)
        {
            try
            {
                if (PipeClient.IsConnected)
                {
                    LogHelper.LocalDebug("Pipe_Send", message);

                    byte[] buffer = Encoding.UTF8.GetBytes(message);
#if NET5_0_OR_GREATER
                    await PipeClient.WriteAsync(new Memory<byte>(buffer));
#else
                    await PipeClient.WriteAsync(buffer, 0, buffer.Length);
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
            }
            else
            {
                HandleClose();
            }
        }

        private async Task ListenForMessages()
        {
            var buffer = new byte[65535];
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
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        new Thread(() => HandleMessage(message)).Start();
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