using System.IO;
using System.Text;

namespace Another_Mirai_Native.Native
{
    /// <summary>
    /// <see cref="BinaryWriter"/> 类的扩展方法集
    /// </summary>
    public static class BinaryWriterExpand
    {
        #region --公开方法--

        /// <summary>
        /// 将写入基础流的 <see cref="short"/> 数值
        /// </summary>
        /// <param name="binary">基础 <see cref="BinaryWriter"/> 对象</param>
        /// <param name="value">要写入的值</param>
        /// <exception cref="IOException">出现 I/O 错误。</exception>
        /// <exception cref="ObjectDisposedException">流已关闭。</exception>
        /// <exception cref="ArgumentNullException">buffer 为 null。</exception>
        public static void Write_Ex(this BinaryWriter binary, short value)
        {
            SetBinary(binary, BitConverter.GetBytes(value), true);
        }

        /// <summary>
        /// 将写入基础流的 <see cref="int"/> 数值
        /// </summary>
        /// <param name="binary">基础 <see cref="BinaryWriter"/> 对象</param>
        /// <param name="value">要写入的值</param>
        /// <exception cref="IOException">出现 I/O 错误。</exception>
        /// <exception cref="ObjectDisposedException">流已关闭。</exception>
        /// <exception cref="ArgumentNullException">buffer 为 null。</exception>
        public static void Write_Ex(this BinaryWriter binary, int value)
        {
            SetBinary(binary, BitConverter.GetBytes(value), true);
        }

        /// <summary>
        /// 将写入基础流的 <see cref="long"/> 数值
        /// </summary>
        /// <param name="binary">基础 <see cref="BinaryWriter"/> 对象</param>
        /// <param name="value">要写入的值</param>
        /// <exception cref="IOException">出现 I/O 错误。</exception>
        /// <exception cref="ObjectDisposedException">流已关闭。</exception>
        /// <exception cref="ArgumentNullException">buffer 为 null。</exception>
        public static void Write_Ex(this BinaryWriter binary, long value)
        {
            SetBinary(binary, BitConverter.GetBytes(value), true);
        }

        /// <summary>
        /// 将写入基础流的 <see cref="string"/> 数值
        /// </summary>
        /// <param name="binary">基础 <see cref="BinaryWriter"/> 对象</param>
        /// <param name="value">要写入的值</param>
        /// <exception cref="IOException">出现 I/O 错误。</exception>
        /// <exception cref="ObjectDisposedException">流已关闭。</exception>
        /// <exception cref="ArgumentNullException">buffer 为 null。</exception>
        public static void Write_Ex(this BinaryWriter binary, string value)
        {
            byte[] buffer = Helper.GB18030.GetBytes(value);
            Write_Ex(binary, (short)buffer.Length);
            SetBinary(binary, buffer, false);
        }

        /// <summary>
        /// 将基础流转换为相同的字节数组
        /// </summary>
        /// <param name="binary">基础 <see cref="BinaryWriter"/> 对象</param>
        public static byte[] ToArray(this BinaryWriter binary)
        {
            long position = binary.BaseStream.Position;     // 记录原指针位置

            if (binary.BaseStream.Length > int.MaxValue)
            {
                throw new IOException("Stream is too large to be converted to a single byte array.");
            }

            byte[] buffer = new byte[binary.BaseStream.Length];
            binary.BaseStream.Position = 0;                 // 设置读取位置为 0

            int offset = 0;
            int count;
            while (offset < buffer.Length && (count = binary.BaseStream.Read(buffer, offset, buffer.Length - offset)) > 0)
            {
                offset += count;
            }

            if (offset != buffer.Length)
            {
                throw new EndOfStreamException("Unable to read the entire stream.");
            }

            binary.BaseStream.Position = position;          // 还原原指针位置
            return buffer;
        }

        #endregion --公开方法--

        #region --私有方法--

        private static void SetBinary(BinaryWriter binary, byte[] buffer, bool isReverse)
        {
            if (isReverse)
            {
                buffer = buffer.Reverse().ToArray();
            }
            binary.Write(buffer);
        }

        #endregion --私有方法--
    }
}