using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/teacher")]
public class TeacherController : ControllerBase
{
    private readonly TeacherService _service;
    public TeacherController(TeacherService service) => _service = service;

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<TeacherDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<TeacherDto>>.Ok(await _service.GetPageAsync(req, GetUser().TenantId));

    [HttpGet("{id}")]
    public async Task<ApiResponse<TeacherDto>> Get(long id)
        => ApiResponse<TeacherDto>.Ok(await _service.GetByIdAsync(id, GetUser().TenantId));

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateTeacherRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req, GetUser()));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateTeacherRequest req)
    {
        await _service.UpdateAsync(id, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(long id, [FromBody] UpdateStatusRequest req)
    {
        await _service.UpdateStatusAsync(id, req.Status);
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("public-list")]
    public async Task<ApiResponse<List<TeacherDto>>> PublicList()
        => ApiResponse<List<TeacherDto>>.Ok(await _service.GetPublicListAsync(GetUser().TenantId));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
