using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Another_Mirai_Native.Abstractions.Attributes
{
    /// <summary>
    /// 将一个方法标记为动态指令处理器。与 <see cref="CommandAttribute"/> 不同的是，
    /// <see cref="MemberName"/> 指向当前类上的一个字符串属性或字段名，
    /// 框架每次调度时通过反射读取该成员的当前值作为指令的匹配模板，
    /// 从而实现运行时热修改指令触发词，无需重启插件。
    /// </summary>
    /// <param name="memberName">当前类上的字符串属性或字段名称（通常使用 <see langword="nameof"/> 表达式）。</param>
    /// <param name="matchMode">指令的匹配模式。</param>
    /// <param name="scope">此指令响应的消息来源范围。默认为 <see cref="MessageScope.All"/>。</param>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DynamicCommandAttribute(string memberName, MatchMode matchMode, MessageScope scope = MessageScope.All) : Attribute
    {
        /// <summary>
        /// 当前类上的字符串属性或字段名称。框架每次调度时将读取该成员的值作为匹配模板。
        /// </summary>
        public string MemberName { get; } = memberName;

        /// <summary>
        /// 指令的匹配模式。
        /// </summary>
        public MatchMode MatchMode { get; } = matchMode;

        /// <summary>
        /// 此指令响应的消息来源范围。默认为 <see cref="MessageScope.All"/>。
        /// </summary>
        public MessageScope Scope { get; set; } = scope;
    }
}
