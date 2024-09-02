using System.Collections.Generic;

namespace Another_Mirai_Native.Protocol.QQ.Model
{
    public class MessageAkrKv
    {
        /// <summary>
        /// key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// value
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// ark obj类型的列表
        /// </summary>
        public List<MessageArkObj> obj { get; set; }
    }
}