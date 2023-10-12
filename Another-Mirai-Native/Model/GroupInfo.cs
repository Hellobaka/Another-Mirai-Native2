using Another_Mirai_Native.Native;

namespace Another_Mirai_Native.Model
{
    public class GroupInfo
    {
        /// <summary>
        /// 获取一个值, 指示当前QQ群对象
        /// </summary>
        public long Group { get; set; }

        /// <summary>
        /// 获取当前QQ群的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取一个值, 指示QQ群的当前人数;
        /// </summary>
        public int CurrentMemberCount { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前QQ群最大可容纳的人数;
        /// </summary>
        public int MaxMemberCount { get; set; }

        public byte[] ToNative(bool isGroupList)
        {
            MemoryStream stream = new();
            BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, Group);
            BinaryWriterExpand.Write_Ex(binaryWriter, Name);
            if (!isGroupList)
            {
                BinaryWriterExpand.Write_Ex(binaryWriter, CurrentMemberCount);
                BinaryWriterExpand.Write_Ex(binaryWriter, MaxMemberCount);
            }
            return stream.ToArray();
        }

        public string ToNativeBase64(bool isGroupList)
        {
            return Convert.ToBase64String(ToNative(isGroupList));
        }

        public static string CollectionToList(List<GroupInfo> list)
        {
            MemoryStream stream = new();
            BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, list.Count);
            foreach (var item in list)
            {
                var buffer = item.ToNative(true);
                BinaryWriterExpand.Write_Ex(binaryWriter, (short)buffer.Length);
                binaryWriter.Write(buffer);
            }
            return Convert.ToBase64String(stream.ToArray());
        }
    }
}