﻿using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.RPC;
using System.Diagnostics;

namespace Another_Mirai_Native.Native
{
    public class PluginManagerProxy
    {
        public PluginManagerProxy()
        {
            Instance = this;
        }

        public static event Action<CQPluginProxy> OnPluginEnableChanged;

        public static event Action<CQPluginProxy> OnPluginProxyAdded;

        public static event Action<CQPluginProxy> OnPluginProxyConnectStatusChanged;

        public static event Action<string, Dictionary<string, object>> OnTestInvoked;

        #region 事件外抛

        /// <summary>
        /// Group QQ GroupMemberType
        /// </summary>
        public static event Action<long, long, QQGroupMemberType> OnAdminChanged;

        /// <summary>
        /// QQ
        /// </summary>
        public static event Action<long> OnFriendAdded;

        /// <summary>
        /// Group QQ
        /// </summary>
        public static event Action<long, long> OnGroupAdded;

        /// <summary>
        /// Group OperatorQQ OperatedQQ time
        /// </summary>
        public static event Action<long, long, long, long> OnGroupBan;

        /// <summary>
        /// Group QQ
        /// </summary>
        public static event Action<long, long> OnGroupLeft;

        /// <summary>
        /// MsgId QQ Msg
        /// </summary>
        public static event Action<int, long, string, DateTime> OnPrivateMsg;

        /// <summary>
        /// MsgId Group QQ Msg
        /// </summary>
        public static event Action<int, long, long, string, DateTime> OnGroupMsg;

        public static event Action<int, long, string> OnGroupMsgRecall;

        public static event Action<int, long, string> OnPrivateMsgRecall;

        public static event Action<long, long, string> OnGroupMemberCardChanged;

        public static event Action<long, string> OnFriendNickChanged;

        public static event Action<long, string> OnGroupNameChanged;

        #endregion 事件外抛

        public static PluginManagerProxy Instance { get; private set; }

        public static List<CQPluginProxy> Proxies { get; private set; } = new();

        public bool PluginLoaded { get; set; }

        public static CQPluginProxy GetProxyByAuthCode(int authCode)
        {
            return Proxies.FirstOrDefault(x => x.AppInfo.AuthCode == authCode);
        }

        public static int MakeAuthCode() => Helper.MakeUniqueID();

        public static void SetProxyConnected(CQPluginProxy proxy)
        {
            proxy.HasConnection = true;
            OnPluginProxyConnectStatusChanged?.Invoke(proxy);
        }

        public static void SetProxyDisconnected(CQPluginProxy proxy)
        {
            proxy.HasConnection = false;
            OnPluginProxyConnectStatusChanged?.Invoke(proxy);
            RequestWaiter.ResetSignalByPluginProxy(proxy);
        }

        public static void TriggerTestInvoke(string methodName, Dictionary<string, object> args)
        {
            new Thread(() =>
            {
                try
                {
                    OnTestInvoked?.Invoke(methodName, args);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnTestInvoked事件", e);
                }
            }).Start();
        }

