namespace Another_Mirai_Native.Abstractions.Services
{
    /// <summary>
    /// 框架提供给插件的核心接口，包含了日志记录、消息处理、好友管理、群组管理和应用程序交互等功能的访问点。插件开发者通过实现此接口来与框架进行交互，实现插件的具体功能。
    /// </summary>
    public interface IPluginApi
    {
        /// <summary>
        /// 用于记录插件相关信息、调试信息和错误的日志记录器实例。插件开发者可以使用此属性来输出日志，以便在开发和运行时跟踪插件的行为和状态。
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// 用于调用框架提供的消息相关功能的接口实例。插件开发者可以通过此属性发送消息、撤回消息等操作，以实现与用户的交互和消息处理功能。
        /// </summary>
        IMessageApi MessageApi { get; }

        /// <summary>
        /// 用于调用框架提供的好友相关功能的接口实例。插件开发者可以通过此属性管理好友列表、获取好友信息等操作，以实现与好友相关的功能。
        /// </summary>
        IFriendApi FriendApi { get; }

        /// <summary>
        /// 用于调用框架提供的群相关功能的接口实例。插件开发者可以通过此属性管理群列表、获取群信息等操作，以实现与群相关的功能。
        /// </summary>
        IGroupApi GroupApi { get; }

        /// <summary>
        /// 用于调用框架提供的应用相关功能的接口实例。插件开发者可以通过此属性获取应用信息、调用应用功能等操作，以实现与应用相关的功能。
        /// </summary>
        IAppApi AppApi { get; }
    }
}
