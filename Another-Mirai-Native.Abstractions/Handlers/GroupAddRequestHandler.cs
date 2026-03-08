using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models.EventArgs;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions.Handlers
{
    /// <summary>
    /// 提供处理群添加请求事件的处理器。
    /// </summary>
    /// <remarks>默认情况下，除非重写处理器，否则群添加请求将被忽略。</remarks>
    public class GroupAddRequestHandler
    {
        /// <summary>
        /// 在收到群添加请求时触发。可以通过重写此方法来处理添加请求，例如自动接受、拒绝或忽略请求。不重写时默认行为是忽略请求，即不做任何处理。处理请求请调用参数的 <see href="GroupAddRequestEventArg.SetRequestResult"/> 方法
        /// </summary>
        /// <param name="e">包含群添加请求的详细信息。</param>
        /// <param name="ct">可用于发出退出操作取消信号的取消令牌。</param>
        /// <returns>事件的处理结果；<see cref="EventHandleResult.Pass"/> 不阻塞事件的继续传递；<see cref="EventHandleResult.Block"/> 将会阻塞事件的继续传递；</returns>
        public virtual Task<EventHandleResult> OnGroupAddRequest(GroupAddRequestEventArg e, CancellationToken ct) => Task.FromResult(EventHandleResult.Pass);
    }
}
