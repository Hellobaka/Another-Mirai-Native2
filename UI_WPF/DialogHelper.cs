using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.Pages;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Another_Mirai_Native.UI
{
    /// <summary>
    /// 帮助类，用于显示各种对话框
    /// </summary>
    public class DialogHelper
    {
        /// <summary>
        /// 当前显示的对话框
        /// </summary>
        private static SimpleMessageBox? CurrentDialog { get; set; }

        /// <summary>
        /// 错误对话框队列
        /// </summary>
        private static Queue<DialogQueueObject> ErrorDialogQueue { get; set; } = new();

        /// <summary>
        /// 退出操作
        /// </summary>
        private static Action ExitAction { get; set; } = () => Environment.Exit(0);

        /// <summary>
        /// 构建通知图标的上下文菜单
        /// </summary>
        /// <param name="plugins">插件列表</param>
        /// <param name="exitAction">退出操作</param>
        /// <param name="webUIAction">WebUI操作</param>
        /// <param name="reloadAction">重载操作</param>
        /// <param name="pluginManageAction">插件管理操作</param>
        /// <param name="logAction">日志操作</param>
        /// <param name="menuAction">菜单操作</param>
        /// <param name="updateAction">更新操作</param>
        /// <returns>上下文菜单</returns>
        public static ContextMenu BuildNotifyIconContextMenu(List<CQPluginProxy> plugins, Action exitAction, Action webUIAction, Action reloadAction, Action pluginManageAction, Action logAction, Action<CQPluginProxy, string> menuAction, Action updateAction)
        {
            var menu = new ContextMenu();
            menu.Items.Add(new MenuItem { Header = $"{AppConfig.Instance.CurrentNickName}({AppConfig.Instance.CurrentQQ})" });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem { Header = $"框架版本: {ServerManager.Server.GetCoreVersion()}" });
#if NET5_0_OR_GREATER
            StackPanel webUIStatus = new StackPanel();
            webUIStatus.Orientation = Orientation.Horizontal;

            webUIStatus.Children.Add(new Ellipse
            {
                Width = 10,
                Height = 10,
                Margin = new System.Windows.Thickness(0, 0, 10, 0),
                Fill = WebUIPage.Instance.StartStatus ? Brushes.Green : Brushes.Red,
            });
            TextBlock webuiUrl = new TextBlock() { Text = string.IsNullOrEmpty(BlazorUI.Entry_Blazor.WebUIURL) ? "WebUI 未启动" : BlazorUI.Entry_Blazor.WebUIURL };
            DynamicResourceExtension dynamicResource = new("TextControlForeground");
            webuiUrl.SetResourceReference(TextBlock.ForegroundProperty, dynamicResource.ResourceKey);

            webUIStatus.Children.Add(webuiUrl);

            MouseButtonEventHandler mouseDownAction = (_, e) =>
            {
                e.Handled = true;
                if (WebUIPage.Instance.StartStatus)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = BlazorUI.Entry_Blazor.WebUIURL,
                        UseShellExecute = true
                    });
                }
                else
                {
                    webUIAction?.Invoke();
                }
            };
            webuiUrl.MouseDown -= mouseDownAction;
            webUIStatus.MouseDown -= mouseDownAction;
            webuiUrl.MouseDown += mouseDownAction;
            webUIStatus.MouseDown += mouseDownAction;
            menu.Items.Add(webUIStatus);
