using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.IO;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 语音消息片段。
    /// </summary>
    public class Record : MessageItemBase
    {
        /// <summary>
        /// 初始化 <see cref="Record"/>。filePath 与 hash 只能二选一，并且最少传递一个。若两个都被传递，则优先使用 filePath。
        /// </summary>
        /// <param name="filePath">语音文件路径。</param>
        /// <param name="hash">语音哈希。</param>
        public Record(string filePath = "", string hash = "")
        {
            FilePath = filePath;
            Hash = hash;
        }

        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Record;

        /// <summary>
        /// 语音文件路径。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 语音哈希。
        /// </summary>
        public string Hash { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                string baseDirectory = Environment.CurrentDirectory;
                string recordDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "data", "record"));

                // 检查路径是否绝对路径或是在 data\record 目录下的相对路径
                string absolute = FilePath;
                string relative = Path.Combine(recordDirectory, FilePath);

                bool isAbsolute = File.Exists(absolute);
                bool isRelative = File.Exists(relative);

                if (isRelative)
                {
                    relative = Helper.GetRelativePath(relative, recordDirectory);
                }
                else if (isAbsolute)
                {
                    // 将语音拷贝到 data\record\cached 下
                    string cachedDirectory = Path.Combine(recordDirectory, "cached");
                    Directory.CreateDirectory(cachedDirectory);
                    string fileName = Path.GetFileName(absolute);
                    string newPath = Path.Combine(cachedDirectory, fileName);
                    File.Copy(absolute, newPath, true);
                    relative = $"cached\\{Path.GetFileName(newPath)}";
                }
                else
                {
                    throw new FileNotFoundException($"无法从提供的路径({FilePath})获取到文件路径");
                }

                return $"[CQ:record,file={relative}]";
            }
            else if (!string.IsNullOrEmpty(Hash))
            {
                return $"[CQ:record,file={Hash}]";
            }
            throw new ArgumentNullException("音频元素参数无效，无法转换出发送文本，需要至少传递一个文件路径或者Hash");
        }
    }
}
