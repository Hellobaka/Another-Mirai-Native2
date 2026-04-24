using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 表示好友信息的类
    /// </summary>
    public class FriendInfo(long qq, string nick, string postscript, long lastUpdateTime)
    {
        /// <summary>
        /// 获取一个值, 指示当前账号的实例
        /// </summary>
        public long QQ { get; private set; } = qq;

        /// <summary>
        /// 获取一个值, 指示当前的QQ昵称
        /// </summary>
        public string Nick { get; private set; } = nick;

        /// <summary>
        /// 获取一个值, 指示当前的备注信息
        /// </summary>
        public string Postscript { get; private set; } = postscript;

        /// <summary>
        /// 最后更新时间(时间戳)
        /// </summary>
        public long LastUpdateTime { get; private set; } = lastUpdateTime;

        /// <summary>
        /// ToString 方法的重写, 用于提供当前好友信息的字符串表示形式
        /// </summary>
        public override string ToString()
        {
            return $"QQ={QQ}; 昵称={Nick}; 备注={Postscript};";
        }
    }
}
