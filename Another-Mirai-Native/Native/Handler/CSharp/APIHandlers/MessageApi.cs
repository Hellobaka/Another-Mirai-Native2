using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.RPC;

namespace Another_Mirai_Native.Native.Handler.CSharp.APIHandlers
{
    public class MessageApi(PluginInfo pluginInfo) : IMessageApi
    {
        private PluginInfo PluginInfo { get; set; } = pluginInfo;

        private int AuthCode => PluginInfo.AuthCode;

        public bool DeleteMessage(long messageId)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_deleteMsg", true, AuthCode, messageId);
            if (ret is int r)
            {
                return r == 1;
            }
            throw new InvalidCastException($"DeleteMessage 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
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
                        Type = (Abstractions.Enums.ChatHistoryType)item.Type
                    });
                }
                return result;
            }
            throw new InvalidCastException($"GetChatHistories 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
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
                    Type = (Abstractions.Enums.ChatHistoryType)item.Type
                };
            }
            throw new InvalidCastException($"GetChatHistoryById 返回值类型错误，应当返回 string，实际返回 {ret?.GetType()}");
        }

        public int SendGroupMessage(long groupId, string message)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_sendGroupMsg", true, AuthCode, groupId, message);
            if (ret is int r)
            {
                return r;
            }
            throw new InvalidCastException($"SendGroupMessage 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }

        public int SendPrivateMessage(long userId, string message)
        {
            var ret = ClientManager.Client.InvokeCQPFuntcion("CQ_sendPrivateMsg", true, AuthCode, userId, message);
            if (ret is int r)
            {
                return r;
            }
            throw new InvalidCastException($"SendPrivateMessage 返回值类型错误，应当返回 int，实际返回 {ret?.GetType()}");
        }
    }
}
