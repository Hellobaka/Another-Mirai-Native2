using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions.Services
{
    /// <summary>
    /// 提供与应用程序环境交互的方法，包括检索用户信息和管理插件状态。
    /// </summary>
    /// <remarks>此接口旨在供插件用于访问应用程序特定的数据并控制其生命周期。
    /// </remarks>
    public interface IAppApi
    {
        /// <summary>
        /// 获取框架分配给插件的数据目录
        /// </summary>
        /// <returns>绝对路径</returns>
        string GetAppDirectory();

        /// <summary>
        /// 异步获取框架分配给插件的数据目录
        /// </summary>
        /// <returns>绝对路径</returns>
        Task<string> GetAppDirectoryAsync();

        /// <summary>
        /// 获取当前已登录的QQ号码。
        /// </summary>
        /// <returns>表示已登录用户QQ号码的 64 位整数。如果用户未登录，则返回 0。</returns>
        long GetLoginQQ();

        /// <summary>
        /// 异步获取当前已登录的QQ号码。
        /// </summary>
        /// <returns>表示已登录用户QQ号码的 64 位整数。如果用户未登录，则返回 0。</returns>
        Task<long> GetLoginQQAsync();

        /// <summary>
        /// 获取当前已登录的QQ昵称。
        /// </summary>
        /// <returns>表示已登录用户QQ号码的昵称。如果用户未登录，则返回 <see cref="string">string.Empty</see></returns>
        string GetLoginQQNick();

        /// <summary>
        /// 异步获取当前已登录的QQ昵称。
        /// </summary>
        /// <returns>表示已登录用户QQ号码的昵称。如果用户未登录，则返回 <see cref="string">string.Empty</see></returns>
        Task<string> GetLoginQQNickAsync();

        /// <summary>
        /// 重载当前插件，会导致插件进程终止
        /// </summary>
        void ReloadPlugin();

        /// <summary>
        /// 异步重载当前插件，会导致插件进程终止
        /// </summary>
        Task ReloadPluginAsync();

        /// <summary>
        /// 禁用当前插件，会导致插件进程终止
        /// </summary>
        void DisablePlugin();

        /// <summary>
        /// 异步禁用当前插件，会导致插件进程终止
        /// </summary>
        Task DisablePluginAsync();
    }
}
