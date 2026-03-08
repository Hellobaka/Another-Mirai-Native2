using Another_Mirai_Native.Abstractions.Models;

namespace Another_Mirai_Native.Abstractions.Context
{
    /// <summary>
    /// 提供用于描述收到群消息事件参数的类
    /// </summary>
    public class GroupMessageContext
    {
        /// <summary>
        /// 获取当前事件的来源群
        /// </summary>
        public Group FromGroup { get; private set; }

        /// <summary>
        /// 获取当前事件的来源QQ
        /// </summary>
        public QQ FromQQ { get; private set; }

        /// <summary>
        /// 获取当前事件的消息内容
        /// </summary>
        public Message Message { get; private set; }
    }
}
