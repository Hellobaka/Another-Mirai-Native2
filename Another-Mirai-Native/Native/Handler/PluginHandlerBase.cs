using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
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

        public Delegate CreateDelegateFromUnmanaged<T>(int address, string apiName) where T : Delegate
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

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public virtual bool LoadPlugin()
        {
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
    }
}