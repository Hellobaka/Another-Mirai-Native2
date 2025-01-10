﻿using Another_Mirai_Native.UI.Controls;
using System;
using System.ComponentModel;

namespace Another_Mirai_Native.UI.ViewModel
{
    public class ChatListItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ChatAvatar.AvatarTypes AvatarType { get; set; } = ChatAvatar.AvatarTypes.Fallback;

        public string Detail { get; set; } = "";

        public string GroupName { get; set; } = "";

        public long Id { get; set; }

        public DateTime Time { get; set; }

        public int UnreadCount { get; set; }

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public override bool Equals(object? obj)
        {
            if (obj is ChatListItemViewModel viewModel)
            {
                return viewModel.Id == Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}