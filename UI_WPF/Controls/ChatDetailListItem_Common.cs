using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Windows;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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

        private static ContextMenu DetailContextMenu { get; set; } = BuildDetailContextMenu();

        private static ContextMenu GroupContextMenu { get; set; } = BuildGroupContextMenu();

        private static ContextMenu AvatarContextMenu { get; set; } = BuildAvatarContextMenu();

        /// <summary>
        /// 信号量限制下载并发
        /// </summary>
        private static SemaphoreSlim Semaphore { get; set; } = new SemaphoreSlim(1, 1);

        public static ContextMenu BuildAvatarContextMenu()
        {
            if (AvatarContextMenu != null)
            {
                return AvatarContextMenu;
            }
            AvatarContextMenu = new ContextMenu();
            AvatarContextMenu.Items.Add(new MenuItem { Header = "复制昵称" });
            AvatarContextMenu.Items.Add(new MenuItem { Header = "复制QQ" });
            AvatarContextMenu.Items.Add(new Separator());
            AvatarContextMenu.Items.Add(new MenuItem { Header = "@" });
            AvatarContextMenu.Opened += (sender, _) =>
            {
                var targetElement = AvatarContextMenu.PlacementTarget as FrameworkElement;
                // 当为发送者时禁用at按钮
                (AvatarContextMenu.Items[3] as MenuItem).IsEnabled = targetElement.DataContext is not ChatDetailListItem_Right right;
            };

            (AvatarContextMenu.Items[0] as MenuItem).Click += (sender, e) =>
            {
                FrameworkElement target = GetContextMenuTarget(sender) as FrameworkElement;
                if (target.DataContext is ChatDetailListItem_Right right)
                {
                    right.ContextMenu_CopyNick(sender, e);
                }
                else if (target.DataContext is ChatDetailListItem_Left left)
                {
                    left.ContextMenu_CopyNick(sender, e);
                }
            };
            (AvatarContextMenu.Items[1] as MenuItem).Click += (sender, e) =>
            {
                FrameworkElement target = GetContextMenuTarget(sender) as FrameworkElement;
                if (target.DataContext is ChatDetailListItem_Right right)
                {
                    right.ContextMenu_CopyId(sender, e);
                }
                else if (target.DataContext is ChatDetailListItem_Left left)
                {
                    left.ContextMenu_CopyId(sender, e);
                }
            };
            (AvatarContextMenu.Items[3] as MenuItem).Click += (sender, e) =>
            {
                FrameworkElement target = GetContextMenuTarget(sender) as FrameworkElement;
                if (target.DataContext is ChatDetailListItem_Right right)
                {
                    right.ContextMenu_At(sender, e);
                }
                else if (target.DataContext is ChatDetailListItem_Left left)
                {
                    left.ContextMenu_At(sender, e);
                }
            };

            return AvatarContextMenu;
        }

        public static ContextMenu BuildDetailContextMenu()
        {
            if (DetailContextMenu != null)
            {
                return DetailContextMenu;
            }
            DetailContextMenu = new ContextMenu();
            DetailContextMenu.Items.Add(new MenuItem { Header = "复制", Icon = new FontIcon { Glyph = "\uE16F" } });
            DetailContextMenu.Items.Add(new MenuItem { Header = "+1", Icon = new FontIcon { Glyph = "\uE8ED" } });
            DetailContextMenu.Items.Add(new MenuItem { Header = "@", Icon = new FontIcon { Glyph = "\uE9B2" } });
            DetailContextMenu.Items.Add(new Separator());
            DetailContextMenu.Items.Add(new MenuItem { Header = "撤回", Icon = new FontIcon { Glyph = "\uE107" } });
            DetailContextMenu.Opened += (sender, _) =>
            {
                var targetElement = DetailContextMenu.PlacementTarget as FrameworkElement;
                // 当为发送者时禁用at按钮
                (DetailContextMenu.Items[2] as MenuItem).IsEnabled = targetElement.DataContext is not ChatDetailListItem_Right right;
            };
            (DetailContextMenu.Items[0] as MenuItem).Click += (sender, e) =>
            {
                FrameworkElement target = GetContextMenuTarget(sender) as FrameworkElement;
                if (target.DataContext is ChatDetailListItem_Right right)
                {
                    right.ContextMenu_CopyMessage(sender, e);
                }
                else if (target.DataContext is ChatDetailListItem_Left left)
                {
                    left.ContextMenu_CopyMessage(sender, e);
                }
            };
            (DetailContextMenu.Items[1] as MenuItem).Click += (sender, e) =>
            {
                FrameworkElement target = GetContextMenuTarget(sender) as FrameworkElement;
                if (target.DataContext is ChatDetailListItem_Right right)
                {
                    right.ContextMenu_Repeat(sender, e);
                }
                else if (target.DataContext is ChatDetailListItem_Left left)
                {
                    left.ContextMenu_Repeat(sender, e);
                }
            };
            (DetailContextMenu.Items[2] as MenuItem).Click += (sender, e) =>
            {
                FrameworkElement target = GetContextMenuTarget(sender) as FrameworkElement;                
                if (target.DataContext is ChatDetailListItem_Right right)
                {
                    right.ContextMenu_At(sender, e);
                }
                else if (target.DataContext is ChatDetailListItem_Left left)
                {
                    left.ContextMenu_At(sender, e);
                }
            };
            (DetailContextMenu.Items[4] as MenuItem).Click += (sender, e) =>
            {
                FrameworkElement target = GetContextMenuTarget(sender) as FrameworkElement;
                if (target.DataContext is ChatDetailListItem_Right right)
                {
                    right.ContextMenu_Recall(sender, e);
                }
                else if (target.DataContext is ChatDetailListItem_Left left)
                {
                    left.ContextMenu_Recall(sender, e);
                }
            };

            return DetailContextMenu;
        }

        public static ContextMenu BuildGroupContextMenu()
        {
            if (GroupContextMenu != null)
            {
                return GroupContextMenu;
            }
            GroupContextMenu = new ContextMenu();
            GroupContextMenu.Items.Add(new MenuItem { Header = "复制名称" });
            GroupContextMenu.Items.Add(new MenuItem { Header = "复制ID" });

            (GroupContextMenu.Items[0] as MenuItem).Click += (sender, e) =>
            {
                object target = GetContextMenuTarget(sender);
                if (target is ChatListItem item)
                {
                    item.ContextMenu_CopyNick(sender, e);
                }
            };
            (GroupContextMenu.Items[1] as MenuItem).Click += (sender, e) =>
            {
                object target = GetContextMenuTarget(sender);
                if (target is ChatListItem item)
                {
                    item.ContextMenu_CopyId(sender, e);
                }
            };

            return GroupContextMenu;
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
            FontIcon fontIcon = new()
            {
                Width = 16,
                Height = 16,
                FontSize = 16,
                Glyph = "\uF384",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Cursor = Cursors.Hand,
                Visibility = Visibility.Collapsed
            };
            fontIcon.SetResourceReference(FontIcon.FontFamilyProperty, "SymbolThemeFontFamily");
            grid.Children.Add(fontIcon);
            // 解析图片路径
            string url = Extend.GetImageUrlOrPathFromCQCode(cqCode);
            LoadImage(url);
            fontIcon.MouseDown += (_, _) =>
            {
                // 加载失败时可点击重试
                progressRing.Visibility = Visibility.Visible;
                progressRing.Visibility = Visibility.Collapsed;
                LoadImage(url);
            };
            
            void LoadImage(string url)
            {
                Task.Run(async () =>
                {
                    var imagePath = await DownloadImageAsync(url);
                    await viewBox.Dispatcher.BeginInvoke(() =>
                    {
                        if (imagePath != null && Uri.TryCreate(imagePath, UriKind.RelativeOrAbsolute, out var uri))
                        {
                            // 显示图片元素
                            Image image = new()
                            {
                                Stretch = Stretch.Uniform,
                            };
                            // 图片双击事件
                            viewBox.MouseLeftButtonDown += (_, e) =>
                            {
                                if (e.ClickCount == 2)
                                {
                                    PictureViewer pictureViewer = new()
                                    {
                                        Image = uri,
                                        Owner = MainWindow.Instance
                                    };
                                    pictureViewer.Show();
                                }
                            };
                            AnimationBehavior.SetSourceUri(image, uri);
                            AnimationBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);
                            var img = new BitmapImage(uri);
                            // 拉伸容器高度
                            viewBox.Height = Math.Min(img.Height, viewBox.MaxHeight);
                            // 计算变化比率用于圆角尺寸统一
                            double rate = img.Height / viewBox.Height;
                            RectangleGeometry clipGeometry = new()
                            {
                                RadiusX = 10 * rate,
                                RadiusY = 10 * rate,
                                Rect = new Rect(0, 0, img.Width, img.Height)
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
                            progressRing.Visibility = Visibility.Collapsed;
                            fontIcon.Visibility = Visibility.Visible;
                        }
                    });
                });
            }
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

        public static async Task<string?> DownloadImageAsync(string imageUrl)
        {
            await Semaphore.WaitAsync();
            try
            {
                string cacheImagePath = Path.Combine("data", "image", "cached");
                Directory.CreateDirectory(cacheImagePath);
                if (!imageUrl.StartsWith("http"))
                {
                    return new FileInfo(imageUrl).FullName;
                }
                string name = Helper.GetPicNameFromUrl(imageUrl);
                if (string.IsNullOrEmpty(name))
                {
                    //LogHelper.Error("DownloadImageAsync", "无法从URL中解析出图片ID");
                    //return null;
                    name = imageUrl.MD5(); // 无法解析时尝试使用哈希作为文件名
                }
                // 检测文件是否已经存在
                string? path = Directory.GetFiles(cacheImagePath).FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == name);
                if (!string.IsNullOrEmpty(path))
                {
                    return new DirectoryInfo(path).FullName;
                }

                using var client = new HttpClient();
                var response = await client.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();
                // 文件类型已知
                if (string.IsNullOrEmpty(response.Content.Headers.ContentType?.MediaType))
                {
                    return null;
                }
                path = Path.Combine(cacheImagePath, name + ".jpg");
                path = Path.ChangeExtension(path, response.Content.Headers.ContentType.MediaType.Split('/').Last());
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(path, imageBytes);

                return new DirectoryInfo(path).FullName;
            }
            catch (Exception ex)
            {
                LogHelper.Error("DownloadImageAsync", ex.Message + ex.StackTrace);
                return null;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public static void AddTextToRichTextBox(RichTextBox textBox, string item)
        {
            var paragraph = textBox.Document.Blocks.FirstBlock as Paragraph;
            Regex urlRegex = new("(https?://[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}(?::\\d+)?(?:/[^\\s]*)?)");
            var urlCaptures = urlRegex.Matches(item).Cast<Match>().Select(m => m.Value).ToList();
            var urlSplit = urlRegex.Split(item);
            foreach (var capture in urlSplit)
            {
                if (urlCaptures.Contains(capture))
                {
                    var hyperlink = new Hyperlink(new Run(capture))
                    {
                        NavigateUri = new Uri(capture),
                    };
                    hyperlink.RequestNavigate += (sender, args) =>
                    {
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = capture,
                            UseShellExecute = true,
                        };
                        Process.Start(startInfo);
                    };

                    paragraph.Inlines.Add(hyperlink);
                }
                else
                {
                    paragraph.Inlines.Add(new Run(capture));
                }
            }

        }

        private static object GetContextMenuTarget(object sender)
        {
            return sender is MenuItem menuItem && menuItem.Parent is ContextMenu contextMenu ? contextMenu.PlacementTarget : (object?)null;
        }
    }
}