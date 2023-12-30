using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.WebSocket;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI
{
    public class DialogHelper
    {
        private static SimpleMessageBox CurrentDialog { get; set; }

        private static Queue<DialogQueueObject> ErrorDialogQueue { get; set; } = new();

        public static ContextMenu BuildNotifyIconContextMenu(List<CQPluginProxy> plugins, Action exitAction, Action reloadAction, Action pluginManageAction, Action logAction, Action<CQPluginProxy, string> menuAction, Action updateAction)
        {
            var menu = new ContextMenu();
            menu.Items.Add(new MenuItem { Header = $"{AppConfig.CurrentNickName}({AppConfig.CurrentQQ})" });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem { Header = $"框架版本: {ServerManager.Server.GetCoreVersion()}" });
            menu.Items.Add(new MenuItem { Header = $"UI版本: {MainWindow.Instance.GetType().Assembly.GetName().Version}" });
            MenuItem updateItem = new() { Header = "检查更新" };
            updateItem.Click += (a, b) => updateAction?.Invoke();
            menu.Items.Add(updateItem);
            menu.Items.Add(new Separator());
            MenuItem menuParentItem = new() { Header = "应用" };
            foreach (var item in plugins.OrderBy(x => x.PluginName))
            {
                MenuItem menuItem = new() { Header = $"{item.PluginName}" };
                foreach (var subMenu in item.AppInfo.menu)
                {
                    MenuItem subMenuItem = new() { Header = subMenu.name };
                    subMenuItem.Click += (a, b) => menuAction?.Invoke(item, subMenu.function);
                    menuItem.Items.Add(subMenuItem);
                }
                menuParentItem.Items.Add(menuItem);
            }
            menuParentItem.Items.Add(new Separator());
            MenuItem pluginManageItem = new() { Header = "插件管理" };
            pluginManageItem.Click += (_, _) => pluginManageAction?.Invoke();
            menuParentItem.Items.Add(pluginManageItem);
            menu.Items.Add(menuParentItem);
            MenuItem logItem = new() { Header = "日志" };
            logItem.Click += (a, b) => logAction?.Invoke();
            menu.Items.Add(logItem);
            menu.Items.Add(new Separator());
            MenuItem reloadItem = new() { Header = "重载插件" };
            reloadItem.Click += (a, b) => reloadAction?.Invoke();
            menu.Items.Add(reloadItem);
            MenuItem exitItem = new() { Header = "退出" };
            exitItem.Click += (a, b) => exitAction?.Invoke();
            menu.Items.Add(exitItem);
            return menu;
        }

        public static async Task<bool> ShowConfirmDialog(string title, string message)
        {
            ContentDialog dialog = new()
            {
                Title = title,
                Content = message,
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = "确认",
                SecondaryButtonText = "取消",
            };
            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }

        public static void ShowErrorDialog(string guid, int authCode, string title, string content, bool canIgnore)
        {
            var plugin = PluginManagerProxy.GetProxyByAuthCode(authCode);
            ShowErrorDialog(plugin, guid, title, content, canIgnore);
        }

        public static void ShowErrorDialog(string message, string detail, bool canIgnore = true)
        {
            ShowErrorDialog(null, "", message, detail, canIgnore);
        }

        public static void ShowErrorDialog(CQPluginProxy proxy, string guid, string message, string detail, bool canIgnore = true)
        {
            MainWindow.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (UIConfig.PopWindowWhenError)
                {
                    MainWindow.Instance.Show();
                }
                SimpleMessageBox dialog = new()
                {
                    ErrorDetail = detail,
                    ErrorMessage = message,
                    IsPrimaryButtonEnabled = canIgnore,
                    DefaultButton = canIgnore ? ContentDialogButton.Primary : ContentDialogButton.Secondary,
                };
                if (canIgnore)
                {
                    dialog.ErrorHint += "，但是这个错误可以被忽略";
                }
                else
                {
                    dialog.ErrorHint += "，插件需要重启";
                }
                if (proxy != null)
                {
                    dialog.ErrorMessage = $"插件 {proxy.PluginName}: {message}";
                }
                var queue = new DialogQueueObject { Dialog = dialog, Plugin = proxy, GUID = guid };
                ErrorDialogQueue.Enqueue(queue);
                if (CurrentDialog != null)
                {
                    CurrentDialog.Title = $"异常捕获 ({ErrorDialogQueue.Count + 1}条)";
                }
                HandleDialogQueue();
            });
        }

        public static void ShowSimpleDialog(string title, string message)
        {
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                MainWindow.Instance.Show();
                MainWindow.Instance.Focus();
                ContentDialog dialog = new()
                {
                    Title = title,
                    Content = message,
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "确认"
                };
                ErrorDialogQueue.Enqueue(new DialogQueueObject
                {
                    ContentDialog = dialog,
                });
            });
            HandleDialogQueue();
        }

        private static async void HandleDialogQueue()
        {
            if (ErrorDialogQueue.Count == 0 || CurrentDialog != null)
            {
                return;
            }
            var dialog = ErrorDialogQueue.Dequeue();
            CurrentDialog = dialog.Dialog;
            if (ErrorDialogQueue.Count > 0)
            {
                CurrentDialog.Title = $"异常捕获 ({ErrorDialogQueue.Count + 1}条)";
            }
            await MainWindow.Instance.Dispatcher.Invoke(async () =>
            {
                if (dialog.Dialog != null)
                {
                    dialog.DialogResult = await dialog.Dialog.ShowAsync();
                }
                else if (dialog.ContentDialog != null)
                {
                    dialog.DialogResult = await dialog.ContentDialog.ShowAsync();
                }
            });
            if (ServerManager.Server.WaitingMessage.TryGetValue(dialog.GUID, out InvokeResult value)
                && value != null)
            {
                value.Result = true;
                value.Success = true;
                RequestWaiter.TriggerByKey(dialog.GUID);
            }
            if (dialog.DialogResult == ContentDialogResult.Secondary && dialog.Plugin != null)
            {
                Task.Run(() => PluginManagerProxy.Instance.ReloadPlugin(dialog.Plugin));
            }
            CurrentDialog = null;
            HandleDialogQueue();
        }

        private class DialogQueueObject
        {
            public SimpleMessageBox Dialog { get; set; }

            public ContentDialog ContentDialog { get; set; }

            public ContentDialogResult DialogResult { get; set; } = ContentDialogResult.None;

            public string GUID { get; set; } = "";

            public CQPluginProxy Plugin { get; set; }
        }
    }
}