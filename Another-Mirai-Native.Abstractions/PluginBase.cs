using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions
{
    /// <summary>
    /// 作为插件的抽象基类，为框架内的日志记录、消息收发、好友管理、群组管理和应用程序交互提供核心功能及接口。
    /// 插件开发者应从此类派生以实现自定义插件行为。
    /// </summary>
    /// <remarks>派生类必须重写所提供的方法来实现具体的插件逻辑。此类公开了用于访问框架 API 的受保护属性，使插件能够与消息、好友、群组以及应用程序级别的功能进行交互。GetPluginInfo 的基础实现会抛出 NotImplementedException，以强制派生类提供插件元数据。</remarks>
    public abstract class PluginBase
    {
        /// <summary>
        /// 用于记录插件相关信息、调试信息和错误的日志记录器实例。插件开发者可以使用此属性来输出日志，以便在开发和运行时跟踪插件的行为和状态。
        /// </summary>
        protected ILogger Logger;
        
        /// <summary>
        /// 用于调用框架提供的消息相关功能的接口实例。插件开发者可以通过此属性发送消息、撤回消息等操作，以实现与用户的交互和消息处理功能。
        /// </summary>
        protected IMessageAPI MessageAPI;
        
        /// <summary>
        /// 用于调用框架提供的好友相关功能的接口实例。插件开发者可以通过此属性管理好友列表、获取好友信息等操作，以实现与好友相关的功能。
        /// </summary>
        protected IFriendAPI FriendAPI;
        
        /// <summary>
        /// 用于调用框架提供的群相关功能的接口实例。插件开发者可以通过此属性管理群列表、获取群信息等操作，以实现与群相关的功能。
        /// </summary>
        protected IGroupAPI GroupAPI;

        /// <summary>
        /// 用于调用框架提供的应用相关功能的接口实例。插件开发者可以通过此属性获取应用信息、调用应用功能等操作，以实现与应用相关的功能。
        /// </summary>
        protected IApp AppAPI;

        /// <summary>
        /// 在插件启用时异步处理逻辑。
        /// </summary>
        /// <remarks>在派生类中重写此方法以实现自定义的启用逻辑。默认实现会立即完成。</remarks>
        /// <param name="ct">可用于取消启用操作的取消令牌。</param>
        /// <returns>表示异步启用操作的任务。</returns>
        public virtual Task OnEnableAsync(CancellationToken ct) => Task.CompletedTask;

        /// <summary>
        /// 在插件禁用时异步执行必要的清理操作。
        /// </summary>
        /// <remarks>可在派生类中重写此方法以实现自定义的退出逻辑。如果取消令牌被触发，应适当处理来中止退出操作。</remarks>
        /// <param name="ct">可用于发出退出操作取消信号的取消令牌。</param>
        /// <returns>表示异步退出操作的任务。当退出操作完成时，任务也将完成。</returns>
        public virtual Task OnDisableAsync(CancellationToken ct) => Task.CompletedTask;

        /// <summary>
        /// 获取插件信息。派生类必须重写此方法以提供特定插件实现的详细信息。
        /// </summary>
        /// <remarks>此方法旨在由插件开发者实现，用于提供其插件的标识和描述信息。
        /// 基类实现会抛出 <see cref="NotImplementedException"/> 异常以强制要求重写。</remarks>
        /// <returns>包含插件元数据和详细信息的 <see cref="PluginInfo"/> 实例。</returns>
        /// <exception cref="NotImplementedException">如果未在派生类中重写此方法，则抛出该异常。</exception>
        public virtual PluginInfo GetPluginInfo()
        {
            throw new NotImplementedException("继承基类的子类必须实现 GetPluginInfo 方法。");
        }
    }
}
