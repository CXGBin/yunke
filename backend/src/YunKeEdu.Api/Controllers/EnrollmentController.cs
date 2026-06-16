using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/enrollment")]
public class EnrollmentController : ControllerBase
{
    private readonly EnrollmentService _service;
    public EnrollmentController(EnrollmentService service) => _service = service;

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<object>>> Page([FromQuery] PageRequest req)
    {
        var items = await _service.GetPagedListAsync(req, GetUser().TenantId);
        return ApiResponse<PagedResult<object>>.Ok(items);
    }

    [HttpPost]
    public async Task<ApiResponse<long>> Enroll([FromBody] CreateEnrollmentRequest req)
        => ApiResponse<long>.Ok(await _service.EnrollAsync(req, GetUser()));

    [HttpGet("my-courses")]
    public async Task<ApiResponse<List<CourseDto>>> MyCourses()
        => ApiResponse<List<CourseDto>>.Ok(await _service.GetMyCoursesAsync(GetUser()));

    [HttpGet("my-schedule")]
    public async Task<ApiResponse<List<MyScheduleDto>>> MySchedule([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        => ApiResponse<List<MyScheduleDto>>.Ok(await _service.GetMyScheduleAsync(GetUser(), startDate, endDate));

    [HttpGet("course-students")]
    public async Task<ApiResponse<PagedResult<EnrollmentDto>>> CourseStudents([FromQuery] PageRequest req, [FromQuery] long courseId)
        => ApiResponse<PagedResult<EnrollmentDto>>.Ok(await _service.GetCourseStudentsAsync(req, courseId, GetUser().TenantId));

    [HttpPost("manual-add")]
    public async Task<ApiResponse<long>> ManualAdd([FromBody] ManualAddEnrollmentRequest req)
        => ApiResponse<long>.Ok(await _service.ManualAddAsync(req, GetUser()));

    [HttpDelete("manual-remove/{id}")]
    public async Task<ApiResponse<bool>> ManualRemove(long id)
    {
        await _service.ManualRemoveAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}

[ApiController]
[Route("api/waitlist")]
public class WaitListController : ControllerBase
{
    private readonly EnrollmentService _service;
    public WaitListController(EnrollmentService service) => _service = service;

    [HttpPost("join")]
    public async Task<ApiResponse<long>> Join([FromQuery] long courseId)
        => ApiResponse<long>.Ok(await _service.JoinWaitlistAsync(courseId, GetUser()));

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Cancel(long id)
    {
        await _service.CancelWaitlistAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("my-list")]
    public async Task<ApiResponse<List<WaitListDto>>> MyList()
        => ApiResponse<List<WaitListDto>>.Ok(await _service.GetMyWaitlistAsync(GetUser()));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
