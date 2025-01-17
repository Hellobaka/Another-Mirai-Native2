﻿using Another_Mirai_Native.Config;
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

                    byte[] buffer = Encoding.UTF8.GetBytes(message + "\0");
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
            var stringBuilder = new StringBuilder();
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

                            // 处理消息
                            new Thread(() => HandleMessage(message)).Start();
                        }

                        // 将剩余的部分放回 StringBuilder
                        stringBuilder.Clear();
                        stringBuilder.Append(completeMessage);
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