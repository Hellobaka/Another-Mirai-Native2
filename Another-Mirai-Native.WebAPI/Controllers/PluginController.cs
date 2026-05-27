using Another_Mirai_Native.Config;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/plugin")]
    [Authorize]
    public class PluginController(ILogger<PluginController> logger) : ControllerBase
    {
        private readonly ILogger<PluginController> _logger = logger;

        [HttpGet("list")]
        [EndpointSummary("获取插件列表")]
        [EndpointDescription("返回所有已加载插件的名称、版本、启用状态等完整信息")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PluginDto>>), StatusCodes.Status200OK)]
        public IActionResult ListPlugins()
        {
            _logger.LogInformation("获取插件列表");
            var plugins = PluginManagerProxy.Proxies.Select(PluginDto.CreateFromPlugin).ToList();
            _logger.LogInformation("获取插件列表成功，共 {Count} 个插件", plugins.Count);
            return Ok(ApiResponse.Ok(plugins));
        }

        [HttpGet("{authCode}/info")]
        [EndpointSummary("获取插件详细信息")]
        [ProducesResponseType(typeof(ApiResponse<PluginDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public IActionResult GetPluginAppInfo([Description("插件授权码")] int authCode)
        {
            _logger.LogInformation("获取插件信息: AuthCode={AuthCode}", authCode);
            var plugin = PluginManagerProxy.Proxies.FirstOrDefault(x => x.AppInfo.AuthCode == authCode);
            if (plugin == null)
            {
                _logger.LogWarning("获取插件信息失败：未找到插件 AuthCode={AuthCode}", authCode);
                return NotFound(ApiResponse.Error(404, "未找到对应 AuthCode 的插件"));
            }

            _logger.LogInformation("获取插件信息成功: AuthCode={AuthCode}, Name={Name}", authCode, plugin.PluginName);
            return Ok(ApiResponse.Ok(PluginDto.CreateFromPlugin(plugin)));
        }

        [HttpPost("{authCode}/enable")]
        [EndpointSummary("启用插件")]
        [ProducesResponseType(typeof(ApiResponse<PluginDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetPluginEnabled([Description("插件授权码")] int authCode)
        {
            _logger.LogInformation("启用插件: AuthCode={AuthCode}", authCode);
            var plugin = PluginManagerProxy.Proxies.FirstOrDefault(x => x.AppInfo.AuthCode == authCode);
            if (plugin == null)
            {
                _logger.LogWarning("启用插件失败：未找到插件 AuthCode={AuthCode}", authCode);
                return NotFound(ApiResponse.Error(404, "未找到对应 AuthCode 的插件"));
            }

            var success = await Task.Run(() => PluginManagerProxy.Instance.SetPluginEnabled(plugin, true));
            if (success)
            {
                AppConfig.Instance.AutoEnablePlugin.Add(plugin.PluginName);
                AppConfig.Instance.AutoEnablePlugin = AppConfig.Instance.AutoEnablePlugin.Distinct().ToList();
                AppConfig.Instance.SetConfig("AutoEnablePlugins", AppConfig.Instance.AutoEnablePlugin);

                _logger.LogInformation("启用插件成功: AuthCode={AuthCode}, Name={Name}", authCode, plugin.PluginName);
                return Ok(ApiResponse.Ok(PluginDto.CreateFromPlugin(plugin)));
            }
            _logger.LogError("启用插件失败：SetPluginEnabled 返回 false AuthCode={AuthCode}", authCode);
            return BadRequest(ApiResponse.Error(400, "使插件启动失败"));
        }

        [HttpPost("{authCode}/disable")]
        [EndpointSummary("禁用插件")]
        [ProducesResponseType(typeof(ApiResponse<PluginDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetPluginDisabled([Description("插件授权码")] int authCode)
        {
            _logger.LogInformation("禁用插件: AuthCode={AuthCode}", authCode);
            var plugin = PluginManagerProxy.Proxies.FirstOrDefault(x => x.AppInfo.AuthCode == authCode);
            if (plugin == null)
            {
                _logger.LogWarning("禁用插件失败：未找到插件 AuthCode={AuthCode}", authCode);
                return NotFound(ApiResponse.Error(404, "未找到对应 AuthCode 的插件"));
            }

            var success = await Task.Run(() => PluginManagerProxy.Instance.SetPluginEnabled(plugin, false));
            if (success)
            {
                AppConfig.Instance.AutoEnablePlugin.Remove(plugin.PluginName);
                AppConfig.Instance.AutoEnablePlugin = AppConfig.Instance.AutoEnablePlugin.Distinct().ToList();
                AppConfig.Instance.SetConfig("AutoEnablePlugins", AppConfig.Instance.AutoEnablePlugin);

                _logger.LogInformation("禁用插件成功: AuthCode={AuthCode}, Name={Name}", authCode, plugin.PluginName);
                return Ok(ApiResponse.Ok(PluginDto.CreateFromPlugin(plugin)));
            }
            _logger.LogError("禁用插件失败：SetPluginEnabled 返回 false AuthCode={AuthCode}", authCode);
            return BadRequest(ApiResponse.Error(400, "使插件停止失败"));
        }

        [HttpPost("{authCode}/reload")]
        [EndpointSummary("重载插件")]
        [EndpointDescription("重新启动指定插件，仅对已启用的插件生效")]
        [ProducesResponseType(typeof(ApiResponse<PluginDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReloadPlugin([Description("插件授权码")] int authCode)
        {
            _logger.LogInformation("重载插件: AuthCode={AuthCode}", authCode);
            var plugin = PluginManagerProxy.Proxies.FirstOrDefault(x => x.AppInfo.AuthCode == authCode);
            if (plugin == null)
            {
                _logger.LogWarning("重载插件失败：未找到插件 AuthCode={AuthCode}", authCode);
                return NotFound(ApiResponse.Error(404, "未找到对应 AuthCode 的插件"));
            }

            if (!plugin.Enabled)
            {
                _logger.LogWarning("重载插件失败：插件处于禁用状态 AuthCode={AuthCode}, Name={Name}", authCode, plugin.PluginName);
                return BadRequest(ApiResponse.Error(400, $"插件 {plugin.PluginName} 处于禁用状态，无法重启"));
            }

            await Task.Run(() => PluginManagerProxy.Instance.ReloadPlugin(plugin));
            _logger.LogInformation("重载插件成功: AuthCode={AuthCode}, Name={Name}", authCode, plugin.PluginName);
            return Ok(ApiResponse.Ok(PluginDto.CreateFromPlugin(plugin)));
        }

        [HttpPost("reload-all")]
        [EndpointSummary("重载全部插件")]
        [EndpointDescription("重新启动所有已加载的插件")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PluginDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReloadAllPlugin()
        {
            _logger.LogInformation("重载全部插件");
            await Task.Run(() => PluginManagerProxy.Instance.ReloadAllPlugins());
            _logger.LogInformation("重载全部插件完成");
            return ListPlugins();
        }
    }
}