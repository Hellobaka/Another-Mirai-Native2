namespace Another_Mirai_Native.Abstractions.Services
{
    /// <summary>
    /// 定义在不同严重级别（包括调试、信息、警告、错误和致命）记录消息的契约。
    /// </summary>
    /// <remarks>此接口的实现应提供记录日志消息的基础机制。这使得应用程序能够以一致的方式记录诊断和操作信息，
    /// 同时在日志的存储位置和显示方式上提供灵活性。每个方法对应特定的日志级别，使开发人员能够根据需要
    /// 对日志输出进行分类和筛选。</remarks>
    public interface ILogger
    {
        /// <summary>
        /// 记录 Debug 等级的日志
        /// </summary>
        /// <param name="message">要记录的消息。</param>
        void Debug(string message);

        /// <summary>
        /// 记录 Info 等级的日志
        /// </summary>
        /// <param name="message">要记录的消息。</param>
        void Info(string message);

        /// <summary>
        /// 记录 Warn 等级的日志，在这个等级下，根据框架的设置可能会有桌面通知提醒。
        /// </summary>
        /// <param name="message">要记录的消息。</param>
        void Warn(string message);

        /// <summary>
        /// 记录 Error 等级的日志，在这个等级下，根据框架的设置可能会有桌面通知提醒。
        /// </summary>
        /// <param name="message">要记录的消息。</param>
        void Error(string message);

        /// <summary>
        /// 记录 Fatal 等级的日志，在这个等级下，根据框架的设置可能会有桌面通知提醒。
        /// </summary>
        /// <param name="message">要记录的消息。</param>
        void Fatal(string message);
    }
}
