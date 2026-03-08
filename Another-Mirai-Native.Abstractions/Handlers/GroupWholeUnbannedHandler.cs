using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models.EventArgs;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions.Handlers
{
    /// <summary>
    /// 为处理群解除全员禁言事件的处理器。
    /// </summary>
    public class GroupWholeUnbannedHandler
    {
        /// <summary>
        /// 在群解除全员禁言时异步执行操作。
        /// </summary>
        /// <param name="e">包含群解除全员禁言事件相关信息的事件参数对象。</param>
        /// <param name="ct">可用于发出退出操作取消信号的取消令牌。</param>
        /// <returns>事件的处理结果；<see cref="EventHandleResult.Pass"/> 不阻塞事件的继续传递；<see cref="EventHandleResult.Block"/> 将会阻塞事件的继续传递；</returns>
        public virtual Task<EventHandleResult> OnGroupWholeUnbanned(GroupWholeUnbannedEventArg e, CancellationToken ct) => Task.FromResult(EventHandleResult.Pass);
    }
}
