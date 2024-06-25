using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Newtonsoft.Json;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Native.Handler
{
    public class CQPlugin : PluginHandlerBase
    {
        public CQPlugin(string path)
            : base(path)
        {
        }

        private Array EventArray { get; set; }

        public override int CallMenu(string menuName)
        {
            var function = AppInfo?.menu.FirstOrDefault(x => x.function == menuName)?.function;
            if (function == null)
            {
                LogHelper.Error("调用Menu事件", $"在AppInfo中未找到 {menuName}");
                return -1;
            }
            var menuMethod = CreateDelegateFromUnmanaged(function, typeof(Type_Menu));
            if (menuMethod == null)
            {
                LogHelper.Error("调用Menu事件", $"{function} 创建方法失败");
                return -1;
            }
            if (UIForm == null)
            {
                LogHelper.Error("调用Menu事件", $"UI线程未创建，无法调用Menu事件");
                return -1;
            }
            UIForm.BeginInvoke(() =>
            {
                menuMethod.DynamicInvoke(null);
            });
            return 1;
        }

        public override bool CreateMethodDelegates()
        {
            try
            {
                if (AppInfo == null || AppInfo._event == null)
                {
                    LogHelper.Error("创建方法委托", $"无法从Json文件解析出事件委托");
                    return false;
                }
                Initialize = (Type_Initialize?)CreateDelegateFromUnmanaged("Initialize", typeof(Type_Initialize));
                AppInfoFunction = (Type_AppInfo?)CreateDelegateFromUnmanaged("AppInfo", typeof(Type_AppInfo));

                foreach (var item in AppInfo._event)
                {
                    LogHelper.Debug("创建方法委托", $"{item.id}: {item.function}");
                    if (!FindEventEnum(item.id))
                    {
                        LogHelper.Error("创建方法委托", $"事件ID: {item.id} 无效");
                        continue;
                    }
                    switch ((PluginEventType)item.id)
                    {
                        case PluginEventType.PrivateMsg:
                            PrivateMsg = (Type_PrivateMsg?)CreateDelegateFromUnmanaged(item.function, typeof(Type_PrivateMsg));
                            break;

                        case PluginEventType.GroupMsg:
                            GroupMsg = (Type_GroupMsg?)CreateDelegateFromUnmanaged(item.function, typeof(Type_GroupMsg));
                            break;

                        case PluginEventType.Upload:
                            Upload = (Type_Upload?)CreateDelegateFromUnmanaged(item.function, typeof(Type_Upload));
                            break;

                        case PluginEventType.AdminChange:
                            AdminChange = (Type_AdminChange?)CreateDelegateFromUnmanaged(item.function, typeof(Type_AdminChange));
                            break;

                        case PluginEventType.GroupMemberDecrease:
                            GroupMemberDecrease = (Type_GroupMemberDecrease?)CreateDelegateFromUnmanaged(item.function, typeof(Type_GroupMemberDecrease));
                            break;

                        case PluginEventType.GroupMemberIncrease:
                            GroupMemberIncrease = (Type_GroupMemberIncrease?)CreateDelegateFromUnmanaged(item.function, typeof(Type_GroupMemberIncrease));
                            break;

                        case PluginEventType.GroupBan:
                            GroupBan = (Type_GroupBan?)CreateDelegateFromUnmanaged(item.function, typeof(Type_GroupBan));
                            break;

                        case PluginEventType.FriendAdded:
                            FriendAdded = (Type_FriendAdded?)CreateDelegateFromUnmanaged(item.function, typeof(Type_FriendAdded));
                            break;

                        case PluginEventType.FriendRequest:
                            FriendAddRequest = (Type_FriendAddRequest?)CreateDelegateFromUnmanaged(item.function, typeof(Type_FriendAddRequest));
                            break;

                        case PluginEventType.GroupAddRequest:
                            GroupAddRequest = (Type_GroupAddRequest?)CreateDelegateFromUnmanaged(item.function, typeof(Type_GroupAddRequest));
                            break;

                        case PluginEventType.StartUp:
                            StartUp = (Type_Startup?)CreateDelegateFromUnmanaged(item.function, typeof(Type_Startup));
                            break;

                        case PluginEventType.Exit:
                            Exit = (Type_Exit?)CreateDelegateFromUnmanaged(item.function, typeof(Type_Exit));
                            break;

                        case PluginEventType.Enable:
                            Enable = (Type_Enable?)CreateDelegateFromUnmanaged(item.function, typeof(Type_Enable));
                            break;

                        case PluginEventType.Disable:
                            Disable = (Type_Disable?)CreateDelegateFromUnmanaged(item.function, typeof(Type_Disable));
                            break;

                        default:
                            break;
                    }
                }
                Initialize?.Invoke(AppInfo.AuthCode);

                if (AppInfo.menu != null && AppInfo.menu.Length > 0)
                {
                    CreateUIThread();
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("创建方法委托", ex);
                return false;
            }
        }

        public override bool LoadAppInfo(string path)
        {
            if (!File.Exists(path))
            {
                LogHelper.Error("加载插件", "Json文件不存在");
                return false;
            }
            AppInfo = JsonConvert.DeserializeObject<AppInfo>(File.ReadAllText(path));
            if (AppInfo == null)
            {
                LogHelper.Error("加载插件", "Json格式错误，无法解析");
                return false;
            }
            PluginName = AppInfo.name;
            AuthCode = AppConfig.Instance.Core_AuthCode;
            AppInfo.AuthCode = AppConfig.Instance.Core_AuthCode;
            AppInfo.AppId = GetAppId().Value;
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "data", "app", AppInfo.AppId));

            return true;
        }

        private bool FindEventEnum(int id)
        {
            EventArray ??= Enum.GetValues(typeof(PluginEventType));
            foreach (int eventType in EventArray)
            {
                if (eventType == id)
                {
                    return true;
                }
            }
            return false;
        }

        [HandleProcessCorruptedStateExceptions]
        private KeyValuePair<int, string> GetAppId()
        {
            if (AppInfoFunction == null)
            {
                return new KeyValuePair<int, string>(0, "");
            }
            string? appInfo = Marshal.PtrToStringAnsi(AppInfoFunction()) ?? throw new Exception("获取AppInfo信息失败");
            string[] pair = appInfo.Split(',');
            if (pair.Length != 2)
            {
                throw new Exception("获取AppInfo信息失败");
            }

            KeyValuePair<int, string> valuePair = new(Convert.ToInt32(pair[0]), pair[1]);
            return valuePair;
        }
    }
}