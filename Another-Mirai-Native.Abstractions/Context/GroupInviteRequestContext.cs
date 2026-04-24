using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions.Context
{
    /// <summary>
    /// 提供用于描述被邀请加入群事件参数的类
    /// </summary>
    public class GroupInviteRequestContext(IPluginApi api, DateTime sendTime, Group fromGroup, QQ fromQQ, string appendMessage, string requestFlag)
    {
        /// <summary>
        /// 获取插件 API 实例
        /// </summary>
        public IPluginApi API { get; } = api;

        /// <summary>
        /// 获取当前事件的发送时间
        /// </summary>
        public DateTime SendTime { get; } = sendTime;

        /// <summary>
        /// 获取当前事件的来源群
        /// </summary>
        public Group FromGroup { get; } = fromGroup;

        /// <summary>
        /// 获取当前事件的来源QQ
        /// </summary>
        public QQ FromQQ { get; } = fromQQ;

        /// <summary>
        /// 获取当前事件的附加消息
        /// </summary>
        public string AppendMessage { get; } = appendMessage;

        /// <summary>
        /// 框架内部对于请求的标识
        /// </summary>
        private string RequestFlag { get; } = requestFlag;

        /// <summary>
        /// 处理请求结果的方法
        /// </summary>
        /// <param name="result">处理结果</param>
        public void SetRequestResult(RequestHandleResult result)
        {
            API.GroupApi.SetGroupInviteRequest(RequestFlag, result == RequestHandleResult.Accept);
        }

        /// <summary>
        /// 异步处理请求结果的方法
        /// </summary>
        /// <param name="result">处理结果</param>
        public async Task SetRequestResultAsync(RequestHandleResult result)
        {
            await API.GroupApi.SetGroupInviteRequestAsync(RequestFlag, result == RequestHandleResult.Accept);
        }
    }
}
