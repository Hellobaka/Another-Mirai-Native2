using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;

namespace Another_Mirai_Native.RPC.Interface
{
    /// <summary>
    /// 服务端的目的是接收客户端发送的RPC请求并处理
    /// 当事件请求来到时，会调用客户端的方法
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// 设置连接参数
        /// </summary>
        /// <returns>是否成功解析配置获取参数</returns>
        public bool SetConnectionConfig();

        /// <summary>
        /// 开启服务端
        /// </summary>
        /// <returns>确保正确打开后返回true</returns>
        public bool Start();

        /// <summary>
        /// 关闭服务端，清理资源
        /// </summary>
        /// <returns></returns>
        public bool Stop();
    }
}
