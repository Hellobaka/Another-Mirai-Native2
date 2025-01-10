using Another_Mirai_Native.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            Instance = this;
        }

        public static AboutPage Instance { get; private set; }

        private Version CurrentVersion => GetType().Assembly.GetName().Version ?? new();

        private UpdateModel.UpdateItem? NewUpdateModel { get; set; }

        public async void CheckUpdateButton_Click(object? sender, RoutedEventArgs? e)
        {
            UpdateUpdateStatus("检查更新中...", true);
            DownloadButton.IsEnabled = false;

            NewVersionDisplay.Visibility = Visibility.Collapsed;
            NewVersionUpdateTimeDisplay.Visibility = Visibility.Collapsed;
            NewVersionDescriptionDisplay.Visibility = Visibility.Collapsed;
            NewUpdateModel = await AppUpdater.GetLatestVersion();
            if (NewUpdateModel == null || (Version.TryParse(NewUpdateModel.Version, out Version? v) && v != null && v <= CurrentVersion))
            {
                UpdateUpdateStatus($"未发现新版本 ({DateTime.Now:G})", false);
                return;
            }
            UpdateUpdateStatus($"新版本发现", false);

            NewVersionDisplay.Text = $"新版本：{NewUpdateModel.Version}";
            NewVersionUpdateTimeDisplay.Text = $"更新时间：{NewUpdateModel.ReleaseTime:G}";
            NewVersionDescriptionDisplay.Text = $"更新说明：\n{NewUpdateModel.ReleaseDescription}";

            NewVersionDisplay.Visibility = Visibility.Visible;
            NewVersionUpdateTimeDisplay.Visibility = Visibility.Visible;
            NewVersionDescriptionDisplay.Visibility = Visibility.Visible;

            DownloadButton.IsEnabled = true;
        }

        private async void ClearAudioButton_Click(object sender, RoutedEventArgs e)
        {
            if (await DialogHelper.ShowConfirmDialog("询问", "确定要删除所有音频缓存(*.cqrecord)吗？"))
            {
                try
                {
                    ClearAudioButton.Content = "清理进行中...";
                    ClearAudioButton.IsEnabled = false;
                    Mouse.OverrideCursor = Cursors.Wait;
                    string path = @"data\record";
                    var list = Directory.GetFiles(path, "*.cqrecord");
                    await Task.Run(() => Parallel.ForEach(list, i =>
                    {
                        File.Delete(i);
                    }));
                    DialogHelper.ShowSimpleDialog("清理完成", $"清理了 {list.Length} 个音频缓存");
                }
                catch (Exception ex)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    DialogHelper.ShowSimpleDialog("清理失败", $"错误信息: {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    ClearAudioButton.Content = "清空音频缓存";
                    ClearAudioButton.IsEnabled = true;
                }
            }
        }

        private async void ClearChatAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            if (await DialogHelper.ShowConfirmDialog("询问", "确定要删除所有聊天页面缓存的头像吗？"))
            {
                try
                {
                    ClearChatAvatarButton.Content = "清理进行中...";
                    ClearChatAvatarButton.IsEnabled = false;
                    Mouse.OverrideCursor = Cursors.Wait;
                    string path = @"data\image\cached\group";
                    var list = Directory.GetFiles(path);
                    int count = list.Length;
                    await Task.Run(() => Parallel.ForEach(list, i =>
                    {
                        File.Delete(i);
                    }));

                    path = @"data\image\cached\private";
                    list = Directory.GetFiles(path);
                    count += list.Length;
                    await Task.Run(() => Parallel.ForEach(list, i =>
                    {
                        File.Delete(i);
                    }));

                    DialogHelper.ShowSimpleDialog("清理完成", $"清理了 {list.Length} 个头像缓存");
                }
                catch (Exception ex)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    DialogHelper.ShowSimpleDialog("清理失败", $"错误信息: {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    ClearChatAvatarButton.Content = "清空聊天头像缓存";
                    ClearChatAvatarButton.IsEnabled = true;
                }
            }
        }

        private async void ClearChatCacheButton_Click(object sender, RoutedEventArgs e)
        {
            if (await DialogHelper.ShowConfirmDialog("询问", "确定要删除*所有*聊天页面产生的缓存(cached文件夹)吗？包括头像、图片与音频"))
            {
                try
                {
                    ClearChatCacheButton.Content = "清理进行中...";
                    ClearChatCacheButton.IsEnabled = false;
                    Mouse.OverrideCursor = Cursors.Wait;
                    string path = @"data\image\cached\group";
                    var list = Directory.GetFiles(path);
                    int count = list.Length;
                    await Task.Run(() => Parallel.ForEach(list, i =>
                    {
                        File.Delete(i);
                    }));

                    path = @"data\image\cached\private";
                    list = Directory.GetFiles(path);
                    count += list.Length;
                    await Task.Run(() => Parallel.ForEach(list, i =>
                    {
                        File.Delete(i);
                    }));

                    path = @"data\image\cached";
                    list = Directory.GetFiles(path);
                    count += list.Length;
                    await Task.Run(() => Parallel.ForEach(list, i =>
                    {
                        File.Delete(i);
                    }));

                    path = @"data\record\cached";
                    list = Directory.GetFiles(path);
                    count += list.Length;
                    await Task.Run(() => Parallel.ForEach(list, i =>
                    {
                        File.Delete(i);
                    }));

                    DialogHelper.ShowSimpleDialog("清理完成", $"清理了 {list.Length} 项缓存");
                }
                catch (Exception ex)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    DialogHelper.ShowSimpleDialog("清理失败", $"错误信息: {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    ClearChatCacheButton.Content = "清空聊天缓存";
                    ClearChatCacheButton.IsEnabled = true;
                }
            }
        }

        private async void ClearChatHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (await DialogHelper.ShowConfirmDialog("询问", "确定要删除聊天历史吗？清空后需要重启程序"))
            {
                try
                {
                    ClearChatHistoryButton.Content = "清理进行中...";
                    ClearChatHistoryButton.IsEnabled = false;
                    Mouse.OverrideCursor = Cursors.Wait;
                    string path = @"logs\ChatHistory";
                    var list = Directory.GetFiles(path, "*.db", SearchOption.AllDirectories);
                    await Task.Run(() => Parallel.ForEach(list, i =>
                    {
                        File.Delete(i);
                    }));

                    Mouse.OverrideCursor = Cursors.Arrow;
                    await DialogHelper.ShowConfirmDialog("清理完成", $"点击任意按钮重启程序");
                    Helper.OpenFolder(GetType().Assembly.Location);
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    DialogHelper.ShowSimpleDialog("清理失败", $"错误信息: {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    ClearChatHistoryButton.Content = "清空聊天历史";
                    ClearChatHistoryButton.IsEnabled = true;
                }
            }
        }

        private async void ClearChatImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (await DialogHelper.ShowConfirmDialog("询问", "确定要删除所有聊天页面缓存的图片吗？"))
            {
                try
                {
                    ClearChatImageButton.Content = "清理进行中...";
                    ClearChatImageButton.IsEnabled = false;
                    Mouse.OverrideCursor = Cursors.Wait;
                    string path = @"data\image\cached";
                    var list = Directory.GetFiles(path);
                    int count = list.Length;
                    await Task.Run(() => Parallel.ForEach(list, i =>
                    {
                        File.Delete(i);
                    }));

                    DialogHelper.ShowSimpleDialog("清理完成", $"清理了 {list.Length} 个图片缓存");
                }
                catch (Exception ex)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    DialogHelper.ShowSimpleDialog("清理失败", $"错误信息: {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    ClearChatImageButton.Content = "清空聊天图片缓存";
                    ClearChatImageButton.IsEnabled = true;
                }
            }
        }

        private async void ClearPictureButton_Click(object sender, RoutedEventArgs e)
        {
            if (await DialogHelper.ShowConfirmDialog("询问", "确定要删除所有图片缓存(*.cqimg)吗？"))
            {
                try
                {
                    ClearPictureButton.Content = "清理进行中...";
                    ClearPictureButton.IsEnabled = false;
                    Mouse.OverrideCursor = Cursors.Wait;
                    string path = @"data\image";
                    var list = Directory.GetFiles(path, "*.cqimg");
                    await Task.Run(() => Parallel.ForEach(list, i =>
                    {
                        File.Delete(i);
                    }));
                    DialogHelper.ShowSimpleDialog("清理完成", $"清理了 {list.Length} 个图片缓存");
                }
                catch (Exception ex)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    DialogHelper.ShowSimpleDialog("清理失败", $"错误信息: {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    ClearPictureButton.Content = "清空图片缓存";
                    ClearPictureButton.IsEnabled = true;
                }
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewUpdateModel == null || string.IsNullOrEmpty(NewUpdateModel.DownloadUrl))
            {
                return;
            }
            DownloadButton.IsEnabled = false;
            UpdateUpdateStatus("下载更新中...", true);
            string? path = await AppUpdater.DownloadVersion(NewUpdateModel);
            if (string.IsNullOrEmpty(path))
            {
                UpdateUpdateStatus("下载更新失败", false);
                return;
            }
            Process.Start("explorer.exe", $"/select,\"{path}\"");
            UpdateUpdateStatus("下载更新完成", false);
        }

        private void UpdateUpdateStatus(string msg, bool ringVisible)
        {
            UpdateStatusDisplay.Text = msg;
            UpdateStatusRing.Visibility = ringVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentVersionDisplay.Text = $"当前版本：{CurrentVersion}";
        }
    }
}