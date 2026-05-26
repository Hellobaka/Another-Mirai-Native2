using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>
    /// 文件片段
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="fileSize">文件大小（字节）</param>
    public class FileItem(string fileName, int fileSize): MessageItemBase
    {
        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.File;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; } = fileName;

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public int FileSize { get; } = fileSize;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[CQ:file,file={FileName},file_size={FileSize}]";
        }
    }
}
