using Another_Mirai_Native.Config;
using Lagrange.Core.Message;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    public static class MessageCacher
    {
        private static Dictionary<(ulong, uint), MessageChain> MessageCache { get; set; } = [];

        private static Dictionary<int, (ulong, uint)> MessageIDCache { get; set; } = [];

        private static int InternalMessageId { get; set; } = 1;

        private static object MessageCacheLock { get; set; } = new();

        public static int RecordMessage(MessageChain chain)
        {
            lock (MessageCacheLock)
            {
                int messageId = InternalMessageId;
                MessageIDCache[messageId] = (chain.MessageId, chain.Sequence);
                MessageCache[(chain.MessageId, chain.Sequence)] = chain;
                InternalMessageId++;

                if (messageId >= AppConfig.Instance.MessageCacheSize)
                {
                    int keyToRemove = messageId - AppConfig.Instance.MessageCacheSize + 1;
                    if (MessageIDCache.TryGetValue(keyToRemove, out var rawKey))
                    {
                        MessageIDCache.Remove(keyToRemove);
                        MessageCache.Remove(rawKey);
                    }
                }

                return messageId;
            }
        }

        public static MessageChain? GetMessageById(int messageId)
        {
            if (MessageIDCache.TryGetValue(messageId, out var rawKey)
                && MessageCache.TryGetValue(rawKey, out var chain))
            {
                return chain;
            }
            return null;
        }

        public static MessageChain? GetMessageByRawId(ulong msgId, uint seq)
        {
            if (MessageCache.TryGetValue((msgId, seq), out var chain))
            {
                return chain;
            }
            return null;
        }

        public static int GetMessageId(ulong msgId, uint seq)
        {
            return MessageIDCache.Any(x => x.Value == (msgId, seq))
                ? MessageIDCache.First(x => x.Value == (msgId, seq)).Key
                : 0;
        }
    }
}
