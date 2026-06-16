using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/parent")]
public class ParentController : ControllerBase
{
    private readonly ParentService _service;
    public ParentController(ParentService service) => _service = service;

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<ParentDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<ParentDto>>.Ok(await _service.GetPageAsync(req, GetUser().TenantId));

    [HttpGet("{id}")]
    public async Task<ApiResponse<ParentDetailDto>> Get(long id)
        => ApiResponse<ParentDetailDto>.Ok(await _service.GetByIdAsync(id, GetUser().TenantId));

    [HttpPost("bind-student")]
    public async Task<ApiResponse<bool>> BindStudent([FromBody] BindStudentRequest req)
    {
        await _service.BindStudentAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("unbind/{id}")]
    public async Task<ApiResponse<bool>> Unbind(long id)
    {
        await _service.UnbindAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("link-student")]
    public async Task<ApiResponse<bool>> LinkStudent([FromBody] LinkStudentRequest req)
    {
        await _service.LinkStudentAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("confirm-link")]
    public async Task<ApiResponse<bool>> ConfirmLink([FromBody] ConfirmLinkRequest req)
    {
        await _service.LinkStudentAsync(new LinkStudentRequest { StudentUserCode = "" }, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("my-children")]
    public async Task<ApiResponse<List<ChildInfo>>> MyChildren()
        => ApiResponse<List<ChildInfo>>.Ok(await _service.GetMyChildrenAsync(GetUser()));

    [HttpGet("my-parents")]
    public async Task<ApiResponse<List<ParentDto>>> MyParents()
        => ApiResponse<List<ParentDto>>.Ok(await _service.GetMyParentsAsync(GetUser()));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
