namespace Another_Mirai_Native.Config
{
    public static class CommonConfig
    {
        public static T GetConfig<T>(string key, string path, T defaultValue = default)
        {
            return new ConfigBase(path).GetConfig(key, defaultValue);
        }

        public static void SetConfig<T>(string key, T value, string path)
        {
            new ConfigBase(path).SetConfig(key, value);
        }
    }
}
