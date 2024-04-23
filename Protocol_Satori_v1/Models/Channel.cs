using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Protocol.Satori.Models
{
    public class Channel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string parent_id { get; set; }
        public ChannelType type { get; set; }

        public enum ChannelType
        {
            TEXT,
            DIRECT,
            CATEGORY,
            VOICE
        }
    }
}
