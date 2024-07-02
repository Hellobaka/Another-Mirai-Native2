using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using System.IO;

namespace Another_Mirai_Native.Model
{
    public class StrangerInfo
    {
        /// <summary>
        /// 获取一个值, 指示当前的账号的实例
        /// </summary>
        public long QQ { get; set; } = 10001;

        /// <summary>
        /// 获取一个值, 指示当前的QQ昵称
        /// </summary>
        public string Nick { get; set; } = "";

        /// <summary>
        /// 获取一个值, 指示当前QQ的性别
        /// </summary>
        public QQSex Sex { get; set; } = QQSex.Unknown;

        /// <summary>
        /// 获取一个值, 指示当前的年龄
        /// </summary>
        public int Age { get; set; } = 0;

        public byte[] ToNative()
        {
            MemoryStream stream = new();
            BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, QQ);
            BinaryWriterExpand.Write_Ex(binaryWriter, Nick);
            BinaryWriterExpand.Write_Ex(binaryWriter, (int)Sex);
            BinaryWriterExpand.Write_Ex(binaryWriter, Age);
            return stream.ToArray();
        }

        public static StrangerInfo FromNative(byte[] buffer)
        {
            StrangerInfo info = new();

            using BinaryReader binaryReader = new(new MemoryStream(buffer));
            info.QQ = binaryReader.ReadInt64();
            short length = binaryReader.ReadInt16();
            info.Nick = Helper.GB18030.GetString(binaryReader.ReadBytes(length));
            info.Sex = (QQSex)binaryReader.ReadInt32();
            info.Age = binaryReader.ReadInt32();

            return info;
        }

        public string ToNativeBase64()
        {
            return Convert.ToBase64String(ToNative());
        }
    }
}