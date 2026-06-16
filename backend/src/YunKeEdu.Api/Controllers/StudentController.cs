using YunKeEdu.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/student")]
public class StudentController : ControllerBase
{
    private readonly StudentService _service;
    public StudentController(StudentService service) => _service = service;

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<StudentDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<StudentDto>>.Ok(await _service.GetPageAsync(req, GetUser().TenantId));

    [HttpGet("{id}")]
    public async Task<ApiResponse<StudentDto>> Get(long id)
        => ApiResponse<StudentDto>.Ok(await _service.GetByIdAsync(id, GetUser().TenantId));

    [HttpPost("import")]
    public async Task<ApiResponse<bool>> Import([FromBody] StudentImportRequest req)
        => ApiResponse<bool>.Ok(await _service.ImportAsync(req, GetUser()));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new BizException("未登录");
}
