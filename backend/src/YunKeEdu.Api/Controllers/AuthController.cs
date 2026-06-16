using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;
    public AuthController(AuthService service) => _service = service;

    [HttpPost("login")]
    public async Task<ApiResponse<LoginResponse>> Login([FromBody] LoginRequest req)
        => ApiResponse<LoginResponse>.Ok(await _service.LoginAsync(req, HttpContext));

    [HttpPost("wx-login")]
    public async Task<ApiResponse<LoginResponse>> WxLogin([FromBody] WxLoginRequest req)
        => ApiResponse<LoginResponse>.Ok(await _service.WxLoginAsync(req));

    [HttpPost("bind-phone")]
    [Authorize]
    public async Task<ApiResponse<bool>> BindPhone([FromBody] BindPhoneRequest req)
    {
        await _service.BindPhoneAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("register-org")]
    [Authorize]
    public async Task<ApiResponse<LoginResponse>> RegisterOrg([FromBody] RegisterOrgRequest req)
        => ApiResponse<LoginResponse>.Ok(await _service.RegisterOrgAsync(req, GetUser()));

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ApiResponse<bool>> ChangePassword([FromBody] ChangePasswordRequest req)
    {
        await _service.ChangePasswordAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("user-info")]
    [Authorize]
    public async Task<ApiResponse<UserInfoDto>> GetUserInfo()
        => ApiResponse<UserInfoDto>.Ok(await _service.GetUserInfoAsync(GetUser()));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
