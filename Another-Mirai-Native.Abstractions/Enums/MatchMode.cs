namespace Another_Mirai_Native.Abstractions.Enums
{
    /// <summary>
    /// 使用于指令特性的匹配模式
    /// </summary>
    public enum MatchMode
    {
        /// <summary>
        /// 指令的匹配文本必须由模板开头
        /// </summary>
        StartWith,

        /// <summary>
        /// 指令的匹配文本必须由模板结尾
        /// </summary>
        EndWith,

        /// <summary>
        /// 指令的匹配文本需要包含模板
        /// </summary>
        Contain,

        /// <summary>
        /// 正则匹配模式，可以使用具名分组来提取参数，以快捷分发到方法参数。详见文档描述。
        /// </summary>
        Regex,

        /// <summary>
        /// 匹配文本必须完全等于模板
        /// </summary>
        FullMatch,
    }
}
