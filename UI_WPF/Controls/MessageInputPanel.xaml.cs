using Another_Mirai_Native.UI.Pages.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// MessageInputPanel.xaml 的交互逻辑
    /// 包含工具栏、输入框、发送按钮的组合控件
    /// </summary>
    public partial class MessageInputPanel : UserControl
    {
        public MessageInputPanel()
        {
            InitializeComponent();
            
            // 绑定粘贴事件
            DataObject.AddPastingHandler(SendText, RichTextboxPasteOverrideAction);
            
            // 绑定键盘事件
            SendText.PreviewKeyDown += SendText_PreviewKeyDown;
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
            DependencyProperty.Register(nameof(IsFaceEnabled), typeof(bool), typeof(MessageInputPanel), new PropertyMetadata(false));

        /// <summary>
        /// At按钮是否启用
        /// </summary>
        public bool IsAtEnabled
        {
            get => (bool)GetValue(IsAtEnabledProperty);
            set => SetValue(IsAtEnabledProperty, value);
        }

        public static readonly DependencyProperty IsAtEnabledProperty =
            DependencyProperty.Register(nameof(IsAtEnabled), typeof(bool), typeof(MessageInputPanel), new PropertyMetadata(false));

        /// <summary>
        /// 图片按钮是否启用
        /// </summary>
        public bool IsPictureEnabled
        {
            get => (bool)GetValue(IsPictureEnabledProperty);
            set => SetValue(IsPictureEnabledProperty, value);
        }

        public static readonly DependencyProperty IsPictureEnabledProperty =
            DependencyProperty.Register(nameof(IsPictureEnabled), typeof(bool), typeof(MessageInputPanel), new PropertyMetadata(false));

        /// <summary>
        /// 音频按钮是否启用
        /// </summary>
        public bool IsAudioEnabled
        {
            get => (bool)GetValue(IsAudioEnabledProperty);
            set => SetValue(IsAudioEnabledProperty, value);
        }

        public static readonly DependencyProperty IsAudioEnabledProperty =
            DependencyProperty.Register(nameof(IsAudioEnabled), typeof(bool), typeof(MessageInputPanel), new PropertyMetadata(false));

        /// <summary>
        /// 清空消息按钮是否启用
        /// </summary>
        public bool IsClearMessageEnabled
        {
            get => (bool)GetValue(IsClearMessageEnabledProperty);
            set => SetValue(IsClearMessageEnabledProperty, value);
        }

        public static readonly DependencyProperty IsClearMessageEnabledProperty =
            DependencyProperty.Register(nameof(IsClearMessageEnabled), typeof(bool), typeof(MessageInputPanel), new PropertyMetadata(false));

        /// <summary>
        /// 清空发送框按钮是否启用
        /// </summary>
        public bool IsClearSendEnabled
        {
            get => (bool)GetValue(IsClearSendEnabledProperty);
            set => SetValue(IsClearSendEnabledProperty, value);
        }

        public static readonly DependencyProperty IsClearSendEnabledProperty =
            DependencyProperty.Register(nameof(IsClearSendEnabled), typeof(bool), typeof(MessageInputPanel), new PropertyMetadata(false));

        /// <summary>
        /// 发送按钮是否启用
        /// </summary>
        public bool IsSendEnabled
        {
            get => (bool)GetValue(IsSendEnabledProperty);
            set => SetValue(IsSendEnabledProperty, value);
        }

        public static readonly DependencyProperty IsSendEnabledProperty =
            DependencyProperty.Register(nameof(IsSendEnabled), typeof(bool), typeof(MessageInputPanel), new PropertyMetadata(false));

        #endregion

        #region 事件

        /// <summary>
        /// 表情选择完成
        /// </summary>
        public event EventHandler<string>? FaceSelected;

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

        /// <summary>
        /// 请求发送消息
        /// </summary>
        public event EventHandler<string>? SendMessageRequested;

        /// <summary>
        /// 请求清空发送框
        /// </summary>
        public event EventHandler? ClearSendBoxRequested;

        /// <summary>
        /// 请求显示At选择器（Shift+@触发）
        /// </summary>
        public event EventHandler? ShowAtSelectorRequested;

        #endregion

        #region 事件处理

        private void FaceImageSelector_ImageSelected(object sender, EventArgs e)
        {
            var cqCode = FaceImageSelector.SelectedImageCQCode;
            FaceSelected?.Invoke(this, cqCode);
            InsertText(cqCode);
            FaceImageFlyout.Hide();
        }

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

        private void CleanSendBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearSendBox();
            ClearSendBoxRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void SendText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                e.Handled = true;
                SendMessage();
            }
            else if (e.Key == Key.D2 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                // 触发@
                e.Handled = true;
                ShowAtSelectorRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RichTextboxPasteOverrideAction(object sender, DataObjectPastingEventArgs e)
        {
            RichTextBoxHelper.HandlePaste(e, SendText);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取输入框的文本内容（CQ码格式）
        /// </summary>
        /// <returns>CQ码格式的消息文本</returns>
        public string GetMessageText()
        {
            return RichTextBoxHelper.ConvertToCQCode(SendText);
        }

        /// <summary>
        /// 在输入框中插入文本
        /// </summary>
        /// <param name="text">要插入的文本</param>
        public void InsertText(string text)
        {
            RichTextBoxHelper.InsertText(SendText, text);
        }

        /// <summary>
        /// 清空发送框
        /// </summary>
        public void ClearSendBox()
        {
            RichTextBoxHelper.Clear(SendText);
        }

        /// <summary>
        /// 更新按钮状态
        /// </summary>
        /// <param name="isGroupChat">是否为群聊（群聊时启用@按钮）</param>
        /// <param name="hasSelected">是否选中了聊天对象</param>
        public void UpdateButtonStates(bool isGroupChat, bool hasSelected)
        {
            if (!hasSelected)
            {
                IsFaceEnabled = false;
                IsAtEnabled = false;
                IsPictureEnabled = false;
                IsAudioEnabled = false;
                IsClearMessageEnabled = false;
                IsClearSendEnabled = false;
                IsSendEnabled = false;
            }
            else
            {
                IsFaceEnabled = true;
                IsAtEnabled = isGroupChat;
                IsPictureEnabled = true;
                IsAudioEnabled = true;
                IsClearMessageEnabled = true;
                IsClearSendEnabled = true;
                IsSendEnabled = true;
            }
        }

        #endregion

        #region 私有方法

        private void SendMessage()
        {
            var text = GetMessageText();
            if (!string.IsNullOrWhiteSpace(text))
            {
                SendMessageRequested?.Invoke(this, text);
                ClearSendBox();
            }
        }

        #endregion
    }
}
