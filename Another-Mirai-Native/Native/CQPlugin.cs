using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Another_Mirai_Native.Native
{
    public class CQPlugin
    {
        public CQPlugin(string path)
        {
            Path = path;
        }

        #region 非托管调用

        public delegate int Type_AdminChange(int subType, int sendTime, long fromGroup, long beingOperateQQ);

        public delegate IntPtr Type_AppInfo();

        public delegate int Type_Disable();

        public delegate int Type_Enable();

        public delegate int Type_Exit();

        public delegate int Type_FriendAdded(int subType, int sendTime, long fromQQ);

        public delegate int Type_FriendAddRequest(int subType, int sendTime, long fromQQ, IntPtr msg, string responseFlag);

        public delegate int Type_GroupAddRequest(int subType, int sendTime, long fromGroup, long fromQQ, IntPtr msg, string responseFlag);

        public delegate int Type_GroupBan(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration);

        public delegate int Type_GroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ);

        public delegate int Type_GroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ);

        public delegate int Type_GroupMsg(int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, IntPtr msg, int font);

        public delegate int Type_Initialize(int authCode);

        public delegate int Type_Menu();

        public delegate int Type_PrivateMsg(int subType, int msgId, long fromQQ, IntPtr msg, int font);

        public delegate int Type_Startup();

        public delegate int Type_Upload(int subType, int sendTime, long fromGroup, long fromQQ, string file);

        public Type_AdminChange AdminChange { get; set; }

        public Type_AppInfo AppInfoFunction { get; set; }

        public Type_Disable Disable { get; set; }

        public Type_Enable Enable { get; set; }

        public Type_Exit Exit { get; set; }

        public Type_FriendAdded FriendAdded { get; set; }

        public Type_FriendAddRequest FriendAddRequest { get; set; }

        public Type_GroupAddRequest GroupAddRequest { get; set; }

        public Type_GroupBan GroupBan { get; set; }

        public Type_GroupMemberDecrease GroupMemberDecrease { get; set; }

        public Type_GroupMemberIncrease GroupMemberIncrease { get; set; }

        public Type_GroupMsg GroupMsg { get; set; }

        public Type_Initialize Initialize { get; set; }

        public Type_PrivateMsg PrivateMsg { get; set; }

        public Type_Startup StartUp { get; set; }

        public Type_Upload Upload { get; set; }

        #endregion 非托管调用

        public AppInfo? AppInfo { get; set; }

        public int AuthCode { get; set; }

        public string Name { get; set; } = "";

        public string Path { get; set; } = "";

        private Array EventArray { get; set; }

        private IntPtr Handle { get; set; }

        public void Init(string json)
        {
            AppInfo = JsonConvert.DeserializeObject<AppInfo>(json);
            if (AppInfo == null)
            {
                LogHelper.Error("Load", "Json格式错误，无法解析");
                return;
            }
            Initialize = (Type_Initialize)CreateDelegateFromUnmanaged("Initialize", typeof(Type_Initialize));
            AppInfoFunction = (Type_AppInfo)CreateDelegateFromUnmanaged("AppInfo", typeof(Type_AppInfo));
            Name = AppInfo.name;
            foreach (var item in AppInfo._event)
            {
                if (AppConfig.DebugMode)
                {
                    LogHelper.Debug("Init", $"{item.id}: {item.function}");
                }
                if (!FindEventEnum(item.id))
                {
                    LogHelper.Error("Load", $"EventID: {item.id} 无效");
                    continue;
                }
                switch ((PluginEventType)item.id)
                {
                    case PluginEventType.PrivateMsg:
                        PrivateMsg = (Type_PrivateMsg)CreateDelegateFromUnmanaged(item.function, typeof(Type_PrivateMsg));
                        break;

                    case PluginEventType.GroupMsg:
                        GroupMsg = (Type_GroupMsg)CreateDelegateFromUnmanaged(item.function, typeof(Type_GroupMsg));
                        break;

                    case PluginEventType.Upload:
                        Upload = (Type_Upload)CreateDelegateFromUnmanaged(item.function, typeof(Type_Upload));
                        break;

                    case PluginEventType.AdminChange:
                        AdminChange = (Type_AdminChange)CreateDelegateFromUnmanaged(item.function, typeof(Type_AdminChange));
                        break;

                    case PluginEventType.GroupMemberDecrease:
                        GroupMemberDecrease = (Type_GroupMemberDecrease)CreateDelegateFromUnmanaged(item.function, typeof(Type_GroupMemberDecrease));
                        break;

                    case PluginEventType.GroupMemberIncrease:
                        GroupMemberIncrease = (Type_GroupMemberIncrease)CreateDelegateFromUnmanaged(item.function, typeof(Type_GroupMemberIncrease));
                        break;

                    case PluginEventType.GroupBan:
                        GroupBan = (Type_GroupBan)CreateDelegateFromUnmanaged(item.function, typeof(Type_GroupBan));
                        break;

                    case PluginEventType.FriendAdded:
                        FriendAdded = (Type_FriendAdded)CreateDelegateFromUnmanaged(item.function, typeof(Type_FriendAdded));
                        break;

                    case PluginEventType.FriendRequest:
                        FriendAddRequest = (Type_FriendAddRequest)CreateDelegateFromUnmanaged(item.function, typeof(Type_FriendAddRequest));
                        break;

                    case PluginEventType.GroupAddRequest:
                        GroupAddRequest = (Type_GroupAddRequest)CreateDelegateFromUnmanaged(item.function, typeof(Type_GroupAddRequest));
                        break;

                    case PluginEventType.StartUp:
                        StartUp = (Type_Startup)CreateDelegateFromUnmanaged(item.function, typeof(Type_Startup));
                        break;

                    case PluginEventType.Exit:
                        Exit = (Type_Exit)CreateDelegateFromUnmanaged(item.function, typeof(Type_Exit));
                        break;

                    case PluginEventType.Enable:
                        Enable = (Type_Enable)CreateDelegateFromUnmanaged(item.function, typeof(Type_Enable));
                        break;

                    case PluginEventType.Disable:
                        Disable = (Type_Disable)CreateDelegateFromUnmanaged(item.function, typeof(Type_Disable));
                        break;

                    default:
                        break;
                }
            }
            AuthCode = new Random(Helper.MakeRandomID()).Next();
            AppInfo.AuthCode = AuthCode;
            AppInfo.AppId = GetAppId().Value;
            AppInfo.PID = Process.GetCurrentProcess().Id;
            Directory.CreateDirectory(System.IO.Path.Combine(Environment.CurrentDirectory, "data", "app", AppInfo.AppId));
            Initialize(AuthCode);
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public bool Load()
        {
            string fileName = new FileInfo(Path).Name;
            try
            {
                Handle = LoadLibrary(Path);
                if (Handle == IntPtr.Zero)
                {
                    LogHelper.Error("加载插件", $"{fileName} 加载失败, GetLastError={GetLastError()}");
                    return false;
                }
                LogHelper.Info("加载插件", $"{fileName}, 加载成功, 开始初始化...");
                if (File.Exists(Path.Replace(".dll", ".json")))
                {
                    Init(File.ReadAllText(Path.Replace(".dll", ".json")));
                    LogHelper.Info("加载插件", $"{Name}, 初始化完成");
                }
                else
                {
                    LogHelper.Info("加载插件", $"{Name} 加载失败, Json不存在");
                    return false;
                }
            }
            catch
            {
                LogHelper.Error("Error", $"{Path} fail, GetLastError={GetLastError()}");
            }
            return Handle != IntPtr.Zero;
        }

        public int CallMenu(string menuName)
        {
            var function = AppInfo.menu.FirstOrDefault(x => x.function == menuName)?.function;
            if (function == null)
            {
                LogHelper.Error("CallMenu", $"AppInfo未找到 {menuName}");
                return -1;
            }
            var menuMethod = CreateDelegateFromUnmanaged(function, typeof(Type_Menu));
            if (menuMethod == null)
            {
                LogHelper.Error("CallMenu", $"{function} 创建方法失败");
                return -1;
            }
            menuMethod.DynamicInvoke(null);
            return 1;
        }

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);

        private Delegate CreateDelegateFromUnmanaged(string apiName, Type t)
        {
            IntPtr api = GetProcAddress(Handle, apiName);
            return api == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer(api, t);
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
            string appInfo = Marshal.PtrToStringAnsi(AppInfoFunction());
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