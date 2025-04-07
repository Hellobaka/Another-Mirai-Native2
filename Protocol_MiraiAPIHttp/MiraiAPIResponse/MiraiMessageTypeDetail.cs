﻿using Another_Mirai_Native.Enums;

namespace Another_Mirai_Native.Protocol.MiraiAPIHttp.MiraiAPIResponse
{
    public class MiraiMessageTypeDetail
    {
        public class Source : IMiraiMessageBase
        {
            public string type { get; set; } = "Source";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Source;

            public int id { get; set; }

            public long time { get; set; }
        }

        public class Quote : IMiraiMessageBase
        {
            public string type { get; set; } = "Quote";

            public long id { get; set; }

            public long groupId { get; set; }

            public long senderId { get; set; }

            public long targetId { get; set; }

            public Origin[] origin { get; set; }

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Quote;

            public class Origin
            {
                public string type { get; set; }

                public string text { get; set; }
            }
        }

        public class At : IMiraiMessageBase
        {
            public string type { get; set; } = "At";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.At;

            public long target { get; set; }

            public string display { get; set; }
        }

        public class AtAll : IMiraiMessageBase
        {
            public string type { get; set; } = "AtAll";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.AtAll;
        }

        public class Face : IMiraiMessageBase
        {
            public string type { get; set; } = "Face";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Face;

            public int faceId { get; set; }

            public string name { get; set; } = "";

            public bool superFace { get; set; }
        }

        public class Plain : IMiraiMessageBase
        {
            public string type { get; set; } = "Plain";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Plain;

            public string text { get; set; }
        }

        public class Image : IMiraiMessageBase
        {
            public string type { get; set; } = "Image";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Image;

            public string imageId { get; set; }

            public string url { get; set; }

            public string path { get; set; }

            public string base64 { get; set; }

            public bool isEmoji { get; set; }
        }

        public class FlashImage : IMiraiMessageBase
        {
            public string type { get; set; } = "FlashImage";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.FlashImage;

            public string imageId { get; set; }

            public string url { get; set; }

            public string path { get; set; }

            public string base64 { get; set; }
        }

        public class Voice : IMiraiMessageBase
        {
            public string type { get; set; } = "Voice";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Voice;

            public string voiceId { get; set; }

            public string url { get; set; }

            public string path { get; set; }

            public string base64 { get; set; }

            public long length { get; set; }
        }

        public class Xml : IMiraiMessageBase
        {
            public string type { get; set; } = "Xml";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Xml;

            public string xml { get; set; }
        }

        public class Json : IMiraiMessageBase
        {
            public string type { get; set; } = "Json";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Json;

            public string json { get; set; }
        }

        public class App : IMiraiMessageBase
        {
            public string type { get; set; } = "App";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.App;

            public string content { get; set; }
        }

        public class Poke : IMiraiMessageBase
        {
            public string type { get; set; } = "Poke";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Poke;

            public string name { get; set; }
        }

        public class Dice : IMiraiMessageBase
        {
            public string type { get; set; } = "Dice";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Dice;

            public long value { get; set; }
        }

        public class MarketFace : IMiraiMessageBase
        {
            public string type { get; set; } = "MarketFace";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.MarketFace;

            public long id { get; set; }

            public string name { get; set; }
        }

        public class MusicShare : IMiraiMessageBase
        {
            public string type { get; set; } = "MusicShare";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.MusicShare;

            public string kind { get; set; }

            public string title { get; set; }

            public string summary { get; set; }

            public string jumpUrl { get; set; }

            public string pictureUrl { get; set; }

            public string musicUrl { get; set; }

            public string brief { get; set; }
        }

        public class Forward : IMiraiMessageBase
        {
            public string type { get; set; } = "Forward";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Forward;

            public Nodelist[] nodeList { get; set; }

            public class Nodelist
            {
                public long senderId { get; set; }

                public long time { get; set; }

                public string senderName { get; set; }

                public object[] messageChain { get; set; }

                public long? messageId { get; set; }
            }
        }

        public class File : IMiraiMessageBase
        {
            public string type { get; set; } = "File";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.File;

            public string id { get; set; }

            public string name { get; set; }

            public long size { get; set; }
        }

        public class MiraiCode : IMiraiMessageBase
        {
            public string type { get; set; } = "MiraiCode";

            public MiraiMessageType messageType { get; set; } = MiraiMessageType.MiraiCode;

            public string code { get; set; }
        }
    }
}