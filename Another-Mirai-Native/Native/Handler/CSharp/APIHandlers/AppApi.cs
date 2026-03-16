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
            throw new NotImplementedException();
        }

        public Task DisablePluginAsync()
        {
            DisablePlugin();
            return Task.CompletedTask;
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

        public Task<string> GetAppDirectoryAsync()
        {
            return Task.FromResult(GetAppDirectory());
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

        public Task<long> GetLoginQQAsync()
        {
            return Task.FromResult(GetLoginQQ());
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

        public Task<string> GetLoginQQNickAsync()
        {
            return Task.FromResult(GetLoginQQNick());
        }

        public void ReloadPlugin()
        {
            throw new NotImplementedException();
        }

        public Task ReloadPluginAsync()
        {
            ReloadPlugin();
            return Task.CompletedTask;
        }
    }
}
