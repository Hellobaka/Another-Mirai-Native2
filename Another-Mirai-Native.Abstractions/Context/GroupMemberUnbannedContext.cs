using Another_Mirai_Native.Abstractions.Models;
using System;

namespace Another_Mirai_Native.Abstractions.Context
{
    /// <summary>
    /// 提供用于描述群群成员被解除禁言事件参数的类
    /// </summary>
    public class GroupMemberUnbannedContext(DateTime sendTime, Group fromGroup, QQ fromQQ, QQ beingOperateQQ)
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

        /// <summary>
        /// 获取当前事件的被操作QQ
        /// </summary>
        public QQ BeingOperateQQ { get; } = beingOperateQQ;
    }
}
