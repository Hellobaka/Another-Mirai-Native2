using Another_Mirai_Native.Abstractions;
using Another_Mirai_Native.Abstractions.Handlers;
using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.DB;
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
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("加载 C# 插件", $"加载过程发生异常：{e}");
                return false;
            }
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
                    return true;
                }
                // 如果没有获取到特性则尝试使用插件基类的PluginInfo属性
                PluginInfo = Plugin.PluginInfo;
                if (PluginInfo == null)
                {
                    LogHelper.Error("加载 C# 插件", $"获取插件元数据失败，由于插件未定义元数据");
                }
                return PluginInfo != null;
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
                        break;
                    case PluginEventType.GroupMsg:
                        break;
                    case PluginEventType.DiscussMsg:
                        break;
                    case PluginEventType.Upload:
                        break;
                    case PluginEventType.AdminChange:
                        break;
                    case PluginEventType.GroupMemberDecrease:
                        break;
                    case PluginEventType.GroupMemberIncrease:
                        break;
                    case PluginEventType.GroupBan:
                        break;
                    case PluginEventType.FriendAdded:
                        break;
                    case PluginEventType.FriendRequest:
                        break;
                    case PluginEventType.GroupAddRequest:
                        break;

                    case PluginEventType.StartUp:
                        CancellationTokenSource = new();
                        await Plugin.OnEnableAsync(CancellationTokenSource.Token);
                        return 1;

                    case PluginEventType.Exit:
                        await Plugin.OnDisableAsync(CancellationTokenSource.Token);
                        CancellationTokenSource.Cancel();
                        return 1;
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
            return true;
        }
    }
}
