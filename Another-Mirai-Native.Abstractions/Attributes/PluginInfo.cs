using System;
using System.Runtime.CompilerServices;

namespace Another_Mirai_Native.Abstractions.Attributes
{
    /// <summary>
    /// 表示插件的元数据信息，包括其标识符、名称、版本、描述和作者。
    /// </summary>
    /// <remarks>
    /// 使用此类存储和访问插件元数据，用于插件发现、用户界面显示或插件管理等目的。
    /// </remarks>
    /// <param name="appId">
    /// 插件应用程序的唯一标识符。该值用于将插件与其他插件区分开。通常建议使用 反向域名 + 插件用途 命名约定来确保唯一性。
    /// <list type="bullet">
    /// <item><description>示例：</description></item>
    /// <item><description>com.demo.dice</description></item>
    /// <item><description>me.cqp.luohuaming.dice</description></item>
    /// </list>
    /// </param>
    /// <param name="name">插件的显示名称。
    /// <list type="bullet">
    /// <item><description>聊天插件</description></item>
    /// <item><description>查天气</description></item>
    /// </list>
    /// </param>
    /// <param name="version">插件的版本号，通常遵循语义化版本控制规范。</param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginInfo(string appId, string name, string version) : Attribute
    {
        /// <summary>
        /// 完全构造函数，允许同时设置所有属性，包括可选的描述和作者信息。
        /// </summary>
        public PluginInfo(string appId, string name, string version, string? description, string? author)
            : this(appId, name, version)
        {
            Description = description;
            Author = author;
        }

        /// <summary>
        /// 插件应用程序的唯一标识符。该值用于将插件与其他插件区分开。通常建议使用 反向域名 +插件用途 命名约定来确保唯一性。
        /// <list type="bullet">
        /// <item><description>示例：</description></item>
        /// <item><description>com.demo.dice</description></item>
        /// <item><description>me.cqp.luohuaming.dice</description></item>
        /// </list>
        /// </summary>
        public string AppId { get; set; } = appId;

        /// <summary>
        /// 插件的显示名称。
        /// <list type="bullet">
        /// <item><description>聊天插件</description></item>
        /// <item><description>查天气</description></item>
        /// </list>
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// 插件的版本号，通常遵循语义化版本控制规范。
        /// <list type="bullet">
        /// <item><description>1.0.0</description></item>
        /// <item><description>2.1.0b2</description></item>
        /// </list>
        /// </summary>
        public string Version { get; set; } = version;

        /// <summary>
        /// 用于插件显示的描述信息，提供有关插件功能、用途或其他相关信息的简要说明。
        /// </summary>
        /// <remarks>可留空或null，插件信息侧会显示<strong>未提供</strong></remarks>
        public string? Description { get; set; }

        /// <summary>
        /// 用于显示插件的作者。提供有关插件开发者或维护者的信息，通常是一个人名、团队名或组织名。
        /// </summary>
        /// <remarks>可留空或null，插件信息侧会显示<strong>未提供</strong></remarks>
        public string? Author { get; set; }

        internal int AuthCode { get; set; }
    }
}
