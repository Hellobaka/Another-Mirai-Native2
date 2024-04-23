using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Protocol.Satori.Models
{
    public class GuildMember
    {
        public string nick { get; set; }
        public string avatar { get; set; }
        public long join_at { get; set; }
    }
}
