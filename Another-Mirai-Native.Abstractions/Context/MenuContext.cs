using Another_Mirai_Native.Abstractions.Services;

namespace Another_Mirai_Native.Abstractions.Context
{
    /// <summary>
    /// 提供用于描述调用窗口事件参数的类
    /// </summary>
    /// <param name="api"></param>
    public class MenuContext(IPluginApi api)
    {
        /// <summary>
        /// 获取插件 API 实例
        /// </summary>
        public IPluginApi API { get; } = api;
    }
}