#endif
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

        /// <summary>
        /// 显示确认对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="message">对话框内容</param>
        /// <returns>用户是否确认</returns>
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

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="guid">错误标识</param>
        /// <param name="authCode">授权码</param>
        /// <param name="title">对话框标题</param>
        /// <param name="content">对话框内容</param>
        /// <param name="canIgnore">是否可以忽略</param>
        public static void ShowErrorDialog(string guid, int authCode, string title, string content, bool canIgnore)
        {
            var plugin = PluginManagerProxy.GetProxyByAuthCode(authCode);
            ShowErrorDialog(plugin, guid, title, content, canIgnore);
        }

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="detail">错误详情</param>
        /// <param name="canIgnore">是否可以忽略</param>
        public static void ShowErrorDialog(string message, string detail, bool canIgnore = true)
        {
            ShowErrorDialog(null, "", message, detail, canIgnore);
        }

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="proxy">插件代理</param>
        /// <param name="guid">错误标识</param>
        /// <param name="message">错误信息</param>
        /// <param name="detail">错误详情</param>
        /// <param name="canIgnore">是否可以忽略</param>
        public static void ShowErrorDialog(CQPluginProxy? proxy, string guid, string message, string detail, bool canIgnore = true)
        {
            MainWindow.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (UIConfig.Instance.PopWindowWhenError)
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
                else if (proxy != null)
                {
                    dialog.ErrorHint += "，插件需要重启";
                }
                else
                {
                    dialog.ErrorHint += "，请重启框架";
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

        /// <summary>
        /// 显示简单对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="message">对话框内容</param>
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

        /// <summary>
        /// 处理对话框队列
        /// </summary>
        public static async void HandleDialogQueue()
        {
            // 如果队列为空或当前已有对话框在显示，则直接返回
            if (ErrorDialogQueue.Count == 0 || CurrentDialog != null)
            {
                return;
            }

            // 从队列中取出一个对话框对象
            var dialog = ErrorDialogQueue.Dequeue();
            CurrentDialog = dialog.Dialog;

            // 如果队列中还有其他对话框，更新当前对话框的标题
            if (ErrorDialogQueue.Count > 0)
            {
                CurrentDialog.Title = $"异常捕获 ({ErrorDialogQueue.Count + 1}条)";
            }

            bool successShown = false;

            // 在主线程上显示对话框
            await MainWindow.Instance.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (dialog.Dialog != null)
                    {
                        // 设置关闭按钮的点击事件
                        dialog.Dialog.CloseButtonClick += (_, _) => ExitAction();
                        dialog.Dialog.IsSecondaryButtonEnabled = dialog.Plugin != null;
                        dialog.DialogResult = await dialog.Dialog.ShowAsync();
                    }
                    else if (dialog.ContentDialog != null)
                    {
                        // 设置关闭按钮的点击事件
                        dialog.ContentDialog.CloseButtonClick += (_, _) => ExitAction();
                        dialog.ContentDialog.IsSecondaryButtonEnabled = dialog.Plugin != null;
                        dialog.DialogResult = await dialog.ContentDialog.ShowAsync();
                    }
                    successShown = true;
                }
                catch (InvalidOperationException)
                {
                    // 如果显示对话框时发生异常，将对话框重新加入队列
                    ErrorDialogQueue.Enqueue(dialog);
                    CurrentDialog = null;
                    successShown = false;
                }
            });

            // 如果对话框未成功显示，直接返回
            if (!successShown)
            {
                return;
            }

            // 处理对话框结果
            if (ServerManager.Server.WaitingMessage.TryGetValue(dialog.GUID, out InvokeResult? value) && value != null)
            {
                value.Result = true;
                value.Success = true;
                RequestWaiter.TriggerByKey(dialog.GUID);
            }

            // 如果用户点击了重载按钮且插件不为空，重新加载插件
            if (dialog.DialogResult == ContentDialogResult.Secondary && dialog.Plugin != null)
            {
                _ = Task.Run(() => PluginManagerProxy.Instance.ReloadPlugin(dialog.Plugin));
            }

            // 清空当前对话框并处理下一个队列中的对话框
            CurrentDialog = null;
            HandleDialogQueue();
        }

        /// <summary>
        /// 对话框队列对象
        /// </summary>
        private class DialogQueueObject
        {
            /// <summary>
            /// 简单消息框
            /// </summary>
            public SimpleMessageBox Dialog { get; set; }

            /// <summary>
            /// 内容对话框
            /// </summary>
            public ContentDialog ContentDialog { get; set; }

            /// <summary>
            /// 对话框结果
            /// </summary>
            public ContentDialogResult DialogResult { get; set; } = ContentDialogResult.None;

            /// <summary>
            /// 等待标识
            /// </summary>
            public string GUID { get; set; } = "";

            /// <summary>
            /// 插件代理
            /// </summary>
            public CQPluginProxy? Plugin { get; set; }
        }
    }
}