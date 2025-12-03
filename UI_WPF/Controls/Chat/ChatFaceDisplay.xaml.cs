using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using XamlAnimatedGif;

namespace Another_Mirai_Native.UI.Controls.Chat
{
    public partial class ChatFaceDisplay : UserControl
    {
        public ChatFaceDisplay()
        {
            InitializeComponent();
        }

        public int FaceId
        {
            get { return (int)GetValue(FaceIdProperty); }
            set { SetValue(FaceIdProperty, value); }
        }

        public static readonly DependencyProperty FaceIdProperty =
            DependencyProperty.Register("FaceId", typeof(int), typeof(ChatFaceDisplay), new PropertyMetadata(0, OnFaceIdChanged));

        private static void OnFaceIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatFaceDisplay control)
            {
                control.UpdateFace();
            }
        }

        public bool HasFace { get; private set; }

        private void UpdateFace()
        {
            string packUri = $"pack://application:,,,/Resources/qq-face/{FaceId}.png";
            if (CheckResourceExists(packUri, out StreamResourceInfo? resource) && resource != null)
            {
                AnimationBehavior.SetSourceStream(FaceImage, resource.Stream);
                AnimationBehavior.SetRepeatBehavior(FaceImage, RepeatBehavior.Forever);
                FaceImage.Visibility = Visibility.Visible;
                HasFace = true;
            }
            else
            {
                FaceImage.Visibility = Visibility.Collapsed;
                HasFace = false;
            }
        }

        private static bool CheckResourceExists(string uri, out StreamResourceInfo? streamInfo)
        {
            streamInfo = null;
            try
            {
                Uri resourceUri = new(uri, UriKind.Absolute);
                streamInfo = Application.GetResourceStream(resourceUri);
                return streamInfo != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}