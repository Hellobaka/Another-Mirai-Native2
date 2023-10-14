using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Newtonsoft.Json;
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

        public string Name { get; private set; }

        public AppInfo? AppInfo { get; private set; }

        public string Path { get; set; } = "";

        public string Json { get; set; } = "";

        public IntPtr Handle { get; set; }

        public int AuthCode { get; set; }

        private Array EventArray { get; set; }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        public delegate IntPtr Type_AppInfo();

        public delegate int Type_Initialize(int authCode);

        public delegate int Type_PrivateMsg(int subType, int msgId, long fromQQ, IntPtr msg, int font);

        public delegate int Type_GroupMsg(int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, IntPtr msg, int font);

        public delegate int Type_Upload(int subType, int sendTime, long fromGroup, long fromQQ, string file);

        public delegate int Type_AdminChange(int subType, int sendTime, long fromGroup, long beingOperateQQ);

        public delegate int Type_GroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ);

        public delegate int Type_GroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ);

        public delegate int Type_GroupBan(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration);

        public delegate int Type_FriendAdded(int subType, int sendTime, long fromQQ);

        public delegate int Type_FriendAddRequest(int subType, int sendTime, long fromQQ, IntPtr msg, string responseFlag);

        public delegate int Type_GroupAddRequest(int subType, int sendTime, long fromGroup, long fromQQ, IntPtr msg, string responseFlag);

        public delegate int Type_Startup();

        public delegate int Type_Exit();

        public delegate int Type_Enable();

        public delegate int Type_Disable();

        public Type_AppInfo AppInfoFunction;

        public Type_Initialize Initialize;

        public Type_PrivateMsg PrivateMsg;

        public Type_GroupMsg GroupMsg;

        public Type_Upload Upload;

        public Type_AdminChange AdminChange;

        public Type_GroupMemberDecrease GroupMemberDecrease;

        public Type_GroupMemberIncrease GroupMemberIncrease;

        public Type_GroupBan GroupBan;

        public Type_FriendAdded FriendAdded;

        public Type_FriendAddRequest FriendAddRequest;

        public Type_GroupAddRequest GroupAddRequest;

        public Type_Startup StartUp;

        public Type_Exit Exit;

        public Type_Enable Enable;

        public Type_Disable Disable;

        public void Init()
        {
            AppInfo = JsonConvert.DeserializeObject<AppInfo>(Json);
            if (AppInfo == null)
            {
                LogHelper.Error("Load", "Json格式错误，无法解析");
                return;
            }
            Initialize = (Type_Initialize)Invoke("Initialize", typeof(Type_Initialize));
            AppInfoFunction = (Type_AppInfo)Invoke("AppInfo", typeof(Type_AppInfo));
            Name = AppInfo.name;
            foreach (var item in AppInfo._event)
            {
                LogHelper.Debug("Init", $"{item.id}: {item.function}");
                if (!FindEventEnum(item.id))
                {
                    LogHelper.Error("Load", $"EventID: {item.id} 无效");
                    continue;
                }
                switch ((PluginEventType)item.id)
                {
                    case PluginEventType.PrivateMsg:
                        PrivateMsg = (Type_PrivateMsg)Invoke(item.function, typeof(Type_PrivateMsg));
                        break;

                    case PluginEventType.GroupMsg:
                        GroupMsg = (Type_GroupMsg)Invoke(item.function, typeof(Type_GroupMsg));
                        break;

                    case PluginEventType.Upload:
                        Upload = (Type_Upload)Invoke(item.function, typeof(Type_Upload));
                        break;

                    case PluginEventType.AdminChange:
                        AdminChange = (Type_AdminChange)Invoke(item.function, typeof(Type_AdminChange));
                        break;

                    case PluginEventType.GroupMemberDecrease:
                        GroupMemberDecrease = (Type_GroupMemberDecrease)Invoke(item.function, typeof(Type_GroupMemberDecrease));
                        break;

                    case PluginEventType.GroupMemberIncrease:
                        GroupMemberIncrease = (Type_GroupMemberIncrease)Invoke(item.function, typeof(Type_GroupMemberIncrease));
                        break;

                    case PluginEventType.GroupBan:
                        GroupBan = (Type_GroupBan)Invoke(item.function, typeof(Type_GroupBan));
                        break;

                    case PluginEventType.FriendAdded:
                        FriendAdded = (Type_FriendAdded)Invoke(item.function, typeof(Type_FriendAdded));
                        break;

                    case PluginEventType.FriendRequest:
                        FriendAddRequest = (Type_FriendAddRequest)Invoke(item.function, typeof(Type_FriendAddRequest));
                        break;

                    case PluginEventType.GroupAddRequest:
                        GroupAddRequest = (Type_GroupAddRequest)Invoke(item.function, typeof(Type_GroupAddRequest));
                        break;

                    case PluginEventType.StartUp:
                        StartUp = (Type_Startup)Invoke(item.function, typeof(Type_Startup));
                        break;

                    case PluginEventType.Exit:
                        Exit = (Type_Exit)Invoke(item.function, typeof(Type_Exit));
                        break;

                    case PluginEventType.Enable:
                        Enable = (Type_Enable)Invoke(item.function, typeof(Type_Enable));
                        break;

                    case PluginEventType.Disable:
                        Disable = (Type_Disable)Invoke(item.function, typeof(Type_Disable));
                        break;

                    default:
                        break;
                }
            }
            AuthCode = new Random().Next();
            AppInfo.AuthCode = AuthCode;
            AppInfo.AppId = GetAppId().Value;
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
                    Json = File.ReadAllText(Path.Replace(".dll", ".json"));
                }
                Init();
                LogHelper.Info("加载插件", $"{Name}, 初始化完成");
            }
            catch
            {
                LogHelper.Error("Error", $"{Path} fail, GetLastError={GetLastError()}");
            }
            return Handle != IntPtr.Zero;
        }

        public Delegate Invoke(string APIName, Type t)
        {
            IntPtr api = GetProcAddress(Handle, APIName);
            return api == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer(api, t);
        }

        [HandleProcessCorruptedStateExceptions]
        public KeyValuePair<int, string> GetAppId()
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
    }
}