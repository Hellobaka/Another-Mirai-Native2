using Another_Mirai_Native.Abstractions.Enums;

namespace Another_Mirai_Native.WebAPI.Models
{
    public class ChatMessageSendRequest
    {
        public ChatHistoryType ChatType { get; set; }
       
        public long ParentId { get; set; }
       
        public string Message { get; set; }
    }
}
