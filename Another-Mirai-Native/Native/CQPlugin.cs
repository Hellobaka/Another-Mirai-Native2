using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public string Path { get; set; } = "";

        public string Json { get; set; } = "";

        public IntPtr Handle { get; set; }

        public int AuthCode { get; set; }

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string path);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        #region

        private delegate IntPtr Type_AppInfo();

        private delegate int Type_Initialize(int authCode);

        private delegate int Type_PrivateMsg(int subType, int msgId, long fromQQ, IntPtr msg, int font);

        private delegate int Type_GroupMsg(int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, IntPtr msg, int font);

        private delegate int Type_Upload(int subType, int sendTime, long fromGroup, long fromQQ, string file);

        private delegate int Type_AdminChange(int subType, int sendTime, long fromGroup, long beingOperateQQ);

        private delegate int Type_GroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ);

        private delegate int Type_GroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ);

        private delegate int Type_GroupBan(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration);

        private delegate int Type_FriendAdded(int subType, int sendTime, long fromQQ);

        private delegate int Type_FriendRequest(int subType, int sendTime, long fromQQ, IntPtr msg, string responseFlag);

        private delegate int Type_GroupAddRequest(int subType, int sendTime, long fromGroup, long fromQQ, IntPtr msg, string responseFlag);

        private delegate int Type_Startup();

        private delegate int Type_Exit();

        private delegate int Type_Enable();

        private delegate int Type_Disable();

        private Type_AppInfo AppInfoFunction;

        public string Name { get; private set; }

        public AppInfo? AppInfo { get; private set; }

        private Type_Initialize Initialize;

        private Type_PrivateMsg PrivateMsg;

        private Type_GroupMsg GroupMsg;

        private Type_Upload Upload;

        private Type_AdminChange AdminChange;

        private Type_GroupMemberDecrease GroupMemberDecrease;

        private Type_GroupMemberIncrease GroupMemberIncrease;

        private Type_GroupBan GroupBan;

        private Type_FriendAdded FriendAdded;

        private Type_FriendRequest FriendRequest;

        private Type_GroupAddRequest GroupAddRequest;

        private Type_Startup Startup;

        private Type_Exit Exit;

        private Type_Enable Enable;

        private Type_Disable Disable;

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
                switch (item.id.ToString())
                {
                    case "1":
                        PrivateMsg = (Type_PrivateMsg)Invoke(item.function, typeof(Type_PrivateMsg));
                        break;

                    case "2":
                        GroupMsg = (Type_GroupMsg)Invoke(item.function, typeof(Type_GroupMsg));
                        break;

                    case "4":
                        Upload = (Type_Upload)Invoke(item.function, typeof(Type_Upload));
                        break;

                    case "5":
                        AdminChange = (Type_AdminChange)Invoke(item.function, typeof(Type_AdminChange));
                        break;

                    case "6":
                        GroupMemberDecrease = (Type_GroupMemberDecrease)Invoke(item.function, typeof(Type_GroupMemberDecrease));
                        break;

                    case "7":
                        GroupMemberIncrease = (Type_GroupMemberIncrease)Invoke(item.function, typeof(Type_GroupMemberIncrease));
                        break;

                    case "8":
                        GroupBan = (Type_GroupBan)Invoke(item.function, typeof(Type_GroupBan));
                        break;

                    case "10":
                        FriendAdded = (Type_FriendAdded)Invoke(item.function, typeof(Type_FriendAdded));
                        break;

                    case "11":
                        FriendRequest = (Type_FriendRequest)Invoke(item.function, typeof(Type_FriendRequest));
                        break;

                    case "12":
                        GroupAddRequest = (Type_GroupAddRequest)Invoke(item.function, typeof(Type_GroupAddRequest));
                        break;

                    case "1001":
                        Startup = (Type_Startup)Invoke(item.function, typeof(Type_Startup));
                        break;

                    case "1002":
                        Exit = (Type_Exit)Invoke(item.function, typeof(Type_Exit));
                        break;

                    case "1003":
                        Enable = (Type_Enable)Invoke(item.function, typeof(Type_Enable));
                        break;

                    case "1004":
                        Disable = (Type_Disable)Invoke(item.function, typeof(Type_Disable));
                        break;
                }
            }
            AuthCode = new Random().Next();
            Initialize(AuthCode);
        }

        #endregion

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
        public KeyValuePair<int, string> GetAppInfo()
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

        public int CallFunction(PluginEventType ApiName, params object[] args)
        {
            int returnValue = 0;
            switch (ApiName)
            {
                case PluginEventType.PrivateMsg:
                    if (PrivateMsg == null)
                    { returnValue = -1; break; }
                    returnValue = PrivateMsg(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt64(args[2]), (IntPtr)args[3], 1);
                    break;

                case PluginEventType.GroupMsg:
                    if (GroupMsg == null)
                    { returnValue = -1; break; }
                    returnValue = GroupMsg(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt64(args[2]), Convert.ToInt64(args[3])
                        , args[4].ToString(), (IntPtr)args[5], 1);
                    break;

                case PluginEventType.Upload:
                    if (Upload == null)
                    { returnValue = -1; break; }
                    returnValue = Upload(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt64(args[2]), Convert.ToInt64(args[3]), args[4].ToString());
                    break;

                case PluginEventType.AdminChange:
                    if (AdminChange == null)
                    { returnValue = -1; break; }
                    returnValue = AdminChange(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt64(args[2]), Convert.ToInt64(args[3]));
                    break;

                case PluginEventType.GroupMemberDecrease:
                    if (GroupMemberDecrease == null)
                    { returnValue = -1; break; }
                    returnValue = GroupMemberDecrease(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt64(args[2]), Convert.ToInt64(args[3]), Convert.ToInt64(args[4]));
                    break;

                case PluginEventType.GroupMemberIncrease:
                    if (GroupMemberIncrease == null)
                    { returnValue = -1; break; }
                    returnValue = GroupMemberIncrease(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt64(args[2]), Convert.ToInt64(args[3]), Convert.ToInt64(args[4]));
                    break;

                case PluginEventType.GroupBan:
                    if (GroupBan == null)
                    { returnValue = -1; break; }
                    returnValue = GroupBan(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt64(args[2]), Convert.ToInt64(args[3]), Convert.ToInt64(args[4]), Convert.ToInt64(args[5]));
                    break;

                case PluginEventType.FriendAdded:
                    if (FriendAdded == null)
                    { returnValue = -1; break; }
                    returnValue = FriendAdded(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt64(args[2]));
                    break;

                case PluginEventType.FriendRequest:
                    if (FriendRequest == null)
                    { returnValue = -1; break; }
                    returnValue = FriendRequest(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt64(args[2]), Marshal.StringToHGlobalAnsi(args[3].ToString()), args[4].ToString());
                    break;

                case PluginEventType.GroupAddRequest:
                    if (GroupAddRequest == null)
                    { returnValue = -1; break; }
                    returnValue = GroupAddRequest(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt64(args[2]), Convert.ToInt64(args[3]), Marshal.StringToHGlobalAnsi(args[4].ToString()), args[5].ToString());
                    break;

                case PluginEventType.StartUp:
                    if (Startup == null)
                    { returnValue = -1; break; }
                    returnValue = Startup();
                    break;

                case PluginEventType.Exit:
                    if (Exit == null)
                    { returnValue = -1; break; }
                    returnValue = Exit();
                    break;

                case PluginEventType.Enable:
                    if (Enable == null)
                    { returnValue = -1; break; }
                    returnValue = Enable();
                    break;

                case PluginEventType.Disable:
                    if (Disable == null)
                    { returnValue = -1; break; }
                    returnValue = Disable();
                    break;

                default:
                    returnValue = -1;
                    break;
            }
            return returnValue;
        }
    }
}