using Another_Mirai_Native;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Another_Mirai_Native.Model;

#if NET5_0_OR_GREATER
using ModelContextProtocol.Server;
#endif

namespace Protocol_NoConnection
{
    #if NET5_0_OR_GREATER
    /// <summary>
    /// NoConnection 协议的 MCP 工具集
    /// 提供消息模拟发送和日志查询能力
    /// </summary>
    [McpServerToolType]
    public static class MCPTools
    {
        /// <summary>
        /// 在 NoConnection 测试协议中模拟接收一条消息。
        /// 支持群聊消息和私聊消息，可选自动撤回和指定消息ID。
        /// </summary>
        [McpServerTool, Description("在 NoConnection 测试协议中模拟接收一条消息。支持群聊和私聊，可选自动撤回。")]
        public static string SendMessage(
            [Description("是否为私聊消息，true=私聊，false=群聊")] bool isPrivateChat,
            [Description("群组ID（私聊时此参数可填0）")] long groupId,
            [Description("发言者QQ号")] long senderId,
            [Description("消息内容文本，支持 CQ 码")] string message,
            [Description("是否自动撤回消息（可选，默认 false）")] bool autoRevoke = false,
            [Description("自动撤回延迟秒数（可选，默认 10）")] int autoRevokeSeconds = 10,
            [Description("指定消息ID（可选，不指定则自动生成）")] int? messageId = null)
        {
            if (senderId <= 0)
            {
                return "错误: senderId 无效";
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                return "错误: 消息内容不能为空";
            }
            if (!isPrivateChat && groupId <= 0)
            {
                return "错误: 群聊模式下 groupId 无效";
            }

            int msgId = messageId ?? Interlocked.Increment(ref Protocol.Instance.MCPMsgIdCounter);
            if (msgId < 0)
            {
                msgId = Interlocked.Increment(ref Protocol.Instance.MCPMsgIdCounter);
            }

            RequestCache.AddMessageCache(msgId, message);

            int logId;
            CQPluginProxy handledPlugin;

            if (isPrivateChat)
            {
                logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架(MCP)", "[↓]收到好友消息",
                    $"QQ:{senderId} 消息: {message}", "处理中...");
                handledPlugin = PluginManagerProxy.Instance.Event_OnPrivateMsg(11, msgId, senderId, message, 0, DateTime.Now);
            }
            else
            {
                logId = LogHelper.WriteLog(LogLevel.InfoReceive, "AMN框架(MCP)", "[↓]收到消息",
                    $"群:{groupId} QQ:{senderId} 消息: {message}", "处理中...");
                handledPlugin = PluginManagerProxy.Instance.Event_OnGroupMsg(1, msgId, groupId, senderId, "", message, 0, DateTime.Now);
            }

            // 自动撤回
            if (autoRevoke && autoRevokeSeconds > 0)
            {
                int capturedMsgId = msgId;
                long capturedGroupId = groupId;
                long capturedSenderId = senderId;
                bool capturedIsPrivate = isPrivateChat;
                _ = Task.Run(() =>
                {
                    Thread.Sleep(autoRevokeSeconds * 1000);
                    if (capturedIsPrivate)
                    {
                        PluginManagerProxy.Instance.Event_OnPrivateMsgRecall(capturedMsgId, capturedSenderId, message);
                    }
                    else
                    {
                        PluginManagerProxy.Instance.Event_OnGroupMsgRecall(capturedMsgId, capturedGroupId, message);
                    }
                });
            }

            string updateMsg = "√ 处理完成";
            if (handledPlugin != null)
            {
                updateMsg += $" (由 {handledPlugin.AppInfo.name} 处理)";
            }
            LogHelper.UpdateLogStatus(logId, updateMsg);

            return $"消息已发送 | MsgId={msgId} | 类型={(isPrivateChat ? "私聊" : "群聊")} | " +
                   $"发送者={senderId} | 群组={groupId} | 处理者={handledPlugin?.AppInfo?.name ?? "无"}";
        }

        /// <summary>
        /// 获取 AMN 框架最近的运行日志条目，用于诊断和监控。
        /// </summary>
        [McpServerTool, Description("获取 AMN 框架最近的运行日志条目，用于诊断和监控。")]
        public static LogModel[] GetLatestLogs(
            [Description("要获取的日志条目数量（默认 10，最大 100）")] int logEntryCount = 10)
        {
            logEntryCount = Math.Clamp(logEntryCount, 1, 100);

            var logs = LogHelper.GetDisplayLogs(0, logEntryCount);
            if (logs == null || logs.Count == 0)
            {
                logs = LogHelper.DetailQueryLogs(0, 1, logEntryCount, "", out _, out _, null, null, false);
            }

            if (logs == null || logs.Count == 0)
            {
                return [];
            }

            return logs.ToArray();
        }

