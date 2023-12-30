using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;

namespace Another_Mirai_Native.RPC.Interface
{
    public interface IClient
    {
        /// <summary>
        /// 当服务端返回心跳时触发
        /// </summary>
        public event Action<bool> OnReceiveHeartBeat;

        /// <summary>
        /// 设置连接参数
        /// </summary>
        /// <returns>是否正确从配置解析出连接配置</returns>
        public bool SetConnectionConfig();

        /// <summary>
        /// 尝试连接到服务端
        /// </summary>
        /// <returns>确保连接成功后返回true</returns>
        public bool Connect();

        /// <summary>
        /// 中断连接，清理资源
        /// </summary>
        public void Close();

        /// <summary>
        /// 发送心跳信息
        /// </summary>
        public void SendHeartBeat();

        /// <summary>
        /// 同步调用CQP方法
        /// </summary>
        /// <param name="function">方法名称</param>
        /// <param name="waiting">是否等待到结果返回</param>
        /// <param name="args">参数</param>
        /// <returns>结果返回</returns>
        public object InvokeCQPFuntcion(string function, bool waiting, params object[] args);

        public int InvokeEvent(PluginEventType eventType, object[] args);

        public void KillProcess();

        public void ClientStartUp();

        public void ShowErrorDialog(string guid, string title, string content, bool canIgnore);

        public void AddLog(LogModel model);
    }
}