        public int InvokeEvent(CQPluginProxy target, PluginEventType eventType, params object[] args)
        {
            if (eventType == PluginEventType.Menu || target.AppInfo._event.Any(x => x.type == (int)eventType))
            {
                var r = ServerManager.Server.InvokeEvents(target, eventType, args);
                return r ?? -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 向所有 Enabled 的插件按优先级发送事件，当某个插件返回 1 时阻塞后续调用
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="args">参数</param>
        /// <returns>阻塞的插件</returns>
        public CQPluginProxy InvokeEvent(PluginEventType eventType, params object[] args)
        {
            if (PluginLoaded is false)
            {
                LogHelper.WriteLog(LogLevel.Warning, "AMN框架", "插件逻辑处理", "插件加载中...", "x 不处理");
                return null;
            }
            foreach (var item in Proxies.Where(x => x.Enabled && x.AppInfo._event.Any(o => o.type == (int)eventType))
                .OrderByDescending(x => x.AppInfo._event.First().priority))
            {
                if (item.AppInfo.AuthCode == AppConfig.Instance.TestingAuthCode)
                {
                    continue;
                }
                int ret = InvokeEvent(item, eventType, args);
                if (ret == 1)
                {
                    return item;
                }
            }
            return null;
        }

        public bool LoadPlugins()
        {
            PluginLoaded = false;
            string pluginTmpPath = Path.Combine("data", "plugins", "tmp");
            if (Directory.Exists(pluginTmpPath))
            {
                Directory.Delete(pluginTmpPath, true);
            }
            Directory.CreateDirectory(pluginTmpPath);
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var item in Directory.GetFiles(@"data\plugins", "*.dll"))
            {
                if (File.Exists(item.Replace(".dll", ".json")))
                {
                    CQPluginProxy plugin = new(item);
                    if (plugin.MovePluginToTmpDir() && plugin.LoadAppInfo())
                    {
                        Proxies.Add(plugin);
                        plugin.OnPluginProcessExited += Plugin_OnPluginProcessExited;
                    }
                }
            };
            LogHelper.Info("加载插件", "加载完成，启用插件...", $"√ {sw.ElapsedMilliseconds} ms");
            return true;
        }

        public void ReloadAllPlugins()
        {
            try
            {
                PluginLoaded = false;
                // 结束所有插件进程
                foreach (var item in Proxies.Where(x => x.Enabled))
                {
                    item.ExitFlag = true;
                    item.KillProcess();
                }
                // 清空插件列表并重新加载
                Proxies.Clear();
                if (LoadPlugins())
                {
                    EnablePluginByConfig();
                    OnPluginLoaded();
                }
            }
            catch (Exception exc)
            {
                LogHelper.Error("ReloadAllPlugins", exc);
            }
        }

        public void OnPluginLoaded()
        {
            PluginLoaded = true;
        }

        public void EnablePluginByConfig()
        {
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var item in Proxies)
            {
                string appName = item.AppInfo.name;
                if (AppConfig.Instance.AutoEnablePlugin.Any(x => x == appName))
                {
                    item.Load();
                }
            };
            LogHelper.Info("启用插件", $"插件启用完成，共加载了 {Proxies.Where(x => x.HasConnection).Count()} 个插件，开始调用启动事件...", $"√ {sw.ElapsedMilliseconds} ms");
            sw = Stopwatch.StartNew();
            foreach (var item in Proxies.Where(x => x.HasConnection))
            {
                SetPluginEnabled(item, true);
            };
            LogHelper.Info("启用插件", "插件启动完成，开始处理消息逻辑", $"√ {sw.ElapsedMilliseconds} ms");
        }

        public void ReloadPlugin(CQPluginProxy plugin)
        {
            if (plugin.Enabled == false)
            {
                LogHelper.Error("插件重启", $"{plugin.AppInfo.name} 处于禁用状态，无法重启");
                return;
            }
            plugin.KillProcess();
            if (!(plugin.MovePluginToTmpDir() && plugin.LoadAppInfo()))
            {
                LogHelper.Error("插件重启", $"{plugin.AppInfo.name} 重启失败");
                return;
            }
            if (!AppConfig.Instance.RestartPluginIfDead)// 根据此配置不启用插件，插件由进程退出事件触发插件启用
            {
                if (SetPluginEnabled(plugin, true))
                {
                    LogHelper.Info("插件重启", $"{plugin.AppInfo.name} 重启成功");
                }
                else
                {
                    LogHelper.Error("插件重启", $"{plugin.AppInfo.name} 重启失败");
                }
            }
            else
            {
                RequestWaiter.Wait($"PluginEnabled_{plugin.AppInfo.name}", AppConfig.Instance.LoadTimeout, out _);
            }
        }

