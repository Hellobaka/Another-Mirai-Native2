using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Protocol.OneBot.Notice
{
    public class GroupMemberJoin
    {
        public long time { get; set; }

        public long self_id { get; set; }

        public string post_type { get; set; }

        public string notice_type { get; set; }

        public string sub_type { get; set; }

        public long group_id { get; set; }

        public long operator_id { get; set; }

        public long user_id { get; set; }
    }
}