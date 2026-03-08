using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Enums
{
    /// <summary>
    /// 表示群成员管理员变化的枚举
    /// </summary>
    public enum AdminChangedType
    {
        /// <summary>
        /// 被取消管理
        /// </summary>
        RemoveManage = 1,

        /// <summary>
        /// 被设置管理
        /// </summary>
        SetManage = 2
    }
}
