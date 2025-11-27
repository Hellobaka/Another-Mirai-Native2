using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.Services
{
    /// <summary>
    /// 聊天列表服务实现
    /// </summary>
    public class ChatListService : IChatListService
    {
        private readonly ICacheService _cacheService;

        public ChatListService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// 添加或更新聊天列表项
        /// </summary>
        public async Task AddOrUpdateChatListAsync(long id, ChatType chatType, long senderId, string message)
        {
            // 清理消息中的换行符
            message = message.Replace("\r", "").Replace("\n", "");

            // 获取显示名称
            string displayName = chatType == ChatType.Group
                ? await _cacheService.GetGroupNameAsync(id)
                : await _cacheService.GetFriendNickAsync(id);

            // 构建详细信息
            string detail;
            if (chatType == ChatType.Group)
            {
                string senderNick = await _cacheService.GetGroupMemberNickAsync(id, senderId);
                detail = $"{senderNick}: {message}";
            }
            else
            {
                detail = message;
            }

            // 返回构建好的信息，由UI层处理列表更新
            // 实际更新逻辑需要在UI层完成，因为需要访问Observable集合
        }

        /// <summary>
        /// 按时间重新排序聊天列表
        /// </summary>
        public List<ChatListItemViewModel> ReorderChatList(List<ChatListItemViewModel> chatList)
        {
            if (chatList == null)
            {
                return new List<ChatListItemViewModel>();
            }

            // 去重并按时间倒序排序
            return chatList
                .GroupBy(x => new { x.Id, x.AvatarType })
                .Select(x => x.First())
                .OrderByDescending(x => x.Time)
                .ToList();
        }

        /// <summary>
        /// 从数据库加载聊天历史
        /// </summary>
        public async Task<List<ChatListItemViewModel>> LoadChatHistoryAsync()
        {
            var historyList = ChatHistoryHelper.GetHistoryCategroies();
            var chatList = new List<ChatListItemViewModel>();

            foreach (var item in historyList)
            {
                string displayName = item.Type == ChatHistoryType.Private
                    ? await _cacheService.GetFriendNickAsync(item.ParentID)
                    : await _cacheService.GetGroupNameAsync(item.ParentID);

                chatList.Add(new ChatListItemViewModel
                {
                    AvatarType = item.Type == ChatHistoryType.Private 
                        ? ChatAvatar.AvatarTypes.QQPrivate 
                        : ChatAvatar.AvatarTypes.QQGroup,
                    Detail = item.Message,
                    GroupName = displayName,
                    Id = item.ParentID,
                    Time = item.Time,
                    UnreadCount = 0
                });
            }

            return chatList;
        }

        /// <summary>
        /// 更新未读消息数
        /// </summary>
        public void UpdateUnreadCount(List<ChatListItemViewModel> chatList, long id, ChatType chatType, int count)
        {
            if (chatList == null)
            {
                return;
            }

            var avatarType = chatType == ChatType.Group 
                ? ChatAvatar.AvatarTypes.QQGroup 
                : ChatAvatar.AvatarTypes.QQPrivate;

            var item = chatList.FirstOrDefault(x => x.Id == id && x.AvatarType == avatarType);
            if (item != null)
            {
                item.UnreadCount = count;
            }
        }
    }
}
