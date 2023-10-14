using Newtonsoft.Json.Linq;

namespace Another_Mirai_Native.Config
{
    /// <summary>
    /// 配置读取帮助类
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="sectionName">需要读取的配置键名</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>目标类型的配置</returns>
        public static T GetConfig<T>(string sectionName, string configPath = @"conf/Config.json", T defaultValue = default)
        {
            if (Directory.Exists("conf") is false)
            {
                Directory.CreateDirectory("conf");
            }

            if (File.Exists(configPath) is false)
            {
                File.WriteAllText(configPath, "{}");
            }

            var o = JObject.Parse(File.ReadAllText(configPath));
            if (o.ContainsKey(sectionName))
            {
                return o[sectionName]!.ToObject<T>();
            }

            if (defaultValue != null)
            {
                SetConfig<T>(sectionName, defaultValue, configPath);
                return defaultValue;
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object)"";
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(object)0;
            }

            if (typeof(T) == typeof(long))
            {
                return default;
            }

            if (typeof(T) == typeof(bool))
            {
                return (T)(object)false;
            }

            return typeof(T) == typeof(object) ? (T)(object)new { } : throw new Exception("无法默认返回");
        }

        public static void SetConfig<T>(string sectionName, T value, string configPath = @"conf/Config.json")
        {
            if (File.Exists(configPath) is false)
            {
                File.WriteAllText(configPath, "{}");
            }

            var o = JObject.Parse(File.ReadAllText(configPath));
            if (o.ContainsKey(sectionName))
            {
                o[sectionName] = JToken.FromObject(value);
            }
            else
            {
                o.Add(sectionName, JToken.FromObject(value));
            }
            File.WriteAllText(configPath, o.ToString(Newtonsoft.Json.Formatting.Indented));
        }
    }
}