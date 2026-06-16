using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/config")]
public class ConfigController : ControllerBase
{
    private readonly ConfigService _service;
    public ConfigController(ConfigService service) => _service = service;

    [HttpGet("org")]
    public async Task<ApiResponse<OrgConfigDto>> GetOrg()
        => ApiResponse<OrgConfigDto>.Ok(await _service.GetOrgConfigAsync(GetUser()));

    [HttpPut("org")]
    public async Task<ApiResponse<bool>> UpdateOrg([FromBody] UpdateOrgConfigRequest req)
    {
        await _service.UpdateOrgConfigAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("sys")]
    public async Task<ApiResponse<List<SysConfigDto>>> GetSys([FromQuery] string? group)
        => ApiResponse<List<SysConfigDto>>.Ok(await _service.GetSysConfigAsync(group));

    [HttpPut("sys")]
    public async Task<ApiResponse<bool>> UpdateSys([FromBody] UpdateSysConfigRequest req)
    {
        await _service.UpdateSysConfigAsync(req);
        return ApiResponse<bool>.Ok(true);
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
