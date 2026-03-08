using Another_Mirai_Native.Abstractions.Models;
using System.Collections.Generic;

namespace Another_Mirai_Native.Abstractions.Services
{
    /// <summary>
    /// 发送消息相关的接口，提供发送私聊消息、群聊消息和撤回消息的方法。
    /// </summary>
    public interface IMessageApi
    {
        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="userId">目标QQ</param>
        /// <param name="message">将要发送的消息</param>
        /// <returns>若发送成功则返回消息ID（根据不同的框架实现，可能会有负数），若发送失败则返回 0</returns>
        int SendPrivateMessage(long userId, string message);

        /// <summary>
        /// 发送群聊消息
        /// </summary>
        /// <param name="groupId">目标群聊ID</param>
        /// <param name="message">将要发送的消息</param>
        /// <returns>若发送成功则返回消息ID（根据不同的框架实现，可能会有负数），若发送失败则返回 0</returns>
        int SendGroupMessage(long groupId, string message);

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="messageId">通过发送消息接口返回的消息ID</param>
        /// <returns>消息撤回成功与否</returns>
        bool DeleteMessage(long messageId);

        /// <summary>
        /// 检索指定来源的聊天记录历史。
        /// </summary>
        /// <remarks>确保提供的 groupId 和 qq 对应有效的群组和用户标识符。
        /// 若只是查询私聊记录，请将 groupId 设置为 0，并使用 qq 参数指定用户标识符。
        /// </remarks>
        /// <param name="groupId">从中检索聊天记录的群组唯一标识符。</param>
        /// <param name="qq">要检索其聊天记录的用户唯一标识符。</param>
        /// <param name="count">要返回的聊天记录条目的最大数量。必须是正整数。</param>
        /// <returns>表示指定群组和用户聊天记录的 <see cref="ChatHistory"/> 对象列表。
        /// 如果未找到聊天记录，则列表为空。</returns>
        List<ChatHistory> GetChatHistories(long groupId, long qq, int count);

        /// <summary>
        /// 通过消息ID检索聊天记录历史。未找到对应记录时返回 <see langword="null"/>。
        /// </summary>
        /// <param name="parentId">关联的来源ID，可能是群号或是QQ</param>
        /// <param name="isGroup">指示关联来源是否为群聊。如果是群聊则设置为 <see langword="true"/>；否则为 <see langword="false"/>。</param>
        /// <param name="messageId">通过发送消息接口返回的消息ID</param>
        ChatHistory? GetChatHistoryById(long parentId, bool isGroup, int messageId);
    }
}
