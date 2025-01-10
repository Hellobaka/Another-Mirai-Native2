using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using XamlAnimatedGif;

namespace Another_Mirai_Native.UI.Windows
{
    /// <summary>
    /// PictureViewer.xaml 的交互逻辑
    /// </summary>
    public partial class PictureViewer : Window
    {
        private bool isDragging = false;

        private Point lastMousePosition;

        private double displayScale = 1;

        public PictureViewer()
        {
            InitializeComponent();
        }

        public Uri Image { get; set; }

        public void UpdateImageScale(double diff)
        {
            if (imageScaleTransform.ScaleX >= 16 || imageScaleTransform.ScaleY >= 16
                || imageScaleTransform.ScaleX <= 0.1 || imageScaleTransform.ScaleY <= 0.1)
            {
                return;
            }
            imageScaleTransform.ScaleX += diff;
            imageScaleTransform.ScaleY += diff;
            displayScale += diff;

            ScaleDisplay.Text = $"{(int)(displayScale * 100)}%";
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
                ResetScale();
            }
        }

        private void ResetScale()
        {
            displayScale -= imageScaleTransform.ScaleX - 1;
            ScaleDisplay.Text = $"{(int)(displayScale * 100)}%";

            imageTranslateTransform.X = 0;
            imageTranslateTransform.Y = 0;

            imageScaleTransform.ScaleX = 1;
            imageScaleTransform.ScaleY = 1;
            imageScaleTransform.CenterX = 0;
            imageScaleTransform.CenterY = 0;
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
            AnimationBehavior.AddLoadedHandler(ImageDisplayer, ImageDisplayer_Loaded);

            AnimationBehavior.SetSourceUri(ImageDisplayer, Image);
            AnimationBehavior.SetRepeatBehavior(ImageDisplayer, RepeatBehavior.Forever);
            ImageDisplayer.ContextMenu = new ContextMenu();
            var menuItem = new MenuItem() { Header = "另存为" };
            ImageDisplayer.ContextMenu.Items.Add(menuItem);
            menuItem.Click += (_, _) =>
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Title = "图片另存为",
                    AddExtension = true,
                    FileName = Path.GetFileName(Image.OriginalString),
                    Filter = "JPG 图片|*.jpg|JPEG 图片|*.jpeg|PNG 图片|*.png|BMP 图片|*.bmp|Webp 图片|*.webp|所有文件|*.*",
                };
                if (saveFileDialog.ShowDialog() is false)
                {
                    return;
                }
                File.Copy(Image.OriginalString, saveFileDialog.FileName, true);
            };
        }

        private void ImageDisplayer_Loaded(object sender, RoutedEventArgs e)
        {
            if (ImageDisplayer.Source == null)
            {
                return;
            }
            double width = ImageDisplayer.Source.Width;
            double height = ImageDisplayer.Source.Height;
            double windowWidth = Width;
            double windowHeight = Height - ControlPanel.ActualHeight;

            double scale = 1;
            if (width > height)
            {
                scale = windowWidth / width;
                if (scale > 1)// 窗口宽度大于图片，显示原图大小
                {
                    UpdateImageScale(1 / scale - 1);
                    imageScaleTransform.CenterX = windowWidth / 2;
                    imageScaleTransform.CenterY = windowHeight / 2;
                    scale = 1;
                }
            }
            else
            {
                scale = windowHeight / height;
                if (scale > 1)
                {
                    UpdateImageScale(1 / scale - 1);
                    imageScaleTransform.CenterX = windowWidth / 2;
                    imageScaleTransform.CenterY = windowHeight / 2;
                    scale = 1;
                }
            }

            displayScale = scale;
            ScaleDisplay.Text = $"{(int)(displayScale * 100)}%";
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetScale();
            ImageDisplayer_Loaded(sender, e);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}