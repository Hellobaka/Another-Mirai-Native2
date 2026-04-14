using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Another_Mirai_Native.Abstractions.Attributes
{
    /// <summary>
    /// 将一个方法标记为指令处理器。框架将根据 <see cref="MatchMode"/> 和 <see cref="Template"/>
    /// 自动将匹配的消息路由到该方法。
    /// </summary>
    /// <param name="matchMode">指令的匹配模式。</param>
    /// <param name="template">用于匹配消息内容的模板字符串。</param>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute(MatchMode matchMode, string template) : Attribute
    {
        /// <summary>
        /// 指令的匹配模式。
        /// </summary>
        public MatchMode MatchMode { get; } = matchMode;

        /// <summary>
        /// 用于匹配消息内容的模板字符串。
        /// 当 <see cref="MatchMode"/> 为 <see cref="MatchMode.Regex"/> 时，此值为正则表达式。
        /// </summary>
        public string Template { get; } = template;

        /// <summary>
        /// 此指令响应的消息来源范围。默认为 <see cref="MessageScope.All"/>。
        /// </summary>
        public MessageScope Scope { get; set; } = MessageScope.All;
    }
}
