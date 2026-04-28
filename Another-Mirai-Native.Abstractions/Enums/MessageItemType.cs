using System.ComponentModel;
using System.Reflection.Emit;

namespace Another_Mirai_Native.Abstractions.Enums
{
    /// <summary>
    /// 消息片段类型。
    /// </summary>
    [DefaultValue(Unknown)]
    public enum MessageItemType
    {
        /// <summary>
        /// 默认值
        /// </summary>
        [Description("unknown")] 
        Unknown,

        /// <summary>
        /// QQ表情
        /// </summary>
        [Description("face")]
        Face,

        /// <summary>
        /// 原创表情
        /// </summary>
        [Description("bface")]
        Bface,

        /// <summary>
        /// 图片
        /// </summary>
        [Description("image")]
        Image,

        /// <summary>
        /// 语音
        /// </summary>
        [Description("record")]
        Record,

        /// <summary>
        /// At默认
        /// </summary>
        [Description("at")]
        At,

        /// <summary>
        /// 猜拳魔法表情
        /// </summary>
        [Description("rps")]
        Rps,

        /// <summary>
        /// 屏幕抖动
        /// </summary>
        [Description("shake")]
        Shake,

        /// <summary>
        /// 掷骰子魔法表情
        /// </summary>
        [Description("dice")]
        Dice,

        /// <summary>
        /// 戳一戳
        /// </summary>
        [Description("poke")]
        Poke,

        /// <summary>
        /// 卡片消息
        /// </summary>
        [Description("rich")]
        Rich,

        /// <summary>
        /// 引用消息
        /// </summary>
        [Description("reply")]
        Reply,

        /// <summary>
        /// 纯文本
        /// </summary>
        [Description("text")]
        Text
    }
}
