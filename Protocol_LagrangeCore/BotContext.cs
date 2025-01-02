using Another_Mirai_Native.Config;
using Another_Mirai_Native;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;
using System.Diagnostics;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    public partial class LagrangeCoreAPI
    {
        private BotContext BotContext { get; set; }

        private BotSign? BotSign { get; set; }

        private CancellationTokenSource? LoginToken { get; set; }

        private bool InitBotContext()
        {
            try
            {
                BotSign ??= new BotSign(LagrangeConfig.SignUrl, LagrangeConfig.SignFallbackPlatform);
                LagrangeConfig.BotConfig.CustomSignProvider = BotSign;
                BotContext = BotFactory.Create(LagrangeConfig.BotConfig,
                    LagrangeConfig.BotDeviceInfo,
                    LagrangeConfig.BotKeystore,
                    BotSign.BotAppInfo);

                BotContext.Invoker.OnBotLogEvent += Invoker_OnBotLogEvent;
                BotContext.Invoker.OnBotCaptchaEvent += Invoker_OnBotCaptchaEvent;
                BotContext.Invoker.OnBotOnlineEvent += Invoker_OnBotOnlineEvent;
                BotContext.Invoker.OnBotOfflineEvent += Invoker_OnBotOfflineEvent;
                BotContext.Invoker.OnPinChangedEvent += Invoker_OnPinChangedEvent;
                BotContext.Invoker.OnBotNewDeviceVerify += Invoker_OnBotNewDeviceVerify;
                BotContext.Invoker.OnDeviceLoginEvent += Invoker_OnDeviceLoginEvent;
                BotContext.Invoker.OnFriendMessageReceived += Invoker_OnFriendMessageReceived;
                BotContext.Invoker.OnFriendPokeEvent += Invoker_OnFriendPokeEvent;
                BotContext.Invoker.OnFriendRecallEvent += Invoker_OnFriendRecallEvent;
                BotContext.Invoker.OnFriendRequestEvent += Invoker_OnFriendRequestEvent;
                BotContext.Invoker.OnGroupAdminChangedEvent += Invoker_OnGroupAdminChangedEvent;
                BotContext.Invoker.OnGroupEssenceEvent += Invoker_OnGroupEssenceEvent;
                BotContext.Invoker.OnGroupInvitationReceived += Invoker_OnGroupInvitationReceived;
                BotContext.Invoker.OnGroupInvitationRequestEvent += Invoker_OnGroupInvitationRequestEvent;
                BotContext.Invoker.OnGroupJoinRequestEvent += Invoker_OnGroupJoinRequestEvent;
                BotContext.Invoker.OnGroupMemberDecreaseEvent += Invoker_OnGroupMemberDecreaseEvent;
                BotContext.Invoker.OnGroupMemberEnterEvent += Invoker_OnGroupMemberEnterEvent;
                BotContext.Invoker.OnGroupMemberIncreaseEvent += Invoker_OnGroupMemberIncreaseEvent;
                BotContext.Invoker.OnGroupMemberMuteEvent += Invoker_OnGroupMemberMuteEvent;
                BotContext.Invoker.OnGroupMessageReceived += Invoker_OnGroupMessageReceived;
                BotContext.Invoker.OnGroupMuteEvent += Invoker_OnGroupMuteEvent;
                BotContext.Invoker.OnGroupNameChangeEvent += Invoker_OnGroupNameChangeEvent;
                BotContext.Invoker.OnGroupPokeEvent += Invoker_OnGroupPokeEvent;
                BotContext.Invoker.OnGroupReactionEvent += Invoker_OnGroupReactionEvent;
                BotContext.Invoker.OnGroupRecallEvent += Invoker_OnGroupRecallEvent;
                BotContext.Invoker.OnGroupTodoEvent += Invoker_OnGroupTodoEvent;
                BotContext.Invoker.OnTempMessageReceived += Invoker_OnTempMessageReceived;

                MessageChainPaser.OnFileUploaded += MessageChainPaser_OnFileUploaded;
                LogHelper.Info("创建Bot实例", $"成功");

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("创建Bot实例", ex);
                return false;
            }
        }

        private void MessageChainPaser_OnFileUploaded(MessageChain chain, FileEntity file)
        {
            Stopwatch sw = Stopwatch.StartNew();
            MemoryStream stream = new();
            BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, file.FileId);
            BinaryWriterExpand.Write_Ex(binaryWriter, file.FileName);
            BinaryWriterExpand.Write_Ex(binaryWriter, file.FileSize);
            BinaryWriterExpand.Write_Ex(binaryWriter, 0);
            PluginManagerProxy.Instance.Event_OnUpload(1, Helper.TimeStamp, chain.GroupUin.ToLong(), chain.FriendUin, Convert.ToBase64String(stream.ToArray()));
            sw.Stop();
            LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "文件上传", $"来源群:{chain.GroupUin}({chain.FriendInfo?.Group.GroupName}) 来源QQ:{chain.FriendUin}({chain.FriendInfo?.Nickname}) " +
                $"文件名:{file.FileName} 大小:{file.FileSize / 1000}KB FileID:{file.FileId}", $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");
        }

        private void Invoker_OnTempMessageReceived(BotContext context, Lagrange.Core.Event.EventArg.TempMessageEvent e)
        {
            
        }

        private void Invoker_OnGroupTodoEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupTodoEvent e)
        {
            
        }

        private void Invoker_OnGroupRecallEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupRecallEvent e)
        {
            
        }

        private void Invoker_OnGroupReactionEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupReactionEvent e)
        {
            
        }

        private void Invoker_OnGroupPokeEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupPokeEvent e)
        {
            
        }

        private void Invoker_OnGroupNameChangeEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupNameChangeEvent e)
        {
            
        }

        private void Invoker_OnGroupMuteEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupMuteEvent e)
        {
            
        }

        private void Invoker_OnGroupMessageReceived(BotContext context, Lagrange.Core.Event.EventArg.GroupMessageEvent e)
        {
            if (e.Chain.FriendUin == AppConfig.Instance.CurrentQQ)
            {
                return;
            }
            Stopwatch sw = Stopwatch.StartNew();
            var message = MessageChainPaser.ParseMessageChainToCQCode(e.Chain);
            int messageId = MessageCacher.CalcMessageHash(e.Chain.MessageId, e.Chain.Sequence);
            Task.Run(() => MessageCacher.RecordMessage(messageId, e.Chain));

            int logId = 0;
            logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架", "[↓]收到消息", $"群:{e.Chain.GroupUin}({e.Chain.FriendInfo?.Group.GroupName}) QQ:{e.Chain.FriendUin}({e.Chain.FriendInfo?.Nickname}) 消息: {message}", "处理中...");
            CQPluginProxy handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMsg(1, messageId, e.Chain.GroupUin.ToLong(), e.Chain.FriendUin, "", message, 0, DateTime.Now);
            
            sw.Stop();
            string updateMsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updateMsg += $"(由 {handledPlugin.AppInfo.name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);
        }

        private void Invoker_OnGroupMemberMuteEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupMemberMuteEvent e)
        {
            
        }

        private void Invoker_OnGroupMemberIncreaseEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupMemberIncreaseEvent e)
        {
            
        }

        private void Invoker_OnGroupMemberEnterEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupMemberEnterEvent e)
        {
            
        }

        private void Invoker_OnGroupMemberDecreaseEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupMemberDecreaseEvent e)
        {
            
        }

        private void Invoker_OnGroupJoinRequestEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupJoinRequestEvent e)
        {
            
        }

        private void Invoker_OnGroupInvitationRequestEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupInvitationRequestEvent e)
        {
            
        }

        private void Invoker_OnGroupInvitationReceived(BotContext context, Lagrange.Core.Event.EventArg.GroupInvitationEvent e)
        {
            
        }

        private void Invoker_OnGroupEssenceEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupEssenceEvent e)
        {
            
        }

        private void Invoker_OnGroupAdminChangedEvent(BotContext context, Lagrange.Core.Event.EventArg.GroupAdminChangedEvent e)
        {
            
        }

        private void Invoker_OnFriendRequestEvent(BotContext context, Lagrange.Core.Event.EventArg.FriendRequestEvent e)
        {
            
        }

        private void Invoker_OnFriendRecallEvent(BotContext context, Lagrange.Core.Event.EventArg.FriendRecallEvent e)
        {
            
        }

        private void Invoker_OnFriendPokeEvent(BotContext context, Lagrange.Core.Event.EventArg.FriendPokeEvent e)
        {
            
        }

        private void Invoker_OnFriendMessageReceived(BotContext context, Lagrange.Core.Event.EventArg.FriendMessageEvent e)
        {
            
        }

        private void Invoker_OnDeviceLoginEvent(BotContext context, Lagrange.Core.Event.EventArg.DeviceLoginEvent e)
        {
            
        }

        private void Invoker_OnBotNewDeviceVerify(BotContext context, Lagrange.Core.Event.EventArg.BotNewDeviceVerifyEvent e)
        {
            
        }

        private void Invoker_OnPinChangedEvent(BotContext context, Lagrange.Core.Event.EventArg.PinChangedEvent e)
        {
            
        }

        private void Invoker_OnBotOfflineEvent(BotContext context, Lagrange.Core.Event.EventArg.BotOfflineEvent e)
        {
            LogHelper.Info("Bot离线", e.EventMessage);
            IsConnected = false;
        }

        private void Invoker_OnBotOnlineEvent(BotContext context, Lagrange.Core.Event.EventArg.BotOnlineEvent e)
        {
            LogHelper.Info("Bot在线", e.EventMessage);
            RequestWaiter.TriggerByKey("LagrangeCoreLogin", true);
            IsConnected = true;
        }

        private void Invoker_OnBotCaptchaEvent(BotContext context, Lagrange.Core.Event.EventArg.BotCaptchaEvent e)
        {
            
        }

        private void Invoker_OnBotLogEvent(BotContext context, Lagrange.Core.Event.EventArg.BotLogEvent e)
        {
            LogLevel level = e.Level switch
            {
                Lagrange.Core.Event.EventArg.LogLevel.Debug => LogLevel.Debug,
                Lagrange.Core.Event.EventArg.LogLevel.Verbose => LogLevel.Debug,
                Lagrange.Core.Event.EventArg.LogLevel.Information => LogLevel.Info,
                Lagrange.Core.Event.EventArg.LogLevel.Warning => LogLevel.Warning,
                Lagrange.Core.Event.EventArg.LogLevel.Exception => LogLevel.Error,
                Lagrange.Core.Event.EventArg.LogLevel.Fatal => LogLevel.Error,
                _ => LogLevel.Info
            };
            LogHelper.WriteLog(level, e.Tag, e.EventMessage);
        }

        private bool DisposeBotContext()
        {
            try
            {
                BotContext.Invoker.OnBotLogEvent -= Invoker_OnBotLogEvent;
                BotContext.Invoker.OnBotCaptchaEvent -= Invoker_OnBotCaptchaEvent;
                BotContext.Invoker.OnBotOnlineEvent -= Invoker_OnBotOnlineEvent;
                BotContext.Invoker.OnBotOfflineEvent -= Invoker_OnBotOfflineEvent;
                BotContext.Invoker.OnPinChangedEvent -= Invoker_OnPinChangedEvent;
                BotContext.Invoker.OnBotNewDeviceVerify -= Invoker_OnBotNewDeviceVerify;
                BotContext.Invoker.OnDeviceLoginEvent -= Invoker_OnDeviceLoginEvent;
                BotContext.Invoker.OnFriendMessageReceived -= Invoker_OnFriendMessageReceived;
                BotContext.Invoker.OnFriendPokeEvent -= Invoker_OnFriendPokeEvent;
                BotContext.Invoker.OnFriendRecallEvent -= Invoker_OnFriendRecallEvent;
                BotContext.Invoker.OnFriendRequestEvent -= Invoker_OnFriendRequestEvent;
                BotContext.Invoker.OnGroupAdminChangedEvent -= Invoker_OnGroupAdminChangedEvent;
                BotContext.Invoker.OnGroupEssenceEvent -= Invoker_OnGroupEssenceEvent;
                BotContext.Invoker.OnGroupInvitationReceived -= Invoker_OnGroupInvitationReceived;
                BotContext.Invoker.OnGroupInvitationRequestEvent -= Invoker_OnGroupInvitationRequestEvent;
                BotContext.Invoker.OnGroupJoinRequestEvent -= Invoker_OnGroupJoinRequestEvent;
                BotContext.Invoker.OnGroupMemberDecreaseEvent -= Invoker_OnGroupMemberDecreaseEvent;
                BotContext.Invoker.OnGroupMemberEnterEvent -= Invoker_OnGroupMemberEnterEvent;
                BotContext.Invoker.OnGroupMemberIncreaseEvent -= Invoker_OnGroupMemberIncreaseEvent;
                BotContext.Invoker.OnGroupMemberMuteEvent -= Invoker_OnGroupMemberMuteEvent;
                BotContext.Invoker.OnGroupMessageReceived -= Invoker_OnGroupMessageReceived;
                BotContext.Invoker.OnGroupMuteEvent -= Invoker_OnGroupMuteEvent;
                BotContext.Invoker.OnGroupNameChangeEvent -= Invoker_OnGroupNameChangeEvent;
                BotContext.Invoker.OnGroupPokeEvent -= Invoker_OnGroupPokeEvent;
                BotContext.Invoker.OnGroupReactionEvent -= Invoker_OnGroupReactionEvent;
                BotContext.Invoker.OnGroupRecallEvent -= Invoker_OnGroupRecallEvent;
                BotContext.Invoker.OnGroupTodoEvent -= Invoker_OnGroupTodoEvent;
                BotContext.Invoker.OnTempMessageReceived -= Invoker_OnTempMessageReceived;
                MessageChainPaser.OnFileUploaded -= MessageChainPaser_OnFileUploaded;

                LoginToken?.Cancel();
                BotContext?.Dispose();

                LogHelper.Info("销毁Bot实例", $"成功");
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("销毁Bot实例", e);
                return false;
            }
        }

        private bool Login()
        {
            try
            {
                LoginToken?.Cancel();
                LoginToken = new();
                // 首先尝试使用缓存的凭据登录
                // 失败后再使用密码登录 (不实现)
                // 若不存在密码，则使用二维码
                var keystore = BotContext.UpdateKeystore();
                bool wait = RequestWaiter.Wait("LagrangeCoreLogin", (int)TimeSpan.FromSeconds(60).TotalMilliseconds, () =>
                {
                    bool success = SessionLogin(keystore)
                        || PasswordLogin(keystore)
                        || QrCodeLogin();

                    LogHelper.Info("账号登录", $"登录结果：{success}；等待在线信号...");

                    if (success)
                    {
                        LagrangeConfig.Instance.Save();
                        LogHelper.Info("账号登录", "已更新登录状态缓存");
                    }
                    else
                    {
                        RequestWaiter.TriggerByKey("LagrangeCoreLogin", false);
                    }
                }, out object r);
                bool waitResult = r != null && (bool)r;
                if (wait)
                {
                    if (waitResult)
                    {
                        LogHelper.Info("账号登录", $"登录成功，{BotContext.BotName}({BotContext.BotUin})");
                    }
                    else
                    {
                        LogHelper.Error("账号登录", "登录失败");
                    }
                }
                else
                {
                    LogHelper.Error("账号登录", "登录超时");
                    LoginToken?.Cancel();
                    return false;
                }
                return waitResult;
            }
            catch (Exception ex)
            {
                LogHelper.Error("账号登录", ex);
                return false;
            }
        }

        private bool QrCodeLogin()
        {
            LogHelper.Info("二维码登录", "获取二维码...");
            (string url, byte[] qrCode)? captcha = BotContext.FetchQrCode().Result;
            if (captcha == null || !captcha.HasValue)
            {
                LogHelper.Info("二维码登录", "获取二维码失败");
                return false;
            }
            LogHelper.Info("二维码登录", "二维码获取成功");
            QRCodeDisplayAction?.Invoke(captcha.Value.url, captcha.Value.qrCode);
            LogHelper.Info("二维码登录", "开始等待扫码状态...");
            var loginResult = ((Task<bool>)BotContext.LoginByQrCode(LoginToken.Token)).Result;
            if (loginResult)
            {
                LogHelper.Info("二维码登录", "扫码成功");
                LagrangeConfig.BotKeystore = BotContext.UpdateKeystore();
            }
            QRCodeFinishedAction?.Invoke();

            return loginResult;
        }

        private bool PasswordLogin(BotKeystore keystore)
        {
            return false;
        }

        private bool SessionLogin(BotKeystore keystore)
        {
            if (keystore.Session == null
                || keystore.Session.TempPassword == null
                || keystore.Session.TempPassword.Length == 0)
            {
                return false;
            }
            LogHelper.Info("缓存登录", "尝试使用会话缓存进行登录");

            return BotContext.LoginByPassword(LoginToken.Token).Result;
        }
    }
}
