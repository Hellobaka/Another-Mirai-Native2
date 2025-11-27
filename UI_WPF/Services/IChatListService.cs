using Another_Mirai_Native.UI.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.Services
{
    /// <summary>
    /// 聊天列表服务接口，管理左侧聊天列表
    /// </summary>
    public interface IChatListService
    {
        /// <summary>
        /// 添加或更新聊天列表项
        /// </summary>
        /// <param name="id">目标ID（群号或QQ号）</param>
        /// <param name="chatType">聊天类型</param>
        /// <param name="senderId">发送者ID</param>
        /// <param name="message">消息内容</param>
        Task AddOrUpdateChatListAsync(long id, ChatType chatType, long senderId, string message);

        /// <summary>
        /// 按时间重新排序聊天列表
        /// </summary>
        /// <param name="chatList">聊天列表</param>
        /// <returns>排序后的列表</returns>
        List<ChatListItemViewModel> ReorderChatList(List<ChatListItemViewModel> chatList);

        /// <summary>
        /// 从数据库加载聊天历史
        /// </summary>
        /// <returns>聊天列表</returns>
        Task<List<ChatListItemViewModel>> LoadChatHistoryAsync();

        /// <summary>
        /// 更新未读消息数
        /// </summary>
        /// <param name="chatList">聊天列表</param>
        /// <param name="id">目标ID</param>
        /// <param name="chatType">聊天类型</param>
        /// <param name="count">未读数量</param>
        void UpdateUnreadCount(List<ChatListItemViewModel> chatList, long id, ChatType chatType, int count);
    }
}
