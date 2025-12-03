using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.UI.ViewModel;
using Another_Mirai_Native.UI.Windows;
using Microsoft.Win32;
using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

namespace Another_Mirai_Native.UI.Controls.Chat
{
#pragma warning disable CS8602 // 解引用可能出现空引用。

    public static class MessageItem_Common
    {
        /// <summary>
        /// 图片容器的最大高度
        /// </summary>
        public static double ImageMaxHeight { get; set; } = 450;

        private static ContextMenu ImageContextMenu { get; set; } = BuildImageContextMenu();

        /// <summary>
        /// 构建QQ表情元素
        /// </summary>
        /// <param name="id">表情Id</param>
        /// <param name="isAnimation">是否为动图</param>
        /// <returns>构建结果 当ID不存在时返回null</returns>
        public static Image? BuildFaceElement(int id, bool isAnimation)
        {
            // TODO: 实现Lottie
            string packUri = $"pack://application:,,,/Resources/qq-face/{id}.{(isAnimation ? "png" : "png")}";
            if (!CheckResourceExists(packUri, out StreamResourceInfo? resource) || resource == null)
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
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
                return image;
            }
        }

        /// <summary>
        /// 构建图片元素 包含自动下载与进度显示
        /// </summary>
        /// <param name="cqCode">图片CQ码</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns>图片元素</returns>
        public static FrameworkElement BuildImageElement(CQCode cqCode, double maxWidth)
        {
            return new ChatImageDisplay
            {
                CQCode = cqCode,
                MaxImageWidth = maxWidth,
                MaxImageHeight = ImageMaxHeight
            };
        }

        /// <summary>
        /// 构建回复消息元素
        /// </summary>
        /// <param name="nick">消息昵称</param>
        /// <param name="msg">消息内容</param>
        /// <param name="jumpAction">点击跳转回调</param>
        public static Border BuildReplyElement(string nick, string msg, Action jumpAction)
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Gray;
            border.BorderThickness = new Thickness(2, 0, 0, 0);
            border.CornerRadius = new CornerRadius(0, 5, 5, 0);
            //border.ClipToBounds = true;
            Grid grid = new();
            border.Child = grid;

            border.Cursor = Cursors.Hand;
            border.Background = new SolidColorBrush(Color.FromArgb(0, 100, 100, 100));
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            nick = nick.Replace("\r", "").Replace("\n", "");
            msg = msg.Replace("\r", "").Replace("\n", "");
            TextBlock nickBlock = new()
            {
                TextWrapping = TextWrapping.NoWrap,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Text = nick,
                Margin = new Thickness(10, 10, 10, 5),
                FontSize = 10
            };
            TextBlock content = new()
            {
                TextWrapping = TextWrapping.NoWrap,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Text = msg,
                Margin = new Thickness(10, 5, 10, 10)
            };
            grid.Children.Add(nickBlock);
            grid.Children.Add(content);
            nickBlock.SetValue(Grid.RowProperty, 0);
            content.SetValue(Grid.RowProperty, 1);

            border.MouseEnter += (_, _) =>
            {
                // 半透明背景色
                border.Background = new SolidColorBrush(Color.FromArgb(100, 100, 100, 100));
            };
            border.MouseLeave += (_, _) =>
            {
                border.Background = new SolidColorBrush(Color.FromArgb(0, 100, 100, 100));
            };
            border.MouseLeftButtonDown += (_, _) =>
            {
                jumpAction?.Invoke();
            };
            return border;
        }

        /// <summary>
        /// 文本框删除点击时发光的外边框
        /// </summary>
        /// <param name="element"></param>
        public static void SetElementNoSelectEffect(UIElement element)
        {
            UIElement? GetBorderElement(DependencyObject element)
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

        internal static ContextMenu GetImageContextMenu()
        {
            return BuildImageContextMenu();
        }

        /// <summary>
        /// 构建图片右键菜单
        /// </summary>
        private static ContextMenu BuildImageContextMenu()
        {
            // TODO: 重构掉
            if (ImageContextMenu != null)
            {
                return ImageContextMenu;
            }
            ImageContextMenu = new ContextMenu();
            ImageContextMenu.Items.Add(new MenuItem { Header = "收藏", Icon = new FontIcon { Glyph = "\uEB51" } });
            ImageContextMenu.Items.Add(new MenuItem { Header = "另存为", Icon = new FontIcon { Glyph = "\uE792" } });
            ImageContextMenu.Items.Add(new Separator());
            ImageContextMenu.Items.Add(new MenuItem { Header = "+1", Icon = new FontIcon { Glyph = "\uE8ED" } });

            (ImageContextMenu.Items[0] as MenuItem).Click += (sender, e) =>
            {
                if (GetContextMenuTarget(sender) is Image image && !string.IsNullOrEmpty(image.Tag?.ToString()))
                {
                    string path = image.Tag?.ToString() ?? "";
                    string collectImagePath = Path.Combine("data", "image", "collected");
                    Directory.CreateDirectory(collectImagePath);
                    if (File.Exists(path))
                    {
                        File.Copy(path, Path.Combine(collectImagePath, $"{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(path)}"));
                    }
                }
            };
            (ImageContextMenu.Items[1] as MenuItem).Click += (sender, e) =>
            {
                if (GetContextMenuTarget(sender) is Image image && !string.IsNullOrEmpty(image.Tag?.ToString()))
                {
                    string path = image.Tag?.ToString() ?? "";
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
            (ImageContextMenu.Items[3] as MenuItem).Click += (sender, e) =>
            {
                if (GetContextMenuTarget(sender) is not FrameworkElement target || target.DataContext is not MessageItem detail)
                {
                    return;
                }
                detail.ViewModel.Message_Repeat(e);
            };

            return ImageContextMenu;
        }

        /// <summary>
        /// 检查指定的资源是否存在。
        /// </summary>
        /// <param name="resourcePath">资源的路径</param>
        /// <returns>如果资源存在返回 true，否则返回 false。</returns>
        private static bool CheckResourceExists(string uri, out StreamResourceInfo? streamInfo)
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

        /// <summary>
        /// 获取右键菜单触发元素
        /// </summary>
        private static object? GetContextMenuTarget(object sender)
        {
            return sender is MenuItem menuItem && menuItem.Parent is ContextMenu contextMenu ? contextMenu.PlacementTarget : (object?)null;
        }
    }

#pragma warning restore CS8602 // 解引用可能出现空引用。
}