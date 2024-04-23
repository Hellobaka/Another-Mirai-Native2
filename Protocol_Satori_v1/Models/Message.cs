using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Protocol.Satori.Models
{
    public class Message
    {
        public string id { get; set; }
        public string content { get; set; }
        public long create_at { get; set; }
        public long update_at { get; set;}
    }
}
