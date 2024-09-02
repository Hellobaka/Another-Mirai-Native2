using System.Collections.Generic;

namespace Another_Mirai_Native.Protocol.QQ.Model
{
    public class MessageArk
    {
        /// <summary>
        /// ark模板id（需要先申请）
        /// </summary>
        public int template_id { get; set; }

        /// <summary>
        /// kv值列表
        /// </summary>
        public List<MessageAkrKv> kv { get; set; }
    }
}