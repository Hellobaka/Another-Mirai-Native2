using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.RPC;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Native.Handler.CSharp.APIHandlers
{
    public class FriendApi(PluginInfo pluginInfo) : IFriendApi
    {
        private PluginInfo PluginInfo { get; set; } = pluginInfo;

        private int AuthCode => PluginInfo.AuthCode;

        public List<FriendInfo> GetFriendInfos()
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_getFriendList", true, AuthCode, false);
            if (ret is string r)
            {
                var l = Model.FriendInfo.RawToList(r);
                return l.Select(x => new FriendInfo(x.QQ, x.Nick, x.Postscript, x.LastUpdateTime)).ToList();
            }
            throw new InvalidCastException($"GetFriendInfos 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public async Task<List<FriendInfo>> GetFriendInfosAsync()
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_getFriendList", true, AuthCode, false);
            if (ret is string r)
            {
                var l = Model.FriendInfo.RawToList(r);
                return l.Select(x => new FriendInfo(x.QQ, x.Nick, x.Postscript, x.LastUpdateTime)).ToList();
            }
            throw new InvalidCastException($"GetFriendInfosAsync 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public bool SendPraise(long qq, int count)
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_sendLikeV2", true, AuthCode, count);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SendPraise 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> SendPraiseAsync(long qq, int count)
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_sendLikeV2", true, AuthCode, count);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SendPraiseAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public bool SetFriendAddRequest(string flag, bool accept, string card = "")
        {
            var ret = ClientManager.Client.InvokeCQPFunction("CQ_setFriendAddRequest", true, AuthCode, flag, accept ? 1 : 2, card);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetFriendAddRequest 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> SetFriendAddRequestAsync(string flag, bool accept, string card = "")
        {
            var ret = await ClientManager.Client.InvokeCQPFunctionAsync("CQ_setFriendAddRequest", true, AuthCode, flag, accept ? 1 : 2, card);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SetFriendAddRequestAsync 返回值类型错误，应当返回 long，实际返回 {ret?.GetType()}");
        }
    }
}
