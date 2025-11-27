using Another_Mirai_Native.Native;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Another_Mirai_Native.UI.Pages.Helpers
{
    /// <summary>
    /// RichTextBox 辅助类，处理富文本框的操作
    /// </summary>
    public static class RichTextBoxHelper
    {
        /// <summary>
        /// 将 RichTextBox 内容转换为 CQ 码字符串
        /// </summary>
        /// <param name="richTextBox">富文本框</param>
        /// <returns>CQ码格式的消息</returns>
        public static string ConvertToCQCode(RichTextBox richTextBox)
        {
            if (richTextBox == null)
            {
                throw new ArgumentNullException(nameof(richTextBox));
            }

            StringBuilder stringBuilder = new();
            foreach (Block block in richTextBox.Document.Blocks)
            {
                // 粘贴的图片块
                if (block is BlockUIContainer blockImgContainer && blockImgContainer.Child is Image blockImg)
                {
                    stringBuilder.Append(blockImg.Tag?.ToString());
                    continue;
                }

                if (block is not Paragraph paragraph)
                {
                    continue;
                }

                // 处理段落中的内联元素
                foreach (Inline inline in paragraph.Inlines)
                {
                    if (inline is InlineUIContainer uiContainer && uiContainer.Child is Image inlineImage)
                    {
                        stringBuilder.Append(inlineImage.Tag?.ToString());
                    }
                    else
                    {
                        stringBuilder.Append(new TextRange(inline.ContentStart, inline.ContentEnd).Text);
                    }
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 处理粘贴事件
        /// </summary>
        /// <param name="e">粘贴事件参数</param>
        /// <param name="target">目标富文本框</param>
        public static void HandlePaste(DataObjectPastingEventArgs e, RichTextBox target)
        {
            if (e == null || target == null)
            {
                return;
            }

            // 处理图片粘贴
            if (e.DataObject.GetDataPresent(DataFormats.Bitmap) 
                && e.DataObject.GetData(DataFormats.Bitmap) is BitmapSource image)
            {
                InsertImage(target, image);
                e.Handled = true;
                e.CancelCommand();
            }
            // 处理文本粘贴
            else if (e.DataObject.GetDataPresent(DataFormats.UnicodeText)
                && e.DataObject.GetData(DataFormats.UnicodeText) is string text
                && !string.IsNullOrEmpty(text))
            {
                InsertText(target, text);
                e.Handled = true;
                e.CancelCommand();
            }
        }

        /// <summary>
        /// 在光标位置插入文本
        /// </summary>
        /// <param name="richTextBox">富文本框</param>
        /// <param name="text">要插入的文本</param>
        public static void InsertText(RichTextBox richTextBox, string text)
        {
            if (richTextBox == null || string.IsNullOrEmpty(text))
            {
                return;
            }

            TextPointer startPosition = richTextBox.Document.ContentStart;
            int start = startPosition.GetOffsetToPosition(richTextBox.CaretPosition);
            richTextBox.CaretPosition.InsertTextInRun(text);
            richTextBox.CaretPosition = startPosition.GetPositionAtOffset(start + text.Length + 2);
            richTextBox.Focus();
        }

        /// <summary>
        /// 在光标位置插入图片
        /// </summary>
        /// <param name="richTextBox">富文本框</param>
        /// <param name="image">要插入的图片</param>
        public static void InsertImage(RichTextBox richTextBox, BitmapSource image)
        {
            if (richTextBox == null || image == null)
            {
                return;
            }

            // 保存图片到缓存文件夹
            string cacheImagePath = Path.Combine("data", "image", "cached");
            Directory.CreateDirectory(cacheImagePath);

            using MemoryStream memoryStream = new();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(memoryStream);
            var buffer = memoryStream.ToArray();
            string md5 = buffer.MD5();

            File.WriteAllBytes(Path.Combine(cacheImagePath, md5 + ".png"), buffer);

            // 创建图片控件
            Image img = new()
            {
                Source = image,
                Width = image.Width,
                Height = image.Height,
                Tag = $"[CQ:image,file=cached\\{md5}.png]"
            };

            // 插入到文档
            if (richTextBox.Document.Blocks.Count == 0)
            {
                richTextBox.Document.Blocks.Add(new Paragraph());
            }

            if (richTextBox.Document.Blocks.LastBlock is Paragraph lastParagraph)
            {
                lastParagraph.Inlines.Add(new InlineUIContainer(img));
            }
        }

        /// <summary>
        /// 清空 RichTextBox 内容
        /// </summary>
        /// <param name="richTextBox">富文本框</param>
        public static void Clear(RichTextBox richTextBox)
        {
            if (richTextBox != null)
            {
                richTextBox.Document.Blocks.Clear();
            }
        }
    }
}
