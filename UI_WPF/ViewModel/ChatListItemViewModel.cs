using Another_Mirai_Native.UI.Controls;
using System;

namespace Another_Mirai_Native.UI.ViewModel
{
    public class ChatListItemViewModel
    {
        public string GroupName { get; set; } = "";

        public long Id { get; set; }

        public string Detail { get; set; } = "";

        public DateTime Time { get; set; }

        public ChatAvatar.AvatarTypes AvatarType { get; set; } = ChatAvatar.AvatarTypes.Fallback;
    }
}