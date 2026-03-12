using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models;
using System;

namespace Another_Mirai_Native.Abstractions.Context
{
    /// <summary>
    /// 提供用于描述群管理变化事件参数的类
    /// </summary>
    public class AdminChangedContext(AdminChangedType adminChangedType, DateTime sendTime, Group fromGroup, QQ beingOperateQQ)
    {
        /// <summary>
        /// 获取管理员成员变更的类型，指示是添加管理员还是移除管理员。
        /// </summary>
        public AdminChangedType AdminChangedType { get; } = adminChangedType;

        /// <summary>
        /// 获取当前事件的发送时间
        /// </summary>
        public DateTime SendTime { get; } = sendTime;

        /// <summary>
        /// 获取当前事件的来源群
        /// </summary>
        public Group FromGroup { get; } = fromGroup;

        /// <summary>
        /// 被操作的QQ
        /// </summary>
        public QQ BeingOperateQQ { get; } = beingOperateQQ;
    }
}
