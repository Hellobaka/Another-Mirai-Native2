using Another_Mirai_Native.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions.Services
{
    /// <summary>
    /// 好友相关的接口，提供获取好友列表的方法。
    /// </summary>
    public interface IFriendApi
    {
        /// <summary>
        /// 获取好友列表
        /// </summary>
        List<FriendInfo> GetFriendInfos();

        /// <summary>
        /// 异步获取好友列表
        /// </summary>
        Task<List<FriendInfo>> GetFriendInfosAsync();

        /// <summary>
        /// 向目标用户发送名片赞。根据不同的框架实现，可能会有一些限制，例如每天只能发送一次赞等。
        /// </summary>
        /// <param name="qq">目标用户</param>
        /// <param name="count">发送赞是数量</param>
        /// <returns>操作是否成功</returns>
        bool SendPraise(long qq, int count);

        /// <summary>
        /// 异步向目标用户发送名片赞。
        /// </summary>
        /// <param name="qq">目标用户</param>
        /// <param name="count">发送赞是数量</param>
        /// <returns>操作是否成功</returns>
        Task<bool> SendPraiseAsync(long qq, int count);
    }
}
