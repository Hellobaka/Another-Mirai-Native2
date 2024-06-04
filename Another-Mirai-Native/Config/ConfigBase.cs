using Newtonsoft.Json.Linq;
using System.IO;

namespace Another_Mirai_Native.Config
{
    /// <summary>
    /// 配置读取帮助类
    /// </summary>
    public class ConfigBase
    {
        public ConfigBase(string configPath)
        {
            ConfigPath = configPath;
        }

        public string ConfigPath { get; set; } = @"conf/Config.json";

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="sectionName">需要读取的配置键名</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>目标类型的配置</returns>
        public T GetConfig<T>(string sectionName, T defaultValue = default)
        {
            if (Directory.Exists(Path.GetDirectoryName(ConfigPath)) is false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
            }

            if (File.Exists(ConfigPath) is false)
            {
                File.WriteAllText(ConfigPath, "{}");
            }

            var o = JObject.Parse(File.ReadAllText(ConfigPath));
            if (o.ContainsKey(sectionName))
            {
                return o[sectionName]!.ToObject<T>();
            }

            if (defaultValue != null)
            {
                SetConfig<T>(sectionName, defaultValue);
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

        public void SetConfig<T>(string sectionName, T value)
        {
            if (File.Exists(ConfigPath) is false)
            {
                File.WriteAllText(ConfigPath, "{}");
            }

            var o = JObject.Parse(File.ReadAllText(ConfigPath));
            if (o.ContainsKey(sectionName))
            {
                o[sectionName] = JToken.FromObject(value);
            }
            else
            {
                o.Add(sectionName, JToken.FromObject(value));
            }
            File.WriteAllText(ConfigPath, o.ToString(Newtonsoft.Json.Formatting.Indented));
        }
    }
}