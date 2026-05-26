using Another_Mirai_Native.Config;
using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/config")]
    [Authorize]
    public class ConfigController : ControllerBase
    {
        public ConfigController()
        {
            if (AppConfigProperties == null)
            {
                LoadCoreConfigProperties();
                LoadWebUIConfigProperties();
            }

            if (ProtocolConfigDefinitions == null)
            {
                LoadProtocolConfigDefinitions();
            }
        }

        private static PropertyInfo[] AppConfigProperties { get; set; }

        private static string[] AppConfigPropertiesKeys { get; set; }

        private static PropertyInfo[] WebUIConfigProperties { get; set; }

        private static string[] WebUIConfigPropertiesKeys { get; set; }

        private static Dictionary<string, Dictionary<string, (string Title, string Description, string ConfigPath, object DefaultValue)>> ProtocolConfigDefinitions { get; set; }

        [HttpGet("core")]
        [EndpointSummary("获取核心配置")]
        [EndpointDescription("返回所有核心配置项的标题、描述、类型及当前值")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, GetConfigResponseItem>>), StatusCodes.Status200OK)]
        public IActionResult GetCoreConfig()
        {
            Dictionary<string, GetConfigResponseItem> response = new()
            {
                { "AutoConnect", new() { Title = "协议自动连接", Description = "", Value = AppConfig.Instance.AutoConnect } },
                { "AutoProtocol", new() { Title = "自动连接协议", Description = "选择启动时自动连接的协议", Value = AppConfig.Instance.AutoProtocol } },
                { "ReconnectTime", new() { Title = "重新连接间隔时间", Description = "协议失去连接时每次重新连接的间隔时间", Value = AppConfig.Instance.ReconnectTime } },
                { "PluginExitWhenCoreExit", new() { Title = "框架退出时插件自动退出", Description = "主程序退出时自动关闭所有插件", Value = AppConfig.Instance.PluginExitWhenCoreExit } },
                { "RestartPluginIfDead", new() { Title = "插件崩溃时自动重启", Description = "插件发生异常时自动重新启动", Value = AppConfig.Instance.RestartPluginIfDead } },
                { "HeartBeatInterval", new() { Title = "心跳间隔时间", Description = "核心与插件之间的心跳检测间隔", Value = AppConfig.Instance.HeartBeatInterval } },
                { "PluginInvokeTimeout", new() { Title = "插件方法调用超时", Description = "", Value = AppConfig.Instance.PluginInvokeTimeout } },
                { "LoadTimeout", new() { Title = "插件载入超时", Description = "", Value = AppConfig.Instance.LoadTimeout } },
                { "UseDatabase", new() { Title = "日志使用数据库", Description = "是否将日志存储到数据库", Value = AppConfig.Instance.UseDatabase } },
                { "MessageCacheSize", new() { Title = "消息缓存数量", Description = "内存中缓存的消息数量限制", Value = AppConfig.Instance.MessageCacheSize } },
                { "EnableChat", new() { Title = "启用聊天功能", Description = "", Value = AppConfig.Instance.EnableChatImageCacheMaxSizeControl } },
                { "EnableChatImageCacheMaxSizeControl", new() { Title = "启用最大缓存图片体积控制", Description = "缓存文件夹超出体积时，会从最久的图片开始删除", Value = AppConfig.Instance.EnableChatImageCacheMaxSizeControl } },
                { "MaxChatImageCacheFolderSize", new() { Title = "缓存文件夹最大大小", Description = "", Value = AppConfig.Instance.MaxChatImageCacheFolderSize } },
                { "EnableChatImageCacheExpireTimeControl", new() { Title = "启用缓存图片最大储存时限控制", Description = "图片最大保留一定天数后，会从最久的图片开始删除", Value = AppConfig.Instance.EnableChatImageCacheExpireTimeControl } },
                { "ChatImageCacheExpireTime", new() { Title = "缓存图片最大储存时限", Description = "", Value = AppConfig.Instance.ChatImageCacheExpireTime } },
                { "DebugMode", new() { Title = "调试模式", Description = "启用调试模式输出详细信息", Value = AppConfig.Instance.DebugMode } },
                { "ActionAfterOfflineSeconds", new() { Title = "离线操作等待时间", Description = "离线后执行操作的等待时间", Value = AppConfig.Instance.ActionAfterOfflineSeconds } },
                { "OfflineActionSendEmail", new() { Title = "启用离线后邮件发送", Description = "", Value = AppConfig.Instance.OfflineActionSendEmail } },
                { "OfflineActionEmail_SMTPServer", new() { Title = "SMTP 服务器", Description = "", Value = AppConfig.Instance.OfflineActionEmail_SMTPServer } },
                { "OfflineActionEmail_SMTPPort", new() { Title = "SMTP 服务器端口", Description = "", Value = AppConfig.Instance.OfflineActionEmail_SMTPPort } },
                { "OfflineActionEmail_SMTPUsername", new() { Title = "SMTP 用户名", Description = "", Value = AppConfig.Instance.OfflineActionEmail_SMTPUsername } },
                { "OfflineActionEmail_SMTPPassport", new() { Title = "SMTP 授权码", Description = "", Value = AppConfig.Instance.OfflineActionEmail_SMTPPassport } },
                { "OfflineActionEmail_SMTPSenderEmail", new() { Title = "邮件发送方邮箱", Description = "", Value = AppConfig.Instance.OfflineActionEmail_SMTPSenderEmail } },
                { "OfflineActionEmail_SMTPReceiveEmail", new() { Title = "邮件接收方邮箱", Description = "", Value = AppConfig.Instance.OfflineActionEmail_SMTPReceiveEmail } },
                { "OfflineActionRunCommand", new() { Title = "启用离线后执行终端指令", Description = "", Value = AppConfig.Instance.OfflineActionRunCommand } },
                { "OfflineActionCommands", new() { Title = "离线后执行终端指令", Description = "", Value = AppConfig.Instance.OfflineActionCommands } },
            };

            return Ok(ApiResponse.Ok(response));
        }

        [HttpGet("protocol/{name}")]
        [EndpointSummary("获取协议配置")]
        [EndpointDescription("返回指定协议所有可配置项的标题、描述及当前值")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, GetConfigResponseItem>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public IActionResult GetProtocolConfig([Description("协议名称，如 OneBot v11、LagrangeCore 等")] string name)
        {
            if (!ProtocolConfigDefinitions.TryGetValue(name, out var definitions))
            {
                return NotFound(ApiResponse.Error(404, $"未能找到名称为 {name} 的协议配置"));
            }

            Dictionary<string, GetConfigResponseItem> response = [];
            foreach (var item in definitions)
            {
                response[item.Key] = new GetConfigResponseItem
                {
                    Title = item.Value.Title,
                    Description = item.Value.Description,
                    Value = GetProtocolConfigValue(item.Key, item.Value.ConfigPath, item.Value.DefaultValue)
                };
            }

            return Ok(ApiResponse.Ok(response));
        }

        [HttpPost("protocol/{name}")]
        [EndpointSummary("修改协议配置")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public IActionResult SetProtocolConfig(
            [Description("协议名称")] string name,
            [Description("包含 Key（配置项名）和 Value（新值）")][FromBody] SetConfigRequest request)
        {
            if (!ProtocolConfigDefinitions.TryGetValue(name, out var definitions))
            {
                return NotFound(ApiResponse.Error(404, $"未能找到名称为 {name} 的协议配置"));
            }

            if (!definitions.TryGetValue(request.Key, out var definition))
            {
                return NotFound(ApiResponse.Error(404, $"未能找到名称为 {request.Key} 的协议配置项"));
            }

            try
            {
                var valueToSet = DeserializeProtocolConfigValue(request.Value!, definition.DefaultValue);
                CommonConfig.SetConfig(request.Key, valueToSet, definition.ConfigPath);
                return Ok(ApiResponse.Ok());
            }
            catch (Exception e) when (e is FormatException or InvalidCastException or JsonException)
            {
                return BadRequest(ApiResponse.Error(400, "无效的数值转换，检查写入值是否与配置类型匹配"));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse.Error(400, "由于服务器异常，设置协议配置时失败"));
            }
        }

        [HttpPost("core")]
        [EndpointSummary("修改核心配置")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public IActionResult SetCoreConfig(
            [Description("包含 Key（配置项名）和 Value（新值）")][FromBody] SetConfigRequest request)
        {
            if (!AppConfigPropertiesKeys.Contains(request.Key))
            {
                return NotFound(ApiResponse.Error(404, $"未能找到名称为 {request.Key} 的配置项"));
            }

            try
            {
                var configItem = AppConfigProperties.First(p => p.Name == request.Key);
                if (request.Key == nameof(AppConfig.OfflineActionCommands))
                {
                    var list = request.Value.Deserialize<List<string>>();
                    configItem.SetValue(AppConfig.Instance, list);
                    AppConfig.Instance.SetConfig(request.Key, list);
                    return Ok(ApiResponse.Ok());
                }
                var valueToSet = request.Value.Deserialize(configItem.PropertyType);
                configItem.SetValue(AppConfig.Instance, valueToSet);
                AppConfig.Instance.SetConfig(request.Key, valueToSet);
                return Ok(ApiResponse.Ok());
            }
            catch (Exception e) when (e is FormatException or InvalidCastException or JsonException)
            {
                return BadRequest(ApiResponse.Error(400, "无效的数值转换，检查写入值是否与配置类型匹配"));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse.Error(400, "由于服务器异常，设置配置时失败"));
            }
        }

        [HttpGet("webui")]
        [EndpointSummary("获取 WebUI 配置")]
        [EndpointDescription("返回 WebUI 自身的配置项（监听地址、端口、HTTPS、证书路径等），每项含标题、描述和当前值")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, GetConfigResponseItem>>), StatusCodes.Status200OK)]
        public IActionResult GetWebUIConfig()
        {
            Dictionary<string, GetConfigResponseItem> response = new()
            {
                { "ListenIP", new() { Title = "监听 IP", Description = "WebUI 服务监听的 IP 地址，* 表示监听所有地址", Value = WebUIConfig.Instance.ListenIP } },
                { "ListenPort", new() { Title = "监听端口", Description = "WebUI 服务监听的端口号", Value = WebUIConfig.Instance.ListenPort } },
                { "EnableHTTPS", new() { Title = "启用 HTTPS", Description = "是否启用 HTTPS 加密连接", Value = WebUIConfig.Instance.EnableHTTPS } },
                { "CertificatePath", new() { Title = "HTTPS 证书路径", Description = "HTTPS 证书文件（PEM 或 PFX）的存放路径", Value = WebUIConfig.Instance.CertificatePath } },
                { "CertificateKeyPath", new() { Title = "证书密钥路径", Description = "HTTPS 证书密钥文件的存放路径", Value = WebUIConfig.Instance.CertificateKeyPath } },
                { "Password", new() { Title = "登录密码", Description = "WebUI 登录密码", Value = WebUIConfig.Instance.Password } },
                { "EnableChat", new() { Title = "", Description = "", Value = WebUIConfig.Instance.EnableChat } },
                { "EnableFileManager", new() { Title = "", Description = "", Value = WebUIConfig.Instance.EnableFileManager } },
                { "EnableTerminal", new() { Title = "", Description = "", Value = WebUIConfig.Instance.EnableTerminal } },
            };

            return Ok(ApiResponse.Ok(response));
        }

        [HttpPost("webui")]
        [EndpointSummary("修改 WebUI 配置")]
        [EndpointDescription("修改 WebUI 监听地址、端口、HTTPS 证书路径及管理密码。修改后需重启 WebUI 生效")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public IActionResult SetWebUIConfig([Description("包含 Key（配置项名）和 Value（新值）")][FromBody] SetConfigRequest request)
        {
            if (!WebUIConfigPropertiesKeys.Contains(request.Key))
            {
                return NotFound(ApiResponse.Error(404, $"未能找到名称为 {request.Key} 的 WebUI 配置项"));
            }

            try
            {
                var configItem = WebUIConfigProperties.First(p => p.Name == request.Key);

                // 特殊校验
                if (configItem.Name == nameof(WebUIConfig.ListenIP))
                {
                    var ip = request.Value.Deserialize<string>() ?? "";
                    if (IPAddress.TryParse(ip, out _) == false && ip != "*")
                    {
                        return BadRequest(ApiResponse.Error(400, $"无效的 IP 地址：{ip}"));
                    }
                }
                else if (configItem.Name == nameof(WebUIConfig.ListenPort))
                {
                    var port = request.Value.Deserialize<int>();
                    if (port < 0 || port > 65535)
                    {
                        return BadRequest(ApiResponse.Error(400, "端口号需在 0 ~ 65535 之间"));
                    }
                }
                else if (configItem.Name == nameof(WebUIConfig.Password))
                {
                    var pwd = request.Value.Deserialize<string>();
                    if (string.IsNullOrWhiteSpace(pwd))
                    {
                        return BadRequest(ApiResponse.Error(400, "密码不能为空"));
                    }
                }
                else if (configItem.Name == nameof(WebUIConfig.EnableChat)
                    || configItem.Name == nameof(WebUIConfig.EnableFileManager)
                    || configItem.Name == nameof(WebUIConfig.EnableTerminal))
                {
                    return BadRequest(ApiResponse.Error(400, "不允许通过 WebAPI 修改此配置"));
                }

                var valueToSet = request.Value.Deserialize(configItem.PropertyType);
                configItem.SetValue(WebUIConfig.Instance, valueToSet);
                WebUIConfig.Instance.SetConfig(request.Key, valueToSet);
                return Ok(ApiResponse.Ok());
            }
            catch (Exception e) when (e is FormatException or InvalidCastException or JsonException)
            {
                return BadRequest(ApiResponse.Error(400, "无效的数值转换，检查写入值是否与配置类型匹配"));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse.Error(400, "由于服务器异常，设置 WebUI 配置时失败"));
            }
        }

        private static void LoadCoreConfigProperties()
        {
            AppConfigProperties = typeof(AppConfig).GetProperties();
            AppConfigPropertiesKeys = AppConfigProperties.Select(p => p.Name).ToArray();
        }

        private static void LoadWebUIConfigProperties()
        {
            WebUIConfigProperties = typeof(WebUIConfig).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            WebUIConfigPropertiesKeys = WebUIConfigProperties.Select(p => p.Name).ToArray();
        }

        private static void LoadProtocolConfigDefinitions()
        {
            ProtocolConfigDefinitions = new()
            {
                ["Lagrange.Core"] = new()
                {
                    ["SignUrl"] = ("签名服务器 Url", "仅限 Lagrange 签名 与其他的签名服务器不通用", @"conf\LagrangeCore.json", "https://sign.lagrangecore.org/api/sign/30366"),
                    ["SignFallbackPlatform"] = ("登录操作系统", "", @"conf\LagrangeCore.json", "Linux"),
                    ["DebugMode"] = ("调试模式", "会输出更多的调试日志", @"conf\LagrangeCore.json", false),
                },
                ["OneBot v11"] = new()
                {
                    ["WebSocketURL"] = ("正向 WebSocket 服务器 Url", "", @"conf\OneBot_v11.json", ""),
                    ["AuthKey"] = ("鉴权 Token", "", @"conf\OneBot_v11.json", ""),
                    ["MessageType"] = ("消息类型", "大部分情况下请用 Array 类型", @"conf\OneBot_v11.json", "Array"),
                    ["DiscardOfflineMessage"] = ("抛弃离线消息", "只有在收到 Online 元数据时才处理消息", @"conf\OneBot_v11.json", true),
                },
                ["MiraiAPIHttp"] = new()
                {
                    ["WebSocketURL"] = ("正向 WebSocket 服务器 Url", "", @"conf\MiraiAPIHttp.json", ""),
                    ["AuthKey"] = ("鉴权 Token", "", @"conf\MiraiAPIHttp.json", ""),
                    ["QQ"] = ("目标 QQ", "", @"conf\MiraiAPIHttp.json", (long)100000),
                    ["FullMemberInfo"] = ("详细群成员信息", "调用更详细的群成员信息接口 但是可能大幅度加长调用时长", @"conf\MiraiAPIHttp.json", false),
                },
                ["NoConnection"] = new()
                {
                    ["PicServerListenIP"] = ("图片服务器监听 IP", "", @"conf\NoConnection_ProtocolConfig.json", "127.0.0.1"),
                    ["PicServerListenPort"] = ("图片服务器监听端口", "", @"conf\NoConnection_ProtocolConfig.json", (ushort)45000),
                    ["NoConnection_Nick"] = ("仿真账号昵称", "", @"conf\NoConnection_ProtocolConfig.json", "测试账号9"),
                    ["NoConnection_QQ"] = ("仿真账号 QQ", "", @"conf\NoConnection_ProtocolConfig.json", (long)999999999),
                    ["ShowTestDialog"] = ("是否显示仿真窗口", "", @"conf\NoConnection_ProtocolConfig.json", true),
                    ["BuildTestPicServer"] = ("是否使用本地图片服务器", "用于仿真 cqimg 文件", @"conf\NoConnection_ProtocolConfig.json", false),
                }
            };
        }

        private static object GetProtocolConfigValue(string key, string configPath, object defaultValue)
        {
            return defaultValue switch
            {
                bool value => CommonConfig.GetConfig(key, configPath, value),
                int value => CommonConfig.GetConfig(key, configPath, value),
                long value => CommonConfig.GetConfig(key, configPath, value),
                ushort value => CommonConfig.GetConfig(key, configPath, value),
                string value => CommonConfig.GetConfig(key, configPath, value),
                _ => defaultValue,
            };
        }

        private static object DeserializeProtocolConfigValue(JsonNode value, object defaultValue)
        {
            return defaultValue switch
            {
                bool _ => value.Deserialize<bool>(),
                int _ => value.Deserialize<int>(),
                long _ => value.Deserialize<long>(),
                ushort _ => value.Deserialize<ushort>(),
                string _ => value.Deserialize<string>() ?? string.Empty,
                _ => value.Deserialize(defaultValue.GetType()) ?? defaultValue,
            };
        }
    }
}