using Another_Mirai_Native.UI.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Another_Mirai_Native.UI.ViewModel
{
    /// <summary>
    /// 聊天工具栏的ViewModel
    /// </summary>
    public class ToolbarViewModel : INotifyPropertyChanged
    {
        private bool _isFaceEnabled;
        private bool _isAtEnabled;
        private bool _isPictureEnabled;
        private bool _isAudioEnabled;
        private bool _isSendEnabled;
        private bool _isClearMessageEnabled;
        private bool _isClearSendEnabled;

        public ToolbarViewModel()
        {
            // 默认状态：全部禁用
            UpdateButtonStates(null);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 表情按钮是否启用
        /// </summary>
        public bool IsFaceEnabled
        {
            get => _isFaceEnabled;
            set
            {
                if (_isFaceEnabled != value)
                {
                    _isFaceEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// At按钮是否启用
        /// </summary>
        public bool IsAtEnabled
        {
            get => _isAtEnabled;
            set
            {
                if (_isAtEnabled != value)
                {
                    _isAtEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 图片按钮是否启用
        /// </summary>
        public bool IsPictureEnabled
        {
            get => _isPictureEnabled;
            set
            {
                if (_isPictureEnabled != value)
                {
                    _isPictureEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 音频按钮是否启用
        /// </summary>
        public bool IsAudioEnabled
        {
            get => _isAudioEnabled;
            set
            {
                if (_isAudioEnabled != value)
                {
                    _isAudioEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 发送按钮是否启用
        /// </summary>
        public bool IsSendEnabled
        {
            get => _isSendEnabled;
            set
            {
                if (_isSendEnabled != value)
                {
                    _isSendEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 清空消息按钮是否启用
        /// </summary>
        public bool IsClearMessageEnabled
        {
            get => _isClearMessageEnabled;
            set
            {
                if (_isClearMessageEnabled != value)
                {
                    _isClearMessageEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 清空发送框按钮是否启用
        /// </summary>
        public bool IsClearSendEnabled
        {
            get => _isClearSendEnabled;
            set
            {
                if (_isClearSendEnabled != value)
                {
                    _isClearSendEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 根据聊天类型更新按钮状态
        /// </summary>
        /// <param name="chatType">聊天类型，null表示未选中任何聊天</param>
        public void UpdateButtonStates(ChatAvatar.AvatarTypes? chatType)
        {
            if (chatType == null)
            {
                // 未选中任何聊天，禁用所有按钮
                IsFaceEnabled = false;
                IsAtEnabled = false;
                IsPictureEnabled = false;
                IsAudioEnabled = false;
                IsSendEnabled = false;
                IsClearMessageEnabled = false;
                IsClearSendEnabled = false;
            }
            else if (chatType == ChatAvatar.AvatarTypes.QQGroup)
            {
                // 群聊：启用所有按钮
                IsFaceEnabled = true;
                IsAtEnabled = true;
                IsPictureEnabled = true;
                IsAudioEnabled = true;
                IsSendEnabled = true;
                IsClearMessageEnabled = true;
                IsClearSendEnabled = true;
            }
            else if (chatType == ChatAvatar.AvatarTypes.QQPrivate)
            {
                // 私聊：禁用At按钮，启用其他按钮
                IsFaceEnabled = true;
                IsAtEnabled = false;
                IsPictureEnabled = true;
                IsAudioEnabled = true;
                IsSendEnabled = true;
                IsClearMessageEnabled = true;
                IsClearSendEnabled = true;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
