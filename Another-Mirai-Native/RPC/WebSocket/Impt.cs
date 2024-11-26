using System.Net.WebSockets;
using System.Text;

namespace Another_Mirai_Native.RPC.WebSocket
{
    public class WebSocketClient
    {
        private CancellationTokenSource cts;
        private object _sendLock = new();

        public event Action<string> OnMessage;
        public event Action<byte[]> OnData;
        public event Action OnOpen;
        public event Action OnClose;
        public event Action<Exception> OnError;

        public WebSocketClient(string uri)
        {
            ServerUri = new Uri(uri);
        }

        public WebSocketState ReadyState => Client?.State ?? WebSocketState.None;

        public Dictionary<string, string> CustomHeader { get; set; } = new Dictionary<string, string>();

        private Uri ServerUri { get; set; }

        private ClientWebSocket Client { get; set; }

        public void Connect()
        {
            Client = new ClientWebSocket();
            cts = new CancellationTokenSource();

            try
            {
                foreach (var item in CustomHeader)
                {
                    Client.Options.SetRequestHeader(item.Key, item.Value);
                }
                Client.ConnectAsync(ServerUri, cts.Token).Wait();
                OnOpen?.Invoke();
                _ = Task.Run(() => ReceiveMessagesAsync());
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                Close();
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];
            var stringBuilder = new StringBuilder();

            try
            {
                while (Client.State == WebSocketState.Open)
                {
                    var result = await Client.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedChunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        stringBuilder.Append(receivedChunk);

                        if (result.EndOfMessage)
                        {
                            string receivedMessage = stringBuilder.ToString();
                            OnMessage?.Invoke(receivedMessage);
                            stringBuilder.Clear();
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        byte[] receivedData = new byte[result.Count];
                        Array.Copy(buffer, receivedData, result.Count);
                        OnData?.Invoke(receivedData);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                Close();
            }
        }

        public void Send(string message)
        {
            lock (_sendLock)
            {
                try
                {
                    if (Client != null && Client.State == WebSocketState.Open)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(message);
                        Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cts.Token).Wait();
                    }
                }
                catch (Exception e)
                {
                    OnError?.Invoke(e);
                    Close();
                }
            }
        }

        public void Send(byte[] data)
        {
            lock (_sendLock)
            {
                try
                {
                    if (Client != null && Client.State == WebSocketState.Open)
                    {
                        Client.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, cts.Token).Wait();
                    }
                }
                catch (Exception e)
                {
                    OnError?.Invoke(e);
                    Close();
                }
            }
        }

        public void Close()
        {
            if (Client == null)
            {
                return;
            }
            try
            {
                Client.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Websocket Error, reconnect requested.", new());
            }
            catch (Exception  ex)
            {
                OnError?.Invoke(ex);
            }
            finally
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;

                Client?.Dispose();
                Client = null;

                OnClose?.Invoke();
            }
        }
    }
}
