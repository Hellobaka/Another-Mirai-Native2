using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Protocol.Satori.Models
{
    public class Events
    {
        public long id { get; set; }
        public string type { get; set; } = "";
        public string platform { get; set; } = "";
        public string self_id { get; set; } = "";
        public long timestamp { get; set; }

        public Argv argv { get; set; }
        public Button button { get; set; }
        public Channel channel { get; set; }
        public Guild guild { get; set; }
        public Login login { get; set; }
        public GuildMember member { get; set; }
        public Message message { get; set; }
        public User opeator { get; set; }
        public GuildRole role { get; set; }
        /// <summary>
        /// 目标用户
        /// </summary>
        public User user { get; set; }
    }
}
