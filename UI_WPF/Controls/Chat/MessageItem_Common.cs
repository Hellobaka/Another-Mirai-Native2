using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Controls.Chat
{
    public static class MessageItem_Common
    {
        /// <summary>
        /// 图片容器的最大高度
        /// </summary>
        public static double ImageMaxHeight { get; set; } = 450;

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
    }
}