using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models
{   
    /// <summary>
    /// 描述消息的类
    /// </summary>
    public class Message(int id, string text)
    {
        /// <summary>
        /// 获取当前消息的全局唯一标识
        /// </summary>
        public int Id { get; private set; } = id;

        /// <summary>
        /// 获取一个值, 指示当前消息是否发送成功
        /// </summary>
        public bool IsSuccess => Id != 0;

        /// <summary>
        /// 获取当前消息的原文
        /// </summary>
        public string Text { get; private set; } = text;
    }
}