        public bool SetPluginEnabled(CQPluginProxy plugin, bool enabled)
        {
            if (plugin == null)
            {
                return false;
            }
            bool success = true;
            if (enabled)
            {
                if (plugin.Enabled)
                {
                    return true;
                }
                if (plugin.PluginProcess == null || plugin.PluginProcess.HasExited)
                {
                    success = plugin.Load();
                }
                success = success && InvokeEvent(plugin, PluginEventType.StartUp) == 0;
                success = success && (InvokeEvent(plugin, PluginEventType.Enable) == 0);
                RequestWaiter.TriggerByKey($"PluginEnabled_{plugin.AppInfo.name}");
            }
            else
            {
                if (!plugin.Enabled)
                {
                    return true;
                }
                success = InvokeEvent(plugin, PluginEventType.Disable) == 0;
                success = success && (InvokeEvent(plugin, PluginEventType.Exit) == 0);
                plugin.KillProcess();
                RequestWaiter.TriggerByKey($"PluginDisabled_{plugin.AppInfo.name}");
            }
            string logMessage = $"插件 {plugin.PluginName} {{0}}";
            if (success)
            {
                plugin.Enabled = enabled;
                OnPluginEnableChanged?.Invoke(plugin);
                logMessage = string.Format(logMessage, enabled ? "启用成功" : "停用成功");
            }
            else
            {
                logMessage = string.Format(logMessage, enabled ? "启用失败" : "停用失败");
            }
            if (enabled is false)
            {
                LogHelper.Info("改变插件状态", logMessage);
            }
            return success;
        }

        private void Plugin_OnPluginProcessExited(CQPluginProxy plugin)
        {
            if (plugin == null)
            {
                return;
            }
            plugin.Enabled = false;
            OnPluginEnableChanged?.Invoke(plugin);
            LogHelper.Info("插件进程监控", $"{plugin.PluginName} 进程不存在");
            RequestWaiter.ResetSignalByProcess(plugin.PluginProcess.Id);// 由于进程退出，中断所有由此进程等待的请求

            if (plugin.ExitFlag is false && AppConfig.Instance.RestartPluginIfDead)
            {
                if (SetPluginEnabled(plugin, true))
                {
                    LogHelper.Info("插件重启", $"{plugin.PluginName} 重启成功");
                }
                else
                {
                    LogHelper.Info("插件重启", $"{plugin.PluginName} 重启失败");
                }
            }
        }

        #region 测试事件调用

        // 以下是封装好参数的调用，协议请调用这个

        public int Event_OnAdminChange(CQPluginProxy target, int subType, long sendTime, long fromGroup, long beingOperateQQ)
        {
            return InvokeEvent(target, PluginEventType.AdminChange, subType, sendTime, fromGroup, beingOperateQQ);
        }

        public CQPluginProxy Event_OnAdminChange(int subType, long sendTime, long fromGroup, long beingOperateQQ)
        {
            new Thread(() =>
            {
                try
                {
                    OnAdminChanged?.Invoke(fromGroup, beingOperateQQ, subType == 1 ? QQGroupMemberType.Member : QQGroupMemberType.Manage);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnAdminChanged事件", e);
                }
            }).Start();

            return InvokeEvent(PluginEventType.AdminChange, subType, sendTime, fromGroup, beingOperateQQ);
        }

        public int Event_OnDisable(CQPluginProxy target)
        {
            return InvokeEvent(target, PluginEventType.Disable);
        }

        public CQPluginProxy Event_OnDisable()
        {
            return InvokeEvent(PluginEventType.Disable);
        }

        public int Event_OnDiscussMsg(CQPluginProxy target, int subType, int msgId, long fromNative, long fromQQ, string msg, int font)
        {
            return InvokeEvent(target, PluginEventType.DiscussMsg, subType, msgId, fromNative, fromQQ, msg, font);
        }

        public CQPluginProxy Event_OnDiscussMsg(int subType, int msgId, long fromNative, long fromQQ, string msg, int font)
        {
            return InvokeEvent(PluginEventType.DiscussMsg, subType, msgId, fromNative, fromQQ, msg, font);
        }

