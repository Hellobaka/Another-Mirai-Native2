using Another_Mirai_Native.Native;
using System.IO;
using System.Text;

namespace Another_Mirai_Native.Model
{
    public class FriendInfo
    {
        /// <summary>
        /// 获取一个值, 指示当前账号的实例
        /// </summary>
        public long QQ { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前的QQ昵称
        /// </summary>
        public string Nick { get; set; } = "";

        /// <summary>
        /// 获取一个值, 指示当前的备注信息
        /// </summary>
        public string Postscript { get; set; } = "";

        public byte[] ToNative()
        {
            MemoryStream stream = new();
            using BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, QQ);
            BinaryWriterExpand.Write_Ex(binaryWriter, Nick);
            BinaryWriterExpand.Write_Ex(binaryWriter, Postscript);
            return stream.ToArray();
        }

        public static FriendInfo FromNative(byte[] buffer)
        {
            FriendInfo info = new();

            using BinaryReader binaryReader = new(new MemoryStream(buffer));

            info.QQ = binaryReader.ReadInt64_Ex();
            info.Nick = binaryReader.ReadString_Ex();
            info.Postscript = binaryReader.ReadString_Ex();
            
            return info;
        }

        public static string CollectionToList(List<FriendInfo> list)
        {
            MemoryStream stream = new();
            BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, list.Count);
            foreach (var item in list)
            {
                var buffer = item.ToNative();
                BinaryWriterExpand.Write_Ex(binaryWriter, (short)buffer.Length);
                binaryWriter.Write(buffer);
            }
            return Convert.ToBase64String(stream.ToArray());
        }

        public static List<FriendInfo> RawToList(byte[] buffer)
        {
            List<FriendInfo> list = [];
            using BinaryReader binaryReader = new(new MemoryStream(buffer));

            int count = binaryReader.ReadInt32_Ex();
            for(int i = 0; i < count; i++)
            {
                list.Add(FromNative(binaryReader.ReadToken_Ex()));
            }
            return list;
        }
    }
}