using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.RPC;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Native.Handler.CSharp.APIHandlers
{
    public class MessageApi(PluginInfo pluginInfo) : IMessageApi
    {
        private PluginInfo PluginInfo { get; set; } = pluginInfo;

        private int AuthCode => PluginInfo.AuthCode;

        public bool DeleteMessage(long messageId)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_deleteMsg", true, AuthCode, messageId);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"DeleteMessage 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public async Task<bool> DeleteMessageAsync(long messageId)
        {
            var ret = await ClientManager.Client.InvokeCQPFuntcionAsync("CQ_deleteMsg", true, AuthCode, messageId);
            if (ret is long r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"DeleteMessageAsync 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public List<ChatHistory> GetChatHistories(long groupId, long qq, int count)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_getChatHistory", true, AuthCode, groupId, qq, count);
            if (ret is string r)
            {
                var l = Model.ChatHistory.RawToList(r);
                List<ChatHistory> result = [];
                foreach (var item in l)
                {
                    result.Add(new ChatHistory
                    {
                        ID = item.ID,
                        Message = item.Message,
                        MessageId = item.MsgId,
                        ParentID = item.ParentID,
                        PluginName = item.PluginName,
                        Recalled = item.Recalled,
                        SenderID = item.SenderID,
                        Time = item.Time,
                        Type = item.Type
                    });
                }
                return result;
            }
            throw new InvalidCastException($"GetChatHistories 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public async Task<List<ChatHistory>> GetChatHistoriesAsync(long groupId, long qq, int count)
        {
            var ret = await ClientManager.Client.InvokeCQPFuntcionAsync("CQ_getChatHistory", true, AuthCode, groupId, qq, count);
            if (ret is string r)
            {
                var l = Model.ChatHistory.RawToList(r);
                List<ChatHistory> result = [];
                foreach (var item in l)
                {
                    result.Add(new ChatHistory
                    {
                        ID = item.ID,
                        Message = item.Message,
                        MessageId = item.MsgId,
                        ParentID = item.ParentID,
                        PluginName = item.PluginName,
                        Recalled = item.Recalled,
                        SenderID = item.SenderID,
                        Time = item.Time,
                        Type = item.Type
                    });
                }
                return result;
            }
            throw new InvalidCastException($"GetChatHistoriesAsync 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public ChatHistory? GetChatHistoryById(long parentId, bool isGroup, int messageId)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_getChatHistoryById", true, AuthCode, parentId, isGroup, messageId);
            if (ret is string r)
            {
                var item = Model.ChatHistory.FromNative(r);
                return new ChatHistory
                {
                    ID = item.ID,
                    Message = item.Message,
                    MessageId = item.MsgId,
                    ParentID = item.ParentID,
                    PluginName = item.PluginName,
                    Recalled = item.Recalled,
                    SenderID = item.SenderID,
                    Time = item.Time,
                    Type = item.Type
                };
            }
            throw new InvalidCastException($"GetChatHistoryById 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public async Task<ChatHistory?> GetChatHistoryByIdAsync(long parentId, bool isGroup, int messageId)
        {
            var ret = await ClientManager.Client.InvokeCQPFuntcionAsync("CQ_getChatHistoryById", true, AuthCode, parentId, isGroup, messageId);
            if (ret is string r)
            {
                var item = Model.ChatHistory.FromNative(r);
                return new ChatHistory
                {
                    ID = item.ID,
                    Message = item.Message,
                    MessageId = item.MsgId,
                    ParentID = item.ParentID,
                    PluginName = item.PluginName,
                    Recalled = item.Recalled,
                    SenderID = item.SenderID,
                    Time = item.Time,
                    Type = item.Type
                };
            }
            throw new InvalidCastException($"GetChatHistoryByIdAsync 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public int SendGroupMessage(long groupId, string message)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_sendGroupMsg", true, AuthCode, groupId, message);
            if (ret is long r)
            {
                return (int)r;
            }
            throw new InvalidCastException($"SendGroupMessage 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public async Task<int> SendGroupMessageAsync(long groupId, string message)
        {
            var ret = await ClientManager.Client.InvokeCQPFuntcionAsync("CQ_sendGroupMsg", true, AuthCode, groupId, message);
            if (ret is long r)
            {
                return (int)r;
            }
            throw new InvalidCastException($"SendGroupMessageAsync 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public int SendPrivateMessage(long userId, string message)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_sendPrivateMsg", true, AuthCode, userId, message);
            if (ret is long r)
            {
                return (int)r;
            }
            throw new InvalidCastException($"SendPrivateMessage 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public async Task<int> SendPrivateMessageAsync(long userId, string message)
        {
            var ret = await ClientManager.Client.InvokeCQPFuntcionAsync("CQ_sendPrivateMsg", true, AuthCode, userId, message);
            if (ret is long r)
            {
                return (int)r;
            }
            throw new InvalidCastException($"SendPrivateMessageAsync 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }
    }
}