        public int Event_OnEnable(CQPluginProxy target)
        {
            return InvokeEvent(target, PluginEventType.Enable);
        }

        public CQPluginProxy Event_OnEnable()
        {
            return InvokeEvent(PluginEventType.Enable);
        }

        public int Event_OnExit(CQPluginProxy target)
        {
            return InvokeEvent(target, PluginEventType.Exit);
        }

        public CQPluginProxy Event_OnExit()
        {
            return InvokeEvent(PluginEventType.Exit);
        }

        public int Event_OnFriendAdded(CQPluginProxy target, int subType, long sendTime, long fromQQ)
        {
            return InvokeEvent(target, PluginEventType.FriendAdded, subType, sendTime, fromQQ);
        }

        public CQPluginProxy Event_OnFriendAdded(int subType, long sendTime, long fromQQ)
        {
            new Thread(() =>
            {
                try
                {
                    OnFriendAdded?.Invoke(fromQQ);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnFriendAdded事件", e);
                }
            }).Start();
            return InvokeEvent(PluginEventType.FriendAdded, subType, sendTime, fromQQ);
        }

        public int Event_OnFriendAddRequest(CQPluginProxy target, int subType, long sendTime, long fromQQ, string msg, string responseFlag)
        {
            return InvokeEvent(target, PluginEventType.FriendRequest, subType, sendTime, fromQQ, msg, responseFlag);
        }

        public CQPluginProxy Event_OnFriendAddRequest(int subType, long sendTime, long fromQQ, string msg, string responseFlag)
        {
            return InvokeEvent(PluginEventType.FriendRequest, subType, sendTime, fromQQ, msg, responseFlag);
        }

        public int Event_OnGroupAddRequest(CQPluginProxy target, int subType, long sendTime, long fromGroup, long fromQQ, string msg, string responseFlag)
        {
            return InvokeEvent(target, PluginEventType.GroupAddRequest, subType, sendTime, fromGroup, fromQQ, msg, responseFlag);
        }

        public CQPluginProxy Event_OnGroupAddRequest(int subType, long sendTime, long fromGroup, long fromQQ, string msg, string responseFlag)
        {
            return InvokeEvent(PluginEventType.GroupAddRequest, subType, sendTime, fromGroup, fromQQ, msg, responseFlag);
        }

        public int Event_OnGroupBan(CQPluginProxy target, int subType, long sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration)
        {
            return InvokeEvent(target, PluginEventType.GroupBan, subType, sendTime, fromGroup, fromQQ, beingOperateQQ, duration);
        }

        public CQPluginProxy Event_OnGroupBan(int subType, long sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration)
        {
            new Thread(() =>
            {
                try
                {
                    OnGroupBan?.Invoke(fromGroup, fromQQ, beingOperateQQ, duration);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnGroupBan事件", e);
                }
            }).Start();
            return InvokeEvent(PluginEventType.GroupBan, subType, sendTime, fromGroup, fromQQ, beingOperateQQ, duration);
        }

        public int Event_OnGroupMemberDecrease(CQPluginProxy target, int subType, long sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            return InvokeEvent(target, PluginEventType.GroupMemberDecrease, subType, sendTime, fromGroup, fromQQ, beingOperateQQ);
        }

        public CQPluginProxy Event_OnGroupMemberDecrease(int subType, long sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            new Thread(() =>
            {
                try
                {
                    OnGroupLeft?.Invoke(fromGroup, fromQQ);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnGroupLeft事件", e);
                }
            }).Start();
            return InvokeEvent(PluginEventType.GroupMemberDecrease, subType, sendTime, fromGroup, fromQQ, beingOperateQQ);
        }

        public int Event_OnGroupMemberIncrease(CQPluginProxy target, int subType, long sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            return InvokeEvent(target, PluginEventType.GroupMemberIncrease, subType, sendTime, fromGroup, fromQQ, beingOperateQQ);
        }

