using Another_Mirai_Native.Abstractions;
using Another_Mirai_Native.Abstractions.Attributes;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Handlers;
using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using System.Reflection;

namespace Another_Mirai_Native.Native.Handler.CSharp
{
    public class Loader(string path)
        : PluginHandlerBase(path)
    {
        private PluginBase Plugin { get; set; }

        private PluginInfo PluginInfo { get; set; }

        private IAdminChangeHandler? AdminChangeHandler { get; set; }

        private IFriendAddedHandler? FriendAddedHandler { get; set; }

        private IFriendAddRequestHandler? FriendAddRequestHandler { get; set; }

        private IGroupInviteRequestHandler? GroupInviteRequestHandler { get; set; }

        private IGroupAddRequestHandler? GroupAddRequestHandler { get; set; }

        private IGroupFileUploadHandler? GroupFileUploadHandler { get; set; }

        private IGroupMemberBannedHandler? GroupMemberBannedHandler { get; set; }

        private IGroupMemberDecreaseHandler? GroupMemberDecreaseHandler { get; set; }

        private IGroupMemberIncreaseHandler? GroupMemberIncreaseHandler { get; set; }

        private IGroupMemberUnbannedHandler? GroupMemberUnbannedHandler { get; set; }

        private IGroupMessageHandler? GroupMessageHandler { get; set; }

        private IGroupWholeBannedHandler? GroupWholeBannedHandler { get; set; }

        private IGroupWholeUnbannedHandler? GroupWholeUnbannedHandler { get; set; }

        private IMenuHandler[] MenuHandlers { get; set; }

        private IPrivateMessageHandler? PrivateMessageHandle { get; set; }

        private CancellationTokenSource CancellationTokenSource { get; set; }

        private IPluginApi PluginApi { get; set; }

        private Type[] AssemblyTypes { get; set; } = [];

        public override bool LoadPlugin()
        {
            try
            {
                var assembly = Assembly.Load(File.ReadAllBytes(PluginPath));
                AssemblyTypes = assembly.GetTypes();
                var pluginType = AssemblyTypes.FirstOrDefault(t => t.IsSubclassOf(typeof(PluginBase)));
                if (pluginType == null)
                {
                    LogHelper.Error("加载 C# 插件", $"无法从 {PluginPath} 中找到插件类型");
                    return false;
                }
                PluginBase? plugin = (PluginBase?)Activator.CreateInstance(pluginType);
                if (plugin == null)
                {
                    LogHelper.Error("加载 C# 插件", $"实例化插件类型失败");
                    return false;
                }
                Plugin = plugin;
                if (LoadAppInfo() && CreateMethodDelegates())
                {
                    AppInfo = ParseNativePluginInfo();
                    PluginName = AppInfo.name;
                    LogHelper.Info("加载 C# 插件", $"{PluginName}, 加载成功");
                    return true;
                }
                else
                {
                    LogHelper.Error("加载 C# 插件", $"加载插件信息或创建插件失败");
                    return false;
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("加载 C# 插件", $"加载过程发生异常：{e}");
                return false;
            }
        }

        private AppInfo ParseNativePluginInfo()
        {
            return new AppInfo
            {
                AppId = PluginInfo.AppId,
                author = PluginInfo.Author ?? "未提供",
                version = PluginInfo.Version,
                name = PluginInfo.Name,
                description = PluginInfo.Description ?? "未提供",
            };
        }

        public override bool LoadAppInfo()
        {
            try
            {
                // 尝试从插件实例中获取PluginInfo特性
                var attribute = Plugin.GetType().GetCustomAttribute<PluginInfo>();
                if (attribute != null)
                {
                    PluginInfo = attribute;
                    if (string.IsNullOrEmpty(PluginInfo.AppId)
                        || string.IsNullOrEmpty(PluginInfo.Name)
                        || string.IsNullOrEmpty(PluginInfo.Version))
                    {
                        LogHelper.Error("加载 C# 插件", $"获取插件元数据失败，由于插件元数据的 AppId 或者插件名称或者版本号为空");
                        return false;
                    }
                    attribute.AuthCode = AppConfig.Instance.Core_AuthCode;
                    return true;
                }
                // 如果没有获取到特性则尝试使用插件基类的PluginInfo属性
                PluginInfo = Plugin.PluginInfo;
                if (PluginInfo == null)
                {
                    LogHelper.Error("加载 C# 插件", $"获取插件元数据失败，由于插件未定义元数据");
                    return false;
                }
                if (string.IsNullOrEmpty(PluginInfo.AppId)
                     || string.IsNullOrEmpty(PluginInfo.Name)
                     || string.IsNullOrEmpty(PluginInfo.Version))
                {
                    LogHelper.Error("加载 C# 插件", $"获取插件元数据失败，由于插件元数据的 AppId 或者插件名称或者版本号为空");
                    return false;
                }

                PluginInfo.AuthCode = AppConfig.Instance.Core_AuthCode;
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("加载 C# 插件", $"获取插件元数据时发生异常：{e}");
                return false;
            }
        }

        public override int CallEvent(PluginEventType eventName, object[] args)
        {
            int r = Task.Run(async () =>
            {
                switch (eventName)
                {
                    case PluginEventType.PrivateMsg:
                        return (int)await CallPrivateMsgEvent(args);

                    case PluginEventType.GroupMsg:
                        return (int)await CallGroupMsgEvent(args);

                    case PluginEventType.Upload:
                        return (int)await CallUploadEvent(args);

                    case PluginEventType.AdminChange:
                        return (int)await CallAdminChangeEvent(args);

                    case PluginEventType.GroupMemberDecrease:
                        return (int)await CallGroupMemberDecreaseEvent(args);

                    case PluginEventType.GroupMemberIncrease:
                        return (int)await CallGroupMemberIncreaseEvent(args);

                    case PluginEventType.GroupBan:
                        return (int)await CallGroupBanEvent(args);

                    case PluginEventType.FriendAdded:
                        return (int)await CallFriendAddedEvent(args);

                    case PluginEventType.FriendRequest:
                        return (int)await CallFriendRequestEvent(args);

                    case PluginEventType.GroupAddRequest:
                        return (int)await CallGroupAddRequestEvent(args);

                    case PluginEventType.Enable:
                        CancellationTokenSource = new();
                        await Plugin.OnEnableAsync(CancellationTokenSource.Token);
                        return 0;

                    case PluginEventType.Disable:
                        await Plugin.OnDisableAsync(CancellationTokenSource.Token);
                        CancellationTokenSource.Cancel();
                        return 0;

                    case PluginEventType.Menu:
                        return CallMenu(args.FirstOrDefault()?.ToString());
                }

                return 0;
            }).Result;

            return r;
        }

        public override int CallMenu(string? menu)
        {
            if (MenuHandlers == null)
            {
                return -1;
            }
            if (string.IsNullOrEmpty(menu))
            {
                LogHelper.Error("调用Menu事件", $"传递的菜单名称无效");
                return -1;
            }
            if (UIForm == null)
            {
                LogHelper.Error("调用Menu事件", $"UI线程未创建，无法调用Menu事件");
                return -1;
            }
            try
            {
                UIForm.BeginInvoke(() =>
                {
                    // 寻找菜单名称匹配的处理器并调用
                    var t = MenuHandlers.FirstOrDefault(m => m.GetType().GetCustomAttribute<MenuAttribute>()?.Name == menu);
                    t?.OnMenu(new(PluginApi));
                });
                return 1;
            }
            catch (Exception e)
            {
                LogHelper.Error("调用 C# 插件 Menu 事件", $"调用菜单 {menu} 时发生异常：{e}");
                return 0;
            }
        }

        public override bool CreateMethodDelegates()
        {
            AdminChangeHandler = FindEventHandler<IAdminChangeHandler>();
            FriendAddedHandler = FindEventHandler<IFriendAddedHandler>();
            FriendAddRequestHandler = FindEventHandler<IFriendAddRequestHandler>();
            GroupInviteRequestHandler = FindEventHandler<IGroupInviteRequestHandler>();
            GroupAddRequestHandler = FindEventHandler<IGroupAddRequestHandler>();
            GroupFileUploadHandler = FindEventHandler<IGroupFileUploadHandler>();
            GroupMemberBannedHandler = FindEventHandler<IGroupMemberBannedHandler>();
            GroupMemberDecreaseHandler = FindEventHandler<IGroupMemberDecreaseHandler>();
            GroupMemberIncreaseHandler = FindEventHandler<IGroupMemberIncreaseHandler>();
            GroupMemberUnbannedHandler = FindEventHandler<IGroupMemberUnbannedHandler>();
            GroupMessageHandler = FindEventHandler<IGroupMessageHandler>();
            GroupWholeBannedHandler = FindEventHandler<IGroupWholeBannedHandler>();
            GroupWholeUnbannedHandler = FindEventHandler<IGroupWholeUnbannedHandler>();
            MenuHandlers = FindEventHandlers<IMenuHandler>();
            PrivateMessageHandle = FindEventHandler<IPrivateMessageHandler>();

            if (MenuHandlers.Length > 0)
            {
                CreateUIThread();
            }
            PluginApi = new API(PluginInfo);
            Plugin.API = PluginApi;

            return true;
        }

        /// <summary>
        /// 搜索指定事件处理器接口类型的具体实现，并创建其实例。
        /// </summary>
        /// <remarks>如果找到接口的多个实现，则使用最先发现的那个。
        /// 此方法使用实现类型的默认构造函数创建新实例。</remarks>
        /// <typeparam name="T">要定位的事件处理器的接口类型。必须是引用类型接口。</typeparam>
        /// <returns>实现了指定接口的类型的实例，如果未找到实现则返回 null。</returns>
        /// <exception cref="ArgumentException">如果指定的类型参数不是接口类型，则抛出此异常。</exception>
        private T? FindEventHandler<T>() where T : class
        {
            Type targetType = typeof(T);
            if (!targetType.IsInterface)
            {
                throw new ArgumentException($"{targetType.FullName} 不是一个接口类型");
            }
            if (Plugin is T pluginHandler)
            {
                return pluginHandler;
            }
            var assignableTypes = AssemblyTypes.Where(t => targetType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract).ToList();
            if (assignableTypes.Count == 0)
            {
                return null;
            }
            if (assignableTypes.Count > 1)
            {
                LogHelper.Warning("加载 C# 插件", $"找到多个实现 {targetType.FullName} 的类型，默认使用第一个找到的类型 {assignableTypes[0].FullName}");
            }
            return (T?)Activator.CreateInstance(assignableTypes[0]);
        }

        /// <summary>
        /// 查找并实例化实现指定事件处理接口的所有类型的实例。
        /// </summary>
        /// <remarks>返回的每个实例均为通过无参数构造函数创建的新对象。仅查找非抽象、非接口类型。</remarks>
        /// <typeparam name="T">要查找的事件处理接口类型。必须为接口类型。</typeparam>
        /// <returns>一个包含所有实现指定接口的事件处理程序实例的数组。如果未找到任何实现，则返回空数组。</returns>
        /// <exception cref="ArgumentException">当类型参数 T 不是接口类型时抛出。</exception>
        private T[] FindEventHandlers<T>() where T : class
        {
            Type targetType = typeof(T);
            if (!targetType.IsInterface)
            {
                throw new ArgumentException($"{targetType.FullName} 不是一个接口类型");
            }
            var assignableTypes = AssemblyTypes.Where(t => targetType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract).ToList();
            List<T> r = [];
            if (Plugin is T pluginHandler)
            {
                r.Add(pluginHandler);
            }
            foreach (var item in assignableTypes)
            {
                if (item == Plugin.GetType())
                {
                    continue;
                }
                var t = Activator.CreateInstance(item);
                if (t != null)
                {
                    r.Add((T)t);
                }
            }
            return r.ToArray();
        }

        #region 事件分发
        private Task<EventHandleResult> CallGroupAddRequestEvent(object[] args)
        {
            // int subType, int sendTime, long fromGroup, long fromQQ, string msg, string responseFlag
            if (args.Length != 6)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupAddRequestAsync; 参数数量不匹配，期望 6 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[0] is not long subType
                ||args[1] is not long sendTime
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[4] is not string msg
                || args[5] is not string responseFlag)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupAddRequestAsync; 参数类型不匹配，期望 (long, long, long, long, string, string) 但实际 ({args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()}, {args[5].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            if (subType == 1)
            {
                if (GroupAddRequestHandler == null)
                {
                    return Task.FromResult(EventHandleResult.Pass);
                }
                GroupAddRequestContext e = new(PluginApi, dateTime, new(PluginApi, fromGroup), new(PluginApi, fromQQ), msg, responseFlag);
                return GroupAddRequestHandler.OnGroupAddRequestAsync(e, CancellationTokenSource.Token);
            }
            else
            {
                if (GroupInviteRequestHandler == null)
                {
                    return Task.FromResult(EventHandleResult.Pass);
                }
                GroupInviteRequestContext e = new(PluginApi, dateTime, new(PluginApi, fromGroup), new(PluginApi, fromQQ), msg, responseFlag);
                return GroupInviteRequestHandler.OnGroupInviteRequestAsync(e, CancellationTokenSource.Token);
            }
        }

        private Task<EventHandleResult> CallFriendRequestEvent(object[] args)
        {
            // int subType, int sendTime, long fromQQ, string msg, string responseFlag
            if (FriendAddRequestHandler == null)
            {
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args.Length != 5)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnFriendAddRequestAsync; 参数数量不匹配，期望 5 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[1] is not long sendTime
                || args[2] is not long fromQQ
                || args[3] is not string msg
                || args[4] is not string responseFlag)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnFriendAddRequestAsync; 参数类型不匹配，期望 (long, long, string, string) 但实际 ({args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            FriendAddRequestContext context = new(PluginApi, dateTime, new QQ(PluginApi, fromQQ), msg, responseFlag);
            return FriendAddRequestHandler.OnFriendAddRequestAsync(context, CancellationTokenSource.Token);
        }

        private Task<EventHandleResult> CallFriendAddedEvent(object[] args)
        {
            // int subType, int sendTime, long fromQQ
            if (FriendAddedHandler == null)
            {
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args.Length != 3)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnFriendAddedAsync; 参数数量不匹配，期望 3 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[1] is not long sendTime
                || args[2] is not long fromQQ)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnFriendAddedAsync; 参数类型不匹配，期望 (long, long) 但实际 ({args[1].GetType()}, {args[2].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            FriendAddedContext context = new(PluginApi, dateTime, new QQ(PluginApi, fromQQ));
            return FriendAddedHandler.OnFriendAddedAsync(context, CancellationTokenSource.Token);
        }

        private Task<EventHandleResult> CallGroupBanEvent(object[] args)
        {
            // int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration
            if (args.Length != 6)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupBan; 参数数量不匹配，期望 6 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[0] is not long subType
                || args[1] is not long sendTime
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[4] is not long beingOperateQQ
                || args[5] is not long duration)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupBan; 参数类型不匹配，期望 (long, long, long, long, long, long) 但实际 ({args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()}, {args[5].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }

            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            Group group = new(PluginApi, fromGroup);
            QQ operatorQQ = new(PluginApi, fromQQ);

            if (beingOperateQQ == 0)
            {
                if (subType == 1)
                {
                    if (GroupWholeUnbannedHandler == null)
                    {
                        return Task.FromResult(EventHandleResult.Pass);
                    }
                    GroupWholeUnbannedContext context = new(PluginApi, dateTime, group, operatorQQ);
                    return GroupWholeUnbannedHandler.OnGroupWholeUnbannedAsync(context, CancellationTokenSource.Token);
                }
                if (subType == 2)
                {
                    if (GroupWholeBannedHandler == null)
                    {
                        return Task.FromResult(EventHandleResult.Pass);
                    }
                    GroupWholeBannedContext context = new(PluginApi, dateTime, group, operatorQQ);
                    return GroupWholeBannedHandler.OnGroupWholeBannedAsync(context, CancellationTokenSource.Token);
                }

                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupBan; 子类型无效，期望 1(解除禁言) 或 2(禁言) 但实际 {subType}");
                return Task.FromResult(EventHandleResult.Pass);
            }

            QQ targetQQ = new(PluginApi, beingOperateQQ);
            if (subType == 1)
            {
                if (GroupMemberUnbannedHandler == null)
                {
                    return Task.FromResult(EventHandleResult.Pass);
                }
                GroupMemberUnbannedContext context = new(PluginApi, dateTime, group, operatorQQ, targetQQ);
                return GroupMemberUnbannedHandler.OnGroupMemberUnbannedAsync(context, CancellationTokenSource.Token);
            }
            if (subType == 2)
            {
                if (GroupMemberBannedHandler == null)
                {
                    return Task.FromResult(EventHandleResult.Pass);
                }
                GroupMemberBannedContext context = new(PluginApi, dateTime, group, operatorQQ, targetQQ, TimeSpan.FromSeconds(duration));
                return GroupMemberBannedHandler.OnGroupMemberBannedAsync(context, CancellationTokenSource.Token);
            }

            LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupBan; 子类型无效，期望 1(解除禁言) 或 2(禁言) 但实际 {subType}");
            return Task.FromResult(EventHandleResult.Pass);
        }

        private Task<EventHandleResult> CallGroupMemberIncreaseEvent(object[] args)
        {
            // int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ
            if (GroupMemberIncreaseHandler == null)
            {
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args.Length != 5)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupMemberIncreaseAsync; 参数数量不匹配，期望 5 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[0] is not long subType
                || args[1] is not long sendTime
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[4] is not long beingOperateQQ)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupMemberIncreaseAsync; 参数类型不匹配，期望 (long, long, long, long, long) 但实际 ({args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (subType is not 1 and not 2)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupMemberIncreaseAsync; 子类型无效，期望 1(主动入群) 或 2(邀请入群) 但实际 {subType}");
                return Task.FromResult(EventHandleResult.Pass);
            }

            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            GroupMemberIncreaseContext context = new(
                PluginApi,
                subType == 2,
                dateTime,
                new(PluginApi, fromGroup),
                new(PluginApi, fromQQ),
                new(PluginApi, beingOperateQQ));
            return GroupMemberIncreaseHandler.OnGroupMemberIncreaseAsync(context, CancellationTokenSource.Token);
        }

        private Task<EventHandleResult> CallGroupMemberDecreaseEvent(object[] args)
        {
            // int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ
            if (GroupMemberDecreaseHandler == null)
            {
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args.Length != 5)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupMemberDecreaseAsync; 参数数量不匹配，期望 5 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[0] is not long subType
                || args[1] is not long sendTime
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[4] is not long beingOperateQQ)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupMemberDecreaseAsync; 参数类型不匹配，期望 (long, long, long, long, long) 但实际 ({args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (subType is not 1 and not 2)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupMemberDecreaseAsync; 子类型无效，期望 1(主动退出) 或 2(被踢出) 但实际 {subType}");
                return Task.FromResult(EventHandleResult.Pass);
            }

            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            GroupMemberDecreaseContext context = new(
                PluginApi,
                subType == 2,
                dateTime,
                new(PluginApi, fromGroup),
                new(PluginApi, fromQQ),
                new(PluginApi, beingOperateQQ));
            return GroupMemberDecreaseHandler.OnGroupMemberDecreaseAsync(context, CancellationTokenSource.Token);
        }

        private Task<EventHandleResult> CallAdminChangeEvent(object[] args)
        {
            // int subType, int sendTime, long fromGroup, long beingOperateQQ
            if (AdminChangeHandler == null)
            {
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args.Length != 4)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnAdminChangedAsync; 参数数量不匹配，期望 4 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[0] is not long subType
                || args[1] is not long sendTime
                || args[2] is not long fromGroup
                || args[3] is not long beingOperateQQ)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnAdminChangedAsync; 参数类型不匹配，期望 (long, long, long, long) 但实际 ({args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if ((subType is not 1) && (subType is not 2))
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnAdminChangedAsync; 子类型无效，期望 1(取消管理员) 或 2(设置管理员) 但实际 {subType}");
                return Task.FromResult(EventHandleResult.Pass);
            }

            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            AdminChangedContext context = new(
                PluginApi,
                (AdminChangedType)subType,
                dateTime,
                new(PluginApi, fromGroup),
                new(PluginApi, beingOperateQQ));
            return AdminChangeHandler.OnAdminChangedAsync(context, CancellationTokenSource.Token);
        }

        private Task<EventHandleResult> CallUploadEvent(object[] args)
        {
            // int subType, int sendTime, long fromGroup, long fromQQ, string file
            if (GroupFileUploadHandler == null)
            {
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args.Length != 5)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupFileUploadedAsync; 参数数量不匹配，期望 5 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[1] is not long sendTime
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[4] is not string file)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupFileUploadedAsync; 参数类型不匹配，期望 (long, long, long, string) 但实际 ({args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }

            GroupFileInfo fileInfo;
            try
            {
                fileInfo = ParseGroupFileInfo(file);
            }
            catch (FormatException e)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupFileUploadedAsync; 文件信息 Base64 解码失败：{e}");
                return Task.FromResult(EventHandleResult.Pass);
            }
            catch (EndOfStreamException e)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupFileUploadedAsync; 文件信息二进制解析失败：{e}");
                return Task.FromResult(EventHandleResult.Pass);
            }

            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            GroupFileUploadedContext context = new(
                PluginApi,
                dateTime,
                new(PluginApi, fromGroup),
                new(PluginApi, fromQQ),
                fileInfo);
            return GroupFileUploadHandler.OnGroupFileUploadedAsync(context, CancellationTokenSource.Token);
        }

        private Task<EventHandleResult> CallGroupMsgEvent(object[] args)
        {
            // int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font
            if (GroupMessageHandler == null)
            {
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args.Length != 7)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnReceiveGroupMessageAsync; 参数数量不匹配，期望 7 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[1] is not long msgId
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[5] is not string msg)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnReceiveGroupMessageAsync; 参数类型不匹配，期望 (long, long, long, string) 但实际 ({args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[5].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }

            GroupMessageContext context = new(
                PluginApi,
                new(PluginApi, fromGroup),
                new(PluginApi, fromQQ),
                new(PluginApi, (int)msgId, msg));
            return GroupMessageHandler.OnReceiveGroupMessageAsync(context, CancellationTokenSource.Token);
        }

        private Task<EventHandleResult> CallPrivateMsgEvent(object[] args)
        {
            if (PrivateMessageHandle == null)
            {
                return Task.FromResult(EventHandleResult.Pass);
            }
            // int subType, int msgId, long fromQQ, string msg, int font
            if (args.Length != 5)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnReceivePrivateMessageAsync; 参数数量不匹配，期望 5 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[1] is not long msgId || args[2] is not long fromQQ || args[3] is not string msg)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnReceivePrivateMessageAsync; 参数类型不匹配，期望 (long, long, string) 但实际 ({args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            PrivateMessageContext e = new(PluginApi, new(PluginApi, fromQQ), new(PluginApi, (int)msgId, msg));
            return PrivateMessageHandle.OnReceivePrivateMessageAsync(e, CancellationTokenSource.Token);
        }

        private static GroupFileInfo ParseGroupFileInfo(string file)
        {
            byte[] binary = Convert.FromBase64String(file);
            using MemoryStream stream = new(binary);
            using BinaryReader reader = new(stream);
            string fileId = reader.ReadString_Ex();
            string fileName = reader.ReadString_Ex();
            long fileSize = reader.ReadInt64_Ex();
            int busId = reader.ReadInt32_Ex();
            return new GroupFileInfo(busId, fileName, fileId, fileSize);
        }
        #endregion
    }
}
