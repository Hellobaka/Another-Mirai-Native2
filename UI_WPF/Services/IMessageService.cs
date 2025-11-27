using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.Services
{
    /// <summary>
    /// 消息类型枚举
    /// </summary>
    public enum ChatType
    {
        Group,
        Private
    }

    /// <summary>
    /// 消息发送参数
    /// </summary>
    public class SendMessageParameters
    {
        public long TargetId { get; set; }
        public ChatType ChatType { get; set; }
        public string Message { get; set; }
        public long SenderId { get; set; }
        public DateTime Time { get; set; }
        public string PluginName { get; set; }
    }

    /// <summary>
    /// 添加聊天项参数
    /// </summary>
    public class AddChatItemParameters
    {
        public long TargetId { get; set; }
        public long SenderId { get; set; }
        public string Message { get; set; }
        public ChatType ChatType { get; set; }
        public DetailItemType ItemType { get; set; }
        public DateTime Time { get; set; }
        public int MsgId { get; set; }
        public string PluginName { get; set; }
        public Action<string> ItemAddedCallback { get; set; }
    }

    /// <summary>
    /// 消息服务接口，处理消息发送、持久化、历史记录
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// 发送消息（群消息或私聊消息）
        /// </summary>
        /// <param name="targetId">目标ID（群号或QQ号）</param>
        /// <param name="chatType">聊天类型</param>
        /// <param name="message">消息内容</param>
        /// <returns>消息ID，失败时返回0</returns>
        int SendMessage(long targetId, ChatType chatType, string message);

        /// <summary>
        /// 添加聊天消息项到数据库和UI
        /// </summary>
        /// <param name="parameters">添加参数</param>
        /// <returns>数据库记录ID</returns>
        Task<int> AddChatItemAsync(AddChatItemParameters parameters);

        /// <summary>
        /// 将历史记录转换为ViewModel
        /// </summary>
        /// <param name="history">聊天历史</param>
        /// <param name="avatarType">头像类型</param>
        /// <returns>ViewModel</returns>
        Task<ChatDetailItemViewModel> ParseHistoryAsync(ChatHistory history, ChatAvatar.AvatarTypes avatarType);

        /// <summary>
        /// 执行完整的发送消息流程（包括UI更新和持久化）
        /// </summary>
        /// <param name="parameters">发送参数</param>
        /// <returns>发送的消息ID</returns>
        Task<int> ExecuteSendMessageAsync(SendMessageParameters parameters);
    }
}
