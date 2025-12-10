using Another_Mirai_Native.DB;
using Another_Mirai_Native.UI.Models;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Controls.Chat
{
    /// <summary>
    /// AtTargetSelector.xaml 的交互逻辑
    /// </summary>
    public partial class AtTargetSelector : UserControl, INotifyPropertyChanged
    {
        public AtTargetSelector(ChatType avatarType, long id)
        {
            InitializeComponent();
            DataContext = this;
            AvatarType = avatarType;
            Id = id;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler ItemSelected;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<ChatListItemViewModel> FilterList { get; set; } = [];

        public ObservableCollection<ChatListItemViewModel> Data { get; set; } = [];

        public string SelectedCQCode { get; set; }

        public bool Loading { get; set; }

        private Timer DebounceTimer { get; set; }

        public ChatType AvatarType { get; }

        public long Id { get; }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loading = true;
            DebounceTimer = new Timer();
            DebounceTimer.Elapsed += DebounceTimer_Elapsed;
            DebounceTimer.Interval = 500;

            await PrepareData();
            FilterList = Data.OrderBy(x => x.GroupName).ToObservableCollection();
            Loading = false;

            OnPropertyChanged(nameof(FilterList));
            SearchBox.Focus();
        }

        private async Task PrepareData()
        {
            if (ChatHistoryHelper.GroupMemberCache.ContainsKey(Id) is false)
            {
                await ChatHistoryHelper.LoadGroupMemberCaches(Id);
            }
            Data = [];
            var members = ChatHistoryHelper.GroupMemberCache[Id];
            foreach (var member in members.Values)
            {
                Data.Add(new ChatListItemViewModel
                {
                    Id = member.QQ,
                    GroupName = string.IsNullOrEmpty(member.Card) ? member.Nick : member.Card,
                    AvatarType = AvatarType
                });
            }
        }

        private void DebounceTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            DebounceTimer.Stop();
            Dispatcher.BeginInvoke(() =>
            {
                if (string.IsNullOrEmpty(SearchBox.Text))
                {
                    FilterList = Data;
                }
                else
                {
                    FilterList = Data.Where(x => x.Id.ToString().Contains(SearchBox.Text)
                        || x.GroupName.Contains(SearchBox.Text)).OrderBy(x => x.Id).ToObservableCollection();
                }
                OnPropertyChanged(nameof(FilterList));
            });
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DebounceTimer.Stop();
            DebounceTimer.Start();
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