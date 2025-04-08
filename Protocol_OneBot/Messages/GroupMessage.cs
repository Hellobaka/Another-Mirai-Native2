﻿using Another_Mirai_Native.Model;
using Newtonsoft.Json.Linq;

namespace Another_Mirai_Native.Protocol.OneBot.Messages
{
    public class GroupMessage
    {
        public string post_type { get; set; }

        public string message_type { get; set; }

        public long time { get; set; }

        public long self_id { get; set; }

        public string sub_type { get; set; }

        public Anonymous anonymous { get; set; }

        public int font { get; set; }

        public long group_id { get; set; }

        public JToken message { get; set; }

        public Sender? sender { get; set; }

        public long user_id { get; set; }

        public int message_id { get; set; }

        public string raw_message { get; set; }

        public string message_format { get; set; }

        public string ParsedMessage { get; set; } = "";

        public class Anonymous
        {
            public long id { get; set; }

            public string name { get; set; }

            public string flag { get; set; }
        }

        public class Sender
        {
            public int age { get; set; }

            public string area { get; set; }

            public string card { get; set; }

            public string level { get; set; }

            public string nickname { get; set; }

            public string role { get; set; }

            public string sex { get; set; }

            public string title { get; set; }

            public long user_id { get; set; }
        }
    }
}