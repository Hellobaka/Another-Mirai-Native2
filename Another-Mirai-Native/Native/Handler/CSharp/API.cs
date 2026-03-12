using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.Native.Handler.CSharp.APIHandlers;

namespace Another_Mirai_Native.Native.Handler.CSharp
{
    public class API(PluginInfo pluginInfo) : IPluginApi
    {
        private readonly Logger _logger = new(pluginInfo);
        private readonly MessageApi _messageApi = new(pluginInfo);
        private readonly FriendApi _friendApi = new(pluginInfo);
        private readonly GroupApi _groupApi = new(pluginInfo);
        private readonly AppApi _appApi = new(pluginInfo);

        public ILogger Logger => _logger;

        public IMessageApi MessageApi => _messageApi;

        public IFriendApi FriendApi => _friendApi;

        public IGroupApi GroupApi => _groupApi;

        public IAppApi AppApi => _appApi;
    }
}
