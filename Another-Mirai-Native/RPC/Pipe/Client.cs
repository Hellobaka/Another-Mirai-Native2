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

                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    buffer = [.. BitConverter.GetBytes(buffer.Length), .. buffer];
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
            using MemoryStream messageBuffer = new();
            int targetLength = -1, readLength = 0;

            while (PipeClient.IsConnected)
            {
                bool lengthRead = false;
                try
                {
#if NET5_0_OR_GREATER
                    int bytesRead = await PipeClient.ReadAsync(new Memory<byte>(buffer));
#else
                    int bytesRead = await PipeClient.ReadAsync(buffer, 0, buffer.Length);
#endif
                    LogHelper.LocalDebug("Pipe_Receive", bytesRead.ToString());
                    if (bytesRead > 0)
                    {
                        if (targetLength < 0 && bytesRead > 4)
                        {
                            targetLength = BitConverter.ToInt32(buffer, 0);
                            lengthRead = true;
                        }
                        if (targetLength <= 0)
                        {
                            throw new InvalidDataException("Pipe 获得的数据长度无效");
                        }

                        if (readLength < targetLength)
                        {
                            if (lengthRead)
                            {
                                messageBuffer.Write(buffer, 4, bytesRead - 4);
                                readLength += bytesRead - 4;
                            }
                            else
                            {
                                messageBuffer.Write(buffer, 0, bytesRead);
                                readLength += bytesRead;
                            }

                            if (readLength < targetLength)
                            {
                                continue;
                            }
                        }
#if NET5_0_OR_GREATER
                        string message = Encoding.UTF8.GetString(new ReadOnlySpan<byte>(messageBuffer.ToArray(), 0, targetLength));
#else
                        string message = Encoding.UTF8.GetString(messageBuffer.ToArray(), 0, targetLength);
#endif
                        LogHelper.Debug("收到客户端消息", message);
                        targetLength = -1;
                        readLength = 0;
                        messageBuffer.Position = 0;
                        // 处理消息
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