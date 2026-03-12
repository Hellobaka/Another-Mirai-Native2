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
    public abstract class PluginBase
    {
        /// <summary>
        /// 框架提供给插件的核心接口，包含了日志记录、消息处理、好友管理、群组管理和应用程序交互等功能的访问点。插件开发者通过实现此接口来与框架进行交互，实现插件的具体功能。
        /// </summary>
        public IPluginApi API { get; internal set; } = default!;

        /// <summary>
        /// 插件信息。
        /// </summary>
        public virtual PluginInfo PluginInfo { get; }

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
        /// <remarks>可在派生类中重写此方法以实现自定义的退出逻辑。默认实现会立即完成。</remarks>
        /// <param name="ct">可用于发出退出操作取消信号的取消令牌。</param>
        /// <returns>表示异步退出操作的任务。</returns>
        public virtual Task OnDisableAsync(CancellationToken ct) => Task.CompletedTask;
    }
}
