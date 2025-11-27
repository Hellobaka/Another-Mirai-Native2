using System;
using System.Windows;
using System.Windows.Controls;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// ChatToolbar.xaml 的交互逻辑
    /// 聊天工具栏，包含表情、At、图片、音频、清空消息等按钮
    /// </summary>
    public partial class ChatToolbar : UserControl
    {
        public ChatToolbar()
        {
            InitializeComponent();
        }

        #region 依赖属性

        /// <summary>
        /// 表情按钮是否启用
        /// </summary>
        public bool IsFaceEnabled
        {
            get => (bool)GetValue(IsFaceEnabledProperty);
            set => SetValue(IsFaceEnabledProperty, value);
        }

        public static readonly DependencyProperty IsFaceEnabledProperty =
            DependencyProperty.Register(nameof(IsFaceEnabled), typeof(bool), typeof(ChatToolbar), new PropertyMetadata(false));

        /// <summary>
        /// At按钮是否启用
        /// </summary>
        public bool IsAtEnabled
        {
            get => (bool)GetValue(IsAtEnabledProperty);
            set => SetValue(IsAtEnabledProperty, value);
        }

        public static readonly DependencyProperty IsAtEnabledProperty =
            DependencyProperty.Register(nameof(IsAtEnabled), typeof(bool), typeof(ChatToolbar), new PropertyMetadata(false));

        /// <summary>
        /// 图片按钮是否启用
        /// </summary>
        public bool IsPictureEnabled
        {
            get => (bool)GetValue(IsPictureEnabledProperty);
            set => SetValue(IsPictureEnabledProperty, value);
        }

        public static readonly DependencyProperty IsPictureEnabledProperty =
            DependencyProperty.Register(nameof(IsPictureEnabled), typeof(bool), typeof(ChatToolbar), new PropertyMetadata(false));

        /// <summary>
        /// 音频按钮是否启用
        /// </summary>
        public bool IsAudioEnabled
        {
            get => (bool)GetValue(IsAudioEnabledProperty);
            set => SetValue(IsAudioEnabledProperty, value);
        }

        public static readonly DependencyProperty IsAudioEnabledProperty =
            DependencyProperty.Register(nameof(IsAudioEnabled), typeof(bool), typeof(ChatToolbar), new PropertyMetadata(false));

        /// <summary>
        /// 清空消息按钮是否启用
        /// </summary>
        public bool IsClearMessageEnabled
        {
            get => (bool)GetValue(IsClearMessageEnabledProperty);
            set => SetValue(IsClearMessageEnabledProperty, value);
        }

        public static readonly DependencyProperty IsClearMessageEnabledProperty =
            DependencyProperty.Register(nameof(IsClearMessageEnabled), typeof(bool), typeof(ChatToolbar), new PropertyMetadata(false));

        #endregion

        #region 事件

        /// <summary>
        /// 点击表情按钮
        /// </summary>
        public event EventHandler? FaceButtonClicked;

        /// <summary>
        /// 点击At按钮
        /// </summary>
        public event EventHandler? AtButtonClicked;

        /// <summary>
        /// 点击图片按钮
        /// </summary>
        public event EventHandler? PictureButtonClicked;

        /// <summary>
        /// 点击音频按钮
        /// </summary>
        public event EventHandler? AudioButtonClicked;

        /// <summary>
        /// 点击清空消息按钮
        /// </summary>
        public event EventHandler? ClearMessageButtonClicked;

        #endregion

        #region 事件处理

        private void AtBtn_Click(object sender, RoutedEventArgs e)
        {
            AtButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void PictureBtn_Click(object sender, RoutedEventArgs e)
        {
            PictureButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void AudioBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void CleanMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearMessageButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 更新按钮状态
        /// </summary>
        /// <param name="chatType">聊天类型</param>
        public void UpdateButtonStates(ChatHistoryType? chatType)
        {
            if (chatType == null)
            {
                // 未选择聊天，禁用所有按钮
                IsFaceEnabled = false;
                IsAtEnabled = false;
                IsPictureEnabled = false;
                IsAudioEnabled = false;
                IsClearMessageEnabled = false;
            }
            else
            {
                // 根据聊天类型启用相应按钮
                IsFaceEnabled = true;
                IsAtEnabled = chatType == ChatHistoryType.Group;
                IsPictureEnabled = true;
                IsAudioEnabled = true;
                IsClearMessageEnabled = true;
            }
        }

        #endregion
    }
}
