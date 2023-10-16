namespace Another_Mirai_Native.Enums
{
    public interface MiraiMessageBase
    {
        string type { get; set; }

        MiraiMessageType messageType { get; set; }
    }

    public enum MiraiMessageType
    {
        Source,

        Quote,

        At,

        AtAll,

        Face,

        Plain,

        Image,

        FlashImage,

        Voice,

        Xml,

        Json,

        App,

        Poke,

        Dice,

        MarketFace,

        MusicShare,

        Forward,

        File,

        MiraiCode
    }
}