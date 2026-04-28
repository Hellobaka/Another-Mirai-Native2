using System.ComponentModel;

namespace Another_Mirai_Native.Abstractions.Enums
{
    /// <summary>
	/// 表示QQ性别的枚举
	/// </summary>
	[Description("unknown")]
    public enum QQSex
    {
        /// <summary>
        /// 男性
        /// </summary>
        [Description("man")]
        Man = 0,

        /// <summary>
        /// 女性
        /// </summary>
        [Description("woman")]
        Woman = 1,

        /// <summary>
        /// 未知
        /// </summary>
        [Description("unknown")]
        Unknown = 255
    }
}