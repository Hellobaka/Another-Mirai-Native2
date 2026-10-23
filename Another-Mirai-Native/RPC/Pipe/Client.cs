using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.RPC.Interface;
using System.IO.Pipes;
using System.Text;

namespace Another_Mirai_Native.RPC.Pipe
{
    public class Client : ClientBase
    {
        private readonly object _lock = new();

        public int ReconnectCount { get; private set; }

        public bool Reconnecting { get; private set; }

        private static string PipeName => $"Another_Mirai_Native2_NamedPipe_{AppConfig.Instance.Core_PID}";

        private NamedPipeClientStream? PipeClient { get; set; }

        public override void Close()
        {
            lock (_lock)
            {
                NamedPipeClientStream? pipe = PipeClient;
                PipeClient = null;
                if (pipe == null)
                {
                    return;
                }
                try
                {
                    pipe.Close();
                    pipe.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        public override void Reconnect()
        {
            lock (_lock)
            {
                if (Reconnecting)
                {
                    return;
                }
            }
            _ = Task.Run(ReconnectLoop);
        }

        private void ReconnectLoop()
        {
            try
            {
                while (true)
                {
                    Reconnecting = true;
                    Close();
                    ReconnectCount++;
                    LogHelper.Error("与服务器连接断开", $"{AppConfig.Instance.ReconnectTime} ms后重新连接...");
                    Thread.Sleep(AppConfig.Instance.ReconnectTime);

                    if (Connect())
                    {
                        Reconnecting = false;
                        UpdateConnection();
                        return;
                    }
                }
            }
            finally
            {
                Reconnecting = false;
            }
        }

        public override bool Connect()
        {
            try
            {
                NamedPipeClientStream pipe = new(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                pipe.Connect(5000);
                PipeClient = pipe;
                // 释放等待
                Connection = pipe;
                HeartBeatLostCount = 0;
                LogHelper.Debug("连接服务端", "连接成功");
                LogHelper.LocalDebug("Pipe_Connect", "Connection Ok.");
                _ = Task.Run(() => ListenForMessages(pipe));
                return true;
            }
            catch (Exception e)
            {
                LogHelper.LocalDebug("Pipe_Connect_Error", e.Message);
                return false;
            }
        }

        public override void Send(string message)
        {
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            lock (_lock)
            {
                try
                {
                    NamedPipeClientStream? pipe = PipeClient;
                    if (pipe == null || !pipe.IsConnected)
                    {
                        return;
                    }

                    LogHelper.LocalDebug("Pipe_Send", message);
                    PipeFraming.WriteFrame(pipe, Encoding.UTF8.GetBytes(message));
                    LogHelper.LocalDebug("Pipe_Send", "Send Ok.");
                }
                catch (Exception e)
                {
                    LogHelper.LocalDebug("Pipe_Send_Error", e.Message);
                }
            }
        }

        public override bool SetConnectionConfig()
        {
            return true;
        }

        private async Task ListenForMessages(NamedPipeClientStream pipe)
        {
            while (true)
            {
                byte[]? payload;
                try
                {
                    payload = await PipeFraming.ReadFrameAsync(pipe, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    LogHelper.LocalDebug("Pipe_Receive_Error", e.Message);
                    break;
                }

                if (payload == null)
                {
                    LogHelper.LocalDebug("Pipe_Close", "Connection Lost...");
                    break;
                }

                LogHelper.LocalDebug("Pipe_Receive", payload.Length.ToString());
                string message = Encoding.UTF8.GetString(payload);
                LogHelper.Debug("收到服务端消息", message);
                _ = Task.Run(() => HandleMessage(message));
            }

            RequestWaiter.ResetSignalByConnection(pipe);

            // 只有当前这条管道仍是在用的那条时才重连；
            // 否则旧的读循环会把刚建立好的新连接一起拆掉。
            bool isCurrent;
            lock (_lock)
            {
                isCurrent = ReferenceEquals(PipeClient, pipe);
            }
            if (isCurrent)
            {
                Reconnect();
            }
        }
    }
}