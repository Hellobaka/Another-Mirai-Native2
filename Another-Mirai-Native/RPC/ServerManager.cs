using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.RPC.Interface;

namespace Another_Mirai_Native.RPC
{
    public class ServerManager
    {
        public static ServerBase Server { get; set; }

        public bool Build(ServerType serverType)
        {
            try
            {
                Server = serverType switch
                {
                    ServerType.WebSocket => new WebSocket.Server(),
                    _ => throw new NotImplementedException(),
                };
                LogHelper.Info("服务器构建", $"加载服务器 {serverType} 成功");
                return true;
            }
            catch(Exception e)
            {
                LogHelper.Error("服务器构建", e.Message + e.StackTrace);
                return false;
            }            
        }
    }
}
