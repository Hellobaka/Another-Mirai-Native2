using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.RPC;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Another_Mirai_Native
{
    public class ProtocolManager
    {
        public static ProtocolManager Instance { get; set; }

        public static List<IProtocol> Protocols { get; set; } = new();

        public IProtocol CurrentProtocol { get; set; }

        public CancellationTokenSource OfflineActionCancel { get; set; }

        private DateTime LastOnlineTime { get; set; }

        private bool OfflineHandled { get; set; }

        public ProtocolManager()
        {
            if (Protocols.Count == 0)
            {
                LoadProtocol();
            }
            Instance = this;
        }

        public void SetQrCodeAction(Action<string, byte[]> displayAction, Action finishedAction)
        {
            foreach (var protocol in Protocols)
            {
                protocol.QRCodeDisplayAction += displayAction;
                protocol.QRCodeFinishedAction += finishedAction;
            }
        }

        public bool Start(IProtocol protocol)
        {
            if (protocol == null)
            {
                return false;
            }
            if (CurrentProtocol != null)
            {
                CurrentProtocol.OnProtocolOnline -= Protocol_OnProtocolOnline;
                CurrentProtocol.OnProtocolOffline -= Protocol_OnProtocolOffline;
            }

            bool flag = protocol.Connect();
            RefreshBotInfo(protocol);
            flag = flag && !string.IsNullOrEmpty(AppConfig.Instance.CurrentNickName);
            if (flag)
            {
                CurrentProtocol = protocol;
                LastOnlineTime = DateTime.Now;
                OfflineHandled = false;
                protocol.OnProtocolOnline += Protocol_OnProtocolOnline;
                protocol.OnProtocolOffline += Protocol_OnProtocolOffline;
                ServerManager.Server.NotifyCurrentQQChanged(AppConfig.Instance.CurrentQQ, AppConfig.Instance.CurrentNickName);
            }
            LogHelper.Info("加载协议", $"加载 {protocol.Name} 协议{(flag ? "成功" : "失败")}");
            return flag;
        }

        private void Protocol_OnProtocolOffline()
        {
            if (OfflineHandled)
            {
                return;
            }
            OfflineHandled = true;
            OfflineActionCancel = new();
            new Debouncer(OfflineActionCancel).Debounce(OfflineAction, TimeSpan.FromSeconds(AppConfig.Instance.ActionAfterOfflineSeconds));
        }

        private void Protocol_OnProtocolOnline()
        {
            LastOnlineTime = DateTime.Now;
            OfflineActionCancel?.Cancel();
            OfflineHandled = false;
        }

        public bool Start(string protocolName)
        {
            if (!string.IsNullOrEmpty(protocolName) && Protocols.Any(x => x.Name == protocolName))
            {
                return Start(Protocols.First(x => x.Name == protocolName));
            }
            else
            {
                LogHelper.Error("加载协议", $"未找到 {protocolName} 协议");
            }
            return false;
        }

        public void LoadProtocol()
        {
            foreach (var item in Directory.GetFiles("protocols", "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(item);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (typeof(IProtocol).IsAssignableFrom(type) && !type.IsInterface)
                        {
                            if (Activator.CreateInstance(type) is IProtocol protocol)
                            {
                                Protocols.Add(protocol);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("加载协议", $"无法加载协议: {ex.Message} {ex.StackTrace}");
                }
            }
        }

        public void RefreshBotInfo(IProtocol protocol)
        {
            if (protocol != null && protocol.IsConnected)
            {
                AppConfig.Instance.CurrentNickName = protocol.GetLoginNick();
                AppConfig.Instance.CurrentQQ = protocol.GetLoginQQ();
            }
        }

        public void OfflineAction()
        {
            if (AppConfig.Instance.OfflineActionSendEmail)
            {
                if (OfflineAction_Email())
                {
                    LogHelper.Info("离线邮件发送", $"邮件发送成功，已投递至: {AppConfig.Instance.OfflineActionEmail_SMTPReceiveEmail}");
                }
                else
                {
                    LogHelper.Error("离线邮件发送", "邮件发送失败，可能需要修改配置，尝试以下操作：1. 更换为另一个端口 2.密码使用授权码而不是登录密码。");
                }
            }
            if (AppConfig.Instance.OfflineActionRunCommand)
            {
                OfflineAction_RunCommand();
            }
        }

        public bool OfflineAction_Email()
        {
            var onlineTimeSpan = DateTime.Now - LastOnlineTime;
            return new EmailSender().SendEmail("离线通知", true, $"{AppConfig.Instance.CurrentNickName}[{AppConfig.Instance.CurrentQQ}] 已离线", $"在线时间：{onlineTimeSpan.Days}天{onlineTimeSpan.Hours}小时{onlineTimeSpan.Minutes}分{onlineTimeSpan.Seconds}秒。");
        }

        public void OfflineAction_RunCommand()
        {
            if (AppConfig.Instance.OfflineActionCommands != null && AppConfig.Instance.OfflineActionCommands.Count > 0)
            {
                Task.Run(() =>
                {
                    int successCount = 0, failCount = 0;

                    foreach (var command in AppConfig.Instance.OfflineActionCommands)
                    {
                        try
                        {
                            var p = Process.Start(new ProcessStartInfo("cmd.exe", "/c " + command)
                            {
                                UseShellExecute = true,
                            });
                            p?.WaitForExit();
                            if (p?.ExitCode == 0)
                            {
                                successCount++;
                            }
                            else
                            {
                                failCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("离线后命令执行", $"执行 {command} 命令失败: {ex}");
                            failCount++;
                        }
                    }
                    LogHelper.Info("离线后命令执行", $"执行了 {successCount + failCount} 条命令，成功 {successCount} 条，失败 {failCount} 条");
                });
            }
        }
    }
}