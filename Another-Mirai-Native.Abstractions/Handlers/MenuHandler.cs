namespace Another_Mirai_Native.Abstractions.Handlers
{
    /// <summary>
    /// 为处理菜单事件的处理器。
    /// </summary>
    public interface IMenuHandler
    {
        /// <summary>
        /// 当框架调用菜单事件时执行的操作。框架上层会在独立的 UI 线程中同步执行此方法，阻塞此方法不会导致框架卡死。
        /// </summary>
        void OnMenu();
    }
}
