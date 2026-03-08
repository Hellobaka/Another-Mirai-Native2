using System;

namespace Another_Mirai_Native.Abstractions.Models.EventArgs
{
    /// <summary>
    /// 提供用于描述群全员禁言解除事件参数的类
    /// </summary>
    public class GroupWholeUnbannedEventArg
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
    }
}
