using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/theme")]
public class ThemeController : ControllerBase
{
    private readonly ThemeService _service;
    public ThemeController(ThemeService service) => _service = service;

    [HttpGet("current")]
    public async Task<ApiResponse<ThemeDto?>> Current()
        => ApiResponse<ThemeDto?>.Ok(await _service.GetCurrentThemeAsync(GetUser()));

    [HttpGet("list")]
    public async Task<ApiResponse<List<ThemeDto>>> List()
        => ApiResponse<List<ThemeDto>>.Ok(await _service.GetThemeListAsync());

    [HttpPut("org")]
    public async Task<ApiResponse<bool>> UpdateOrg([FromBody] UpdateOrgThemeRequest req)
    {
        await _service.UpdateOrgThemeAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("switch")]
    public async Task<ApiResponse<bool>> SwitchTheme([FromBody] SwitchThemeRequest req)
    {
        await _service.SwitchThemeAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
