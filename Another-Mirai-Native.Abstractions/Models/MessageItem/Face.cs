using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class Face : MessageItemBase
    {
        public Face(CQFace face)
        {
            FaceId = (int)face;
        }

        public Face(int face)
        {
            FaceId = face;
        }

        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Face;

        public CQFace FaceType => (CQFace)FaceId;

        public int FaceId { get; set; }

        public override string ToString()
        {
            return $"[CQ:face,id={FaceId}]";
        }
    }
}
