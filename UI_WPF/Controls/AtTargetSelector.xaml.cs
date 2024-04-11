using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// AtTargetSelector.xaml 的交互逻辑
    /// </summary>
    public partial class AtTargetSelector : UserControl, INotifyPropertyChanged
    {
        public AtTargetSelector(List<ChatListItemViewModel> list)
        {
            InitializeComponent();
            DataContext = this;
            OriginList = list;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler ItemSelected;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<ChatListItemViewModel> GroupMemeberList { get; set; } = new();

        public string SelectedCQCode { get; set; }

        private List<ChatListItemViewModel> OriginList { get; set; } = new();

        private Timer DeboundTimer { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DeboundTimer = new Timer();
            DeboundTimer.Elapsed += DeboundTimer_Elapsed;
            DeboundTimer.Interval = 500;

            GroupMemeberList = OriginList.OrderBy(x => x.GroupName).ToList();
            OnPropertyChanged(nameof(GroupMemeberList));
            SearchBox.Focus();
        }

        private void DeboundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DeboundTimer.Stop();
            Dispatcher.BeginInvoke(() =>
            {
                if (string.IsNullOrEmpty(SearchBox.Text))
                {
                    GroupMemeberList = OriginList;
                }
                else
                {
                    GroupMemeberList = OriginList.Where(x => x.Id.ToString().Contains(SearchBox.Text)
                        || x.GroupName.Contains(SearchBox.Text)).OrderBy(x => x.Id).ToList();
                }
                OnPropertyChanged(nameof(GroupMemeberList));
            });
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DeboundTimer.Stop();
            DeboundTimer.Start();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemContainer.SelectedItem == null
                || ItemContainer.SelectedItem is ChatListItemViewModel item is false)
            {
                return;
            }
            SelectedCQCode = $"[CQ:at,qq={item.Id}]";
            ItemSelected?.Invoke(this, e);
        }
    }
}
