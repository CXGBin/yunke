using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/schedule")]
public class ScheduleController : ControllerBase
{
    private readonly ScheduleService _service;
    public ScheduleController(ScheduleService service) => _service = service;

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateScheduleRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req, GetUser()));

    [HttpPost("recurrence")]
    public async Task<ApiResponse<long>> Recurrence([FromBody] CreateRecurrenceRequest req)
        => ApiResponse<long>.Ok(await _service.CreateRecurrenceAsync(req, GetUser()));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateScheduleRequest req)
    {
        await _service.UpdateAsync(id, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ApiResponse<bool>> Cancel(long id, [FromBody] CancelScheduleRequest req)
    {
        await _service.CancelAsync(id, req, null!);
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("{id}/publish")]
    public async Task<ApiResponse<bool>> Publish(long id)
    {
        await _service.PublishAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<ScheduleDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<ScheduleDto>>.Ok(await _service.GetPageAsync(req, GetUser()));

    [HttpGet("calendar")]
    public async Task<ApiResponse<List<CalendarEventDto>>> Calendar([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        => ApiResponse<List<CalendarEventDto>>.Ok(await _service.GetCalendarAsync(startDate, endDate, GetUser()));

    [HttpGet("check-conflict")]
    public async Task<ApiResponse<ConflictCheckResult>> CheckConflict([FromQuery] ConflictCheckRequest req)
        => ApiResponse<ConflictCheckResult>.Ok(await _service.CheckConflictAsync(req, GetUser()));

    [HttpGet("change-log/{scheduleId}")]
    public async Task<ApiResponse<List<ScheduleChangeLogDto>>> ChangeLog(long scheduleId)
        => ApiResponse<List<ScheduleChangeLogDto>>.Ok(await _service.GetChangeLogsAsync(scheduleId, GetUser().TenantId));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
