using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.RPC.Pipe
{
    /// <summary>
    /// 命名管道的分帧：长度（int）+ 内容
    /// </summary>
    public static class PipeFraming
    {
        public const int HeaderLength = 4;

        public const int MaxFrameLength = 100 * 1024 * 1024; // 100 MB

        /// <summary>
        /// 写出一帧
        /// </summary>
        public static void WriteFrame(Stream stream, byte[] payload)
        {
            byte[] frame = new byte[HeaderLength + payload.Length];
            BitConverter.GetBytes(payload.Length).CopyTo(frame, 0);
            payload.CopyTo(frame, HeaderLength);
            stream.Write(frame, 0, frame.Length);
            stream.Flush();
        }

        /// <summary>
        /// 读取一帧。返回 null 表示对端已关闭。
        /// </summary>
        public static async Task<byte[]?> ReadFrameAsync(Stream stream, CancellationToken cancellationToken)
        {
            byte[]? header = await ReadExactAsync(stream, HeaderLength, cancellationToken).ConfigureAwait(false);
            if (header == null)
            {
                return null;
            }

            int length = BitConverter.ToInt32(header, 0);
            if (length < 0 || length > MaxFrameLength)
            {
                throw new InvalidDataException($"Pipe 帧长度无效: {length}");
            }
            if (length == 0)
            {
                return Array.Empty<byte>();
            }

            byte[]? payload = await ReadExactAsync(stream, length, cancellationToken).ConfigureAwait(false);
            if (payload == null)
            {
                throw new EndOfStreamException("Pipe 读取负载时连接被关闭");
            }
            return payload;
        }

        /// <summary>读满 count 字节；对端提前关闭时返回 null。</summary>
        private static async Task<byte[]?> ReadExactAsync(Stream stream, int count, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[count];
            int read = 0;
            while (read < count)
            {
                int bytesRead;
#if NET5_0_OR_GREATER
                bytesRead = await stream.ReadAsync(buffer.AsMemory(read, count - read), cancellationToken).ConfigureAwait(false);
#else
                bytesRead = await stream.ReadAsync(buffer, read, count - read, cancellationToken).ConfigureAwait(false);
#endif
                if (bytesRead <= 0)
                {
                    return null;
                }
                read += bytesRead;
            }
            return buffer;
        }
    }
}
