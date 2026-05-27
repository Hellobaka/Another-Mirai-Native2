using Another_Mirai_Native.WebAPI.Models;
using Another_Mirai_Native.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/dashboard")]
    [Authorize]
    public class DashboardController(DashboardService dashboardService, ILogger<DashboardController> logger) : ControllerBase
    {
        private readonly ILogger<DashboardController> _logger = logger;
        public DashboardService DashboardService { get; } = dashboardService;

        [HttpGet("base-information")]
        [EndpointSummary("获取系统基础信息")]
        [EndpointDescription("返回操作系统版本、框架版本、运行时长、当前 Bot QQ 和已启用插件数量")]
        [ProducesResponseType(typeof(ApiResponse<DashboardInfoData>), StatusCodes.Status200OK)]
        public IActionResult GetBaseInformation()
        {
            _logger.LogInformation("获取系统基础信息");
            var data = DashboardService.GetBaseInformation().MapTo<DashboardInfoData>();
            _logger.LogInformation("获取系统基础信息成功: OS={OS}, Version={Version}", data.OsVersion, data.Version);
            return Ok(ApiResponse.Ok(data));
        }

        [HttpGet("usages")]
        [EndpointSummary("获取系统资源占用")]
        [EndpointDescription("返回系统整体 CPU 占用、内存使用量等实时数据")]
        [ProducesResponseType(typeof(ApiResponse<UsageData>), StatusCodes.Status200OK)]
        public IActionResult GetUsage()
        {
            _logger.LogInformation("获取系统资源占用");
            var data = DashboardService.GetUsages().MapTo<UsageData>();
            _logger.LogInformation("获取系统资源占用成功: CPU={CPU}%, Memory={Memory}%", data.CpuUsage, data.MemoryUsage);
            return Ok(ApiResponse.Ok(data));
        }

        [HttpGet("plugin-usages")]
        [EndpointSummary("获取各进程资源占用")]
        [EndpointDescription("返回所有插件进程及主框架的 CPU、内存占用详情")]
        [ProducesResponseType(typeof(ApiResponse<PluginUsageData>), StatusCodes.Status200OK)]
        public IActionResult GetPluginUsage()
        {
            _logger.LogInformation("获取插件资源占用");
            var data = DashboardService.GetPluginUsages().MapTo<PluginUsageData>();
            _logger.LogInformation("获取插件资源占用成功: 插件数={Count}, 总内存={Memory}MB, 总CPU={CPU}%", data.PluginUsages.Count, data.TotalProcessMemory, data.TotalProcessCPU);
            return Ok(ApiResponse.Ok(data));
        }
    }
}