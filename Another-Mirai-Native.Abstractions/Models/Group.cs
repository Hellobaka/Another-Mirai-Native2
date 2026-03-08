using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 表示群的类，可进行与这个群相关快捷操作
    /// </summary>
    public class Group(long groupId)
    {
        /// <summary>
        /// 群号
        /// </summary>
        public long Id { get; private set; } = groupId;

    }
}
