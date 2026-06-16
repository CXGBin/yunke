using YunKeEdu.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/org-subscription")]
public class OrgSubscriptionController : ControllerBase
{
    private readonly OrgSubscriptionService _service;
    public OrgSubscriptionController(OrgSubscriptionService service) => _service = service;

    [HttpPost("purchase")]
    public async Task<ApiResponse<SubscriptionDto>> Purchase([FromBody] PurchaseRequest req)
        => ApiResponse<SubscriptionDto>.Ok(await _service.PurchaseAsync(req, GetUser()));

    [HttpPost("renew")]
    public async Task<ApiResponse<SubscriptionDto>> Renew([FromBody] RenewRequest req)
        => ApiResponse<SubscriptionDto>.Ok(await _service.RenewAsync(req, GetUser()));

    [HttpPost("upgrade")]
    public async Task<ApiResponse<UpgradeOrderDto>> Upgrade([FromBody] UpgradeRequest req)
        => ApiResponse<UpgradeOrderDto>.Ok(await _service.UpgradeAsync(req, GetUser()));

    [HttpGet("current")]
    public async Task<ApiResponse<SubscriptionDto?>> Current()
        => ApiResponse<SubscriptionDto?>.Ok(await _service.GetCurrentAsync(GetUser()));

    [HttpGet("history")]
    public async Task<ApiResponse<PagedResult<SubscriptionDto>>> History([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<SubscriptionDto>>.Ok(await _service.GetHistoryAsync(req, GetUser().Role == 1 ? null : GetUser().TenantId));

    [HttpGet("upgrade-detail/{id}")]
    public async Task<ApiResponse<UpgradeOrderDto>> UpgradeDetail(long id)
        => ApiResponse<UpgradeOrderDto>.Ok(await _service.GetUpgradeDetailAsync(id));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new BizException("未登录");
}
