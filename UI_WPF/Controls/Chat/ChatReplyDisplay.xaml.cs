using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Controls.Chat
{
    public partial class ChatReplyDisplay : UserControl
    {
        public ChatReplyDisplay()
        {
            InitializeComponent();
        }

        public string Nick
        {
            get { return (string)GetValue(NickProperty); }
            set { SetValue(NickProperty, value); }
        }

        public static readonly DependencyProperty NickProperty =
            DependencyProperty.Register("Nick", typeof(string), typeof(ChatReplyDisplay), new PropertyMetadata("", OnNickChanged));

        private static void OnNickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatReplyDisplay control && e.NewValue is string val)
            {
                control.NickBlock.Text = val.Replace("\r", "").Replace("\n", "");
            }
        }

        public string Msg
        {
            get { return (string)GetValue(MsgProperty); }
            set { SetValue(MsgProperty, value); }
        }

        public static readonly DependencyProperty MsgProperty =
            DependencyProperty.Register("Msg", typeof(string), typeof(ChatReplyDisplay), new PropertyMetadata("", OnMsgChanged));

        private static void OnMsgChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatReplyDisplay control && e.NewValue is string val)
            {
                control.MsgBlock.Text = val.Replace("\r", "").Replace("\n", "");
            }
        }

        public Action? JumpAction { get; set; }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            MainBorder.Background = new SolidColorBrush(Color.FromArgb(100, 100, 100, 100));
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            MainBorder.Background = new SolidColorBrush(Color.FromArgb(0, 100, 100, 100));
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            JumpAction?.Invoke();
        }
    }
}