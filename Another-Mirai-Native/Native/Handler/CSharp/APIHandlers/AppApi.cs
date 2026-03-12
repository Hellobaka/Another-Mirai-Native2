using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.RPC;

namespace Another_Mirai_Native.Native.Handler.CSharp.APIHandlers
{
    public class AppApi(PluginInfo pluginInfo) : IAppApi
    {
        private PluginInfo PluginInfo { get; set; } = pluginInfo;

        private int AuthCode => PluginInfo.AuthCode;

        public void DisablePlugin()
        {
            throw new NotImplementedException();
        }

        public string GetAppDirectory()
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_getAppDirectory", true, AuthCode);
            if (ret is string dir)
            {
                return dir;
            }
            throw new InvalidCastException($"GetAppDirectory 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public long GetLoginQQ()
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_getLoginQQ", true, AuthCode);
            if (ret is long qq)
            {
                return qq;
            }
            throw new InvalidCastException($"GetLoginQQ 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public string GetLoginQQNick()
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_getLoginNick", true, AuthCode);
            if (ret is string dir)
            {
                return dir;
            }
            throw new InvalidCastException($"GetLoginQQNick 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public void ReloadPlugin()
        {
            throw new NotImplementedException();
        }
    }
}
