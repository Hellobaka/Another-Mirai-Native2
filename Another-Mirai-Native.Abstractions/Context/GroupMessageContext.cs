using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions.Context
{
    /// <summary>
    /// 提供用于描述收到群消息事件参数的类
    /// </summary>
    public class GroupMessageContext(IPluginApi api, Group fromGroup, QQ fromQQ, Message message)
    {
        /// <summary>
        /// 获取插件 API 实例
        /// </summary>
        public IPluginApi API { get; } = api;

        /// <summary>
        /// 获取当前事件的来源群
        /// </summary>
        public Group FromGroup { get; } = fromGroup;

        /// <summary>
        /// 获取当前事件的来源QQ
        /// </summary>
        public QQ FromQQ { get; } = fromQQ;

        /// <summary>
        /// 获取当前事件的消息内容
        /// </summary>
        public Message Message { get; } = message;

        /// <summary>
        /// 回复当前消息
        /// </summary>
        /// <param name="msg">回复内容</param>
        /// <returns>回复后消息</returns>
        public Message Reply(string msg)
        {
            string content = $"[CQ:reply,id={Message.Id}]" + msg;
            int msgId = API.MessageApi.SendGroupMessage(FromGroup.Id, content);
            return new(API, msgId, content);
        }

        /// <summary>
        /// 向当前消息的来源群发送消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        /// <returns>发送后消息</returns>
        public Message SendMessage(string msg)
        {
            int msgId = API.MessageApi.SendGroupMessage(FromGroup.Id, msg);
            return new(API, msgId, msg);
        }

        /// <summary>
        /// 异步向当前消息的来源群发送消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        /// <returns>发送后消息</returns>
        public async Task<Message> SendMessageAsync(string msg)
        {
            int msgId = await API.MessageApi.SendGroupMessageAsync(FromGroup.Id, msg);
            return new(API, msgId, msg);
        }
    }
}
