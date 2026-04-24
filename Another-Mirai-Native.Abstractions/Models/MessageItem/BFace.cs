using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 原创表情消息片段。
    /// </summary>
    /// <param name="face">原创表情 ID。</param>
    public class BFace(int face) : MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Bface;

        /// <summary>
        /// 原创表情 ID。
        /// </summary>
        public int FaceId { get; set; } = face;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[CQ:bface,id={FaceId}]";
        }
    }
}
