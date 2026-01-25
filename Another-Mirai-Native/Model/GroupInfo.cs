using Another_Mirai_Native.Native;
using System.IO;

namespace Another_Mirai_Native.Model
{
    public class GroupInfo
    {
        /// <summary>
        /// 获取一个值, 指示当前QQ群对象
        /// </summary>
        public long Group { get; set; } = 10001;

        /// <summary>
        /// 获取当前QQ群的名称
        /// </summary>
        public string Name { get; set; } = "Err";

        /// <summary>
        /// 获取一个值, 指示QQ群的当前人数;
        /// </summary>
        public int CurrentMemberCount { get; set; } = 1;

        /// <summary>
        /// 获取一个值, 指示当前QQ群最大可容纳的人数;
        /// </summary>
        public int MaxMemberCount { get; set; } = 10;

        /// <summary>
        /// 最后更新时间(时间戳)
        /// </summary>
        public long LastUpdateTime { get; set; }

        public override string ToString()
        {
            return $"ID={Group}; 名称={Name}; 当前人数={CurrentMemberCount}; 最大人数={MaxMemberCount}";
        }

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

        public static GroupInfo FromNative(byte[] buffer)
        {
            GroupInfo info = new();

            using BinaryReader binaryReader = new(new MemoryStream(buffer));
            info.Group = binaryReader.ReadInt64_Ex();
            info.Name = binaryReader.ReadString_Ex();
            info.CurrentMemberCount = binaryReader.ReadInt32_Ex();
            info.MaxMemberCount = binaryReader.ReadInt32_Ex();

            return info;
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

        public static List<GroupInfo> RawToList(byte[] buffer)
        {
            List<GroupInfo> list = [];
            using BinaryReader binaryReader = new(new MemoryStream(buffer));

            int count = binaryReader.ReadInt32_Ex();
            for (int i = 0; i < count; i++)
            {
                list.Add(FromNative(binaryReader.ReadToken_Ex()));
            }
            return list;
        }
    }
}