using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models;
using System;

namespace Another_Mirai_Native.Abstractions.Context
{
    /// <summary>
    /// 提供用于描述好友添加请求事件参数的类
    /// </summary>
    public class FriendAddRequestContext
    {
        /// <summary>
        /// 获取当前事件的发送时间
        /// </summary>
        public DateTime SendTime { get; private set; }

        /// <summary>
        /// 获取当前事件的来源QQ
        /// </summary>
        public QQ FromQQ { get; private set; }

        /// <summary>
        /// 获取当前事件的附加消息
        /// </summary>
        public string AppendMessage { get; private set; }

        /// <summary>
        /// 框架内部对于请求的标识
        /// </summary>
        private string RequestFlag { get; set; }

        internal FriendAddRequestContext(string requestFlag)
        {
            RequestFlag = requestFlag;
        }

        /// <summary>
        /// 处理请求结果的方法
        /// </summary>
        /// <param name="result">处理结果</param>
        public void SetRequestResult(RequestHandleResult result)
        {
            // TODO: Handle Friend Add Request
        }
    }
}
