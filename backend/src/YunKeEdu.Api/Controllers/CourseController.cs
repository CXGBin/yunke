using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/course")]
public class CourseController : ControllerBase
{
    private readonly CourseService _service;
    public CourseController(CourseService service) => _service = service;

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<CourseDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<CourseDto>>.Ok(await _service.GetPageAsync(req, GetUser()));

    [HttpGet("{id}")]
    public async Task<ApiResponse<CourseDto>> Get(long id)
        => ApiResponse<CourseDto>.Ok(await _service.GetByIdAsync(id, GetUser()));

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateCourseRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req, GetUser()));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateCourseRequest req)
    {
        await _service.UpdateAsync(id, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _service.DeleteAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("{id}/publish")]
    public async Task<ApiResponse<bool>> Publish(long id)
    {
        await _service.PublishAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("{id}/offline")]
    public async Task<ApiResponse<bool>> Offline(long id)
    {
        await _service.OfflineAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
