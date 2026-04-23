using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.RPC;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Native.Handler.CSharp.APIHandlers
{
    public class AppApi(PluginInfo pluginInfo) : IAppApi
    {
        private PluginInfo PluginInfo { get; set; } = pluginInfo;

        private int AuthCode => PluginInfo.AuthCode;

        public void DisablePlugin()
        {
            ClientManager.Client.InvokeCQPFunction("InvokeCore_DisablePlugin", true, AuthCode);
        }

        public async Task DisablePluginAsync()
        {
            await ClientManager.Client.InvokeCQPFunctionAsync("InvokeCore_DisablePlugin", true, AuthCode);
        }

        public string GetAppDirectory()
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_getAppDirectory", true, AuthCode);
            if (ret is string dir)
            {
                return dir;
            }
            throw new InvalidCastException($"GetAppDirectory 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public async Task<string> GetAppDirectoryAsync()
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_getAppDirectory", true, AuthCode);
            if (ret is string dir)
            {
                return dir;
            }
            throw new InvalidCastException($"GetAppDirectoryAsync 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public long GetLoginQQ()
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_getLoginQQ", true, AuthCode);
            if (ret is long qq)
            {
                return qq;
            }
            throw new InvalidCastException($"GetLoginQQ 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<long> GetLoginQQAsync()
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_getLoginQQ", true, AuthCode);
            if (ret is long qq)
            {
                return qq;
            }
            throw new InvalidCastException($"GetLoginQQAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public string GetLoginQQNick()
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_getLoginNick", true, AuthCode);
            if (ret is string dir)
            {
                return dir;
            }
            throw new InvalidCastException($"GetLoginQQNick 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public async Task<string> GetLoginQQNickAsync()
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_getLoginNick", true, AuthCode);
            if (ret is string dir)
            {
                return dir;
            }
            throw new InvalidCastException($"GetLoginQQNickAsync 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public void ReloadPlugin()
        {
            ClientManager.Client.InvokeCQPFunction("InvokeCore_Restart", true, AuthCode);
        }

        public async Task ReloadPluginAsync()
        {
            await ClientManager.Client.InvokeCQPFunctionAsync("InvokeCore_Restart", true, AuthCode);
        }
    }
}
