using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Attributes
{
    /// <summary>
    /// 指定与特定事件类型关联的插件事件处理器的优先级。
    /// </summary>
    /// <param name="eventType">优先级所适用的插件事件类型。</param>
    /// <param name="priority">事件处理器的优先级值。数值越高表示优先级越高。默认值为 1。</param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventPriorityAttribute(PluginEventType eventType, int priority = 1) : Attribute
    {
        /// <summary>
        /// 需要设置优先级的插件事件类型。
        /// </summary>
        public PluginEventType EventType { get; set; } = eventType;

        /// <summary>
        /// 事件优先级，数值越高表示优先级越高。默认值为 1。
        /// </summary>
        public int Priority { get; set; } = priority;
    }
}
