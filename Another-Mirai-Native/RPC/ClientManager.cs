using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.RPC.Interface;

namespace Another_Mirai_Native.RPC
{
    public class ClientManager
    {
        public static ClientBase Client { get; set; }

        private System.Timers.Timer HeartBeatTimer { get; set; }

        public bool Build(ServerType serverType)
        {
            try
            {
                Client = serverType switch
                {
                    ServerType.gRPC => new gRPC.Client(),
                    ServerType.WebSocket => new WebSocket.Client(),
                    _ => throw new NotImplementedException()
                };
                if (HeartBeatTimer == null)
                {
                    HeartBeatTimer = new System.Timers.Timer();
                    HeartBeatTimer.Elapsed += HeartBeatTimer_Elapsed;
                    HeartBeatTimer.Interval = AppConfig.HeartBeatInterval;
                    HeartBeatTimer.Start();
                }
                return true;
            }
            catch(Exception e)
            {
                LogHelper.Debug("BuildClient", e.Message + e.StackTrace);
                return false;
            }
        }

        private void HeartBeatTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(Client == null)
            {
                return;
            }
            Client.HeartBeatLostCount++;
            if (Client.HeartBeatLostCount > 5)
            {
                Client.Close();
                Client.Connect();
            }
            Client.SendHeartBeat();
        }
    }
}
