using Another_Mirai_Native.DB;
using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/log")]
    [Authorize]
    public class LogController : ControllerBase
    {
        [HttpGet]
        [EndpointSummary("分页查询日志")]
        [EndpointDescription("按日志等级、关键词、时间范围等条件分页查询日志")]
        [ProducesResponseType(typeof(ApiResponse<LogQueryResultData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Query(
            [Description("日志等级：0=Debug, 10=Info, 11=InfoSuccess, 12=InfoReceive, 13=InfoSend, 20=Warning, 30=Error, 40=Fatal")] int priority,
            [Description("页码，从 1 开始")] int pageIndex,
            [Description("每页条数")] int pageSize,
            [Description("搜索关键词，模糊匹配日志详情")] string search = "",
            [Description("开始时间（可选）")] DateTime? start = null,
            [Description("结束时间（可选）")] DateTime? end = null,
            [Description("是否时间升序排列")] bool asc = false)
        {
            try
            {
                var logs = await Task.Run(() =>
                {
                    var items = LogHelper.DetailQueryLogs(priority, pageIndex, pageSize, search, out int totalCount, out int totalPage, start, end, asc);
                    return (items, totalCount, totalPage);
                });

                return Ok(ApiResponse.Ok(new LogQueryResultData
                {
                    Items = logs.items.Select(LogDto.CreateFromLogModel),
                    TotalCount = logs.totalCount,
                    TotalPage = logs.totalPage
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse.Error(500, "由于服务器异常，无法查询日志。"));
            }
        }
    }
}