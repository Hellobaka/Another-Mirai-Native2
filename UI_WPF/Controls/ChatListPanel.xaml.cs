using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// ChatListPanel.xaml 的交互逻辑
    /// 包含聊天列表和空状态提示的组合控件
    /// </summary>
    public partial class ChatListPanel : UserControl
    {
        public ChatListPanel()
        {
            InitializeComponent();
        }

        #region 依赖属性

        /// <summary>
        /// 列表数据源
        /// </summary>
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ChatListPanel), 
                new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatListPanel panel)
            {
                panel.UpdateEmptyHintVisibility();
                
                // 订阅集合变化事件
                if (e.OldValue is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= panel.OnCollectionChanged;
                }
                if (e.NewValue is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged += panel.OnCollectionChanged;
                }
            }
        }

        private void OnCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateEmptyHintVisibility();
        }

        /// <summary>
        /// 当前选中的项
        /// </summary>
        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(ChatListPanel),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 空状态提示的可见性
        /// </summary>
        public Visibility EmptyHintVisibility
        {
            get => (Visibility)GetValue(EmptyHintVisibilityProperty);
            set => SetValue(EmptyHintVisibilityProperty, value);
        }

        public static readonly DependencyProperty EmptyHintVisibilityProperty =
            DependencyProperty.Register(nameof(EmptyHintVisibility), typeof(Visibility), typeof(ChatListPanel),
                new PropertyMetadata(Visibility.Visible));

        #endregion

        #region 事件

        /// <summary>
        /// 选中项变化事件
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

        #endregion

        #region 事件处理

        private void ChatListDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 更新空状态提示的可见性
        /// </summary>
        public void UpdateEmptyHintVisibility()
        {
            if (ItemsSource == null)
            {
                EmptyHintVisibility = Visibility.Visible;
                return;
            }

            var count = 0;
            foreach (var _ in ItemsSource)
            {
                count++;
                break; // 只需要知道是否有元素即可
            }

            EmptyHintVisibility = count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 获取内部的ListView控件
        /// </summary>
        /// <returns>ListView控件</returns>
        public ListView GetListView()
        {
            return ChatListDisplay;
        }

        #endregion
    }
}
