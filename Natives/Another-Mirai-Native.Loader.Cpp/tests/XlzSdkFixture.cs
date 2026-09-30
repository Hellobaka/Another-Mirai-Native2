using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using SDK.Core;
using SDK.Events;
using SDK.Enum;

// Compiled against the user's original SDK, without copying loader ABI types.
namespace Amn.XlzSdkTests
{
    public static class Fixture
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int EventCallback(IntPtr value);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int LifecycleCallback();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void VoidCallback();
        static readonly EventCallback privateCallback = Private;
        static readonly EventCallback groupCallback = Group;
        static readonly EventCallback eventCallback = Event;
        static readonly LifecycleCallback enableCallback = () => 1;
        static readonly LifecycleCallback menuCallback = () => { menuCalls++; return 0; };
        static readonly VoidCallback disableCallback = () => { };
        static readonly VoidCallback exitCallback = () => { };
        static readonly API api = new API();
        static int menuCalls;

        static void Check(bool condition, string name)
        {
            if (!condition) throw new InvalidOperationException("SDK assertion failed: " + name);
        }

        // Native shim supplies pointers to its input strings and output slot.
        public static int Initialize(string context)
        {
            try
            {
                var pointers = context.Split('|');
                API.jsonstr = Marshal.PtrToStringAnsi(new IntPtr(long.Parse(pointers[0])));
                API.pluginkey = Marshal.PtrToStringAnsi(new IntPtr(long.Parse(pointers[1])));
                Check(JObject.Parse(API.jsonstr).Count == 399, "API table");
                Check(Marshal.SizeOf(typeof(PrivateMessageEvent)) == 128, "private size");
                Check(Marshal.OffsetOf(typeof(PrivateMessageEvent), "MessageContent").ToInt32() == 68, "private content offset");
                Check(Marshal.SizeOf(typeof(GroupMessageEvent)) == 120, "group size");
                Check(Marshal.OffsetOf(typeof(GroupMessageEvent), "MessageContent").ToInt32() == 76, "group content offset");
                Check(Marshal.SizeOf(typeof(EventTypeBase)) == 68, "event size");
                Check(api.GetThisQQ() == "1111", "account");
                api.OutLog("fixture");
                Check(!string.IsNullOrEmpty(api.SendPrivateMessage(1111, 3333, "你好[@3333][bq1]")), "send + encoding");
                Check(api.SetAdministratorEvent(1111, 4444, 3333, true), "BOOL return");
                var friends = api.GetFriendList(1111);
                Check(friends.Count == 1 && friends[0].QQNumber == 3333 && friends[0].Name == "测试好友", "friend list");
                var groups = api.Getgrouplist(1111);
                Check(groups.Count == 1 && groups[0].GroupQQ == 4444 && groups[0].GroupName == "测试群", "group list");
                var members = api.GetgroupMemberlist(1111, 4444);
                Check(members.Count == 1 && members[0].QQNumber == "3333" && members[0].Nickname == "card", "member list");
                Check(!api.GroupPermission_UploadFile(1111, 4444, true), "unimplemented BOOL");
                Check(api.GetSKey(1111, "example.invalid") == "", "unimplemented string");
                var info = new AppInfo {
                    appname = "XLZ SDK ABI fixture", author = "AMN tests", appv = "1.0", describe = "SDK compatibility checks",
                    friendmsaddres = Marshal.GetFunctionPointerForDelegate(privateCallback).ToInt64(),
                    groupmsaddres = Marshal.GetFunctionPointerForDelegate(groupCallback).ToInt64(),
                    eventmsaddres = Marshal.GetFunctionPointerForDelegate(eventCallback).ToInt64(),
                    useproaddres = Marshal.GetFunctionPointerForDelegate(enableCallback).ToInt64(),
                    banproaddres = Marshal.GetFunctionPointerForDelegate(disableCallback).ToInt64(),
                    unitproaddres = Marshal.GetFunctionPointerForDelegate(exitCallback).ToInt64(),
                    setproaddres = Marshal.GetFunctionPointerForDelegate(menuCallback).ToInt64()
                };
                Marshal.WriteIntPtr(new IntPtr(long.Parse(pointers[2])), Marshal.StringToCoTaskMemAnsi(info.Info(info)));
                return 1;
            }
            catch (Exception error) { Console.Error.WriteLine(error); return 0; }
        }

        static int Private(IntPtr pointer)
        {
            try {
                var value = (PrivateMessageEvent)Marshal.PtrToStructure(pointer, typeof(PrivateMessageEvent));
                Check(value.ThisQQ == 2222 && value.SenderQQ == 3333 && value.MessageReq == 42, "private numbers");
                Check((int)value.MessageType == 166 && value.MessageContent == "你好[@3333][bq1]", "private content");
                Check(value.SessionToken == IntPtr.Zero && value.FileMD5 == null, "absent byte sets");
                return 0;
            } catch (Exception error) { Console.Error.WriteLine(error); return -81; }
        }

        static int Group(IntPtr pointer)
        {
            try {
                var value = (GroupMessageEvent)Marshal.PtrToStructure(pointer, typeof(GroupMessageEvent));
                Check(value.ThisQQ == 2222 && value.SenderQQ == 3333 && value.MessageGroupQQ == 4444, "group numbers");
                Check((int)value.MessageType == 134 && value.FontId == 7 && value.MessageContent == "[Reply,Req=9]群消息", "group content");
                return 0;
            } catch (Exception error) { Console.Error.WriteLine(error); return -82; }
        }

        static int Event(IntPtr pointer)
        {
            try {
                var value = (EventTypeBase)Marshal.PtrToStructure(pointer, typeof(EventTypeBase));
                Check(value.ThisQQ == 2222 && value.TriggerQQ == 3333, "event numbers");
                if ((int)value.EventType == 105)
                    api.FriendVerificationEvent(value.ThisQQ, value.TriggerQQ, value.MessageSeq, (FriendVerificationOperateEnum)1);
                return 0;
            } catch (Exception error) { Console.Error.WriteLine(error); return -83; }
        }
        public static int MenuCalls(string unused) { return menuCalls; }
    }
}
