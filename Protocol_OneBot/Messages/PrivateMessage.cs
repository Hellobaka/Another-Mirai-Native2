using Another_Mirai_Native.Model;
using Newtonsoft.Json.Linq;

namespace Another_Mirai_Native.Protocol.OneBot.Messages
{
    public class PrivateMessage
    {
        public string post_type { get; set; }

        public string message_type { get; set; }

        public long time { get; set; }

        public long self_id { get; set; }

        public string sub_type { get; set; }

        public string raw_message { get; set; }

        public int font { get; set; }

        public Sender? sender { get; set; }

        public int message_id { get; set; }

        public long user_id { get; set; }

        public long target_id { get; set; }

        public JToken message { get; set; }

        public List<CQCode> CQCodes { get; set; } = new();

        public string ParsedMessage { get; set; } = "";

        public class Sender
        {
            public int age { get; set; }

            public string nickname { get; set; }

            public string sex { get; set; }

            public long user_id { get; set; }

            public long group_id { get; set; }
        }
    }
}