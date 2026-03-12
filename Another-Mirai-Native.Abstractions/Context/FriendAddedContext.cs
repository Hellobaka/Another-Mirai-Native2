using Another_Mirai_Native.Abstractions.Models;
using System;

namespace Another_Mirai_Native.Abstractions.Context
{
    /// <summary>
    /// 提供用于描述好友已添加事件参数的类
    /// </summary>
    public class FriendAddedContext(DateTime sendTime, QQ fromQQ)
    {
        /// <summary>
        /// 获取当前事件的发送时间
        /// </summary>
        public DateTime SendTime { get; } = sendTime;

        /// <summary>
        /// 获取当前事件的来源QQ
        /// </summary>
        public QQ FromQQ { get; } = fromQQ;
    }
}
