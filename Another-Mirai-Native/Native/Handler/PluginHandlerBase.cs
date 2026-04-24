using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Another_Mirai_Native.Native.Handler
{
    public class PluginHandlerBase
    {
        public PluginHandlerBase(string path)
        {
            PluginPath = path;
        }

        #region Native Delegates

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

        public delegate int Type_StartUp();

        public delegate int Type_Upload(int subType, int sendTime, long fromGroup, long fromQQ, string file);

        public Type_AdminChange? AdminChange { get; set; }

        public Type_AppInfo? AppInfoFunction { get; set; }

        public Type_Disable? Disable { get; set; }

        public Type_Enable? Enable { get; set; }

        public Type_Exit? Exit { get; set; }

        public Type_FriendAdded? FriendAdded { get; set; }

        public Type_FriendAddRequest? FriendAddRequest { get; set; }

        public Type_GroupAddRequest? GroupAddRequest { get; set; }

        public Type_GroupBan? GroupBan { get; set; }

        public Type_GroupMemberDecrease? GroupMemberDecrease { get; set; }

        public Type_GroupMemberIncrease? GroupMemberIncrease { get; set; }

        public Type_GroupMsg? GroupMsg { get; set; }

        public Type_Initialize? Initialize { get; set; }

        public Type_PrivateMsg? PrivateMsg { get; set; }

        public Type_StartUp? StartUp { get; set; }

        public Type_Upload? Upload { get; set; }

        #endregion Native Delegates

        public AppInfo? AppInfo { get; set; }

        public int AuthCode { get; set; }

        public string PluginName { get; set; } = "";

        public string PluginPath { get; set; }

        public Form? UIForm { get; set; }

        public IntPtr CQPHandle { get; set; }

        private IntPtr NativeHandle { get; set; }

        private Thread UIThread { get; set; }

        public virtual int CallMenu(string menu)
        {
            throw new NotImplementedException();
        }

        public T? CreateDelegateFromUnmanaged<T>(string apiName) where T : Delegate
        {
            IntPtr api = WinNative.GetProcAddress(NativeHandle, apiName);
            return api == IntPtr.Zero ? null : (T?)Marshal.GetDelegateForFunctionPointer(api, typeof(T));
        }

        public Delegate? CreateDelegateFromUnmanaged<T>(int address, string apiName) where T : Delegate
        {
            if (address > 0)
            {
                return Marshal.GetDelegateForFunctionPointer(new IntPtr(address), typeof(T));
            }
            else
            {
                return CreateDelegateFromUnmanaged<T>(apiName);
            }
        }

        public virtual bool CreateMethodDelegates()
        {
            throw new NotImplementedException();
        }

        public virtual void CreateUIThread()
        {
            if (UIThread == null)
            {
                UIThread = new Thread(() =>
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
                });
                UIThread.SetApartmentState(ApartmentState.STA);
                UIThread.Start();
            }
        }

        public virtual bool LoadAppInfo()
        {
            throw new NotImplementedException();
        }
        
#if NET48
        [HandleProcessCorruptedStateExceptions]
#endif
        [SecurityCritical]
        public virtual bool LoadPlugin()
        {
            Directory.CreateDirectory("libraries");
            foreach (var item in new DirectoryInfo("libraries").GetFiles().Where(x => x.Extension == ".dll" || x.Extension == ".exe"))
            {
                if (WinNative.LoadLibrary(item.FullName) != IntPtr.Zero)
                {
                    LogHelper.Info("依赖载入", $"载入第三方库: {item.Name}, 载入成功");
                }
                else
                {
                    LogHelper.Info("依赖载入", $"载入第三方库: {item.Name}, 载入失败");
                }
            }

            CQPHandle = WinNative.LoadLibrary(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CQP.dll"));
            if (CQPHandle == IntPtr.Zero)
            {
                LogHelper.Error("加载插件", $"CQP.dll加载失败, GetLastError={WinNative.GetLastError()}");
            }
            try
            {
                NativeHandle = WinNative.LoadLibrary(PluginPath);
                if (NativeHandle == IntPtr.Zero)
                {
                    LogHelper.Error("加载插件", $"{Path.GetFileName(PluginPath)} 加载失败, GetLastError={WinNative.GetLastError()}");
                    return false;
                }
                if (LoadAppInfo() && CreateMethodDelegates())
                {
                    LogHelper.Info("加载插件", $"{PluginName}, 加载成功");
                }
                else
                {
                    LogHelper.Info("加载插件", $"{PluginName} 加载失败, Json不存在或无法解析");
                    return false;
                }
            }
            catch (Exception exc)
            {
                LogHelper.Error("加载插件", $"{PluginPath} 加载失败, GetLastError={WinNative.GetLastError()}, Exception: {exc.Message} {exc.StackTrace}");
            }
            return NativeHandle != IntPtr.Zero;
        }

#if NET48
        [HandleProcessCorruptedStateExceptions]
#endif
        public virtual int CallEvent(PluginEventType eventName, object[] args)
        {
            int result = -1;
            try
            {
                var methodInfo = GetType().GetMethod("Event_On" + eventName.ToString());
                if (methodInfo == null)
                {
                    LogHelper.Error("调用事件", $"调用 {eventName} 未找到对应实现");
                    return result;
                }
                var argumentList = methodInfo.GetParameters();
                if (args.Length != argumentList.Length)
                {
                    LogHelper.Error("调用事件", $"调用 {eventName} 参数表数量不对应");
                    return result;
                }
                object[] transformedArgs = new object[argumentList.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    switch (argumentList[i].ParameterType.Name)
                    {
                        case "Int64":
                            transformedArgs[i] = Convert.ToInt64(args[i]);
                            break;

                        case "Int32":
                            transformedArgs[i] = Convert.ToInt32(args[i]);
                            break;

                        case "String":
                            transformedArgs[i] = args[i]?.ToString() ?? "";
                            break;

                        case "Boolean":
                            transformedArgs[i] = Convert.ToBoolean(args[i]);
                            break;
                    }
                }

                return (int)(methodInfo.Invoke(this, transformedArgs) ?? -1);
            }
            catch (Exception ex)
            {
                LogHelper.Error("调用插件方法", ex);
                ExceptionDispatchInfo.Capture(ex);
            }
            return result;
        }

        public int Event_OnAdminChange(int subType, int sendTime, long fromGroup, long beingOperateQQ)
        {
            return AdminChange == null ? 0 : AdminChange(subType, sendTime, fromGroup, beingOperateQQ);
        }

        public int Event_OnDisable()
        {
            return Disable == null ? 0 : Disable();
        }

        public int Event_OnDiscussMsg(int subType, int msgId, long fromNative, long fromQQ, string msg, int font)
        {
            return 0;
        }

        public int Event_OnEnable()
        {
            return Enable == null ? 0 : Enable();
        }

        public int Event_OnExit()
        {
            return Exit == null ? 0 : Exit();
        }

        public int Event_OnFriendAdded(int subType, int sendTime, long fromQQ)
        {
            return FriendAdded == null ? 0 : FriendAdded(subType, sendTime, fromQQ);
        }

        public int Event_OnFriendAddRequest(int subType, int sendTime, long fromQQ, string msg, string responseFlag)
        {
            return FriendAddRequest == null ? 0 : FriendAddRequest(subType, sendTime, fromQQ, msg.ToNativeV2(), responseFlag);
        }

        public int Event_OnGroupAddRequest(int subType, int sendTime, long fromGroup, long fromQQ, string msg, string responseFlag)
        {
            return GroupAddRequest == null
                ? 0
                : GroupAddRequest(subType, sendTime, fromGroup, fromQQ, msg.ToNativeV2(), responseFlag);
        }

        public int Event_OnGroupBan(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration)
        {
            return GroupBan == null ? 0 : GroupBan(subType, sendTime, fromGroup, fromQQ, beingOperateQQ, duration);
        }

        public int Event_OnGroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            return GroupMemberDecrease == null
                ? 0
                : GroupMemberDecrease(subType, sendTime, fromGroup, fromQQ, beingOperateQQ);
        }

        public int Event_OnGroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            return GroupMemberIncrease == null
                ? 0
                : GroupMemberIncrease(subType, sendTime, fromGroup, fromQQ, beingOperateQQ);
        }

        public int Event_OnGroupMsg(int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font)
        {
            return GroupMsg == null ? 0 : GroupMsg(subType, msgId, fromGroup, fromQQ, fromAnonymous, msg.ToNativeV2(), font);
        }

        public int Event_OnMenu(string menuName)
        {
            return CallMenu(menuName);
        }

        public int Event_OnPrivateMsg(int subType, int msgId, long fromQQ, string msg, int font)
        {
            return PrivateMsg == null ? 0 : PrivateMsg(subType, msgId, fromQQ, msg.ToNativeV2(), font);
        }

        public int Event_OnStartUp()
        {
            return StartUp == null ? 0 : StartUp();
        }

        public int Event_OnUpload(int subType, int sendTime, long fromGroup, long fromQQ, string file)
        {
            return Upload == null ? 0 : Upload(subType, sendTime, fromGroup, fromQQ, file);
        }
    }
}