        public CQPluginProxy Event_OnGroupMemberIncrease(int subType, long sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            new Thread(() =>
            {
                try
                {
                    OnGroupAdded?.Invoke(fromGroup, fromQQ);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnGroupAdded事件", e);
                }
            }).Start();
            return InvokeEvent(PluginEventType.GroupMemberIncrease, subType, sendTime, fromGroup, fromQQ, beingOperateQQ);
        }

        public int Event_OnGroupMsg(CQPluginProxy target, int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font)
        {
            return InvokeEvent(target, PluginEventType.GroupMsg, subType, msgId, fromGroup, fromQQ, fromAnonymous, msg, font);
        }

        public CQPluginProxy Event_OnGroupMsg(int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font, DateTime time)
        {
            new Thread(() =>
            {
                try
                {
                    OnGroupMsg?.Invoke(msgId, fromGroup, fromQQ, msg, time);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnGroupMsg事件", e);
                }
            }).Start();
            return InvokeEvent(PluginEventType.GroupMsg, subType, msgId, fromGroup, fromQQ, fromAnonymous, msg, font);
        }

        public int Event_OnPrivateMsg(CQPluginProxy target, int subType, int msgId, long fromQQ, string msg, int font)
        {
            return InvokeEvent(target, PluginEventType.PrivateMsg, subType, msgId, fromQQ, msg, font);
        }

        public CQPluginProxy Event_OnPrivateMsg(int subType, int msgId, long fromQQ, string msg, int font, DateTime time)
        {
            new Thread(() =>
            {
                try
                {
                    OnPrivateMsg?.Invoke(msgId, fromQQ, msg, time);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnPrivateMsg事件", e);
                }
            }).Start();
            return InvokeEvent(PluginEventType.PrivateMsg, subType, msgId, fromQQ, msg, font);
        }

        public int Event_OnStartUp(CQPluginProxy target)
        {
            return InvokeEvent(target, PluginEventType.StartUp);
        }

        public CQPluginProxy Event_OnStartUp()
        {
            return InvokeEvent(PluginEventType.StartUp);
        }

        public int Event_OnUpload(CQPluginProxy target, int subType, long sendTime, long fromGroup, long fromQQ, string file)
        {
            return InvokeEvent(target, PluginEventType.Upload, subType, sendTime, fromGroup, fromQQ, file);
        }

        public CQPluginProxy Event_OnUpload(int subType, long sendTime, long fromGroup, long fromQQ, string file)
        {
            return InvokeEvent(PluginEventType.Upload, subType, sendTime, fromGroup, fromQQ, file);
        }

        public void Event_OnGroupMsgRecall(int msgId, long groupId, string msg)
        {
            new Thread(() =>
            {
                try
                {
                    OnGroupMsgRecall?.Invoke(msgId, groupId, msg);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnGroupMsgRecall事件", e);
                }
            }).Start();
        }

        public void Event_OnPrivateMsgRecall(int msgId, long qq, string msg)
        {
            new Thread(() =>
            {
                try
                {
                    OnPrivateMsgRecall?.Invoke(msgId, qq, msg);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnPrivateMsgRecall事件", e);
                }
            }).Start();
        }

        public void Event_OnGroupMemberCardChanged(long group, long qq, string card)
        {
            new Thread(() =>
            {
                try
                {
                    OnGroupMemberCardChanged?.Invoke(group, qq, card);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnGroupMemberCardChanged事件", e);
                }
            }).Start();
        }

        public void Event_OnFriendNickChanged(long qq, string nick)
        {
            new Thread(() =>
            {
                try
                {
                    OnFriendNickChanged?.Invoke(qq, nick);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnFriendNickChanged事件", e);
                }
            }).Start();
        }

        public void Event_OnGroupNameChanged(long group, string name)
        {
            new Thread(() =>
            {
                try
                {
                    OnGroupNameChanged?.Invoke(group, name);
                }
                catch (Exception e)
                {
                    LogHelper.Error("OnGroupNameChanged", e);
                }
            }).Start();
        }

        #endregion 测试事件调用
    }
}