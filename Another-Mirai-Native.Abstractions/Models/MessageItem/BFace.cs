using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class BFace(int face) : MessageItemBase
    {
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Bface;

        public int FaceId { get; set; } = face;

        public override string ToString()
        {
            return $"[CQ:bface,id={FaceId}]";
        }
    }
}
