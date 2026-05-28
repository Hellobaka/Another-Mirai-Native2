using Another_Mirai_Native.Abstractions.Enums;
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

        [HttpPost("{authCode}/menu")]
        [EndpointSummary("调用插件菜单事件")]
        [EndpointDescription("根据授权码触发插件的菜单事件")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CallPluginMenu(
            [Description("插件授权码")] int authCode,
            [Description("菜单名称")] string menuName)
        {
            _logger.LogInformation("调用插件菜单: AuthCode={AuthCode}, MenuName={MenuName}", authCode, menuName);
            var plugin = PluginManagerProxy.Proxies.FirstOrDefault(x => x.AppInfo.AuthCode == authCode);
            if (plugin == null)
            {
                _logger.LogWarning("调用插件菜单失败：未找到插件 AuthCode={AuthCode}", authCode);
                return NotFound(ApiResponse.Error(404, "未找到对应 AuthCode 的插件"));
            }
            if (string.IsNullOrWhiteSpace(menuName))
            {
                _logger.LogWarning("调用插件菜单失败：菜单名称为空");
                return BadRequest(ApiResponse.Error(400, "菜单名称不能为空"));
            }

            _ = Task.Run(() => PluginManagerProxy.Instance.InvokeEvent(plugin, PluginEventType.Menu, menuName));
            _logger.LogInformation("调用插件菜单完成: AuthCode={AuthCode}, MenuName={MenuName}", authCode, menuName);
            return Ok(ApiResponse.Ok());
        }

        [HttpPost("add")]
        [EndpointSummary("添加插件")]
        [EndpointDescription("上传 DLL 与 JSON 文件以添加新插件")]
        [ProducesResponseType(typeof(ApiResponse<PluginDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<IActionResult> AddPlugin(
            [Description("插件 DLL 文件")] IFormFile dll,
            [Description("插件 JSON 清单文件")] IFormFile json)
        {
            _logger.LogInformation("添加插件: Dll={Dll}, Json={Json}", dll?.FileName, json?.FileName);
            if (dll == null || dll.Length == 0)
            {
                _logger.LogWarning("添加插件失败：DLL 文件为空");
                return BadRequest(ApiResponse.Error(400, "请选择要上传的插件 DLL 文件"));
            }
            if (!string.Equals(Path.GetExtension(dll.FileName), ".dll", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("添加插件失败：文件不是 DLL FileName={FileName}", dll.FileName);
                return BadRequest(ApiResponse.Error(400, "仅支持 DLL 格式的插件文件"));
            }
            if (json == null || json.Length == 0)
            {
                _logger.LogWarning("添加插件失败：JSON 文件为空");
                return BadRequest(ApiResponse.Error(400, "请选择要上传的插件 JSON 清单文件"));
            }

            var tmpDir = Path.Combine(Path.GetTempPath(), $"amn_plugin_{Guid.NewGuid()}");
            try
            {
                Directory.CreateDirectory(tmpDir);

                var safeName = Path.GetFileName(dll.FileName);
                var dllPath = Path.Combine(tmpDir, safeName);

                // 校验 PE 头 (MZ)，仅读头 2 字节
                using var readStream = dll.OpenReadStream();
                var header = new byte[2];
                if (await readStream.ReadAsync(header.AsMemory(0, 2)) != 2 || header[0] != 0x4D || header[1] != 0x5A)
                {
                    _logger.LogWarning("添加插件失败：PE 头校验失败 FileName={FileName}", dll.FileName);
                    return BadRequest(ApiResponse.Error(400, "文件不是有效的 DLL 格式"));
                }

                // 校验通过，从头写入文件
                using (var writeStream = new FileStream(dllPath, FileMode.Create))
                {
                    await writeStream.WriteAsync(header.AsMemory(0, 2));
                    await readStream.CopyToAsync(writeStream);
                }

                var jsonPath = Path.ChangeExtension(dllPath, ".json");
                using (var stream = new FileStream(jsonPath, FileMode.Create))
                    await json.CopyToAsync(stream);

                var success = await Task.Run(() => PluginManagerProxy.Instance.AddPlugin(dllPath));
                if (success)
                {
                    _logger.LogInformation("添加插件成功: {FileName}", dll.FileName);
                    var plugin = PluginManagerProxy.Proxies.FirstOrDefault(x =>
                        string.Equals(Path.GetFileName(x.PluginBasePath), safeName, StringComparison.OrdinalIgnoreCase));
                    return Ok(ApiResponse.Ok(plugin != null ? PluginDto.CreateFromPlugin(plugin) : null));
                }
                else
                {
                    _logger.LogError("添加插件失败: AddPlugin 返回 false FileName={FileName}", dll.FileName);
                    return BadRequest(ApiResponse.Error(400, "添加插件失败，查看日志排查原因"));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "添加插件异常: FileName={FileName}", dll.FileName);
                return StatusCode(500, ApiResponse.Error(500, $"由于服务器内部错误，添加插件失败: {e.Message}"));
            }
            finally
            {
                try { Directory.Delete(tmpDir, true); } catch { }
            }
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