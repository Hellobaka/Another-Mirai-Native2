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
        public IActionResult GetBaseInformation()
        {
            return Ok(ApiResponse.Ok(DashboardService.GetBaseInformation()));
        }

        [HttpGet("usages")]
        public IActionResult GetUsage()
        {
            return Ok(ApiResponse.Ok(DashboardService.GetUsages()));
        }

        [HttpGet("plugin-usages")]
        public IActionResult GetPluginUsage()
        {
            return Ok(ApiResponse.Ok(DashboardService.GetPluginUsages()));
        }
    }
}
