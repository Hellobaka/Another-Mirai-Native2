using System.Collections.Generic;

namespace Another_Mirai_Native.Protocol.QQ.Model
{
    public class MessageEmbed
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 消息弹窗内容
        /// </summary>
        public string prompt { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        public MessageEmbedThumbnail thumbnail { get; set; }

        /// <summary>
        /// embed 字段数据
        /// </summary>
        public List<MessageEmbedField> fields { get; set; }
    }
}