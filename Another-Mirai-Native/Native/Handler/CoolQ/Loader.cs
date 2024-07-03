using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Newtonsoft.Json;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Native.Handler.CoolQ
{
    public class Loader : PluginHandlerBase
    {
        public Loader(string path)
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
            var menuMethod = CreateDelegateFromUnmanaged<Type_Menu>(function);
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
                Initialize = CreateDelegateFromUnmanaged<Type_Initialize>("Initialize");
                AppInfoFunction = CreateDelegateFromUnmanaged<Type_AppInfo>("AppInfo");

                foreach (var item in AppInfo._event)
                {
                    LogHelper.Debug("创建方法委托", $"{item.type}: {item.function}");
                    if (!FindEventEnum(item.type))
                    {
                        LogHelper.Error("创建方法委托", $"事件ID: {item.type} 无效");
                        continue;
                    }
                    switch ((PluginEventType)item.type)
                    {
                        case PluginEventType.PrivateMsg:
                            PrivateMsg = CreateDelegateFromUnmanaged<Type_PrivateMsg>(item.function);
                            break;

                        case PluginEventType.GroupMsg:
                            GroupMsg = CreateDelegateFromUnmanaged<Type_GroupMsg>(item.function);
                            break;

                        case PluginEventType.Upload:
                            Upload = CreateDelegateFromUnmanaged<Type_Upload>(item.function);
                            break;

                        case PluginEventType.AdminChange:
                            AdminChange = CreateDelegateFromUnmanaged<Type_AdminChange>(item.function);
                            break;

                        case PluginEventType.GroupMemberDecrease:
                            GroupMemberDecrease = CreateDelegateFromUnmanaged<Type_GroupMemberDecrease>(item.function);
                            break;

                        case PluginEventType.GroupMemberIncrease:
                            GroupMemberIncrease = CreateDelegateFromUnmanaged<Type_GroupMemberIncrease>(item.function);
                            break;

                        case PluginEventType.GroupBan:
                            GroupBan = CreateDelegateFromUnmanaged<Type_GroupBan>(item.function);
                            break;

                        case PluginEventType.FriendAdded:
                            FriendAdded = CreateDelegateFromUnmanaged<Type_FriendAdded>(item.function);
                            break;

                        case PluginEventType.FriendRequest:
                            FriendAddRequest = CreateDelegateFromUnmanaged<Type_FriendAddRequest>(item.function);
                            break;

                        case PluginEventType.GroupAddRequest:
                            GroupAddRequest = CreateDelegateFromUnmanaged<Type_GroupAddRequest>(item.function);
                            break;

                        case PluginEventType.StartUp:
                            StartUp = CreateDelegateFromUnmanaged<Type_StartUp>(item.function);
                            break;

                        case PluginEventType.Exit:
                            Exit = CreateDelegateFromUnmanaged<Type_Exit>(item.function);
                            break;

                        case PluginEventType.Enable:
                            Enable = CreateDelegateFromUnmanaged<Type_Enable>(item.function);
                            break;

                        case PluginEventType.Disable:
                            Disable = CreateDelegateFromUnmanaged<Type_Disable>(item.function);
                            break;

                        default:
                            break;
                    }
                }
                Initialize?.Invoke(AppInfo.AuthCode);

                AppInfo.AppId = GetAppId().Value;
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "data", "app", AppInfo.AppId));

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

        public override bool LoadAppInfo()
        {
            string path = Path.ChangeExtension(PluginPath, ".json");
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