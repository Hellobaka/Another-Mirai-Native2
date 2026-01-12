using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Models;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// ChatPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChatPage : Page, INotifyPropertyChanged
    {
        public ChatPage()
        {
            ViewModel = new ChatViewModel();
            DataContext = ViewModel;
            Instance = this;
            InitializeComponent();
            Page_Loaded(null, null);
        }

        public ChatViewModel ViewModel { get; set; }

        /// <summary>
        /// 主窗体尺寸变化事件, 各个消息块均订阅此事件
        /// </summary>

        public static event Action<SizeChangedEventArgs> WindowSizeChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public static ChatPage Instance { get; private set; }

        /// <summary>
        /// 当前选择项的组名称
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// 窗体加载完成事件
        /// </summary>
        private bool FormLoaded { get; set; }

        /// <summary>
        /// MVVM
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Page_Loaded(object? sender, RoutedEventArgs? e)
        {
            if (FormLoaded)
            {
                return;
            }

            if (AppConfig.Instance.EnableChat is false)
            {
                DisableDisplay.Visibility = Visibility.Visible;
                MainContent.Visibility = Visibility.Collapsed;
                return;
            }
            DisableDisplay.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;

            FormLoaded = true;
            SizeChanged += (_, e) => WindowSizeChanged?.Invoke(e);

            PluginManagerProxy.OnGroupBan += PluginManagerProxy_OnGroupBan;
            PluginManagerProxy.OnGroupAdded += PluginManagerProxy_OnGroupAdded;
            PluginManagerProxy.OnGroupMsg += PluginManagerProxy_OnGroupMsg;
            PluginManagerProxy.OnGroupLeft += PluginManagerProxy_OnGroupLeft;
            PluginManagerProxy.OnPrivateMsg += PluginManagerProxy_OnPrivateMsg;

            CQPImplementation.OnPrivateMessageSend += CQPImplementation_OnPrivateMessageSend;
            CQPImplementation.OnGroupMessageSend += CQPImplementation_OnGroupMessageSend;

            await ViewModel.LoadChatList();
        }

        private void PluginManagerProxy_OnGroupAdded(long group, long qq)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                await ViewModel.AddGroupChatItem(group, qq, $"{await ChatHistoryHelper.GetGroupMemberNick(group, qq)} 加入了本群", DetailItemType.Notice, DateTime.Now);
            });
        }

        private void PluginManagerProxy_OnGroupBan(long group, long qq, long operatedQQ, long time)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                await ViewModel.AddGroupChatItem(group, qq, $"{await ChatHistoryHelper.GetGroupMemberNick(group, qq)} 禁言了 {await ChatHistoryHelper.GetGroupMemberNick(group, operatedQQ)} {time}秒", DetailItemType.Notice, DateTime.Now);
            });
        }

        private void PluginManagerProxy_OnGroupLeft(long group, long qq)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                await ViewModel.AddGroupChatItem(group, AppConfig.Instance.CurrentQQ, $"{await ChatHistoryHelper.GetGroupMemberNick(group, qq)} 离开了群", DetailItemType.Notice, DateTime.Now);
            });
        }

        private void PluginManagerProxy_OnGroupMsg(int msgId, long group, long qq, string msg, DateTime time)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                await ViewModel.AddGroupChatItem(group, qq, msg, DetailItemType.Receive, time, msgId);
            });
        }

        private void PluginManagerProxy_OnPrivateMsg(int msgId, long qq, string msg, DateTime time)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                await ViewModel.AddPrivateChatItem(qq, qq, msg, DetailItemType.Receive, time, msgId);
            });
        }

        /// <summary>
        /// CQP事件_群消息发送
        /// </summary>
        private void CQPImplementation_OnGroupMessageSend(int msgId, long group, string msg, CQPluginProxy plugin)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                await ViewModel.AddGroupChatItem(group, AppConfig.Instance.CurrentQQ, msg, DetailItemType.Send, DateTime.Now, msgId, plugin: plugin);
            });
        }

        private void CQPImplementation_OnPrivateMessageSend(int msgId, long qq, string msg, CQPluginProxy plugin)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                await ViewModel.AddPrivateChatItem(qq, AppConfig.Instance.CurrentQQ, msg, DetailItemType.Send, DateTime.Now, msgId, plugin: plugin);
            });
        }
    }
}