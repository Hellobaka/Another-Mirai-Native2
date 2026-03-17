using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 图片消息片段。
    /// </summary>
    public class Image : MessageItemBase
    {
        /// <summary>
        /// 初始化 <see cref="Image"/>。filePath 与 hash 只能二选一，并且最少传递一个。若两个都被传递，则优先使用 filePath。
        /// </summary>
        /// <param name="filePath">图片文件路径。</param>
        /// <param name="hash">图片哈希。</param>
        /// <param name="isFlash">是否为闪照。</param>
        /// <param name="isEmoji">是否为表情包样式。</param>
        public Image(string filePath = "", string hash = "", bool isFlash = false, bool isEmoji = false)
        {
            FilePath = filePath;
            Hash = hash;
            IsFlash = isFlash;
            IsEmoji = isEmoji;
        }

        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Image;

        /// <summary>
        /// 图片文件路径。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 图片哈希。
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// 是否为闪照。
        /// </summary>
        public bool IsFlash { get; set; }

        /// <summary>
        /// 是否为表情包子类型。
        /// </summary>
        public bool IsEmoji { get; set; }

        /// <inheritdoc/>
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
