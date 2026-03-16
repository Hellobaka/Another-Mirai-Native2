using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.IO;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class Record : MessageItemBase
    {
        public Record(string filePath = "", string hash = "")
        {
            FilePath = filePath;
            Hash = hash;
        }

        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Record;

        public string FilePath { get; set; }

        public string Hash { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                string baseDirectory = Environment.CurrentDirectory;
                // 检查路径是否在data\record下
                string absolute = Path.GetFullPath(FilePath);
                if (!File.Exists(absolute))
                {
                    throw new FileNotFoundException($"无法从提供的路径({FilePath})获取到文件路径");
                }

                bool isInRecordFolder = absolute.StartsWith(baseDirectory);
                string relative;
                if (isInRecordFolder)
                {
                    relative = Helper.GetRelativePath(absolute, Path.Combine(baseDirectory, "data", "record"));
                }
                else
                {
                    // 将图片拷贝到 data\record\cached下
                    string newPath = Path.Combine(baseDirectory, "data", "record", "cached", Path.GetFileName(absolute));
                    File.Copy(absolute, newPath);
                    relative = $"cached\\{Path.GetFileName(newPath)}";
                }

                return $"[CQ:record,file={relative}]";
            }
            else if (!string.IsNullOrEmpty(Hash))
            {
                return $"[CQ:record,file={Hash}]";
            }
            throw new ArgumentNullException("图片元素参数无效，无法转换出发送文本，需要至少传递一个文件路径或者Hash");
        }
    }
}
