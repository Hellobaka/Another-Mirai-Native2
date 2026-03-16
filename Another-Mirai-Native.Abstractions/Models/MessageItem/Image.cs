using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    public class Image : MessageItemBase
    {
        public Image(string filePath = "", string hash = "", bool isFlash = false, bool isEmoji = false)
        {
            FilePath = filePath;
            Hash = hash;
            IsFlash = isFlash;
            IsEmoji = isEmoji;
        }

        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Image;

        public string FilePath { get; set; }

        public string Hash { get; set; }

        public bool IsFlash { get; set; }

        public bool IsEmoji { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                string baseDirectory = Environment.CurrentDirectory;
                // 检查路径是否在data\image下
                string absolute = Path.GetFullPath(FilePath);
                if (!File.Exists(absolute))
                {
                    throw new FileNotFoundException($"无法从提供的路径({FilePath})获取到文件路径");
                }

                bool isInImageFolder = absolute.StartsWith(baseDirectory);
                string relative;
                if (isInImageFolder)
                {
                    relative = Helper.GetRelativePath(absolute, Path.Combine(baseDirectory, "data", "image"));
                }
                else
                {
                    // 将图片拷贝到 data\image\cached下
                    string newPath = Path.Combine(baseDirectory, "data", "image", "cached", Path.GetFileName(absolute));
                    File.Copy(absolute, newPath);
                    relative = $"cached\\{Path.GetFileName(newPath)}";
                }

                return $"[CQ:image,file={relative}{(IsFlash ? ",flash=true" : "")}{(IsEmoji ? $",sub_type={(IsEmoji ? 1 : 0)}" : "")}]";
            }
            else if (!string.IsNullOrEmpty(Hash))
            {
                return $"[CQ:image,file={Hash}{(IsFlash ? ",flash=true" : "")}{(IsEmoji ? $",sub_type={(IsEmoji ? 1 : 0)}" : "")}]";
            }
            throw new ArgumentNullException("图片元素参数无效，无法转换出发送文本，需要至少传递一个文件路径或者Hash");
        }
    }
}
