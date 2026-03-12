using Another_Mirai_Native.Abstractions;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Handlers;
using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
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

        private IGroupAddRequestHandler? GroupAddRequestHandler { get; set; }

        private IGroupFileUploadHandler? GroupFileUploadHandler { get; set; }

        private IGroupMemberBannedHandler? GroupMemberBannedHandler { get; set; }

        private IGroupMemberDecreaseHandler? GroupMemberDecreaseHandler { get; set; }

        private IGroupMemberIncreaseHandler? GroupMemberIncreaseHandler { get; set; }

        private IGroupMemberUnbannedHandler? GroupMemberUnbannedHandler { get; set; }

        private IGroupMessageHandler? GroupMessageHandler { get; set; }

        private IGroupWholeBannedHandler? GroupWholeBannedHandler { get; set; }

        private IGroupWholeUnbannedHandler? GroupWholeUnbannedHandler { get; set; }

        private IMenuHandler? MenuHandler { get; set; }

        private IPrivateMessageHandle? PrivateMessageHandle { get; set; }

        private CancellationTokenSource CancellationTokenSource { get; set; }

        private IPluginApi PluginApi { get; set; }

        public override bool LoadPlugin()
        {
            try
            {
                var assembly = Assembly.Load(File.ReadAllBytes(PluginPath));
                var pluginType = assembly.GetTypes()
                    .FirstOrDefault(t => t.IsSubclassOf(typeof(PluginBase)));
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
                author = PluginInfo.Author ?? string.Empty,
                version = PluginInfo.Version,
                name = PluginInfo.Name,
                description = PluginInfo.Description ?? string.Empty,
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

                    case PluginEventType.StartUp:
                        CancellationTokenSource = new();
                        await Plugin.OnEnableAsync(CancellationTokenSource.Token);
                        return 0;

                    case PluginEventType.Exit:
                        await Plugin.OnDisableAsync(CancellationTokenSource.Token);
                        CancellationTokenSource.Cancel();
                        return 0;
                }

                return 0;
            }).Result;

            return r;
        }

        public override int CallMenu(string menu)
        {
            if (MenuHandler == null)
            {
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
                    MenuHandler.OnMenu(menu);
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
            var type = Plugin.GetType();

            AdminChangeHandler = typeof(IAdminChangeHandler).IsAssignableFrom(type) ? (IAdminChangeHandler)Plugin : null;
            FriendAddedHandler = typeof(IFriendAddedHandler).IsAssignableFrom(type) ? (IFriendAddedHandler)Plugin : null;
            FriendAddRequestHandler = typeof(IFriendAddRequestHandler).IsAssignableFrom(type) ? (IFriendAddRequestHandler)Plugin : null;
            GroupAddRequestHandler = typeof(IGroupAddRequestHandler).IsAssignableFrom(type) ? (IGroupAddRequestHandler)Plugin : null;
            GroupFileUploadHandler = typeof(IGroupFileUploadHandler).IsAssignableFrom(type) ? (IGroupFileUploadHandler)Plugin : null;
            GroupMemberBannedHandler = typeof(IGroupMemberBannedHandler).IsAssignableFrom(type) ? (IGroupMemberBannedHandler)Plugin : null;
            GroupMemberDecreaseHandler = typeof(IGroupMemberDecreaseHandler).IsAssignableFrom(type) ? (IGroupMemberDecreaseHandler)Plugin : null;
            GroupMemberIncreaseHandler = typeof(IGroupMemberIncreaseHandler).IsAssignableFrom(type) ? (IGroupMemberIncreaseHandler)Plugin : null;
            GroupMemberUnbannedHandler = typeof(IGroupMemberUnbannedHandler).IsAssignableFrom(type) ? (IGroupMemberUnbannedHandler)Plugin : null;
            GroupMessageHandler = typeof(IGroupMessageHandler).IsAssignableFrom(type) ? (IGroupMessageHandler)Plugin : null;
            GroupWholeBannedHandler = typeof(IGroupWholeBannedHandler).IsAssignableFrom(type) ? (IGroupWholeBannedHandler)Plugin : null;
            GroupWholeUnbannedHandler = typeof(IGroupWholeUnbannedHandler).IsAssignableFrom(type) ? (IGroupWholeUnbannedHandler)Plugin : null;
            MenuHandler = typeof(IMenuHandler).IsAssignableFrom(type) ? (IMenuHandler)Plugin : null;
            PrivateMessageHandle = typeof(IPrivateMessageHandle).IsAssignableFrom(type) ? (IPrivateMessageHandle)Plugin : null;

            if (MenuHandler != null)
            {
                CreateUIThread();
            }
            PluginApi = new API(PluginInfo);

            return true;
        }

        private Task<EventHandleResult> CallGroupAddRequestEvent(object[] args)
        {
            if (GroupAddRequestHandler == null)
            {
                return Task.FromResult(EventHandleResult.Pass);
            }
            // int subType, int sendTime, long fromGroup, long fromQQ, string msg, string responseFlag
            if (args.Length != 6)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupAddRequestAsync; 参数数量不匹配，期望 6 个但实际 {args.Length} 个");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (args[1] is not int sendTime
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[4] is not string msg
                || args[5] is not string responseFlag)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupAddRequestAsync; 参数类型不匹配，期望 (int, long, long, string, string) 但实际 ({args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()}, {args[5].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            GroupAddRequestContext e = new(dateTime, new(PluginApi, fromGroup), new(PluginApi, fromQQ), msg, responseFlag);
            return GroupAddRequestHandler.OnGroupAddRequestAsync(e, CancellationTokenSource.Token);
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
            if (args[1] is not int sendTime
                || args[2] is not long fromQQ
                || args[3] is not string msg
                || args[4] is not string responseFlag)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnFriendAddRequestAsync; 参数类型不匹配，期望 (int, long, string, string) 但实际 ({args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            FriendAddRequestContext context = new(dateTime, new QQ(PluginApi, fromQQ), msg, responseFlag);
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
            if (args[1] is not int sendTime
                || args[2] is not long fromQQ)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnFriendAddedAsync; 参数类型不匹配，期望 (int, long) 但实际 ({args[1].GetType()}, {args[2].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            FriendAddedContext context = new(dateTime, new QQ(PluginApi, fromQQ));
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
            if (args[0] is not int subType
                || args[1] is not int sendTime
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[4] is not long beingOperateQQ
                || args[5] is not long duration)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupBan; 参数类型不匹配，期望 (int, int, long, long, long, long) 但实际 ({args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()}, {args[5].GetType()})");
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
                    GroupWholeUnbannedContext context = new(dateTime, group, operatorQQ);
                    return GroupWholeUnbannedHandler.OnGroupWholeUnbannedAsync(context, CancellationTokenSource.Token);
                }
                if (subType == 2)
                {
                    if (GroupWholeBannedHandler == null)
                    {
                        return Task.FromResult(EventHandleResult.Pass);
                    }
                    GroupWholeBannedContext context = new(dateTime, group, operatorQQ);
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
                GroupMemberUnbannedContext context = new(dateTime, group, operatorQQ, targetQQ);
                return GroupMemberUnbannedHandler.OnGroupMemberUnbannedAsync(context, CancellationTokenSource.Token);
            }
            if (subType == 2)
            {
                if (GroupMemberBannedHandler == null)
                {
                    return Task.FromResult(EventHandleResult.Pass);
                }
                GroupMemberBannedContext context = new(dateTime, group, operatorQQ, targetQQ, TimeSpan.FromSeconds(duration));
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
            if (args[0] is not int subType
                || args[1] is not int sendTime
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[4] is not long beingOperateQQ)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupMemberIncreaseAsync; 参数类型不匹配，期望 (int, int, long, long, long) 但实际 ({args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (subType is not 1 and not 2)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupMemberIncreaseAsync; 子类型无效，期望 1(主动入群) 或 2(邀请入群) 但实际 {subType}");
                return Task.FromResult(EventHandleResult.Pass);
            }

            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            GroupMemberIncreaseContext context = new(
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
            if (args[0] is not int subType
                || args[1] is not int sendTime
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[4] is not long beingOperateQQ)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupMemberDecreaseAsync; 参数类型不匹配，期望 (int, int, long, long, long) 但实际 ({args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (subType is not 1 and not 2)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupMemberDecreaseAsync; 子类型无效，期望 1(主动退出) 或 2(被踢出) 但实际 {subType}");
                return Task.FromResult(EventHandleResult.Pass);
            }

            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            GroupMemberDecreaseContext context = new(
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
            if (args[0] is not int subType
                || args[1] is not int sendTime
                || args[2] is not long fromGroup
                || args[3] is not long beingOperateQQ)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnAdminChangedAsync; 参数类型不匹配，期望 (int, int, long, long) 但实际 ({args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            if (!Enum.IsDefined(typeof(AdminChangedType), subType))
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnAdminChangedAsync; 子类型无效，期望 1(取消管理员) 或 2(设置管理员) 但实际 {subType}");
                return Task.FromResult(EventHandleResult.Pass);
            }

            DateTime dateTime = Helper.TimeStamp2DateTime(sendTime);
            AdminChangedContext context = new(
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
            if (args[1] is not int sendTime
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[4] is not string file)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnGroupFileUploadedAsync; 参数类型不匹配，期望 (int, long, long, string) 但实际 ({args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[4].GetType()})");
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
            if (args[1] is not int msgId
                || args[2] is not long fromGroup
                || args[3] is not long fromQQ
                || args[5] is not string msg)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnReceiveGroupMessageAsync; 参数类型不匹配，期望 (int, long, long, string) 但实际 ({args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()}, {args[5].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }

            GroupMessageContext context = new(
                new(PluginApi, fromGroup),
                new(PluginApi, fromQQ),
                new(PluginApi, msgId, msg));
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
            if (args[1] is not int msgId || args[2] is not long fromQQ || args[3] is not string msg)
            {
                LogHelper.Error("调用 C# 插件事件", $"事件：OnReceivePrivateMessageAsync; 参数类型不匹配，期望 (int, long, string) 但实际 ({args[1].GetType()}, {args[2].GetType()}, {args[3].GetType()})");
                return Task.FromResult(EventHandleResult.Pass);
            }
            PrivateMessageContext e = new(new(PluginApi, fromQQ), new(PluginApi, msgId, msg));
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
    }
}
