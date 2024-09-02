using System;
using System.Collections.Generic;

namespace Another_Mirai_Native.Protocol.QQ.Model
{
    public class Message
    {
        /// <summary>
        /// 消息 id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 子频道 id
        /// </summary>
        public string channel_id { get; set; }

        /// <summary>
        /// 频道 id
        /// </summary>
        public string guild_id { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 消息创建时间
        /// </summary>
        public DateTime timestamp { get; set; }

        /// <summary>
        /// 消息编辑时间
        /// </summary>
        public DateTime edited_timestamp { get; set; }

        /// <summary>
        /// 是否是@全员消息
        /// </summary>
        public bool mention_everyone { get; set; }

        /// <summary>
        /// 消息创建者
        /// </summary>
        public User author { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<MessageAttachment> attachments { get; set; }

        /// <summary>
        /// embed
        /// </summary>
        public List<MessageEmbed> embeds { get; set; }

        /// <summary>
        /// 消息中 @的人
        /// </summary>
        public List<User> mentions { get; set; }

        /// <summary>
        /// 消息创建者的member信息
        /// </summary>
        public Member member { get; set; }

        /// <summary>
        /// ark消息
        /// </summary>
        public MessageArk ark { get; set; }

        /// <summary>
        /// 剔除了At的文本
        /// </summary>
        public string nonATMsg
        {
            get
            {
                string cloneMsg = content;
                foreach (var item in mentions)
                {
                    cloneMsg = cloneMsg.Replace($"<@!{item.id}> ", "");
                }
                return cloneMsg;
            }
        }

        public void RemoveAT()
        {
            foreach (var item in mentions)
            {
                content = content.Replace($"<@!{item.id}> ", "");
            }
        }
    }
}