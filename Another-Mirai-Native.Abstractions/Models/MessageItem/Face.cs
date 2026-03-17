using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// QQ 表情消息片段。
    /// </summary>
    public class Face : MessageItemBase
    {
        /// <summary>
        /// 使用预定义表情类型初始化 <see cref="Face"/>。
        /// </summary>
        /// <param name="face">表情类型。</param>
        public Face(CQFace face)
        {
            FaceId = (int)face;
        }

        /// <summary>
        /// 使用表情 ID 初始化 <see cref="Face"/>。
        /// </summary>
        /// <param name="face">表情 ID。</param>
        public Face(int face)
        {
            FaceId = face;
        }

        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Face;

        /// <summary>
        /// 表情类型。
        /// </summary>
        public CQFace FaceType => (CQFace)FaceId;

        /// <summary>
        /// 表情 ID。
        /// </summary>
        public int FaceId { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[CQ:face,id={FaceId}]";
        }
    }
}
