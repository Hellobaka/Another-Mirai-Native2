using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models.MessageItem;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;

namespace Another_Mirai_Native.WebAPI.Models
{
    public class ChatCategoryDto
    {
        public long ParentID { get; set; }
       
        public long SenderID { get; set; }
       
        public ChatHistoryType Type { get; set; }
       
        public DateTime Time { get; set; }
       
        public MessageItemBase[] Message { get; set; }
       
        public int UnreadCount { get; set; }
       
        public bool IsPinned { get; set; }

        public static ChatCategoryDto CreateFromChatCategoryEntity(ChatCategoryEntity entity)
        {
            return new ChatCategoryDto
            {
                ParentID = entity.ParentID,
                SenderID = entity.SenderID,
                Type = entity.Type,
                Time = Helper.TimeStamp2DateTime(entity.Time),
                Message = entity.Message.ToMessageChain(),
                UnreadCount = entity.UnreadCount,
                IsPinned = entity.IsPinned
            };
        }
    }

    public class ChatHistoryDto
    {
        public int ID { get; set; }
        
        public DateTime Time { get; set; }
        
        public ChatHistoryType Type { get; set; }
        
        public long ParentID { get; set; }
        
        public long SenderID { get; set; }
        
        public MessageItemBase[] Message { get; set; }
        
        public int MsgId { get; set; }
        
        public bool Recalled { get; set; }
        
        public string PluginName { get; set; } = string.Empty;

        public static ChatHistoryDto CreateFromChatHistoryEntity(ChatHistory entity)
        {
            return new ChatHistoryDto
            {
                ID = entity.ID,
                Time = entity.Time,
                Type = entity.Type,
                ParentID = entity.ParentID,
                SenderID = entity.SenderID,
                Message = entity.Message.ToMessageChain(),
                MsgId = entity.MsgId,
                Recalled = entity.Recalled,
                PluginName = entity.PluginName
            };
        }
    }
}
