using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
public class CourseAttachmentController : ControllerBase
{
    private readonly CourseAttachmentService _service;
    public CourseAttachmentController(CourseAttachmentService service) => _service = service;

    [HttpPost("api/course/{courseId}/attachment")]
    public async Task<ApiResponse<long>> Create(long courseId, [FromBody] CreateAttachmentRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(courseId, req, GetUser()));

    [HttpDelete("api/course/attachment/{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _service.DeleteAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
