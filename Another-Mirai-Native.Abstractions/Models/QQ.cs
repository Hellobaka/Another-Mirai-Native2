using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 表示个人的类，可进行与此QQ相关快捷操作
    /// </summary>
    public class QQ(long id)
    {

        /// <summary>
        /// 关联的QQ
        /// </summary>
        public long Id { get; private set; } = id;
    }
}
