using System;

namespace Another_Mirai_Native.Abstractions.Models.EventArgs
{
    /// <summary>
    /// 提供用于描述群文件上传事件参数的类
    /// </summary>
    public class GroupFileUploadedEventArg
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
        /// 获取当前事件的来源QQ
        /// </summary>
        public QQ FromQQ { get; private set; }

        /// <summary>
        /// 获取当前事件的文件信息
        /// </summary>
        public GroupFileInfo FileInfo { get; private set; }
    }
}
