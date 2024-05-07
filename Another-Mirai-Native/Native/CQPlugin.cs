using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
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

        private static Thread UIThread { get; set; }

        private Array EventArray { get; set; }

        private IntPtr Handle { get; set; }

        private static Form UIForm { get; set; }

        public int CallMenu(string menuName)
        {
            var function = AppInfo.menu.FirstOrDefault(x => x.function == menuName)?.function;
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

        public bool Init(string json)
        {
            AppInfo = JsonConvert.DeserializeObject<AppInfo>(json);
            if (AppInfo == null)
            {
                LogHelper.Error("加载插件", "Json格式错误，无法解析");
                return false;
            }
            Initialize = (Type_Initialize)CreateDelegateFromUnmanaged("Initialize", typeof(Type_Initialize));
            AppInfoFunction = (Type_AppInfo)CreateDelegateFromUnmanaged("AppInfo", typeof(Type_AppInfo));
            Name = AppInfo.name;
            foreach (var item in AppInfo._event)
            {
                LogHelper.Debug("加载插件", $"{item.id}: {item.function}");
                if (!FindEventEnum(item.id))
                {
                    LogHelper.Error("加载插件", $"事件ID: {item.id} 无效");
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
            AuthCode = AppConfig.Instance.Core_AuthCode;
            AppInfo.AuthCode = AppConfig.Instance.Core_AuthCode;
            AppInfo.AppId = GetAppId().Value;
            Directory.CreateDirectory(System.IO.Path.Combine(Environment.CurrentDirectory, "data", "app", AppInfo.AppId));
            Initialize(AppInfo.AuthCode);

            if (AppInfo.menu != null && AppInfo.menu.Length > 0)
            {
                BuildMenuThread();
            }
            return true;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public bool Load()
        {
            var cqpHandle = LoadLibrary(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CQP.dll"));
            if (cqpHandle == IntPtr.Zero)
            {
                LogHelper.Error("加载插件", $"CQP.dll加载失败, GetLastError={GetLastError()}");
            }
            string fileName = new FileInfo(Path).Name;
            try
            {
                Handle = LoadLibrary(Path);
                if (Handle == IntPtr.Zero)
                {
                    LogHelper.Error("加载插件", $"{fileName} 加载失败, GetLastError={GetLastError()}");
                    return false;
                }
                if (File.Exists(Path.Replace(".dll", ".json")) && Init(File.ReadAllText(Path.Replace(".dll", ".json"))))
                {
                    LogHelper.Info("加载插件", $"{Name}, 加载成功");
                }
                else
                {
                    LogHelper.Info("加载插件", $"{Name} 加载失败, Json不存在");
                    return false;
                }
            }
            catch
            {
                LogHelper.Error("加载插件", $"{Path} 加载失败, GetLastError={GetLastError()}");
            }
            return Handle != IntPtr.Zero;
        }

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);

        private static void StartUIThread()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            UIForm = new Form
            {
                Size = new Size(1, 1),
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                //WindowState = FormWindowState.Minimized
            };
            UIForm.Show();
            UIForm.Visible = false;
            Application.Run();
        }

        private void BuildMenuThread()
        {
            if (UIThread == null)
            {
                UIThread = new Thread(StartUIThread);
                UIThread.SetApartmentState(ApartmentState.STA);
                UIThread.Start();
            }
        }

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