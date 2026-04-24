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
                string recordDirectoryWithSeparator = recordDirectory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                    ? recordDirectory
                    : recordDirectory + Path.DirectorySeparatorChar;
                // 检查路径是否在 data\record 下
                string absolute = Path.GetFullPath(FilePath);
                if (!File.Exists(absolute))
                {
                    throw new FileNotFoundException($"无法从提供的路径({FilePath})获取到文件路径");
                }

                bool isInRecordFolder = absolute.StartsWith(recordDirectoryWithSeparator, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(absolute, recordDirectory, StringComparison.OrdinalIgnoreCase);
                string relative;
                if (isInRecordFolder)
                {
                    relative = Helper.GetRelativePath(absolute, recordDirectory);
                }
                else
                {
                    // 将语音拷贝到 data\record\cached 下
                    string cachedDirectory = Path.Combine(recordDirectory, "cached");
                    Directory.CreateDirectory(cachedDirectory);
                    string fileName = Path.GetFileName(absolute);
                    string newPath = Path.Combine(cachedDirectory, fileName);
                    if (File.Exists(newPath))
                    {
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        string extension = Path.GetExtension(fileName);
                        int counter = 1;
                        do
                        {
                            fileName = $"{fileNameWithoutExtension}_{counter}{extension}";
                            newPath = Path.Combine(cachedDirectory, fileName);
                            counter++;
                        }
                        while (File.Exists(newPath));
                    }
                    File.Copy(absolute, newPath);
                    relative = $"cached\\{Path.GetFileName(newPath)}";
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
