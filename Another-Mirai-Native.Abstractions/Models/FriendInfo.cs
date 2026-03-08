using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 表示好友信息的类
    /// </summary>
    public class FriendInfo
    {
        /// <summary>
        /// 获取一个值, 指示当前账号的实例
        /// </summary>
        public long QQ { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前的QQ昵称
        /// </summary>
        public string Nick { get; set; } = "";

        /// <summary>
        /// 获取一个值, 指示当前的备注信息
        /// </summary>
        public string Postscript { get; set; } = "";

        /// <summary>
        /// 最后更新时间(时间戳)
        /// </summary>
        public long LastUpdateTime { get; set; }

        /// <summary>
        /// ToString 方法的重写, 用于提供当前好友信息的字符串表示形式
        /// </summary>
        public override string ToString()
        {
            return $"QQ={QQ}; 昵称={Nick}; 备注={Postscript};";
        }
    }
}
