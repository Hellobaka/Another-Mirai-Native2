namespace Another_Mirai_Native.Export
{
    public static class Static
    {
        public static int ToInt(this object o, int defaultValue = 0) => o != null && int.TryParse(o.ToString(), out int value) ? value : defaultValue;

        public static long ToLong(this object o, long defaultValue = 0) => o != null && long.TryParse(o.ToString(), out long value) ? value : defaultValue;
    }
}
