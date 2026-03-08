using Another_Mirai_Native.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions
{
    internal static class PluginContext
    {
        internal static ILogger Logger { get; set; }
       
        internal static IFriendApi FriendApi { get; set; }
       
        internal static IGroupApi GroupApi { get; set; }
       
        internal static IAppApi AppApi { get; set; }
       
        internal static IMessageApi MessageApi { get; set; }
    }
}
