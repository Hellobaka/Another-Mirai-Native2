using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Another_Mirai_Native.UI.Windows
{
    /// <summary>
    /// PictureViewer.xaml 的交互逻辑
    /// </summary>
    public partial class PictureViewer : Window
    {
        private bool isDragging = false;

        private Point lastMousePosition;

        public PictureViewer()
        {
            InitializeComponent();
        }

        public BitmapImage Image { get; set; }

        public void UpdateImageScale(double scale)
        {
            if (imageScaleTransform.ScaleX >= 16 || imageScaleTransform.ScaleY >= 16
                || imageScaleTransform.ScaleX <= 0.1 || imageScaleTransform.ScaleY <= 0.1)
            {
                return;
            }
            imageScaleTransform.ScaleX += scale;
            imageScaleTransform.ScaleY += scale;

            ScaleDisplay.Text = $"{(int)(imageScaleTransform.ScaleX * 100)}%";
        }

        private void ImageDisplayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image)
            {
                lastMousePosition = e.GetPosition(null);
                isDragging = true;
                image.CaptureMouse();
            }
        }

        private void ImageDisplayer_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var currentPosition = e.GetPosition(null);
                var dx = currentPosition.X - lastMousePosition.X;
                var dy = currentPosition.Y - lastMousePosition.Y;

                imageTranslateTransform.X += dx;
                imageTranslateTransform.Y += dy;

                lastMousePosition = currentPosition;
            }
        }

        private void ImageDisplayer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var image = sender as Image;
            image?.ReleaseMouseCapture();
        }

        private void ImageDisplayer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            UpdateImageScale(e.Delta > 0 ? 0.1 : -0.1);
            var position = e.GetPosition(ImageDisplayer);

            imageScaleTransform.CenterX = position.X;
            imageScaleTransform.CenterY = position.Y;
        }

        private void ScaleDisplay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                imageTranslateTransform.X = 0;
                imageTranslateTransform.Y = 0;

                imageScaleTransform.ScaleX = 1;
                imageScaleTransform.ScaleY = 1;
                ScaleDisplay.Text = "100%";
            }
        }

        private void ScaleMinusBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateImageScale(-0.1);
        }

        private void ScalePlusBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateImageScale(0.1);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImageDisplayer.Source = Image;
        }
    }
}