using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/org-binding")]
public class OrgBindingController : ControllerBase
{
    private readonly OrgBindingService _service;
    public OrgBindingController(OrgBindingService service) => _service = service;

    [HttpGet("my-orgs")]
    public async Task<ApiResponse<List<UserOrgInfo>>> MyOrgs()
        => ApiResponse<List<UserOrgInfo>>.Ok(await _service.GetMyOrgsAsync(GetUser()));

    [HttpGet("detail/{orgId}")]
    public async Task<ApiResponse<UserOrgInfo>> Detail(long orgId)
        => ApiResponse<UserOrgInfo>.Ok(await _service.GetDetailAsync(orgId, GetUser()));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
