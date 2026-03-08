using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.EventArgs
{
    /// <summary>
    /// 提供用于描述群成员被禁言事件参数的类
    /// </summary>
    public class GroupMemberBannedEventArg
    {
        /// <summary>
        /// 获取当前事件的发送时间
        /// </summary>
        public DateTime SendTime { get; private set; }

        /// <summary>
        /// 获取当前事件的来源群
        /// </summary>
        public Group FromGroup { get; private set; }

        /// <summary>
        /// 获取当前事件的操作者QQ
        /// </summary>
        public QQ FromQQ { get; private set; }

        /// <summary>
        /// 获取当前事件的被操作QQ
        /// </summary>
        public QQ BeingOperateQQ { get; private set; }

        /// <summary>
        /// 获取当前事件的禁言时长
        /// </summary>
        public TimeSpan? BanSpeakTimeSpan { get; private set; }
    }
}
