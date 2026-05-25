using Another_Mirai_Native.Abstractions.Models.MessageItem;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.WebAPI.Hubs;
using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.SignalR;

namespace Another_Mirai_Native.WebAPI.Services
{
    public class EventBridgeService(IHubContext<MainHub> hub, DashboardService dashboardService) : IHostedService
    {
        private IHubContext<MainHub> Hub { get; } = hub;
        
        private DashboardService DashboardService { get; } = dashboardService;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 订阅性能信息事件
            DashboardService.OnPluginUsageUpdated += DashboardService_OnPluginUsageUpdated;
            DashboardService.OnUsageUpdated += DashboardService_OnUsageUpdated;
            // 订阅核心事件
            LogHelper.LogAdded += LogHelper_LogAdded;
            LogHelper.LogStatusUpdated += LogHelper_LogStatusUpdated;

            PluginManagerProxy.OnPluginEnableChanged += PluginManagerProxy_OnPluginEnableChanged;
            PluginManagerProxy.OnPluginProxyAdded += PluginManagerProxy_OnPluginProxyAdded;
            PluginManagerProxy.OnPluginProxyRemoved += PluginManagerProxy_OnPluginProxyRemoved;
            PluginManagerProxy.OnPluginProxyConnectStatusChanged += PluginManagerProxy_OnPluginProxyConnectStatusChanged;

            PluginManagerProxy.OnGroupBan += PluginManagerProxy_OnGroupBan;
            PluginManagerProxy.OnGroupAdded += PluginManagerProxy_OnGroupAdded;
            PluginManagerProxy.OnGroupMsg += PluginManagerProxy_OnGroupMsg;
            PluginManagerProxy.OnGroupLeft += PluginManagerProxy_OnGroupLeft;
            PluginManagerProxy.OnPrivateMsg += PluginManagerProxy_OnPrivateMsg;
            PluginManagerProxy.OnGroupMsgRecall += PluginManagerProxy_OnGroupMsgRecall;
            PluginManagerProxy.OnPrivateMsgRecall += PluginManagerProxy_OnPrivateMsgRecall;

            CQPImplementation.OnPrivateMessageSend += CQPImplementation_OnPrivateMessageSend;
            CQPImplementation.OnGroupMessageSend += CQPImplementation_OnGroupMessageSend;

            ProtocolManager.CurrentBotInfoChanged += ProtocolManager_CurrentBotInfoChanged;
            ProtocolManager.CurrentProtocolOnline += ProtocolManager_CurrentProtocolOnline;
            ProtocolManager.CurrentProtocolOffline += ProtocolManager_CurrentProtocolOffline;

            return Task.CompletedTask;
        }

        private void PluginManagerProxy_OnPrivateMsgRecall(int msgId, long qq, string msg)
        {
            Hub.Clients.All.SendAsync(SignalREvents.OnPrivateMsgRecall, new { msgId, qq, msg });
        }

        private void PluginManagerProxy_OnGroupMsgRecall(int msgId, long qq, string msg)
        {
            Hub.Clients.All.SendAsync(SignalREvents.OnGroupMsgRecall, new { msgId, qq, msg });
        }

        private void ProtocolManager_CurrentProtocolOnline(IProtocol protocol)
        {
            Hub.Clients.All.SendAsync(SignalREvents.ProtocolOnline, new { protocol.Name });
        }

        private void ProtocolManager_CurrentProtocolOffline(IProtocol protocol)
        {
            Hub.Clients.All.SendAsync(SignalREvents.ProtocolOffline, new { protocol.Name });
        }

        private void ProtocolManager_CurrentBotInfoChanged((string nick, long qq) botInfo)
        {
            Hub.Clients.All.SendAsync(SignalREvents.CurrentBotInfoChanged, new { botInfo.nick, botInfo.qq });
        }

        private void CQPImplementation_OnGroupMessageSend(int msgId, long group, string msg, CQPluginProxy plugin)
        {
            Hub.Clients.All.SendAsync(SignalREvents.OnGroupMessageSend, new { msgId, group, msg, plugin = PluginDto.CreateFromPlugin(plugin) });
        }

        private void CQPImplementation_OnPrivateMessageSend(int msgId, long qq, string msg, CQPluginProxy plugin)
        {
            Hub.Clients.All.SendAsync(SignalREvents.OnPrivateMessageSend, new { msgId, qq, msg, plugin = PluginDto.CreateFromPlugin(plugin) });
        }

        private void PluginManagerProxy_OnPrivateMsg(int msgId, long qq, string msg, DateTime time)
        {
            Hub.Clients.All.SendAsync(SignalREvents.OnPrivateMsg, new { msgId, qq, msg = msg.ToMessageChain(), time });
        }

        private void PluginManagerProxy_OnGroupLeft(long group, long qq)
        {
            Hub.Clients.All.SendAsync(SignalREvents.OnGroupLeft, new { group, qq });
        }

