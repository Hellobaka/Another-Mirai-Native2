using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/protocol")]
    [Authorize]
    public class ProtocolController(ILogger<ProtocolController> logger) : ControllerBase
    {
        private readonly ILogger<ProtocolController> _logger = logger;

        [HttpGet("list")]
        [EndpointSummary("获取可用协议列表")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), StatusCodes.Status200OK)]
        public IActionResult ListProtocols()
        {
            _logger.LogInformation("获取可用协议列表");
            var protocols = ProtocolManager.Protocols.Select(x => x.Name).ToList();
            _logger.LogInformation("获取可用协议列表成功: {Protocols}", string.Join(", ", protocols));
            return Ok(ApiResponse.Ok(protocols));
        }

        [HttpGet("current")]
        [EndpointSummary("获取当前协议状态")]
        [EndpointDescription("返回当前已连接协议的名称和连接状态")]
        [ProducesResponseType(typeof(ApiResponse<ProtocolStatusData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public IActionResult GetCurrentProtocol()
        {
            _logger.LogInformation("获取当前协议状态");
            var p = ProtocolManager.Instance.CurrentProtocol;
            if (p == null)
            {
                _logger.LogWarning("获取当前协议状态：当前没有已连接协议");
                return NotFound(ApiResponse.Error(404, "当前没有已连接协议"));
            }

            _logger.LogInformation("当前协议: Name={Name}, IsConnected={IsConnected}", p.Name, p.IsConnected);
            return Ok(ApiResponse.Ok(new ProtocolStatusData
            {
                Name = p.Name,
                IsConnected = p.IsConnected
            }));
        }

        [HttpPost("disconnect")]
        [EndpointSummary("断开当前协议")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Disconnect()
        {
            _logger.LogInformation("断开当前协议");
            var p = ProtocolManager.Instance.CurrentProtocol;
            if (p == null)
            {
                _logger.LogWarning("断开协议失败：当前没有已连接协议");
                return BadRequest(ApiResponse.Error(400, "当前没有已连接协议，无法执行断开"));
            }

            try
            {
                bool success = await Task.Run(ProtocolManager.Instance.CurrentProtocol.Disconnect);
                if (success)
                {
                    _logger.LogInformation("断开协议成功: Name={Name}", p.Name);
                    return Ok(ApiResponse.Ok());
                }
                else
                {
                    _logger.LogError("断开协议失败: Name={Name}", p.Name);
                    return BadRequest(ApiResponse.Error(500, "断开连接失败"));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "断开协议异常: Name={Name}", p.Name);
                return StatusCode(500, ApiResponse.Error(500, "由于服务器发生异常，断开连接失败"));
            }
        }

        [HttpPost("connect/{name}")]
        [EndpointSummary("连接到指定协议")]
        [EndpointDescription("连接到指定名称的协议。连接前需先断开当前已连接协议")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Connect([Description("协议名称，如 OneBot v11、LagrangeCore 等")] string name)
        {
            _logger.LogInformation("连接协议: Name={Name}", name);
            var p = ProtocolManager.Instance.CurrentProtocol;
            if (p != null && p.IsConnected)
            {
                _logger.LogWarning("连接协议失败：当前已连接到 {CurrentName}", p.Name);
                return BadRequest(ApiResponse.Error(400, $"当前已连接到 {p.Name} 协议，连接到其他协议前请先断开"));
            }

            try
            {
                if (p != null && !p.IsDisposed)
                {
                    bool disconnectSuccess = await Task.Run(p.Disconnect);
                    if (!disconnectSuccess)
                    {
                        _logger.LogError("连接协议失败：断开当前协议 {CurrentName} 失败", p.Name);
                        return BadRequest(ApiResponse.Error(500, $"断开当前协议 {p.Name} 失败，无法连接到 {name} 协议"));
                    }
                }
                bool success = await Task.Run(() => ProtocolManager.Instance.Start(name));
                if (success)
                {
                    _logger.LogInformation("连接协议成功: Name={Name}", name);
                    return Ok(ApiResponse.Ok());
                }
                else
                {
                    _logger.LogError("连接协议失败: Name={Name}", name);
                    return BadRequest(ApiResponse.Error(500, $"连接到 {name} 协议失败"));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "连接协议异常: Name={Name}", name);
                return StatusCode(500, ApiResponse.Error(500, $"由于服务器发生异常，连接到 {name} 协议失败"));
            }
        }

        [HttpGet("config/{name}")]
        [EndpointSummary("获取协议连接参数")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult GetCurrentProtocolConfig([Description("协议名称")] string name)
        {
            _logger.LogInformation("获取协议配置: Name={Name}", name);
            var p = ProtocolManager.Protocols.FirstOrDefault(x => x.Name == name);
            if (p == null)
            {
                _logger.LogWarning("获取协议配置失败：未找到协议 Name={Name}", name);
                return NotFound(ApiResponse.Error(404, $"未找到名称为 {name} 的协议"));
            }

            try
            {
                var config = p.GetConnectionConfig();
                _logger.LogInformation("获取协议配置成功: Name={Name}, Keys={Keys}", name, string.Join(",", config.Keys));
                return Ok(ApiResponse.Ok(config));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "获取协议配置异常: Name={Name}", name);
                return StatusCode(500, ApiResponse.Error(500, "由于服务器发生异常，获取协议配置参数失败"));
            }
        }

        [HttpPost("config/{name}")]
        [EndpointSummary("设置协议连接参数")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult SetCurrentProtocolConfig(
            [Description("协议名称")] string name,
            [Description("键值对形式的连接参数")][FromBody] Dictionary<string, string> request)
        {
            _logger.LogInformation("设置协议配置: Name={Name}, Keys={Keys}", name, string.Join(",", request.Keys));
            var p = ProtocolManager.Protocols.FirstOrDefault(x => x.Name == name);
            if (p == null)
            {
                _logger.LogWarning("设置协议配置失败：未找到协议 Name={Name}", name);
                return NotFound(ApiResponse.Error(404, $"未找到名称为 {name} 的协议"));
            }

            try
            {
                var success = p.SetConnectionConfig(request);
                if (success)
                {
                    _logger.LogInformation("设置协议配置成功: Name={Name}", name);
                    return Ok(ApiResponse.Ok());
                }
                else
                {
                    _logger.LogError("设置协议配置失败: Name={Name}", name);
                    return BadRequest(ApiResponse.Error(400, "设置协议连接参数失败，检查传递参数是否缺失键或无效值"));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "设置协议配置异常: Name={Name}", name);
                return StatusCode(500, ApiResponse.Error(500, "由于服务器发生异常，设置协议连接参数失败"));
            }
        }
    }
}