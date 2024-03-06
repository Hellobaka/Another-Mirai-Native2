using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.UI.Controls;
using System;
using System.ComponentModel;

namespace Another_Mirai_Native.UI.ViewModel
{
    public enum DetailItemType
    {
        Notice,

        Receive,

        Send
    }

    public class ChatDetailItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChatAvatar.AvatarTypes AvatarType { get; set; } = ChatAvatar.AvatarTypes.Fallback;

        public string Content { get; set; } = "";

        public DetailItemType DetailItemType { get; set; } = DetailItemType.Send;

        public QQGroupMemberType GroupMemberType { get; set; } = QQGroupMemberType.Member;

        public long Id { get; set; }

        public string Nick { get; set; } = "";

        public DateTime Time { get; set; } = DateTime.Now;

        public string GUID { get; set; } = Guid.NewGuid().ToString();

        public bool Recalled { get; set; }

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public int MsgId { get; set; }
    }
}