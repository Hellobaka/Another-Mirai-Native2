using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Context;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions.Handlers
{
    /// <summary>
    /// 为处理群聊管理员成员变更事件的处理器。
    /// </summary>
    /// <remarks>派生类应实现 OnAdminChanged 方法来定义响应管理员成员变更的具体操作。
    /// 此方法旨在被重写以提供处理管理员变更的自定义行为。</remarks>
    public abstract class AdminChangeHandler
    {
        /// <summary>
        /// 在群聊的管理员成员发生变化时异步执行操作。
        /// </summary>
        /// <param name="e">包含管理员成员变更事件相关信息的事件参数对象。</param>
        /// <param name="ct">可用于发出退出操作取消信号的取消令牌。</param>
        /// <returns>事件的处理结果；<see cref="EventHandleResult.Pass"/> 不阻塞事件的继续传递；<see cref="EventHandleResult.Block"/> 将会阻塞事件的继续传递；</returns>
        public virtual Task<EventHandleResult> OnAdminChanged(AdminChangedContext e, CancellationToken ct) => Task.FromResult(EventHandleResult.Pass);
    }
}
