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
        /// <returns>好友列表</returns>
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

        /// <summary>
        /// 处理好友添加请求
        /// </summary>
        /// <param name="flag">框架内部标记添加请求的标志</param>
        /// <param name="accept">是否接受</param>
        /// <param name="card">接受后的备注</param>
        /// <returns></returns>
        bool SetFriendAddRequest(string flag, bool accept, string card = "");

        /// <summary>
        /// 异步处理好友添加请求
        /// </summary>
        /// <param name="flag">框架内部标记添加请求的标志</param>
        /// <param name="accept">是否接受</param>
        /// <param name="card">接受后的备注</param>
        /// <returns></returns>
        Task<bool> SetFriendAddRequestAsync(string flag, bool accept, string card = "");
    }
}
