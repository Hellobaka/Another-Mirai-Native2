using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions.Handlers
{
    /// <summary>
    /// 提供处理好友添加完成事件的处理器。
    /// </summary>
    public interface IFriendAddedHandler
    {
        /// <summary>
        /// 在好友添加完成时触发。
        /// </summary>
        /// <param name="e">包含已添加好友的详细信息。</param>
        /// <param name="ct">可用于发出退出操作取消信号的取消令牌。</param>
        /// <returns>事件的处理结果；<see cref="EventHandleResult.Pass"/> 不阻塞事件的继续传递；<see cref="EventHandleResult.Block"/> 将会阻塞事件的继续传递；</returns>
        Task<EventHandleResult> OnFriendAddedAsync(FriendAddedContext e, CancellationToken ct);
    }
}
