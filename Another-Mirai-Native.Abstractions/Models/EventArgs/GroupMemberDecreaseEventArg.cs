using System;

namespace Another_Mirai_Native.Abstractions.Models.EventArgs
{
    /// <summary>
    /// 为处理群成员退出群聊事件的处理器。
    /// </summary>
    public class GroupMemberDecreaseEventArg
    {
        /// <summary>
        /// 是否是被管理员踢出，如果是成员主动退群则为 false
        /// </summary>
        public bool IsKicked { get; set; }

        /// <summary>
        /// 获取当前事件的发送时间
        /// </summary>
        public DateTime SendTime { get; private set; }

        /// <summary>
        /// 获取当前事件的来源群
        /// </summary>
        public Group FromGroup { get; private set; }

        /// <summary>
        /// 获取当前事件的来源QQ
        /// </summary>
        public QQ FromQQ { get; private set; }

        /// <summary>
        /// 获取当前事件被操作的QQ
        /// </summary>
        public QQ BeingOperateQQ { get; private set; }

    }
}
