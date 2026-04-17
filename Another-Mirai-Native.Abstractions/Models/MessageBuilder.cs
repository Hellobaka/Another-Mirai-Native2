using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models.MessageItem;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 用于构建 CQ 消息内容的构建器。
    /// </summary>
    public class MessageBuilder
    {
        /// <summary>
        /// 当前构建中的消息片段集合。
        /// </summary>
        public List<MessageItemBase> Items { get; set; }

        /// <summary>
        /// 构建最终消息字符串。
        /// </summary>
        /// <returns>拼接后的消息文本。</returns>
        public string Build()
        {
            StringBuilder builder = new();
            foreach (var item in Items)
            {
                builder.Append(item.ToString());
            }

            return builder.ToString();
        }

        /// <summary>
        /// 添加文本消息片段。
        /// </summary>
        /// <param name="text">文本内容。</param>
        /// <returns>当前构建器实例。</returns>
        public MessageBuilder Text(string text)
        {
            Items.Add(new Text(text));
            return this;
        }

        /// <summary>
        /// 添加图片消息片段（本地路径）。
        /// </summary>
        /// <param name="filePath">图片路径，绝对路径或相对于 data\image 路径</param>
        /// <returns>当前构建器实例。</returns>
        public MessageBuilder Image(string filePath)
        {
            Items.Add(new Image(filePath: filePath));
            return this;
        }

        /// <summary>
        /// 添加图片消息片段（图片哈希）。
        /// </summary>
        /// <param name="hash">图片哈希值。</param>
        /// <returns>当前构建器实例。</returns>
        public MessageBuilder ImageHash(string hash)
        {
            Items.Add(new Image(hash: hash));
            return this;
        }

        /// <summary>
        /// 添加语音消息片段（本地路径）。
        /// </summary>
        /// <param name="filePath">语音文件路径，绝对路径或相对于 data\record 路径</param>
        /// <returns>当前构建器实例。</returns>
        public MessageBuilder Record(string filePath)
        {
            Items.Add(new Record(filePath));
            return this;
        }

        /// <summary>
        /// 添加语音消息片段（语音哈希）。
        /// </summary>
        /// <param name="hash">语音哈希值。</param>
        /// <returns>当前构建器实例。</returns>
        public MessageBuilder RecordHash(string hash)
        {
            Items.Add(new Record(hash));
            return this;
        }

        /// <summary>
        /// 添加 @某人 消息片段。
        /// </summary>
        /// <param name="qq">目标 QQ。</param>
        /// <returns>当前构建器实例。</returns>
        public MessageBuilder At(long qq)
        {
            Items.Add(new At(qq, false));
            return this;
        }

        /// <summary>
        /// 添加 @全体成员 消息片段。
        /// </summary>
        /// <returns>当前构建器实例。</returns>
        public MessageBuilder AtAll()
        {
            Items.Add(new At(0, true));
            return this;
        }

        /// <summary>
        /// 添加表情消息片段。
        /// </summary>
        /// <returns>当前构建器实例。</returns>
        public MessageBuilder Face(CQFace face)
        {
            Items.Add(new Face(face));
            return this;
        }
    }
}
