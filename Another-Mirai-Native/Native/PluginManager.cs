using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.WebSocket;

namespace Another_Mirai_Native.Native
{
    public class PluginManager
    {
        public PluginManager()
        {
            Instance = this;
        }

        public static PluginManager Instance { get; private set; }

        public static CQPlugin LoadedPlugin { get; private set; }

        public bool Load(string pluginPath)
        {
            if (!File.Exists(pluginPath))
            {
                LogHelper.Error("加载插件", $"{pluginPath} 文件不存在");
                return false;
            }
            CQPlugin plugin = new(pluginPath);
            var ret = plugin.Load();
            if (ret)
            {
                LoadedPlugin = plugin;
                Client.Instance.Send(new InvokeResult() { Type = "PluginInfo", Result = LoadedPlugin.AppInfo }.ToJson());
            }
            return ret;
        }

        public int CallEvent(PluginEventType eventName, object[] args)
        {
            int result = 0;
            try
            {
                var methodInfo = typeof(PluginManager).GetMethod("Event_On" + eventName.ToString());
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
                            transformedArgs[i] = args[i].ToString();
                            break;

                        case "Boolean":
                            transformedArgs[i] = Convert.ToBoolean(args[i]);
                            break;
                    }
                }

                return (int)methodInfo.Invoke(this, transformedArgs);
            }
            catch (Exception ex)
            {
                LogHelper.Error("调用插件方法", ex);
            }
            return result;
        }

        public int Event_OnMenu(string menuName)
        {
            return LoadedPlugin.CallMenu(menuName);
        }

        public int Event_OnPrivateMsg(int subType, int msgId, long fromQQ, string msg, int font)
        {
            return LoadedPlugin.PrivateMsg == null ? 0 : LoadedPlugin.PrivateMsg(subType, msgId, fromQQ, msg.ToNativeV2(), font);
        }

        public int Event_OnGroupMsg(int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font)
        {
            return LoadedPlugin.GroupMsg == null ? 0 : LoadedPlugin.GroupMsg(subType, msgId, fromGroup, fromQQ, fromAnonymous, msg.ToNativeV2(), font);
        }

        public int Event_OnDiscussMsg(int subType, int msgId, long fromNative, long fromQQ, string msg, int font)
        {
            return 0;
        }

        public int Event_OnUpload(int subType, int sendTime, long fromGroup, long fromQQ, string file)
        {
            return LoadedPlugin.Upload == null ? 0 : LoadedPlugin.Upload(subType, sendTime, fromGroup, fromQQ, file);
        }

        public int Event_OnAdminChange(int subType, int sendTime, long fromGroup, long beingOperateQQ)
        {
            return LoadedPlugin.AdminChange == null ? 0 : LoadedPlugin.AdminChange(subType, sendTime, fromGroup, beingOperateQQ);
        }

        public int Event_OnGroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            return LoadedPlugin.GroupMemberDecrease == null
                ? 0
                : LoadedPlugin.GroupMemberDecrease(subType, sendTime, fromGroup, fromQQ, beingOperateQQ);
        }

        public int Event_OnGroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            return LoadedPlugin.GroupMemberIncrease == null
                ? 0
                : LoadedPlugin.GroupMemberIncrease(subType, sendTime, fromGroup, fromQQ, beingOperateQQ);
        }

        public int Event_OnGroupBan(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration)
        {
            return LoadedPlugin.GroupBan == null ? 0 : LoadedPlugin.GroupBan(subType, sendTime, fromGroup, fromQQ, beingOperateQQ, duration);
        }

        public int Event_OnFriendAdded(int subType, int sendTime, long fromQQ)
        {
            return LoadedPlugin.FriendAdded == null ? 0 : LoadedPlugin.FriendAdded(subType, sendTime, fromQQ);
        }

        public int Event_OnFriendAddRequest(int subType, int sendTime, long fromQQ, string msg, string responseFlag)
        {
            return LoadedPlugin.FriendAddRequest == null ? 0 : LoadedPlugin.FriendAddRequest(subType, sendTime, fromQQ, msg.ToNativeV2(), responseFlag);
        }

        public int Event_OnGroupAddRequest(int subType, int sendTime, long fromGroup, long fromQQ, string msg, string responseFlag)
        {
            return LoadedPlugin.GroupAddRequest == null
                ? 0
                : LoadedPlugin.GroupAddRequest(subType, sendTime, fromGroup, fromQQ, msg.ToNativeV2(), responseFlag);
        }

        public int Event_OnStartUp()
        {
            return LoadedPlugin.StartUp == null ? 0 : LoadedPlugin.StartUp();
        }

        public int Event_OnExit()
        {
            return LoadedPlugin.Exit == null ? 0 : LoadedPlugin.Exit();
        }

        public int Event_OnEnable()
        {
            return LoadedPlugin.Enable == null ? 0 : LoadedPlugin.Enable();
        }

        public int Event_OnDisable()
        {
            return LoadedPlugin.Disable == null ? 0 : LoadedPlugin.Disable();
        }
    }
}