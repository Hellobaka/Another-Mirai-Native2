using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;

namespace Another_Mirai_Native.Abstractions.Models.MessageItem
{
    /// <summary>视频消息片段。</summary>
    public class Video : MessageItemBase
    {
        /// <param name="filePath">本地路径（可相对于 data/video）、URL 或 base64:// 数据。</param>
        /// <param name="hash">已缓存的视频哈希；filePath 优先。</param>
        public Video(string filePath = "", string hash = "")
        {
            FilePath = filePath;
            Hash = hash;
        }

        internal Video(CQCode code) : this()
        {
            string file = code.Items["file"];
            if (file.Contains("\\") || file.Contains("/") || file.Contains("."))
            {
                FilePath = file;
            }
            else
            {
                Hash = file;
            }
            foreach (var parameter in code.Items)
            {
                if (parameter.Key != "file") Parameters.Add(parameter.Key, parameter.Value);
            }
        }

        /// <summary>附加 CQ 参数，如 cover 或 Mirai 视频转发所需的元数据。</summary>
        public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();

        /// <inheritdoc/>
        public override MessageItemType MessageItemType { get; set; } = MessageItemType.Video;

        /// <summary>本地路径、URL 或 base64:// 数据。</summary>
        public string FilePath { get; set; }

        /// <summary>视频缓存哈希。</summary>
        public string Hash { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            string file = string.IsNullOrEmpty(FilePath) ? Hash : FilePath;
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("视频需要提供文件路径、URL、base64 或缓存哈希");
            }
            var code = new CQCode(MessageItemType.Video, new KeyValuePair<string, string>("file", file));
            foreach (var parameter in Parameters)
            {
                if (parameter.Key != "file") code.Items.Add(parameter.Key, parameter.Value);
            }
            return code.ToSendString();
        }
    }
}
