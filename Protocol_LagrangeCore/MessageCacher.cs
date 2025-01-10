using Another_Mirai_Native.Config;
using Lagrange.Core.Message;
using System.Reflection;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    public static class MessageCacher
    {
        private static Dictionary<(ulong, uint), MessageChain> MessageCache { get; set; } = [];

        private static Dictionary<int, (ulong, uint)> MessageIDCache { get; set; } = [];

        private static object MessageCacheLock { get; set; } = new();

        public static int RecordMessage(MessageChain chain, ulong id, uint seq)
        {
            typeof(MessageChain).GetField("<MessageId>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(chain, id);
            typeof(MessageChain).GetField("<Sequence>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(chain, seq);
            return RecordMessage(chain);
        }

        public static int RecordMessage(MessageChain chain)
        {
            lock (MessageCacheLock)
            {
                int messageId = MakeUniqueID();
                MessageIDCache[messageId] = (chain.MessageId, chain.Sequence);
                MessageCache[(chain.MessageId, chain.Sequence)] = chain;

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

        public static void UpdateChainByRawMessageId(MessageChain chain)
        {
            if (!MessageIDCache.Any(x => x.Value.Item1 == chain.MessageId))
            {
                return;
            }
            var id = MessageIDCache.FirstOrDefault(x => x.Value.Item1 == chain.MessageId);
            MessageIDCache[id.Key] = (chain.MessageId, chain.Sequence);
            var c = MessageCache.FirstOrDefault(x => x.Key.Item1 == chain.MessageId);
            MessageCache[c.Key] = chain;
        }

        public static int GetMessageId(ulong msgId, uint seq)
        {
            return MessageIDCache.Any(x => x.Value == (msgId, seq))
                ? MessageIDCache.First(x => x.Value == (msgId, seq)).Key
                : 0;
        }

        public static int GetMessageIdBySeq(uint seq)
        {
            return MessageIDCache.Any(x => x.Value.Item2 == seq)
                ? MessageIDCache.First(x => x.Value.Item2 == seq).Key
                : 0;
        }

        private static int MakeUniqueID()
        {
            int id;
            do
            {
                id = Math.Abs(Helper.Random.Next());
            }while (MessageIDCache.ContainsKey(id));

            return id;
        }
    }
}
