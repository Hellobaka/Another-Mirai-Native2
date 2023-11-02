using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Protocol.OneBot.Notice
{
    public class FriendMessageRecall
    {
        public long time { get; set; }

        public long self_id { get; set; }

        public string post_type { get; set; }

        public string notice_type { get; set; }

        public long user_id { get; set; }

        public long message_id { get; set; }
    }
}