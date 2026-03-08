namespace Another_Mirai_Native.Abstractions.Enums
{
    /// <summary>
    /// 处理申请之后的结果
    /// </summary>
    public enum RequestHandleResult
    {
        /// <summary>
        /// 表示接受申请
        /// </summary>
        Accept,

        /// <summary>
        /// 表示拒绝申请
        /// </summary>
        Deny,

        /// <summary>
        /// 表示忽略申请，既不接受也不拒绝，保持申请状态不变
        /// </summary>
        Ignore
    }
}
