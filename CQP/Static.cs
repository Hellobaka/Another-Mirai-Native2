using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Another_Mirai_Native.Export
{
    public static class Static
    {
        public static int ToInt(this object o, int defaultValue = 0) => o != null && int.TryParse(o.ToString(), out int value) ? value : defaultValue;

        public static long ToLong(this object o, long defaultValue = 0) => o != null && long.TryParse(o.ToString(), out long value) ? value : defaultValue;
    
        public static Random Random => new();
    }
}