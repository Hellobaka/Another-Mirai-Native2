using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Windows;
using Microsoft.Win32;
using ModernWpf.Controls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
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
using System.Windows.Resources;
using XamlAnimatedGif;

namespace Another_Mirai_Native.UI.Controls
{
    public static class ChatDetailListItem_Common
    {
        /// <summary>
        /// 图片容器的最大高度
        /// </summary>
        public static double ImageMaxHeight { get; set; } = 450;

        /// <summary>
        /// 消息菜单
        /// </summary>
        private static ContextMenu DetailContextMenu { get; set; } = BuildDetailContextMenu();

        /// <summary>
        /// 名称菜单
        /// </summary>
        private static ContextMenu GroupContextMenu { get; set; } = BuildGroupContextMenu();

        /// <summary>
        /// 个人头像菜单
        /// </summary>
        private static ContextMenu AvatarContextMenu { get; set; } = BuildAvatarContextMenu();

        private static ContextMenu ImageContextMenu { get; set; } = BuildImageContextMenu();

        /// <summary>
        /// 下载并发限制
        /// </summary>
        private static ConcurrentDictionary<string, Task<string?>> DownloadTasks { get; set; } = new();

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

        private static ContextMenu BuildImageContextMenu()
        {
            if (ImageContextMenu != null)
            {
                return ImageContextMenu;
            }
            ImageContextMenu = new ContextMenu();
            ImageContextMenu.Items.Add(new MenuItem { Header = "收藏" });
            ImageContextMenu.Items.Add(new MenuItem { Header = "另存为" });

            (ImageContextMenu.Items[0] as MenuItem).Click += (sender, e) =>
            {
                if (GetContextMenuTarget(sender) is Image image && !string.IsNullOrEmpty(image.Tag?.ToString()))
                {
                    string path = image.Tag?.ToString();
                    string collectImagePath = Path.Combine("data", "image", "collected");
                    Directory.CreateDirectory(collectImagePath);
                    if (File.Exists(path))
                    {
                        File.Copy(path, Path.Combine(collectImagePath, $"{DateTime.Now:yyyyMMddHHmmss}.{Path.GetExtension(path)}"));
                    }
                }
            };
            (ImageContextMenu.Items[1] as MenuItem).Click += (sender, e) =>
            {
                if (GetContextMenuTarget(sender) is Image image && !string.IsNullOrEmpty(image.Tag?.ToString()))
                {
                    string path = image.Tag?.ToString();
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
            };

            return ImageContextMenu;
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
                                Tag = imagePath,
                                ContextMenu = BuildImageContextMenu()
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

        /// <summary>
        /// 构建QQ表情元素
        /// </summary>
        /// <param name="id">表情Id</param>
        /// <returns></returns>
        public static Image? BuildFaceElement(int id, bool isAnimation)
        {
            string packUri = $"pack://application:,,,/Resources/qq-face/{id}.{(isAnimation ? "gif" : "png")}";
            if (!CheckResourceExists(packUri, out StreamResourceInfo resource))
            {
                return null;
            }
            else
            {
                Image image = new()
                {
                    Stretch = Stretch.Uniform,
                    Width = 30,
                    Height = 30,
                };
                AnimationBehavior.SetSourceStream(image, resource.Stream);
                AnimationBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);
                return image;
            }
        }

        public static TextBox BuildTextElement(string text)
        {
            var textbox = new TextBox
            {
                Text = text,
                Padding = new Thickness(10),
                TextWrapping = TextWrapping.Wrap,
                IsReadOnly = true,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                ContextMenu = null
            };
            SetElementNoSelectEffect(textbox);
            return textbox;
        }

        /// <summary>
        /// 文本框删除点击时发光的外边框
        /// </summary>
        /// <param name="element"></param>
        public static void SetElementNoSelectEffect(UIElement element)
        {
            UIElement GetBorderElement(DependencyObject element)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    var e = VisualTreeHelper.GetChild(element, i);
                    if (e is Border border && border.Name == "BorderElement")
                    {
                        return border;
                    }
                    else if (e is Grid ignoreGrid && ignoreGrid.Name == "ContentRoot")
                    {
                        continue;
                    }
                    else
                    {
                        var result = GetBorderElement(e);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
                return null;
            }

            var borderElement = GetBorderElement(element);
            if (borderElement != null)
            {
                if (borderElement is Border border)
                {
                    // 设置边框画笔为透明
                    border.BorderBrush = Brushes.Transparent;
                }
            }
        }

        /// <summary>
        /// 下载或读取缓存图片
        /// </summary>
        /// <param name="imageUrl">欲下载的图片URL</param>
        /// <returns>本地图片路径</returns>
        public static async Task<string?> DownloadImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return null;
                }
                string cacheImagePath = Path.Combine("data", "image", "cached");
                Directory.CreateDirectory(cacheImagePath);
                if (!imageUrl.StartsWith("http"))// 下载并非http请求, 则更改为本地文件
                {
                    return new FileInfo(imageUrl).FullName;
                }
                string name = Helper.GetPicNameFromUrl(imageUrl);// 解析图片唯一ID
                if (string.IsNullOrEmpty(name))
                {
                    //LogHelper.Error("DownloadImageAsync", "无法从URL中解析出图片ID");
                    //return null;
                    name = imageUrl.MD5(); // 无法解析时尝试使用哈希作为文件名
                }
                // 尝试从本地读取缓存
                string? path = await GetFromCacheAsync(cacheImagePath, name);
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }
                // 如果没有缓存则进行下载, 从同步字典中获取或新建一个下载任务
                var downloadTask = DownloadTasks.GetOrAdd(imageUrl, _ => DownloadFileFromWebAsync(cacheImagePath, name, imageUrl));

                string? data = await downloadTask;
                DownloadTasks.TryRemove(imageUrl, out var _);
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.Error("DownloadImageAsync", ex.Message + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 向富文本框添加文本, 同步解析URL为超链接
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="item"></param>
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
                        args.Handled = true;
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

        private static async Task<string?> DownloadFileFromWebAsync(string cacheImagePath, string name, string imageUrl)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();
            // 文件类型已知
            if (string.IsNullOrEmpty(response.Content.Headers.ContentType?.MediaType))
            {
                return null;
            }
            string path = Path.Combine(cacheImagePath, name + ".jpg");
            path = Path.ChangeExtension(path, response.Content.Headers.ContentType.MediaType.Split('/').Last());
            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            File.WriteAllBytes(path, imageBytes);

            return new DirectoryInfo(path).FullName;
        }

        private static Task<string?> GetFromCacheAsync(string cacheImagePath, string name)
        {
            // 检测文件是否已经存在
            string? path = Directory.GetFiles(cacheImagePath).FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == name);
            if (!string.IsNullOrEmpty(path))
            {
                return Task.FromResult<string?>(new DirectoryInfo(path).FullName);
            }

            return Task.FromResult<string?>(null);
        }

        /// <summary>
        /// 检查指定的资源是否存在。
        /// </summary>
        /// <param name="resourcePath">资源的路径</param>
        /// <returns>如果资源存在返回 true，否则返回 false。</returns>
        private static bool CheckResourceExists(string uri, out StreamResourceInfo streamInfo)
        {
            streamInfo = null;
            try
            {
                Uri resourceUri = new(uri, UriKind.Absolute);
                // 尝试获取资源流
                streamInfo = Application.GetResourceStream(resourceUri);

                // 如果能获取到资源流，说明资源存在
                return streamInfo != null;
            }
            catch (Exception)
            {
                // 如果在尝试获取资源流的过程中发生了异常，说明资源不存在
                return false;
            }
        }
    }
}