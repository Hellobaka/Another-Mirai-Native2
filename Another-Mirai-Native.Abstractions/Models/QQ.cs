using Another_Mirai_Native.Abstractions.Services;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 表示个人的类，可进行与此QQ相关快捷操作
    /// </summary>
    public class QQ(IPluginApi pluginApi, long id)
    {
        internal IPluginApi PluginApi => pluginApi;

        /// <summary>
        /// 关联的QQ
        /// </summary>
        public long Id { get; private set; } = id;

        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="message">将要发送的消息</param>
        /// <returns>若发送成功则返回消息ID（根据不同的框架实现，可能会有负数），若发送失败则返回 0</returns>
        public int SendPrivateMessage(string message)
        {
            return PluginApi.MessageApi.SendPrivateMessage(Id, message);
        }

        /// <summary>
        /// 向目标用户发送名片赞。根据不同的框架实现，可能会有一些限制，例如每天只能发送一次赞等。
        /// </summary>
        /// <param name="count">发送赞是数量</param>
        /// <returns>操作是否成功</returns>
        public bool SendPraise(int count = 1)
        {
            return PluginApi.FriendApi.SendPraise(Id, count);
        }
    }
}
