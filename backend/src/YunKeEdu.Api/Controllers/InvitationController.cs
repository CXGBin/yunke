using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/invitation")]
public class InvitationController : ControllerBase
{
    private readonly InvitationService _service;
    public InvitationController(InvitationService service) => _service = service;

    [HttpPost("generate")]
    public async Task<ApiResponse<InvitationDto>> Generate([FromBody] GenerateInvitationRequest req)
        => ApiResponse<InvitationDto>.Ok(await _service.GenerateAsync(req, GetUser()));

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<InvitationDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<InvitationDto>>.Ok(await _service.GetPageAsync(req, GetUser().TenantId));

    [HttpPut("{id}/cancel")]
    public async Task<ApiResponse<bool>> Cancel(long id)
    {
        await _service.CancelAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("accept")]
    public async Task<ApiResponse<bool>> Accept([FromBody] AcceptInvitationRequest req)
    {
        await _service.AcceptAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

[Microsoft.AspNetCore.Authorization.AllowAnonymous]
    [HttpGet("validate/{inviteCode}")]
    public async Task<ApiResponse<ValidateInvitationDto>> Validate(string inviteCode)
        => ApiResponse<ValidateInvitationDto>.Ok(await _service.ValidateAsync(inviteCode));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
