using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.WebSocket;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;

namespace Another_Mirai_Native.UI
{
    public class ErrorDialogHelper
    {
        private static SimpleMessageBox CurrentDialog { get; set; }

        private static Queue<DialogQueueObject> ErrorDialogQueue { get; set; } = new();

        private class DialogQueueObject
        {
            public SimpleMessageBox Dialog { get; set; }

            public CQPluginProxy Plugin { get; set; }

            public string GUID { get; set; }

            public ContentDialogResult DialogResult { get; set; } = ContentDialogResult.None;
        }

        public static void ShowErrorDialog(InvokeBody caller)
        {
            if (caller == null || caller.Args.Length != 4)
            {
                return;
            }
            var plugin = PluginManagerProxy.GetProxyByAuthCode(Convert.ToInt32(caller.Args[0]));
            ShowErrorDialog(plugin, caller.GUID, caller.Args[1].ToString(), caller.Args[2].ToString(), caller.Args[3].ToString() == "1");
        }

        public static void ShowErrorDialog(CQPluginProxy proxy, string guid, string message, string detail, bool canIgnore = true)
        {
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
                dialog.ErrorMessage = $"插件 {proxy.PluginName}:";
            }
            var queue = new DialogQueueObject { Dialog = dialog, Plugin = proxy, GUID = guid };
            ErrorDialogQueue.Enqueue(queue);
            if (CurrentDialog != null)
            {
                CurrentDialog.Title = $"异常捕获 ({ErrorDialogQueue.Count + 1}条)";
            }
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
                dialog.DialogResult = await dialog.Dialog.ShowAsync();
            });
            if (Server.Instance.WaitingMessage.ContainsKey(dialog.GUID))
            {
                Server.Instance.WaitingMessage[dialog.GUID].Result = true;
                Server.Instance.WaitingMessage[dialog.GUID].Success = true;
            }
            if (dialog.DialogResult == ContentDialogResult.Primary && dialog.Plugin != null)
            {
                PluginManagerProxy.Instance.ReloadPlugin(dialog.Plugin);
            }
            CurrentDialog = null;
            HandleDialogQueue();
        }
    }
}