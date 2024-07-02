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
            info.Group = binaryReader.ReadInt64();
            short length = binaryReader.ReadInt16();
            info.Name = Helper.GB18030.GetString(binaryReader.ReadBytes(length));
            info.CurrentMemberCount = binaryReader.ReadInt32();
            info.MaxMemberCount = binaryReader.ReadInt32();

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

            int count = binaryReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                short length = binaryReader.ReadInt16();
                list.Add(FromNative(binaryReader.ReadBytes(length)));
            }
            return list;
        }
    }
}