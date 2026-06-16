using YunKeEdu.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/attendance")]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceService _service;
    public AttendanceController(AttendanceService service) => _service = service;

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<object>>> Page([FromQuery] PageRequest req)
    {
        var items = await _service.GetPagedListAsync(req, GetUser().TenantId);
        return ApiResponse<PagedResult<object>>.Ok(items);
    }

    [HttpPost("sign-in")]
    public async Task<ApiResponse<bool>> SignIn([FromBody] SignInRequest req)
    {
        await _service.SignInAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("sign-all")]
    public async Task<ApiResponse<bool>> SignAll([FromBody] SignAllRequest req)
    {
        await _service.SignAllAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("schedule/{scheduleId}")]
    public async Task<ApiResponse<List<AttendanceDto>>> BySchedule(long scheduleId)
        => ApiResponse<List<AttendanceDto>>.Ok(await _service.GetByScheduleAsync(scheduleId, GetUser().TenantId));

    [HttpGet("my-records")]
    public async Task<ApiResponse<List<AttendanceDto>>> MyRecords([FromQuery] int? limit)
        => ApiResponse<List<AttendanceDto>>.Ok(await _service.GetMyRecordsAsync(GetUser(), limit));

    [HttpGet("statistics/student")]
    public async Task<ApiResponse<object>> StudentStatistics()
        => ApiResponse<object>.Ok(await _service.GetStudentStatisticsAsync(GetUser()));

    [HttpGet("statistics/course/{courseId}")]
    public async Task<ApiResponse<object>> CourseStatistics(long courseId)
        => ApiResponse<object>.Ok(await _service.GetCourseStatisticsAsync(courseId, GetUser().TenantId));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new BizException("未登录");
}