        /// <summary>
        /// 添加或重新上传插件到框架。
        /// 将指定的 DLL 和 JSON 文件复制到插件目录并加载元数据。
        /// </summary>
        [McpServerTool, Description("添加或重新上传插件。将 DLL 和 JSON 文件复制到插件目录并注册。")]
        public static string AddPlugin(
            [Description("插件 DLL 文件的绝对路径")] string dllPath,
            [Description("插件 JSON 配置文件的绝对路径")] string jsonPath)
        {
            if (string.IsNullOrWhiteSpace(dllPath))
            {
                return "错误: DLL 路径不能为空";
            }
            if (string.IsNullOrWhiteSpace(jsonPath))
            {
                return "错误: JSON 路径不能为空";
            }
            if (!File.Exists(dllPath))
            {
                return $"错误: DLL 文件不存在: {dllPath}";
            }
            if (!File.Exists(jsonPath))
            {
                return $"错误: JSON 文件不存在: {jsonPath}";
            }

            try
            {
                var (success, alreadyExisted) = PluginManagerProxy.Instance.AddPlugin(dllPath);

                if (success)
                {
                    if (alreadyExisted)
                    {
                        return $"插件已更新: {Path.GetFileName(dllPath)}。请使用重载插件功能使其生效。";
                    }
                    return $"插件添加成功: {Path.GetFileName(dllPath)}。请在插件列表中启用它。";
                }
                else
                {
                    return $"插件添加失败: {Path.GetFileName(dllPath)}。请检查日志获取详细错误信息。";
                }
            }
            catch (Exception ex)
            {
                return $"添加插件时发生异常: {ex.Message}";
            }
        }

