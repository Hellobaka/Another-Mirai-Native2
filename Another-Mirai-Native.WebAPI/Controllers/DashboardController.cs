using Another_Mirai_Native.WebAPI.Models;
using Another_Mirai_Native.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/dashboard")]
    [Authorize]
    public class DashboardController(DashboardService dashboardService) : ControllerBase
    {
        public DashboardService DashboardService { get; } = dashboardService;

        [HttpGet("base-information")]
        [EndpointSummary("获取系统基础信息")]
        [EndpointDescription("返回操作系统版本、框架版本、运行时长、当前 Bot QQ 和已启用插件数量")]
        [ProducesResponseType(typeof(ApiResponse<DashboardInfoData>), StatusCodes.Status200OK)]
        public IActionResult GetBaseInformation()
        {
            return Ok(ApiResponse.Ok(DashboardService.GetBaseInformation().MapTo<DashboardInfoData>()));
        }

        [HttpGet("usages")]
        [EndpointSummary("获取系统资源占用")]
        [EndpointDescription("返回系统整体 CPU 占用、内存使用量等实时数据")]
        [ProducesResponseType(typeof(ApiResponse<UsageData>), StatusCodes.Status200OK)]
        public IActionResult GetUsage()
        {
            return Ok(ApiResponse.Ok(DashboardService.GetUsages().MapTo<UsageData>()));
        }

        [HttpGet("plugin-usages")]
        [EndpointSummary("获取各进程资源占用")]
        [EndpointDescription("返回所有插件进程及主框架的 CPU、内存占用详情")]
        [ProducesResponseType(typeof(ApiResponse<PluginUsageData>), StatusCodes.Status200OK)]
        public IActionResult GetPluginUsage()
        {
            return Ok(ApiResponse.Ok(DashboardService.GetPluginUsages().MapTo<PluginUsageData>()));
        }
    }
}