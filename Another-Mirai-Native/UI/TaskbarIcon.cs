using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC;

namespace Another_Mirai_Native
{
    public partial class Entry
    {
        private static Thread UIThread { get; set; }

        private static NotifyIcon NotifyIcon { get; set; }

        private static ToolStripMenuItem TaskBarMenuParent { get; set; }

        private static IntPtr ConsoleHandle { get; set; }

        private static void BuildTaskBar()
        {
            if (UIThread == null)
            {
                UIThread = new Thread(() =>
                {
                    ConsoleHandle = WinNative.GetConsoleWindow();
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    WinNative.SetProcessDPIAware();

                    NotifyIcon = new NotifyIcon();
                    NotifyIcon.Icon = new Icon(new MemoryStream(Convert.FromBase64String(Resources.TaskBarIconResources.IconBase64)));
                    var menu = new ContextMenuStrip();
                    NotifyIcon.ContextMenuStrip = menu;

                    menu.Items.Add(new ToolStripMenuItem { Text = $"{AppConfig.Instance.CurrentNickName}({AppConfig.Instance.CurrentQQ})" });
                    menu.Items.Add("-");
                    menu.Items.Add(new ToolStripMenuItem { Text = $"框架版本: {ServerManager.Server.GetCoreVersion()}" });
                    menu.Items.Add("-");
                    TaskBarMenuParent = new ToolStripMenuItem() { Text = "应用" };
                    menu.Items.Add(TaskBarMenuParent);
                    menu.Items.Add("-");
                    ToolStripMenuItem reloadItem = new() { Text = "重载插件" };
                    reloadItem.Click += async (a, b) =>
                    {
                        await Task.Run(() =>
                        {
                            if (MessageBox.Show("确定要重载插件吗？", "嗯？", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                PluginManagerProxy.Instance.ReloadAllPlugins();
                            }
                        });
                    };
                    menu.Items.Add(reloadItem);
                    ToolStripMenuItem exitItem = new() { Text = "退出" };
                    exitItem.Click += async (a, b) =>
                    {
                        await Task.Run(() =>
                        {
                            if (MessageBox.Show("确定要退出框架吗？", "嗯？", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                Environment.Exit(0);
                            }
                        });
                    };
                    menu.Items.Add(exitItem);

                    NotifyIcon.Text = $"{AppConfig.Instance.CurrentNickName}({AppConfig.Instance.CurrentQQ})\n已启用 {PluginManagerProxy.Proxies.Count(x => x.Enabled)} 个插件";
                    NotifyIcon.Visible = true;
                    NotifyIcon.DoubleClick += (_, _) => WinNative.SetForegroundWindow(ConsoleHandle);
                    RebuildTaskBarMenu();
                    PluginManagerProxy.OnPluginEnableChanged -= (_) => Invoke(RebuildTaskBarMenu);
                    PluginManagerProxy.OnPluginEnableChanged += (_) => Invoke(RebuildTaskBarMenu);
                    Application.Run();
                });
                UIThread.SetApartmentState(ApartmentState.STA);
                UIThread.Start();
            }
        }

        private static void RebuildTaskBarMenu()
        {
            NotifyIcon.Text = $"{AppConfig.Instance.CurrentNickName}({AppConfig.Instance.CurrentQQ})\n已启用 {PluginManagerProxy.Proxies.Count(x => x.Enabled)} 个插件";
            TaskBarMenuParent.DropDownItems.Clear();
            foreach (var item in PluginManagerProxy.Proxies.OrderBy(x => x.PluginName))
            {
                ToolStripMenuItem menuItem = new() { Text = $"{item.PluginName}", Tag = item };
                ToolStripMenuItem enableItem = new() { Text = item.Enabled ? "√ 启用" : "启用", Tag = item };
                ToolStripMenuItem disableItem = new() { Text = !item.Enabled ? "√ 禁用" : "禁用", Tag = item };
                enableItem.Click += async (sender, b) =>
                {
                    await Task.Run(() =>
                    {
                        if (sender is not ToolStripMenuItem selectItem || selectItem.Tag is not CQPluginProxy plugin || plugin.Enabled)
                        {
                            return;
                        }

                        if (plugin.MovePluginToTmpDir()
                            && plugin.LoadAppInfo()
                            && PluginManagerProxy.Instance.SetPluginEnabled(plugin, true))
                        {
                            Invoke(() =>
                            {
                                enableItem.Text = "√ 启用";
                                disableItem.Text = "禁用";
                            });

                            AppConfig.Instance.AutoEnablePlugin.Add(plugin.PluginName);
                            AppConfig.Instance.AutoEnablePlugin = AppConfig.Instance.AutoEnablePlugin.Distinct().ToList();
                            AppConfig.Instance.SetConfig("AutoEnablePlugins", AppConfig.Instance.AutoEnablePlugin);
                        }
                        else
                        {
                            MessageBox.Show("插件启用失败", "啊嘞？", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                };
                disableItem.Click += async (sender, b) =>
                {
                    await Task.Run(() =>
                    {
                        if (sender is not ToolStripMenuItem selectItem || selectItem.Tag is not CQPluginProxy plugin || !plugin.Enabled)
                        {
                            return;
                        }

                        if (PluginManagerProxy.Instance.SetPluginEnabled(plugin, false))
                        {
                            Invoke(() =>
                            {
                                enableItem.Text = "启用";
                                disableItem.Text = "√ 禁用";
                            });

                            AppConfig.Instance.AutoEnablePlugin.Remove(plugin.PluginName);
                            AppConfig.Instance.AutoEnablePlugin = AppConfig.Instance.AutoEnablePlugin.Distinct().ToList();
                            AppConfig.Instance.SetConfig("AutoEnablePlugins", AppConfig.Instance.AutoEnablePlugin);
                        }
                        else
                        {
                            MessageBox.Show("插件禁用失败", "啊嘞？", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                };
                menuItem.DropDownItems.Add(enableItem);
                menuItem.DropDownItems.Add(disableItem);
                if (item.AppInfo.menu.Length > 0)
                {
                    menuItem.DropDownItems.Add("-");
                }
                foreach (var subMenu in item.AppInfo.menu)
                {
                    ToolStripMenuItem subMenuItem = new() { Text = subMenu.name, Tag = item };
                    subMenuItem.Click += async (sender, b) =>
                    {
                        await Task.Run(() =>
                        {
                            if (sender is not ToolStripMenuItem selectItem || selectItem.Tag is not CQPluginProxy plugin)
                            {
                                return;
                            }

                            if (plugin.Enabled is false)
                            {
                                MessageBox.Show("当前插件未启用，无法调用窗口事件", "嗯哼", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            PluginManagerProxy.Instance.InvokeEvent(plugin, PluginEventType.Menu, subMenu.function);
                        });
                    };
                    menuItem.DropDownItems.Add(subMenuItem);
                }

                TaskBarMenuParent.DropDownItems.Add(menuItem);
            }
        }

        private static void Invoke(Action action)
        {
            if (NotifyIcon.ContextMenuStrip != null && NotifyIcon.ContextMenuStrip.InvokeRequired)
            {
                NotifyIcon.ContextMenuStrip.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        private static void SetQRCodeAction(ProtocolManager protocolManager)
        {
            string guid = Guid.NewGuid().ToString().Replace("-", "");
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, guid + ".png");
            Action<string, byte[]> displayAction = (string url, byte[] data) =>
            {
                File.WriteAllBytes(filePath, data);
                LogHelper.Info("二维码", $"二维码已写出到运行目录：{filePath}");
                QRCodeHelper.Output(url, AppConfig.Instance.QRCodeCompatibilityMode);
            };
            Action finishedAction = () =>
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            };
            protocolManager.SetQrCodeAction(displayAction, finishedAction);
        }
    }
}