        /// <summary>
        /// 列举所有已加载的插件信息（包括启用和未启用的）。
        /// </summary>
        [McpServerTool, Description("获取所有已加载插件的详细信息，包括启用状态、名称、AuthCode 等。")]
        public static string ListPlugins()
        {
            var proxies = PluginManagerProxy.Proxies;
            if (proxies == null || proxies.Count == 0)
            {
                return "当前没有已加载的插件。";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"=== 插件列表（共 {proxies.Count} 个）===");
            sb.AppendLine();

            int index = 1;
            foreach (var plugin in proxies)
            {
                sb.AppendLine($"--- 插件 {index++} ---");
                sb.AppendLine($"  名称: {plugin.PluginName}");
                sb.AppendLine($"  AppId: {plugin.PluginId}");
                sb.AppendLine($"  AuthCode: {plugin.AppInfo.AuthCode}");
                sb.AppendLine($"  版本: {plugin.Version}");
                sb.AppendLine($"  作者: {plugin.Author}");
                sb.AppendLine($"  描述: {plugin.Description}");
                sb.AppendLine($"  状态: {(plugin.Enabled ? "✅ 已启用" : "❌ 未启用")}");
                sb.AppendLine($"  类型: {plugin.PluginType}");
                sb.AppendLine($"  路径: {plugin.PluginBasePath}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// 重新加载指定的插件（通过 AuthCode 查找）。
        /// </summary>
        [McpServerTool, Description("通过 AuthCode 重新加载指定的插件。插件必须处于启用状态。")]
        public static string ReloadPlugin(
            [Description("插件的 AuthCode（唯一标识码）")] int authCode)
        {
            if (authCode <= 0)
            {
                return "错误: AuthCode 无效";
            }

            var plugin = PluginManagerProxy.GetProxyByAuthCode(authCode);
            if (plugin == null)
            {
                return $"错误: 未找到 AuthCode 为 {authCode} 的插件";
            }
            if (!plugin.Enabled)
            {
                return $"错误: 插件 \"{plugin.PluginName}\" 处于禁用状态，无法重载。请先启用插件。";
            }

            try
            {
                PluginManagerProxy.Instance.ReloadPlugin(plugin);
                return $"插件 \"{plugin.PluginName}\" (AuthCode: {authCode}) 已重新加载。";
            }
            catch (Exception ex)
            {
                return $"重载插件时发生异常: {ex.Message}";
            }
        }

        /// <summary>
        /// 禁用指定的插件（通过 AuthCode 查找）。
        /// </summary>
        [McpServerTool, Description("通过 AuthCode 禁用指定的插件。")]
        public static string DisablePlugin(
            [Description("插件的 AuthCode（唯一标识码）")] int authCode)
        {
            if (authCode <= 0)
            {
                return "错误: AuthCode 无效";
            }

            var plugin = PluginManagerProxy.GetProxyByAuthCode(authCode);
            if (plugin == null)
            {
                return $"错误: 未找到 AuthCode 为 {authCode} 的插件";
            }
            if (!plugin.Enabled)
            {
                return $"插件 \"{plugin.PluginName}\" 已经处于禁用状态。";
            }

            try
            {
                bool success = PluginManagerProxy.Instance.SetPluginEnabled(plugin, false);
                if (success)
                {
                    return $"插件 \"{plugin.PluginName}\" (AuthCode: {authCode}) 已禁用。";
                }
                else
                {
                    return $"禁用插件 \"{plugin.PluginName}\" 失败。请检查日志获取详细错误信息。";
                }
            }
            catch (Exception ex)
            {
                return $"禁用插件时发生异常: {ex.Message}";
            }
        }

        /// <summary>
        /// 启用指定的插件（通过中文名称查找）。
        /// </summary>
        [McpServerTool, Description("通过插件的中文名称启用指定的插件。")]
        public static string EnablePlugin(
            [Description("插件的中文名称（与插件 JSON 中的 name 字段匹配）")] string pluginName)
        {
            if (string.IsNullOrWhiteSpace(pluginName))
            {
                return "错误: 插件名称不能为空";
            }

            var plugin = PluginManagerProxy.Proxies.FirstOrDefault(x =>
                x.AppInfo.name == pluginName || x.PluginName == pluginName);
            if (plugin == null)
            {
                return $"错误: 未找到名称为 \"{pluginName}\" 的插件。请检查名称是否正确（区分大小写）。";
            }
            if (plugin.Enabled)
            {
                return $"插件 \"{plugin.PluginName}\" 已经处于启用状态。";
            }

            try
            {
                // 确保插件文件在临时目录中且元数据已加载
                if (!plugin.MovePluginToTmpDir() || !plugin.LoadAppInfo())
                {
                    return $"启用插件 \"{plugin.PluginName}\" 失败: 无法准备插件文件或读取元数据。";
                }

                bool success = PluginManagerProxy.Instance.SetPluginEnabled(plugin, true);
                if (success)
                {
                    return $"插件 \"{plugin.PluginName}\" (AuthCode: {plugin.AppInfo.AuthCode}) 已启用。";
                }
                else
                {
                    return $"启用插件 \"{plugin.PluginName}\" 失败。请检查日志获取详细错误信息。";
                }
            }
            catch (Exception ex)
            {
                return $"启用插件时发生异常: {ex.Message}";
            }
        }
    }
#else
public static class MCPTools
{
        public static string SendMessage(
            [Description("是否为私聊消息，true=私聊，false=群聊")] bool isPrivateChat,
            [Description("群组ID（私聊时此参数可填0）")] long groupId,
            [Description("发言者QQ号")] long senderId,
            [Description("消息内容文本，支持 CQ 码")] string message,
            [Description("是否自动撤回消息（可选，默认 false）")] bool autoRevoke = false,
            [Description("自动撤回延迟秒数（可选，默认 10）")] int autoRevokeSeconds = 10,
            [Description("指定消息ID（可选，不指定则自动生成）")] int? messageId = null)
        {
            return "此功能需要 .NET 5.0 或更高版本";
        }

        public static string GetLatestLogs(
            [Description("要获取的日志条目数量（默认 10，最大 100）")] int logEntryCount = 10)
        {
            return "此功能需要 .NET 5.0 或更高版本";
        }

        public static string AddPlugin(
            [Description("插件 DLL 文件的绝对路径")] string dllPath,
            [Description("插件 JSON 配置文件的绝对路径")] string jsonPath)
        {
            return "此功能需要 .NET 5.0 或更高版本";
        }

        public static string ListPlugins()
        {
            return "此功能需要 .NET 5.0 或更高版本";
        }

        public static string ReloadPlugin(
            [Description("插件的 AuthCode（唯一标识码）")] int authCode)
        {
            return "此功能需要 .NET 5.0 或更高版本";
        }

        public static string DisablePlugin(
            [Description("插件的 AuthCode（唯一标识码）")] int authCode)
        {
            return "此功能需要 .NET 5.0 或更高版本";
        }

        public static string EnablePlugin(
            [Description("插件的中文名称（与插件 JSON 中的 name 字段匹配）")] string pluginName)
        {
            return "此功能需要 .NET 5.0 或更高版本";
        }
}
#endif
}