        private void PluginManagerProxy_OnGroupMsg(int msgId, long group, long qq, string msg, DateTime time)
        {
            Hub.Clients.All.SendAsync(SignalREvents.OnGroupMsg, new { msgId, group, qq, msg = msg.ToMessageChain(), time });
        }

        private void PluginManagerProxy_OnGroupAdded(long group, long qq)
        {
            Hub.Clients.All.SendAsync(SignalREvents.OnGroupAdded, new { group, qq });
        }

        private void PluginManagerProxy_OnGroupBan(long group, long qq, long operatedQQ, long time)
        {
            Hub.Clients.All.SendAsync(SignalREvents.OnGroupBan, new { group, qq, operatedQQ, time = Helper.TimeStamp2DateTime(time) });
        }

        private void PluginManagerProxy_OnPluginProxyConnectStatusChanged(CQPluginProxy proxy)
        {
            Hub.Clients.All.SendAsync(SignalREvents.PluginConnectStatusChanged, new { plugin = PluginDto.CreateFromPlugin(proxy) });
        }

        private void PluginManagerProxy_OnPluginProxyAdded(CQPluginProxy proxy)
        {
            Hub.Clients.All.SendAsync(SignalREvents.PluginAdded, new { plugin = PluginDto.CreateFromPlugin(proxy) });
        }

        private void PluginManagerProxy_OnPluginProxyRemoved(CQPluginProxy proxy)
        {
            Hub.Clients.All.SendAsync(SignalREvents.PluginRemoved, new { plugin = PluginDto.CreateFromPlugin(proxy) });
        }

        private void PluginManagerProxy_OnPluginEnableChanged(CQPluginProxy proxy)
        {
            Hub.Clients.All.SendAsync(SignalREvents.PluginEnableChanged, new { plugin = PluginDto.CreateFromPlugin(proxy) });
        }

        private void LogHelper_LogStatusUpdated(int logId, string status)
        {
            Hub.Clients.All.SendAsync(SignalREvents.LogStatusUpdated, new { logId, status });
        }

        private void LogHelper_LogAdded(int logId, LogModel log)
        {
            Hub.Clients.All.SendAsync(SignalREvents.LogAdded, new { logId, log = LogDto.CreateFromLogModel(log) });
        }

        private void DashboardService_OnUsageUpdated()
        {
            Hub.Clients.All.SendAsync(SignalREvents.UsageUpdated, DashboardService.GetUsages().MapTo<UsageData>());
        }

        private void DashboardService_OnPluginUsageUpdated()
        {
            Hub.Clients.All.SendAsync(SignalREvents.PluginUsageUpdated, DashboardService.GetPluginUsages().MapTo<PluginUsageData>());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            DashboardService.OnPluginUsageUpdated -= DashboardService_OnPluginUsageUpdated;
            DashboardService.OnUsageUpdated -= DashboardService_OnUsageUpdated;

            LogHelper.LogAdded -= LogHelper_LogAdded;
            LogHelper.LogStatusUpdated -= LogHelper_LogStatusUpdated;

            PluginManagerProxy.OnPluginEnableChanged -= PluginManagerProxy_OnPluginEnableChanged;
            PluginManagerProxy.OnPluginProxyAdded -= PluginManagerProxy_OnPluginProxyAdded;
            PluginManagerProxy.OnPluginProxyConnectStatusChanged -= PluginManagerProxy_OnPluginProxyConnectStatusChanged;
            PluginManagerProxy.OnGroupBan -= PluginManagerProxy_OnGroupBan;
            PluginManagerProxy.OnGroupAdded -= PluginManagerProxy_OnGroupAdded;
            PluginManagerProxy.OnGroupMsg -= PluginManagerProxy_OnGroupMsg;
            PluginManagerProxy.OnGroupLeft -= PluginManagerProxy_OnGroupLeft;
            PluginManagerProxy.OnPrivateMsg -= PluginManagerProxy_OnPrivateMsg;
            PluginManagerProxy.OnGroupMsgRecall -= PluginManagerProxy_OnGroupMsgRecall;
            PluginManagerProxy.OnPrivateMsgRecall -= PluginManagerProxy_OnPrivateMsgRecall;

            CQPImplementation.OnPrivateMessageSend -= CQPImplementation_OnPrivateMessageSend;
            CQPImplementation.OnGroupMessageSend -= CQPImplementation_OnGroupMessageSend;

            ProtocolManager.CurrentBotInfoChanged -= ProtocolManager_CurrentBotInfoChanged;
            ProtocolManager.CurrentProtocolOnline -= ProtocolManager_CurrentProtocolOnline;
            ProtocolManager.CurrentProtocolOffline -= ProtocolManager_CurrentProtocolOffline;
            return Task.CompletedTask;
        }
    }
}
