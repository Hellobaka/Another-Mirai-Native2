using Another_Mirai_Native;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.ViewModel;
using Another_Mirai_Native.UI.Windows;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using XamlAnimatedGif;

namespace Another_Mirai_Native.UI.Controls.Chat
{
    public partial class ChatImageDisplay : UserControl
    {
        private CancellationTokenSource? _loadCts;
        private bool _isLoaded;
        private Brush? _initialBackground;
        private Uri? _currentImageUri;
        private string? _currentImagePath;

        public ChatImageDisplay()
        {
            InitializeComponent();
            Loaded += ChatImageDisplay_Loaded;
            Unloaded += ChatImageDisplay_Unloaded;
            _initialBackground = RootGrid.Background;
            RootGrid.SetBinding(HeightProperty, new Binding("ActualHeight")
            {
                Source = ImageHost,
                Mode = BindingMode.OneWay
            });
        }

        public CQCode? CQCode
        {
            get => (CQCode?)GetValue(CQCodeProperty);
            set => SetValue(CQCodeProperty, value);
        }

        public static readonly DependencyProperty CQCodeProperty = DependencyProperty.Register(
            nameof(CQCode),
            typeof(CQCode),
            typeof(ChatImageDisplay),
            new PropertyMetadata(null, OnDependencyPropertyChanged));

        public double MaxImageWidth
        {
            get => (double)GetValue(MaxImageWidthProperty);
            set => SetValue(MaxImageWidthProperty, value);
        }

        public static readonly DependencyProperty MaxImageWidthProperty = DependencyProperty.Register(
            nameof(MaxImageWidth),
            typeof(double),
            typeof(ChatImageDisplay),
            new PropertyMetadata(450d));

        public double MaxImageHeight
        {
            get => (double)GetValue(MaxImageHeightProperty);
            set => SetValue(MaxImageHeightProperty, value);
        }

        public static readonly DependencyProperty MaxImageHeightProperty = DependencyProperty.Register(
            nameof(MaxImageHeight),
            typeof(double),
            typeof(ChatImageDisplay),
            new PropertyMetadata(MessageItem_Common.ImageMaxHeight));

        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatImageDisplay control)
            {
                control.StartLoadImage();
            }
        }

        private void ChatImageDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            StartLoadImage();
        }

        private void ChatImageDisplay_Unloaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;
            CancelLoading();
        }

        private void RetryIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartLoadImage();
            e.Handled = true;
        }

        private void ImageHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && _currentImageUri != null)
            {
                PictureViewer pictureViewer = new()
                {
                    Image = _currentImageUri,
                    Owner = MainWindow.Instance
                };
                pictureViewer.Show();
                e.Handled = true;
            }
        }

        private void StartLoadImage()
        {
            if (!_isLoaded || CQCode is null)
            {
                return;
            }

            CancelLoading();
            ResetVisualState();
            _loadCts = new CancellationTokenSource();
            _ = LoadImageAsync(CQCode, _loadCts.Token);
        }

        private void CancelLoading()
        {
            if (_loadCts != null)
            {
                _loadCts.Cancel();
                _loadCts.Dispose();
                _loadCts = null;
            }
        }

        private void ResetVisualState()
        {
            LoadingRing.Visibility = Visibility.Visible;
            LoadingRing.IsActive = true;
            RetryIcon.Visibility = Visibility.Collapsed;
            ImageHost.Child = null;
            ImageHost.Visibility = Visibility.Collapsed;
            RootGrid.Background = _initialBackground ?? RootGrid.Background;
            RootGrid.Clip = InitialClip;
            _currentImageUri = null;
            _currentImagePath = null;
        }

        private async Task LoadImageAsync(CQCode cqCode, CancellationToken token)
        {
            string url = Helper.GetImageUrlOrPathFromCQCode(cqCode);
            string fileName = cqCode.GetPicName();
            try
            {
                var imagePath = await Helper.DownloadImageAsync(url, fileName);
                if (token.IsCancellationRequested)
                {
                    return;
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!string.IsNullOrEmpty(imagePath)
                        && File.Exists(imagePath)
                        && Uri.TryCreate(imagePath, UriKind.RelativeOrAbsolute, out var uri))
                    {
                        ApplyImage(uri, imagePath!);
                    }
                    else
                    {
                        ShowRetry();
                    }
                });
            }
            catch
            {
                if (!token.IsCancellationRequested)
                {
                    await Dispatcher.InvokeAsync(ShowRetry);
                }
            }
        }

        private void ApplyImage(Uri uri, string imagePath)
        {
            Image image = new()
            {
                Stretch = Stretch.Uniform,
                Tag = imagePath,
                ContextMenu = (ContextMenu)Resources["ImageContextMenu"]
            };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            AnimationBehavior.SetSourceUri(image, uri);
            AnimationBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);

            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = uri;
            bitmap.EndInit();
            bitmap.Freeze();

            double imageHeight = bitmap.PixelHeight > 0 ? bitmap.PixelHeight : bitmap.Height;
            double imageWidth = bitmap.PixelWidth > 0 ? bitmap.PixelWidth : bitmap.Width;
            double hostHeight = Math.Min(imageHeight, MaxImageHeight);
            if (hostHeight <= 0)
            {
                hostHeight = MaxImageHeight;
            }

            ImageHost.Height = hostHeight;
            double rate = imageHeight / hostHeight;
            if (double.IsNaN(rate) || double.IsInfinity(rate) || rate <= 0)
            {
                rate = 1;
            }

            RectangleGeometry clipGeometry = new()
            {
                RadiusX = 10 * rate,
                RadiusY = 10 * rate,
                Rect = new Rect(0, 0, imageWidth, imageHeight)
            };
            image.Clip = clipGeometry;

            ImageHost.Child = image;
            ImageHost.Visibility = Visibility.Visible;
            LoadingRing.IsActive = false;
            LoadingRing.Visibility = Visibility.Collapsed;
            RetryIcon.Visibility = Visibility.Collapsed;
            RootGrid.Background = Brushes.Transparent;
            RootGrid.Clip = null;

            _currentImageUri = uri;
            _currentImagePath = imagePath;
        }

        private void ShowRetry()
        {
            LoadingRing.IsActive = false;
            LoadingRing.Visibility = Visibility.Collapsed;
            RetryIcon.Visibility = Visibility.Visible;
        }

        private void CollectImage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentImagePath))
            {
                string path = _currentImagePath!;
                string collectImagePath = Path.Combine("data", "image", "collected");
                Directory.CreateDirectory(collectImagePath);
                if (File.Exists(path))
                {
                    File.Copy(path, Path.Combine(collectImagePath, $"{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(path)}"));
                }
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentImagePath))
            {
                string path = _currentImagePath!;
                if (File.Exists(path) is false)
                {
                    return;
                }
                SaveFileDialog saveFileDialog = new()
                {
                    Title = "图片另存为",
                    AddExtension = true,
                    FileName = Path.GetFileName(path),
                    Filter = "JPG 图片|*.jpg|JPEG 图片|*.jpeg|PNG 图片|*.png|BMP 图片|*.bmp|Webp 图片|*.webp|所有文件|*.*",
                };
                if (saveFileDialog.ShowDialog() is false)
                {
                    return;
                }
                File.Copy(path, saveFileDialog.FileName, true);
            }
        }

        private void PlusOne_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MessageViewModel viewModel)
            {
                viewModel.Message_Repeat(e);
            }
        }
    }
}
