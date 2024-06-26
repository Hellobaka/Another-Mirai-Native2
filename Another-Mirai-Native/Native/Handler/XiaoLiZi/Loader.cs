using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Other.XiaoLiZi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Native.Handler.XiaoLiZi
{
    public partial class Loader : PluginHandlerBase
    {
        public Loader(string path) : base(path)
        {
        }

        #region 
        public delegate int Type_ReceivePrivateMsg(nint msg);

        public delegate int Type_ReceiveGroupMsg(nint msg);

        public delegate int Type_AppEnabled();

        public delegate int Type_ReceiveEvent(nint e);

        public delegate int Type_AppMenu();

        public delegate void Type_AppExit();

        public delegate void Type_AppDisabled();

        public delegate void Type_SMSVerification(long sourceQQ, nint phone);

        public delegate void Type_SliderRecognition(long sourceQQ, nint url);

        public delegate string Type_GetAppInfo(string entryInfo, string authcode);

        public Delegate? GetAppInfo { get; set; }

        public Delegate? ReceivePrivateMsg { get; set; }

        public Delegate? ReceiveGroupMsg { get; set; }

        public Delegate? AppEnabled { get; set; }

        public Delegate? ReceiveEvent { get; set; }

        public Delegate? AppMenu { get; set; }

        public Delegate? AppExit { get; set; }

        public Delegate? AppDisabled { get; set; }

        public Delegate? SMSVerification { get; set; }

        public Delegate? SliderRecognition { get; set; }
        #endregion

        private AppInfo_XiaoLiZi? AppInfo_XiaoLiZi { get; set; }

        public override int CallMenu(string menu)
        {
            if (AppMenu == null)
            {
                LogHelper.Error("调用Menu事件", $"创建方法失败");
                return -1;
            }
            if (UIForm == null)
            {
                LogHelper.Error("调用Menu事件", $"UI线程未创建，无法调用Menu事件");
                return -1;
            }
            UIForm.BeginInvoke(() =>
            {
                SafeInvoke(AppMenu);
            });
            return 1;
        }

        public override bool CreateMethodDelegates()
        {
            foreach (var e in AppInfo!._event)
            {
                switch (e.type)
                {
                    case 2:
                        ReceiveGroupMsg = CreateDelegateFromUnmanaged<Type_ReceiveGroupMsg>(e.address, e.function);
                        break;
                    case 21:
                        ReceivePrivateMsg = CreateDelegateFromUnmanaged<Type_ReceivePrivateMsg>(e.address, e.function);
                        break;
                    case 1002:
                        AppExit = CreateDelegateFromUnmanaged<Type_AppExit>(e.address, e.function);
                        break;
                    case 1003:
                        AppEnabled = CreateDelegateFromUnmanaged<Type_AppEnabled>(e.address, e.function);
                        break;
                    case 1004:
                        AppDisabled = CreateDelegateFromUnmanaged<Type_AppDisabled>(e.address, e.function);
                        break;
                    case 114:
                        ReceiveEvent = CreateDelegateFromUnmanaged<Type_ReceiveEvent>(e.address, e.function);
                        break;
                    case 115:
                        SliderRecognition = CreateDelegateFromUnmanaged<Type_SliderRecognition>(e.address, e.function);
                        break;
                    case 116:
                        SMSVerification = CreateDelegateFromUnmanaged<Type_SMSVerification>(e.address, e.function);
                        break;
                    case 117:
                        // channel
                        break;
                    case 118:
                        // plugin message
                        break;
                    case 119:
                        // ticket
                        break;
                    case 120:
                        // verifycode
                        break;
                    default:
                        LogHelper.Error("创建插件事件", $"未定义插件事件ID: {e.type}. name={e.name}, function:{e.function}, address={e.address}");
                        break;
                }
            }
            if (AppInfo.menu != null && AppInfo.menu.Length > 0)
            {
                var menu = AppInfo.menu.FirstOrDefault();
                if (menu != null)
                {
                    AppMenu = CreateDelegateFromUnmanaged<Type_AppMenu>(menu.address, menu.function);
                }
            }
            CreateProxy();
            return true;
        }

        public override bool LoadAppInfo()
        {
            GetAppInfo = CreateDelegateFromUnmanaged<Type_GetAppInfo>("初始化");
            if (GetAppInfo == null && (GetAppInfo = CreateDelegateFromUnmanaged<Type_GetAppInfo>("apprun")) == null)
            {
                LogHelper.Error("读取插件信息", $"无法获取插件入口点，{Path.GetFileName(PluginPath)}可能并非 小栗子 插件");
                return false;
            }

            string? nativeInfo = (string?)GetAppInfo?.DynamicInvoke(BuildSelfEntryInfo(), AppConfig.Instance.Core_AuthCode.ToString());
            if (string.IsNullOrEmpty(nativeInfo))
            {
                LogHelper.Error("读取插件信息", $"无法从插件返回解析出插件信息，{Path.GetFileName(PluginPath)}可能并非 小栗子 插件");
                return false;
            }
            string path = Path.ChangeExtension(PluginPath, ".json");
            if (File.Exists(path))
            {
                AppInfo_XiaoLiZi = JsonConvert.DeserializeObject<AppInfo_XiaoLiZi>(nativeInfo);
            }
            else
            {
                AppInfo_XiaoLiZi = JsonConvert.DeserializeObject<AppInfo_XiaoLiZi>(File.ReadAllText(path));

                if (AppInfo_XiaoLiZi != null)
                {
                    try
                    {
                        AppInfo_XiaoLiZi.appname = JObject.Parse(nativeInfo)["appname"].ToString();
                    }
                    catch
                    {
                        LogHelper.Error("读取插件信息", $"Native: {nativeInfo}");
                        AppInfo_XiaoLiZi = null;
                    }
                }
            }

            if (AppInfo_XiaoLiZi == null)
            {
                LogHelper.Error("读取插件信息", $"无法从插件返回解析出插件信息，{Path.GetFileName(PluginPath)}可能并非 小栗子 插件");
                return false;
            }
            AppInfo = AppInfo_XiaoLiZi.ConvertToBase();
            return AppInfo != null;
        }

        private static string BuildSelfEntryInfo()
        {
            JObject json = [];
            foreach(var item in typeof(API).GetMethods().OrderBy(x=>x.Name))
            {
                string attribute = API.GetProxyName(item);
                if (string.IsNullOrEmpty(attribute))
                {
                    continue;
                }
                FieldInfo fieldInfo = typeof(API).GetField($"{item.Name}_Action", BindingFlags.Public | BindingFlags.Static);
                if (fieldInfo == null)
                {
                    continue;
                }
                Delegate d = (Delegate)fieldInfo.GetValue(null);
                if (d == null)
                {
                    continue;
                }
                json.Add(new JProperty(attribute, Marshal.GetFunctionPointerForDelegate(d).ToInt64()));
            }
            return json.ToString();
        }
    }
}
