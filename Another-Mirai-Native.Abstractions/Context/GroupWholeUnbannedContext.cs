using Another_Mirai_Native.Abstractions.Models;
using System;

namespace Another_Mirai_Native.Abstractions.Context
{
    /// <summary>
    /// 提供用于描述群全员禁言解除事件参数的类
    /// </summary>
    public class GroupWholeUnbannedContext(DateTime sendTime, Group fromGroup, QQ fromQQ)
    {
        /// <summary>
        /// 获取当前事件的发送时间
        /// </summary>
        public DateTime SendTime { get; } = sendTime;

        /// <summary>
        /// 获取当前事件的来源群
        /// </summary>
        public Group FromGroup { get; } = fromGroup;

        /// <summary>
        /// 获取当前事件的操作者QQ
        /// </summary>
        public QQ FromQQ { get; } = fromQQ;
    }
}
