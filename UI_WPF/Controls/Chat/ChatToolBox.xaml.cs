using Another_Mirai_Native.UI.ViewModel;
using ModernWpf.Controls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Another_Mirai_Native.UI.Controls.Chat
{
    /// <summary>
    /// ChatToolBox.xaml 的交互逻辑
    /// </summary>
    public partial class ChatToolBox : UserControl
    {
        public ChatToolBox()
        {
            InitializeComponent();
        }

        public ChatViewModel ViewModel => (ChatViewModel)DataContext;

        private AtTargetSelector AtTargetSelector { get; set; }

        private Flyout AtFlyout { get; set; }

        private bool ControlLoaded { get; set; }

        private void SendText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                e.Handled = true;
                _ = ViewModel.ToolSendText(e);
            }
            else if (e.Key == Key.D2 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                // 触发@
                e.Handled = true;
                AtButton_Click(sender, e);
            }
        }

        private void AtButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedChat == null)
            {
                return;
            }
            // 显示Flyout
            AtTargetSelector = new(ViewModel.SelectedChat.AvatarType, ViewModel.SelectedChat.Id);
            AtTargetSelector.ItemSelected -= AtTargetSelector_ItemSelected;
            AtTargetSelector.ItemSelected += AtTargetSelector_ItemSelected;
            AtFlyout = new Flyout
            {
                Content = AtTargetSelector,
                Placement = ModernWpf.Controls.Primitives.FlyoutPlacementMode.TopEdgeAlignedLeft
            };
            AtFlyout.ShowAt(AtBtn);
        }

        private void AtTargetSelector_ItemSelected(object? sender, EventArgs e)
        {
            ViewModel.AddTextToSendBox(AtTargetSelector.SelectedCQCode);
            AtFlyout.Hide();
        }

        private void ViewModel_OnTextAddRequested(string text)
        {
            Dispatcher.BeginInvoke(() =>
            {
                TextPointer startPosition = SendText.Document.ContentStart;
                int start = startPosition.GetOffsetToPosition(SendText.CaretPosition);
                SendText.CaretPosition.InsertTextInRun(text);
                SendText.CaretPosition = startPosition.GetPositionAtOffset(start + text.Length + 2);
                SendText.Focus();
            });
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ControlLoaded)
            {
                return;
            }
            ControlLoaded = true;
            ViewModel.SendText = SendText.Document;
            ViewModel.OnTextAddRequested += ViewModel_OnTextAddRequested;
            DataObject.AddPastingHandler(SendText, RichTextboxPasteOverrideAction);
        }

        /// <summary>
        /// 发送文本框响应粘贴事件
        /// </summary>
        private void RichTextboxPasteOverrideAction(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Bitmap) && e.DataObject.GetData(DataFormats.Bitmap) is BitmapSource image)
            {
                // 粘贴内容为图片 将图片保存进缓存文件夹
                string cacheImagePath = Path.Combine("data", "image", "cached");
                using MemoryStream memoryStream = new();
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memoryStream);
                var buffer = memoryStream.ToArray();
                string md5 = buffer.MD5();

                File.WriteAllBytes(Path.Combine(cacheImagePath, md5 + ".png"), buffer);
                int maxWidth = 200;
                Image img = new()
                {
                    Source = image,
                    Tag = $"[CQ:image,file=cached\\{md5}.png]"
                };
                if (image.PixelWidth > maxWidth)
                {
                    double ratio = (double)image.PixelHeight / image.PixelWidth;
                    img.Width = maxWidth;
                    img.Height = maxWidth * ratio;
                }
                if (SendText.Document.Blocks.Count == 0)
                {
                    SendText.Document.Blocks.Add(new Paragraph());
                }
                if (SendText.Document.Blocks.LastBlock is Paragraph lastParagraph)
                {
                    lastParagraph.Inlines.Add(new InlineUIContainer(img));
                }
                e.Handled = true;
                e.CancelCommand();
            }
            else if (e.DataObject.GetDataPresent(DataFormats.UnicodeText)
                && e.DataObject.GetData(DataFormats.UnicodeText) is string text
                && string.IsNullOrEmpty(text) is false)
            {
                // 粘贴内容为文本
                ViewModel.AddTextToSendBox(text);
                e.Handled = true;
                e.CancelCommand();
            }
        }

        private void FaceImageSelector_ImageSelected(object sender, EventArgs e)
        {
            ViewModel.AddTextToSendBox(FaceImageSelector.SelectedImageCQCode);
            FaceImageFlyout.Hide();
        }
    }
}
