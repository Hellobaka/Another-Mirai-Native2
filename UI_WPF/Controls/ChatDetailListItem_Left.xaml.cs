using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Pages;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// ChatDetailListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ChatDetailListItem_Left : UserControl
    {
        public ChatDetailListItem_Left()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ChatAvatar.AvatarTypes AvatarType { get; set; } = ChatAvatar.AvatarTypes.Fallback;

        public bool ControlLoaded { get; set; }

        public DetailItemType DetailItemType { get; set; }

        public string DisplayName { get; set; }

        public string GUID { get; set; }

        public long Id { get; set; }

        public string Message { get; set; } = "";

        public DateTime Time { get; set; }

        private static Dictionary<string, BitmapImage> CachedImage { get; set; } = new();

        private static double ImageMaxHeight { get; set; } = 450;

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
            void SetBorderBackground(Dispatcher dispatcher, Border border, ModernWpf.Controls.ProgressRing progressRing, BitmapImage image)
            {
                dispatcher.BeginInvoke(() =>
                {
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
                Height = ImageMaxHeight,
                Width = maxWidth,
                CornerRadius = new CornerRadius(3),
            };
            border.MouseLeftButtonDown += (_, e) =>
            {
                if (e.ClickCount == 2)
                {
                    Debug.WriteLine("DbClick");
                }
            };
            DynamicResourceExtension dynamicResource = new("SystemControlPageBackgroundChromeMediumLowBrush");
            border.SetResourceReference(Border.BackgroundProperty, dynamicResource.ResourceKey);
            RenderOptions.SetBitmapScalingMode(border, BitmapScalingMode.Fant);
            var progressRing = new ModernWpf.Controls.ProgressRing
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

        public void ParseAndBuildDetail()
        {
            var ls = CQCode.Parse(Message);
            int imageCount = ls.Count(x => x.IsImageCQCode);
            int recordCount = ls.Count(x => x.IsRecordCQCode);
            StackPanel imgContainer = imageCount == 1 ? ImageDisplay : DetailContainer;
            if (imageCount == 1)
            {
                ImageBorder.Visibility = Visibility.Visible;
                DetailBorder.Visibility = Visibility.Collapsed;
            }
            if (recordCount == 1) // 不会与Image同时出现
            {
                DetailBorder.Visibility = Visibility.Collapsed;
                ImageBorder.Visibility = Visibility.Collapsed;
            }
            string msg = Message;
            foreach (var item in ls)
            {
                msg = msg.Replace(item.ToString(), "<!cqCode!>");
            }
            var p = msg.Split("<!cqCode!>");
            int cqCode_index = 0;
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] == "<!cqCode!>")
                {
                    var item = ls[cqCode_index];
                    if (item.Function == Model.Enums.CQCodeType.Image)
                    {
                        imgContainer.Children.Add(BuildImageElement(item, MaxWidth * 0.5));
                    }
                    else if (item.Function == Model.Enums.CQCodeType.Record)
                    {
                        // TODO: 创建音频元素
                    }
                    else if (item.Function == Model.Enums.CQCodeType.Rich)
                    {
                        // TODO: 填充折叠框
                    }
                    else if(item.Function == Model.Enums.CQCodeType.At)
                    {
                        DetailContainer.Children.Add(BuildTextElement($" @{ChatPage.Instance.GetGroupMemberNick(0, Id)} "));
                        // TODO: 实现昵称获取
                    }
                    else
                    {
                        DetailContainer.Children.Add(BuildTextElement(item.ToSendString()));
                    }
                    cqCode_index++;
                }
                else
                {
                    DetailContainer.Children.Add(BuildTextElement(p[i]));
                }
            }
        }

        private void ChatPage_WindowSizeChanged(SizeChangedEventArgs e)
        {
            MaxWidth = e.NewSize.Width * 0.6;
            ImageBorder.MaxWidth = MaxWidth;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ControlLoaded)
            {
                return;
            }
            ControlLoaded = true;
            ParseAndBuildDetail();
            ImageDisplay.MaxWidth = MaxWidth * 0.6;
            Avatar.Item = new ChatListItemViewModel
            {
                AvatarType = AvatarType,
                GroupName = DisplayName,
                Id = Id
            };
            ChatPage.WindowSizeChanged += ChatPage_WindowSizeChanged;
            // TODO: 每分钟刷新时间显示，添加 n分钟前 样式，评估性能
            // TODO: 实现消息撤回
        }
    }
}