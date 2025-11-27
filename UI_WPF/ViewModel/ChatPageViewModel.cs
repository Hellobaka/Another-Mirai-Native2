using Another_Mirai_Native.Config;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Another_Mirai_Native.UI.ViewModel
{
    /// <summary>
    /// ChatPage的ViewModel
    /// </summary>
    public class ChatPageViewModel : INotifyPropertyChanged
    {
        private ChatListItemViewModel? _selectedChatItem;
        private string _groupName = "";
        private bool _isChatEnabled;

        public ChatPageViewModel()
        {
            ChatList = new ObservableCollection<ChatListItemViewModel>();
            ToolbarViewModel = new ToolbarViewModel();
            
            // 初始化Command
            SendMessageCommand = new RelayCommand(ExecuteSendMessage, CanSendMessage);
            ClearMessageCommand = new RelayCommand(ExecuteClearMessage, CanClearMessage);
            ClearSendBoxCommand = new RelayCommand(ExecuteClearSendBox, CanClearSendBox);
            ScrollToBottomCommand = new RelayCommand(ExecuteScrollToBottom);
            ShowAtSelectorCommand = new RelayCommand(ExecuteShowAtSelector, CanShowAtSelector);
            SelectPictureCommand = new RelayCommand(ExecuteSelectPicture, CanSelectPicture);
            SelectAudioCommand = new RelayCommand(ExecuteSelectAudio, CanSelectAudio);
            
            // 检查聊天功能是否启用
            IsChatEnabled = AppConfig.Instance.EnableChat;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 聊天列表
        /// </summary>
        public ObservableCollection<ChatListItemViewModel> ChatList { get; }

        /// <summary>
        /// 工具栏ViewModel
        /// </summary>
        public ToolbarViewModel ToolbarViewModel { get; }

        /// <summary>
        /// 当前选中的聊天项
        /// </summary>
        public ChatListItemViewModel? SelectedChatItem
        {
            get => _selectedChatItem;
            set
            {
                if (_selectedChatItem != value)
                {
                    _selectedChatItem = value;
                    OnPropertyChanged();
                    OnSelectedChatItemChanged();
                }
            }
        }

        /// <summary>
        /// 当前聊天对象的名称
        /// </summary>
        public string GroupName
        {
            get => _groupName;
            set
            {
                if (_groupName != value)
                {
                    _groupName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 聊天功能是否启用
        /// </summary>
        public bool IsChatEnabled
        {
            get => _isChatEnabled;
            set
            {
                if (_isChatEnabled != value)
                {
                    _isChatEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 发送消息命令
        /// </summary>
        public ICommand SendMessageCommand { get; }

        /// <summary>
        /// 清空消息容器命令
        /// </summary>
        public ICommand ClearMessageCommand { get; }

        /// <summary>
        /// 清空发送框命令
        /// </summary>
        public ICommand ClearSendBoxCommand { get; }

        /// <summary>
        /// 滚动到底部命令
        /// </summary>
        public ICommand ScrollToBottomCommand { get; }

        /// <summary>
        /// 显示At选择器命令
        /// </summary>
        public ICommand ShowAtSelectorCommand { get; }

        /// <summary>
        /// 选择图片命令
        /// </summary>
        public ICommand SelectPictureCommand { get; }

        /// <summary>
        /// 选择音频命令
        /// </summary>
        public ICommand SelectAudioCommand { get; }

        /// <summary>
        /// 选中项改变时的事件
        /// </summary>
        public event EventHandler<ChatListItemViewModel?>? SelectedChatItemChanged;

        /// <summary>
        /// 请求发送消息的事件
        /// </summary>
        public event EventHandler<SendMessageEventArgs>? SendMessageRequested;

        /// <summary>
        /// 请求清空消息容器的事件
        /// </summary>
        public event EventHandler? ClearMessageRequested;

        /// <summary>
        /// 请求清空发送框的事件
        /// </summary>
        public event EventHandler? ClearSendBoxRequested;

        /// <summary>
        /// 请求滚动到底部的事件
        /// </summary>
        public event EventHandler? ScrollToBottomRequested;

        /// <summary>
        /// 请求显示At选择器的事件
        /// </summary>
        public event EventHandler? ShowAtSelectorRequested;

        /// <summary>
        /// 请求选择图片的事件
        /// </summary>
        public event EventHandler? SelectPictureRequested;

        /// <summary>
        /// 请求选择音频的事件
        /// </summary>
        public event EventHandler? SelectAudioRequested;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnSelectedChatItemChanged()
        {
            // 更新工具栏状态
            ToolbarViewModel.UpdateButtonStates(SelectedChatItem?.AvatarType);
            
            // 清空未读数
            if (SelectedChatItem != null)
            {
                SelectedChatItem.UnreadCount = 0;
            }

            // 触发事件
            SelectedChatItemChanged?.Invoke(this, SelectedChatItem);
        }

        private void ExecuteSendMessage(object? parameter)
        {
            if (SelectedChatItem == null) return;
            
            SendMessageRequested?.Invoke(this, new SendMessageEventArgs
            {
                TargetId = SelectedChatItem.Id,
                ChatType = SelectedChatItem.AvatarType == ChatAvatar.AvatarTypes.QQGroup 
                    ? ChatType.Group 
                    : ChatType.Private
            });
        }

        private bool CanSendMessage(object? parameter)
        {
            return SelectedChatItem != null && IsChatEnabled;
        }

        private void ExecuteClearMessage(object? parameter)
        {
            ClearMessageRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanClearMessage(object? parameter)
        {
            return SelectedChatItem != null && IsChatEnabled;
        }

        private void ExecuteClearSendBox(object? parameter)
        {
            ClearSendBoxRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanClearSendBox(object? parameter)
        {
            return SelectedChatItem != null && IsChatEnabled;
        }

        private void ExecuteScrollToBottom(object? parameter)
        {
            ScrollToBottomRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ExecuteShowAtSelector(object? parameter)
        {
            ShowAtSelectorRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanShowAtSelector(object? parameter)
        {
            return SelectedChatItem?.AvatarType == ChatAvatar.AvatarTypes.QQGroup && IsChatEnabled;
        }

        private void ExecuteSelectPicture(object? parameter)
        {
            SelectPictureRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanSelectPicture(object? parameter)
        {
            return SelectedChatItem != null && IsChatEnabled;
        }

        private void ExecuteSelectAudio(object? parameter)
        {
            SelectAudioRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanSelectAudio(object? parameter)
        {
            return SelectedChatItem != null && IsChatEnabled;
        }
    }

    /// <summary>
    /// 发送消息事件参数
    /// </summary>
    public class SendMessageEventArgs : EventArgs
    {
        public long TargetId { get; set; }
        public ChatType ChatType { get; set; }
    }
}
