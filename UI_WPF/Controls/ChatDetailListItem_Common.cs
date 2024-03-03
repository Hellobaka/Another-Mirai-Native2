using Another_Mirai_Native.Model;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.Controls
{
    public static class ChatDetailListItem_Common
    {
        public static Dictionary<string, BitmapImage> CachedImage { get; set; } = new();

        public static double ImageMaxHeight { get; set; } = 450;

        public static TextBox BuildAtElement(string nick)
        {
            var textBox = BuildTextElement($" @{nick} ");
            //textBox.SetResourceReference(TextBox.ForegroundProperty, "SystemControlHighlightAccentBrush");
            return textBox;
        }

        public static Border BuildImageElement(CQCode cqCode, double maxWidth)
        {
            ImageBrush CreateImageBrush(BitmapImage image)
            {
                var brush = new ImageBrush(image)
                {
                    Stretch = Stretch.Uniform
                };
                return brush;
            }
            void SetBorderBackground(Dispatcher dispatcher, Border border, ProgressRing progressRing, BitmapImage image)
            {
                dispatcher.BeginInvoke(() =>
                {
                    border.Width = Math.Min(image.Width, maxWidth);
                    border.Height = Math.Min(image.Height, ImageMaxHeight);

                    border.Background = CreateImageBrush(image);
                    if (image.Height > ImageMaxHeight)
                    {
                        // 超长图居中问题
                        border.Width = Math.Min(border.Width, image.Width * (ImageMaxHeight / image.Height));
                        border.Height = Math.Min(border.Height, image.Height * (border.Width / image.Width));
                    }
                    else
                    {
                        border.Width = Math.Min(border.MaxWidth, border.Width * 1.2);
                        border.Height = image.Height * (border.Width / image.Width);
                    }
                    progressRing.Visibility = Visibility.Collapsed;
                });
            }

            var border = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                MinHeight = 100,
                MinWidth = 100,
                CornerRadius = new CornerRadius(10),
            };
            border.MouseLeftButtonDown += (_, e) =>
            {
                if (e.ClickCount == 2)
                {
                    Debug.WriteLine("DbClick");
                }
            };
            border.SetResourceReference(Border.BackgroundProperty, "SystemControlPageBackgroundChromeMediumLowBrush");
            RenderOptions.SetBitmapScalingMode(border, BitmapScalingMode.Fant);
            var progressRing = new ProgressRing
            {
                IsActive = true,
                Width = 30,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            border.Child = progressRing;

            string url = Extend.GetImageUrlOrPathFromCQCode(cqCode);
            if (CachedImage.TryGetValue(url, out BitmapImage? img))
            {
                SetBorderBackground(border.Dispatcher, border, progressRing, img);
                return border;
            }

            var bitmapImage = new BitmapImage();
            bitmapImage.DownloadCompleted += (_, _) =>
            {
                if (CachedImage.ContainsKey(url))
                {
                    CachedImage[url] = bitmapImage;
                }
                else
                {
                    CachedImage.Add(url, bitmapImage);
                }
                SetBorderBackground(border.Dispatcher, border, progressRing, bitmapImage);
            };
            bitmapImage.DownloadFailed += (_, _) =>
            {
                FontIcon fontIcon = new()
                {
                    Width = 16,
                    Height = 16,
                    FontSize = 16,
                    Glyph = "\uF384"
                };
                border.Dispatcher.BeginInvoke(() =>
                {
                    border.Child = fontIcon;
                });
            };
            bitmapImage.BeginInit();
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            {
                bitmapImage.UriSource = uri;
            }
            bitmapImage.EndInit();

            // local pic
            if (!url.StartsWith("http"))
            {
                SetBorderBackground(border.Dispatcher, border, progressRing, bitmapImage);
            }
            return border;
        }

        public static TextBox BuildTextElement(string text)
        {
            return new TextBox
            {
                Text = text,
                Padding = new Thickness(10),
                TextWrapping = TextWrapping.Wrap,
                IsReadOnly = true,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            };
        }
    }
}
