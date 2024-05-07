using Another_Mirai_Native.Config;
using Another_Mirai_Native.Native;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// Test_MessagePage.xaml 的交互逻辑
    /// </summary>
    public partial class Test_MessagePage : Page, INotifyPropertyChanged
    {
        public Test_MessagePage()
        {
            InitializeComponent();
        }

        public long CurrentQQ { get; private set; }

        public bool FormLoaded { get; private set; }

        public bool PageEnabled { get; set; }

        private List<string> MessageHistories { get; set; } = new();

        private int MessageHistoryIndex { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void AddChatBlock(string text, bool right)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            TestMessageContainer.Dispatcher.BeginInvoke(() =>
            {
                var block = BuildChatBlock(text, right);
                TestMessageContainer.Children.Add(block);
                TestMessageScrollViewer.ScrollToEnd();
            });
        }

        private void AtBtn_Click(object sender, RoutedEventArgs e)
        {
            SendMessage.Text += $"[CQ:at,qq={CurrentQQ}]";
        }

        private Border BuildChatBlock(string text, bool right)
        {
            TextBox chatBlock = new()
            {
                Text = text,
                Padding = new Thickness(10),
                TextWrapping = TextWrapping.Wrap,
                IsReadOnly = true,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            };
            Border border = new()
            {
                Margin = new Thickness(right ? 100 : 0, 10, right ? 0 : 100, 0),
                CornerRadius = new CornerRadius(10),
                HorizontalAlignment = right ? System.Windows.HorizontalAlignment.Right : System.Windows.HorizontalAlignment.Left,
                Child = chatBlock
            };
            DynamicResourceExtension dynamicResource = new(right ? "SystemControlPageBackgroundChromeMediumLowBrush" : "SystemControlHighlightAltListAccentMediumBrush");
            border.SetResourceReference(Border.BackgroundProperty, dynamicResource.ResourceKey);

            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(new TextBlock() { Text = "复制", TextAlignment = TextAlignment.Center });
            contextMenu.Items.Add(new TextBlock() { Text = "+1", TextAlignment = TextAlignment.Center });
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new TextBlock() { Text = "读取图片", TextAlignment = TextAlignment.Center });

            (contextMenu.Items[0] as UIElement).MouseDown += (sender, _) =>
            {
                System.Windows.Clipboard.SetText(text);
            };
            (contextMenu.Items[1] as UIElement).MouseDown += (sender, e) =>
            {
                SendMessage.Text = text;
                SendBtn_Click(sender, e);
            };
            (contextMenu.Items[3] as UIElement).MouseDown += (sender, _) =>
            {
                string img = text.ToString();
                var c = Regex.Matches(img, "\\[CQ:image,file=(.*?)\\]");
                if (c.Count > 0)
                {
                    string img_file = c[0].Groups[1].Value;
                    string img_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", img_file);
                    if (File.Exists(img_path))
                    {
                        Helper.OpenFolder(img_path);
                    }
                    else if (File.Exists(img_path + ".cqimg"))
                    {
                        string picTmp = File.ReadAllText(img_path + ".cqimg");
                        picTmp = picTmp.Split('\n').Last().Replace("url=", "");
                        Helper.OpenFolder(picTmp);
                    }
                }
            };
            chatBlock.ContextMenu = contextMenu;
            return border;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            TestMessageContainer.Children.Clear();
        }

        private void ImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "CoolQ Image|*.cqimg|图片文件|*.png;*.jpg;*.jpeg;*.webp|所有文件|*.*",
                Title = "选择发送的图片",
                Multiselect = false
            };
            dialog.ShowDialog();
            string path = dialog.FileName;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            string fileName = Path.GetFileName(path);
            if (!File.Exists($@"data\image\{fileName}"))
            {
                File.Copy(path, $@"data\image\{fileName}");
            }
            SendMessage.Text += $"[CQ:image,file={fileName.Replace(".cqimg", "")}]";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PageEnabled = TestPage.CurrentPlugin != null;
            OnPropertyChanged(nameof(PageEnabled));
            if (FormLoaded)
            {
                return;
            }
            FormLoaded = true;
            GroupDisplay.Text = CommonConfig.GetConfig("TesterGroup", @"conf/Test.json", (long)100000).ToString();
            QQDisplay.Text = CommonConfig.GetConfig("TesterQQ", @"conf/Test.json", (long)100000).ToString();
            MessageHistories = CommonConfig.GetConfig("MessageHistories", @"conf/Test.json", new List<string>());
            CurrentQQ = ProtocolManager.Instance.CurrentProtocol.GetLoginQQ();
            PluginManagerProxy.OnTestInvoked -= PluginManagerProxy_OnTestInvoked;
            PluginManagerProxy.OnTestInvoked += PluginManagerProxy_OnTestInvoked;
            DataContext = this;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PluginManagerProxy_OnTestInvoked(string functionName, Dictionary<string, object> obj)
        {
            try
            {
                switch (functionName)
                {
                    case "CQ_sendPrivateMsg":
                    case "CQ_sendGroupMsg":
                    case "CQ_sendGroupQuoteMsg":
                        string msg = obj["msg"].ToString();
                        AddChatBlock(msg, false);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            string msg = SendMessage.Text;
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            AddChatBlock(msg, true);
            CommonConfig.SetConfig("TesterGroup", Convert.ToInt64(GroupDisplay.Text), @"conf\Test.json");
            CommonConfig.SetConfig("TesterQQ", Convert.ToInt64(QQDisplay.Text), @"conf\Test.json");
            SendMessage.Text = "";
            if (MessageHistories.Contains(msg))
            {
                MessageHistories.Remove(msg);
            }
            MessageHistories.Add(msg);
            CommonConfig.SetConfig("MessageHistories", MessageHistories, @"conf\Test.json");
            MessageHistoryIndex = 0;
            bool useGroup = GroupMessageSelector.IsChecked.Value;
            Thread thread = new(() =>
            {
                Stopwatch sw = new();
                sw.Start();
                bool flag = useGroup ? SendGroupMessage(msg)
                        : SendPrivateMessage(msg);
                AddChatBlock($"插件{(flag ? "结束" : "放弃")}了处理({sw.ElapsedMilliseconds}ms)", false);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private bool SendGroupMessage(string msg)
        {
            try
            {
                long groupId = 0, qqId = 0;
                Dispatcher.Invoke(() =>
                {
                    groupId = Convert.ToInt64(GroupDisplay.Text);
                    qqId = Convert.ToInt64(QQDisplay.Text);
                });
                return PluginManagerProxy.Instance
                    .Event_OnGroupMsg(TestPage.CurrentPlugin, 1, 1, groupId, qqId, "", msg, 0)
                    == 1;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowSimpleDialog("嗯...", $"发生了错误: {ex.Message}");
                return false;
            }
        }

        private void SendMessage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Up)
            {
                MessageHistoryIndex = Math.Min(MessageHistories.Count, MessageHistoryIndex + 1);
                if (MessageHistories.Count > 0 && MessageHistories.Count > MessageHistoryIndex - 1)
                {
                    // SendMessage.Text = MessageHistories[MessageHistories.Count - MessageHistoryIndex];
                    SendMessage.Text = MessageHistories[MessageHistories.Count - MessageHistoryIndex];
                }
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
                MessageHistoryIndex = Math.Max(MessageHistoryIndex - 1, 0);
                try
                {
                    SendMessage.Text = MessageHistories[MessageHistories.Count - MessageHistoryIndex];
                }
                catch
                {
                    SendMessage.Text = "";
                }
            }
            else if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendBtn_Click(sender, null);
            }
        }

        private bool SendPrivateMessage(string msg)
        {
            try
            {
                long qqId = 0;
                Dispatcher.Invoke(() =>
                {
                    qqId = Convert.ToInt64(QQDisplay.Text);
                });
                return PluginManagerProxy.Instance
                    .Event_OnPrivateMsg(TestPage.CurrentPlugin, 1, 1, qqId, msg, 0)
                    == 1;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowSimpleDialog("嗯...", $"发生了错误: {ex.Message}");
                return false;
            }
        }
    }
}