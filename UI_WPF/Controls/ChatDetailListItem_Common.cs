using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Pages;
using Another_Mirai_Native.UI.Windows;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using XamlAnimatedGif;

namespace Another_Mirai_Native.UI.Controls
{
    public static class ChatDetailListItem_Common
    {
        public static Dictionary<string, BitmapImage> CachedImage { get; set; } = new();

        public static double ImageMaxHeight { get; set; } = 450;

        public static ContextMenu BuildAvatarContextMenu(Action copyNick = null, Action copyQQ = null, Action at = null)
        {
            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(new MenuItem { Header = "复制昵称" });
            contextMenu.Items.Add(new MenuItem { Header = "复制QQ" });
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new MenuItem { Header = "@" });

            (contextMenu.Items[0] as MenuItem).Click += (sender, _) =>
            {
                copyNick?.Invoke();
            };
            (contextMenu.Items[1] as MenuItem).Click += (sender, e) =>
            {
                copyQQ?.Invoke();
            };
            (contextMenu.Items[3] as MenuItem).Click += (sender, e) =>
            {
                at?.Invoke();
            };

            return contextMenu;
        }

        public static ContextMenu BuildDetailContextMenu(Action repeat = null, Action recall = null, Action at = null, Action copy = null)
        {
            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(new MenuItem { Header = "复制", Icon = new FontIcon { Glyph = "\uE16F" } });
            contextMenu.Items.Add(new MenuItem { Header = "+1", Icon = new FontIcon { Glyph = "\uE8ED" } });
            contextMenu.Items.Add(new MenuItem { Header = "@", Icon = new FontIcon { Glyph = "\uE9B2" } });
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new MenuItem { Header = "撤回", Icon = new FontIcon { Glyph = "\uE107" } });

            (contextMenu.Items[0] as MenuItem).Click += (sender, _) =>
            {
                copy?.Invoke();
            };
            (contextMenu.Items[1] as MenuItem).Click += (sender, e) =>
            {
                repeat?.Invoke();
            };
            (contextMenu.Items[2] as MenuItem).Click += (sender, e) =>
            {
                at?.Invoke();
            };
            (contextMenu.Items[4] as MenuItem).Click += (sender, _) =>
            {
                recall?.Invoke();
            };

            return contextMenu;
        }

        public static ContextMenu BuildGroupContextMenu(Action copyName = null, Action copyGroupId = null)
        {
            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(new MenuItem { Header = "复制名称" });
            contextMenu.Items.Add(new MenuItem { Header = "复制ID" });

            (contextMenu.Items[0] as MenuItem).Click += (sender, _) =>
            {
                copyName?.Invoke();
            };
            (contextMenu.Items[1] as MenuItem).Click += (sender, e) =>
            {
                copyGroupId?.Invoke();
            };

            return contextMenu;
        }

        public static Grid BuildImageElement(CQCode cqCode, double maxWidth)
        {
            Grid grid = new()
            {
                MinHeight = 100,
                MinWidth = 100,
                MaxWidth = maxWidth,
                Clip = new RectangleGeometry
                {
                    RadiusX = 10,
                    RadiusY = 10,
                    Rect = new Rect(0, 0, 100, 100)
                }
            };
            // 背景色绑定
            grid.SetResourceReference(Border.BackgroundProperty, "SystemControlPageBackgroundChromeMediumLowBrush");
            var viewBox = new Viewbox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                MinHeight = 100,
                MinWidth = 100,
                MaxHeight = ImageMaxHeight,
                Visibility = Visibility.Collapsed
            };
            grid.Children.Add(viewBox);
            // 图片渲染质量
            RenderOptions.SetBitmapScalingMode(viewBox, BitmapScalingMode.Fant);
            // 进度环
            var progressRing = new ProgressRing
            {
                IsActive = true,
                Width = 30,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(progressRing);
            // 解析图片路径
            string url = Extend.GetImageUrlOrPathFromCQCode(cqCode);

            Task.Run(async () =>
            {
                var bitmapImage = await DownloadImageAsync(url);
                await viewBox.Dispatcher.BeginInvoke(() =>
                {
                    if (bitmapImage != null)
                    {
                        // 显示图片元素
                        Image image = new()
                        {
                            Stretch = Stretch.Uniform,
                            Source = bitmapImage
                        };
                        // 图片双击事件
                        viewBox.MouseLeftButtonDown += (_, e) =>
                        {
                            if (e.ClickCount == 2)
                            {
                                PictureViewer pictureViewer = new() { Image = bitmapImage };
                                pictureViewer.Owner = MainWindow.Instance;
                                pictureViewer.Show();
                            }
                        };
                        AnimationBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);
                        // 拉伸容器高度
                        viewBox.Height = Math.Min(bitmapImage.Height, viewBox.MaxHeight);
                        // 计算变化比率用于圆角尺寸统一
                        double rate = bitmapImage.Height / viewBox.Height;
                        RectangleGeometry clipGeometry = new()
                        {
                            RadiusX = 10 * rate,
                            RadiusY = 10 * rate,
                            Rect = new Rect(0, 0, bitmapImage.Width, bitmapImage.Height)
                        };
                        image.Clip = clipGeometry;
                        // 设置背景容器背景色透明
                        // 将Grid的Height与ViewBox绑定
                        var binding = new Binding
                        {
                            Source = viewBox,
                            Path = new PropertyPath("ActualHeight"),
                            Mode = BindingMode.OneWay
                        };
                        grid.SetBinding(Grid.HeightProperty, binding);
                        grid.Background = Brushes.Transparent;
                        grid.Clip = null;
                        viewBox.Child = image;
                        // 显示图片元素
                        viewBox.Visibility = Visibility.Visible;
                        progressRing.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        FontIcon fontIcon = new()
                        {
                            Width = 16,
                            Height = 16,
                            FontSize = 16,
                            Glyph = "\uF384",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                        grid.Children.Remove(progressRing);
                        grid.Children.Add(fontIcon);
                    }
                });
            });

            return grid;
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
                BorderThickness = new Thickness(0),
                ContextMenu = null
            };
        }

        public static async Task<BitmapImage> DownloadImageAsync(string imageUrl)
        {
            try
            {
                if (CachedImage.ContainsKey(imageUrl))
                {
                    return CachedImage[imageUrl];
                }

                if (!imageUrl.StartsWith("http"))
                {
                    BitmapImage localImage = new();
                    localImage.BeginInit();
                    if (Uri.TryCreate(imageUrl, UriKind.RelativeOrAbsolute, out var uri))
                    {
                        localImage.UriSource = uri;
                    }
                    localImage.EndInit();
                    localImage.Freeze();
                    CachedImage.Add(imageUrl, localImage);
                    return localImage;
                }

                using var client = new HttpClient();
                var response = await client.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();
                var imageBytes = await response.Content.ReadAsByteArrayAsync();

                var image = new BitmapImage();
                using (var mem = new MemoryStream(imageBytes))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze(); // 使图片在跨线程时不会出问题
                CachedImage.Add(imageUrl, image);
                return image;
            }
            catch (Exception ex)
            {
                LogHelper.Error("DownloadImageAsync", ex.Message + ex.StackTrace);
                return null;
            }
        }
    }
}