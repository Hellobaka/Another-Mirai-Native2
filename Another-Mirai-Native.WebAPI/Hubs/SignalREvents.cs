namespace Another_Mirai_Native.WebAPI.Hubs
{
    public class SignalREvents
    {
        public const string UsageUpdated = "UsageUpdated";
        public const string PluginUsageUpdated = "PluginUsageUpdated";

        public const string LogStatusUpdated = "LogStatusUpdated";
        public const string LogAdded = "LogAdded";

        public const string PluginEnableChanged = "PluginEnableChanged";
        public const string PluginAdded = "PluginAdded";
        public const string PluginConnectStatusChanged = "PluginConnectStatusChanged";

        public const string ProtocolOnline = "ProtocolOnline";
        public const string ProtocolOffline = "ProtocolOffline";
        public const string CurrentBotInfoChanged = "CurrentBotInfoChanged";

        public const string OnGroupMessageSend = "OnGroupMessageSend";
        public const string OnPrivateMessageSend = "OnPrivateMessageSend";

        public const string OnPrivateMsg = "OnPrivateMsg";
        public const string OnGroupLeft = "OnGroupLeft";
        public const string OnGroupMsg = "OnGroupMsg";
        public const string OnGroupAdded = "OnGroupAdded";
        public const string OnGroupBan = "OnGroupBan";
        public const string OnPrivateMsgRecall = "OnPrivateMsgRecall";
        public const string OnGroupMsgRecall = "OnGroupMsgRecall";
    }
}
