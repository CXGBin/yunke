using YunKeEdu.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/evaluation")]
public class EvaluationController : ControllerBase
{
    private readonly EvaluationService _service;
    public EvaluationController(EvaluationService service) => _service = service;

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateEvaluationRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req, GetUser()));

    [HttpGet("received")]
    public async Task<ApiResponse<PagedResult<EvaluationDto>>> Received([FromQuery] PageRequest req, [FromQuery] int targetType = 0)
        => ApiResponse<PagedResult<EvaluationDto>>.Ok(await _service.GetReceivedAsync(req, GetUser(), targetType));

    [HttpGet("course/{courseId}")]
    public async Task<ApiResponse<PagedResult<EvaluationDto>>> ByCourse(long courseId, [FromQuery] PageRequest req)
        => ApiResponse<PagedResult<EvaluationDto>>.Ok(await _service.GetByCourseAsync(req, courseId, GetUser().TenantId));

    [HttpGet("my")]
    public async Task<ApiResponse<PagedResult<EvaluationDto>>> My([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<EvaluationDto>>.Ok(await _service.GetMyAsync(req, GetUser()));

    [HttpPost("{id}/reply")]
    public async Task<ApiResponse<bool>> Reply(long id, [FromBody] ReplyEvaluationRequest req)
    {
        await _service.ReplyAsync(id, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("{id}/hide")]
    public async Task<ApiResponse<bool>> Hide(long id)
    {
        await _service.HideAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("{id}/top")]
    public async Task<ApiResponse<bool>> Top(long id)
    {
        await _service.TopAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("statistics/course/{courseId}")]
    public async Task<ApiResponse<EvaluationStatisticsDto>> CourseStatistics(long courseId)
        => ApiResponse<EvaluationStatisticsDto>.Ok(await _service.GetCourseStatisticsAsync(courseId, GetUser().TenantId));

    [HttpGet("statistics/teacher/{teacherId}")]
    public async Task<ApiResponse<TeacherEvaluationStatisticsDto>> TeacherStatistics(long teacherId)
        => ApiResponse<TeacherEvaluationStatisticsDto>.Ok(await _service.GetTeacherStatisticsAsync(teacherId));

    [HttpGet("tags")]
    public async Task<ApiResponse<List<EvaluationTagDto>>> Tags()
        => ApiResponse<List<EvaluationTagDto>>.Ok(await _service.GetTagsAsync(GetUser().TenantId));

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<EvaluationDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<EvaluationDto>>.Ok(await _service.GetPageAsync(req, GetUser().TenantId));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new BizException("未登录");
}
