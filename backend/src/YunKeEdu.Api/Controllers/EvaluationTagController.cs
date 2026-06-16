using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/evaluation-tag")]
public class EvaluationTagController : ControllerBase
{
    private readonly EvaluationTagService _service;
    public EvaluationTagController(EvaluationTagService service) => _service = service;

    [HttpGet("list")]
    public async Task<ApiResponse<List<EvaluationTagDto>>> List()
        => ApiResponse<List<EvaluationTagDto>>.Ok(await _service.GetListAsync(GetUser().TenantId));

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateEvaluationTagRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req, GetUser()));

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _service.DeleteAsync(id, GetUser().TenantId);
        return ApiResponse<bool>.Ok(true);
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
