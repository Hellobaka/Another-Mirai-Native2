using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;

namespace Another_Mirai_Native.Model
{
    public class StrangerInfo
    {
        /// <summary>
        /// 获取一个值, 指示当前的账号的实例
        /// </summary>
        public long QQ { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前的QQ昵称
        /// </summary>
        public string Nick { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前QQ的性别
        /// </summary>
        public QQSex Sex { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前的年龄
        /// </summary>
        public int Age { get; set; }

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

        public string ToNativeBase64()
        {
            return Convert.ToBase64String(ToNative());
        }
    }
}