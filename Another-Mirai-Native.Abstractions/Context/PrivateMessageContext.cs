using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;

namespace Another_Mirai_Native.Abstractions.Context
{
    /// <summary>
    /// 提供用于描述收到私聊消息事件参数的类
    /// </summary>
    public class PrivateMessageContext(IPluginApi api, QQ fromQQ, Message message)
    {
        /// <summary>
        /// 获取插件 API 实例
        /// </summary>
        public IPluginApi API { get; } = api;

        /// <summary>
        /// 获取当前事件的来源QQ
        /// </summary>
        public QQ FromQQ { get; } = fromQQ;

        /// <summary>
        /// 获取当前事件的消息内容
        /// </summary>
        public Message Message { get; } = message;
    }
}
