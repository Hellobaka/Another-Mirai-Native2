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
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_getFriendList", true, AuthCode);
            if (ret is string r)
            {
                var l = Model.FriendInfo.RawToList(r);
                return l.Select(x => new FriendInfo(x.QQ, x.Nick, x.Postscript, x.LastUpdateTime)).ToList();
            }
            throw new InvalidCastException($"GetFriendInfos 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public Task<List<FriendInfo>> GetFriendInfosAsync()
        {
            return Task.FromResult(GetFriendInfos());
        }

        public bool SendPraise(long qq, int count)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_sendLikeV2", true, AuthCode, count);
            if (ret is int r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"SendPraise 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public Task<bool> SendPraiseAsync(long qq, int count)
        {
            return Task.FromResult(SendPraise(qq, count));
        }
    }
}
