using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/lesson-unit")]
public class LessonUnitController : ControllerBase
{
    private readonly LessonUnitService _service;
    public LessonUnitController(LessonUnitService service) => _service = service;

    [HttpGet("course/{courseId}")]
    public async Task<ApiResponse<List<LessonUnitDto>>> GetByCourse(long courseId)
        => ApiResponse<List<LessonUnitDto>>.Ok(await _service.GetByCourseAsync(courseId, GetUser()));

    [HttpPost("batch-generate")]
    public async Task<ApiResponse<bool>> BatchGenerate(long courseId, [FromBody] BatchGenerateLessonRequest req)
    {
        await _service.BatchGenerateAsync(courseId, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateLessonUnitRequest req)
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

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
