using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/protocol")]
    [Authorize]
    public class ProtocolController : ControllerBase
    {
        [HttpGet("list")]
        [EndpointSummary("获取可用协议列表")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), StatusCodes.Status200OK)]
        public IActionResult ListProtocols()
        {
            return Ok(ApiResponse.Ok(ProtocolManager.Protocols.Select(x => x.Name)));
        }

        [HttpGet("current")]
        [EndpointSummary("获取当前协议状态")]
        [EndpointDescription("返回当前已连接协议的名称和连接状态")]
        [ProducesResponseType(typeof(ApiResponse<ProtocolStatusData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public IActionResult GetCurrentProtocol()
        {
            var p = ProtocolManager.Instance.CurrentProtocol;
            if (p == null)
            {
                return NotFound(ApiResponse.Error(404, "当前没有已连接协议"));
            }

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
            var p = ProtocolManager.Instance.CurrentProtocol;
            if (p == null)
            {
                return BadRequest(ApiResponse.Error(400, "当前没有已连接协议，无法执行断开"));
            }

            try
            {
                bool success = await Task.Run(ProtocolManager.Instance.CurrentProtocol.Disconnect);
                return success ? Ok(ApiResponse.Ok()) : BadRequest(ApiResponse.Error(500, "断开连接失败"));
            }
            catch (Exception)
            {
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
            var p = ProtocolManager.Instance.CurrentProtocol;
            if (p != null && p.IsConnected)
            {
                return BadRequest(ApiResponse.Error(400, $"当前已连接到 {p.Name} 协议，连接到其他协议前请先断开"));
            }

            try
            {
                if (p != null && !p.IsDisposed)
                {
                    bool disconnectSuccess = await Task.Run(p.Disconnect);
                    if (!disconnectSuccess)
                    {
                        return BadRequest(ApiResponse.Error(500, $"断开当前协议 {p.Name} 失败，无法连接到 {name} 协议"));
                    }
                }
                bool success = await Task.Run(() => ProtocolManager.Instance.Start(name));
                return success ? Ok(ApiResponse.Ok()) : BadRequest(ApiResponse.Error(500, $"连接到 {name} 协议失败"));
            }
            catch (Exception)
            {
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
            var p = ProtocolManager.Protocols.FirstOrDefault(x => x.Name == name);
            if (p == null)
            {
                return NotFound(ApiResponse.Error(404, $"未找到名称为 {name} 的协议"));
            }

            try
            {
                return Ok(ApiResponse.Ok(p.GetConnectionConfig()));
            }
            catch (Exception)
            {
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
            var p = ProtocolManager.Protocols.FirstOrDefault(x => x.Name == name);
            if (p == null)
            {
                return NotFound(ApiResponse.Error(404, $"未找到名称为 {name} 的协议"));
            }

            try
            {
                return p.SetConnectionConfig(request)
                    ? Ok(ApiResponse.Ok())
                    : BadRequest(ApiResponse.Error(400, "设置协议连接参数失败，检查传递参数是否缺失键或无效值"));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse.Error(500, "由于服务器发生异常，设置协议连接参数失败"));
            }
        }
    }
}