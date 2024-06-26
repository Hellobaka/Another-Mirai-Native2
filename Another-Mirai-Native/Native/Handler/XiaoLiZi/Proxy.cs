using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Other.XiaoLiZi;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Native.Handler.XiaoLiZi
{
    // HandlerBase => Handler => Proxy => Plugin
    public partial class Loader
    {
        private void CreateProxy()
        {
            AdminChange = new Type_AdminChange(Proxy_AdminChange);
            AppInfoFunction = new Type_AppInfo(Proxy_AppInfoFunction);
            Disable = new Type_Disable(Proxy_Disable);
            Enable = new Type_Enable(Proxy_Enable);
            Exit = new Type_Exit(Proxy_Exit);
            FriendAdded = new Type_FriendAdded(Proxy_FriendAdded);
            FriendAddRequest = new Type_FriendAddRequest(Proxy_FriendAddRequest);
            GroupAddRequest = new Type_GroupAddRequest(Proxy_GroupAddRequest);
            GroupBan = new Type_GroupBan(Proxy_GroupBan);
            GroupMemberDecrease = new Type_GroupMemberDecrease(Proxy_GroupMemberDecrease);
            GroupMemberIncrease = new Type_GroupMemberIncrease(Proxy_GroupMemberIncrease);
            GroupMsg = new Type_GroupMsg(Proxy_GroupMsg);
            Initialize = new Type_Initialize(Proxy_Initialize);
            PrivateMsg = new Type_PrivateMsg(Proxy_PrivateMsg);
            StartUp = new Type_StartUp(Proxy_StartUp);
            Upload = new Type_Upload(Proxy_Upload);
        }

        private int Proxy_AdminChange(int subType, int sendTime, long fromGroup, long beingOperateQQ)
        {
            EventTypeBase e = new()
            {
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private IntPtr Proxy_AppInfoFunction()
        {
            return Marshal.StringToHGlobalAuto($"9,{PluginName}");
        }

        private int Proxy_Disable()
        {
            return SafeInvoke(AppDisabled);
        }

        private int Proxy_Enable()
        {
            return SafeInvoke(AppEnabled);
        }

        private int Proxy_Exit()
        {
            return SafeInvoke(AppExit);
        }

        private int Proxy_FriendAdded(int subType, int sendTime, long fromQQ)
        {
            EventTypeBase e = new()
            {
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_FriendAddRequest(int subType, int sendTime, long fromQQ, IntPtr msg, string responseFlag)
        {
            EventTypeBase e = new()
            {
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_GroupAddRequest(int subType, int sendTime, long fromGroup, long fromQQ, IntPtr msg, string responseFlag)
        {
            EventTypeBase e = new()
            {
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_GroupBan(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration)
        {
            EventTypeBase e = new()
            {
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_GroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            EventTypeBase e = new()
            {
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_GroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            EventTypeBase e = new()
            {
            };

            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int Proxy_GroupMsg(int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, IntPtr msg, int font)
        {
            GroupMessageEvent e = new()
            {
                MessageContent = msg.ToString(Helper.GB18030),
                SenderQQ = fromQQ,
                MessageSendTime = (int)Helper.TimeStamp,
                ThisQQ = AppConfig.Instance.CurrentQQ,
                MessageGroupQQ = fromGroup,
                AnonymousNickname = fromAnonymous,
                FontId = font,
            };

            return SafeInvokeAndFreeMemory(e, ReceiveGroupMsg);
        }

        private int Proxy_Initialize(int authCode)
        {
            return 1;
        }

        private int Proxy_PrivateMsg(int subType, int msgId, long fromQQ, IntPtr msg, int font)
        {
            PrivateMessageEvent e = new()
            {
            };

            return SafeInvokeAndFreeMemory(e, ReceivePrivateMsg);
        }

        private int Proxy_StartUp()
        {
            return SafeInvoke(AppEnabled);
        }

        private int Proxy_Upload(int subType, int sendTime, long fromGroup, long fromQQ, string file)
        {
            EventTypeBase e = new()
            {
            };
            return SafeInvokeAndFreeMemory(e, ReceiveEvent);
        }

        private int SafeInvoke(Delegate? action)
        {
            try
            {
                int? ret = (int?)(action?.DynamicInvoke());
                if (ret.HasValue)
                {
                    return ret.Value;
                }
                return 0;
            }
            catch (Exception exc)
            {
                LogHelper.Error($"Proxy_{action?.Method.Name ?? "Unkown"}", exc);
                return 0;
            }
        }

        private int SafeInvokeAndFreeMemory<T>(T e, Delegate? action) where T : struct
        {
            IntPtr p = IntPtr.Zero;
            try
            {
                p = Marshal.AllocHGlobal(Marshal.SizeOf(e));
                Marshal.StructureToPtr(e, p, false);

                int? ret = (int?)(action?.DynamicInvoke(p));
                if (ret.HasValue)
                {
                    return ret.Value;
                }
                return 0;
            }
            catch (Exception exc)
            {
                LogHelper.Error($"Proxy_{action?.Method.Name ?? "Unkown"}", exc);
                return 0;
            }
            finally
            {
                if (p != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(p);
                }
            }
        }
    }
}