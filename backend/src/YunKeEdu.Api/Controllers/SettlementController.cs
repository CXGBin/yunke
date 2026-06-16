using YunKeEdu.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/settlement")]
public class SettlementController : ControllerBase
{
    private readonly SettlementService _service;
    public SettlementController(SettlementService service) => _service = service;

    [HttpGet("rule/{courseId}")]
    public async Task<ApiResponse<SettlementRuleDto>> Rule(long courseId)
        => ApiResponse<SettlementRuleDto>.Ok(await _service.GetRuleAsync(courseId, GetUser().TenantId));

    [HttpGet("wallet")]
    public async Task<ApiResponse<WalletDto>> Wallet()
        => ApiResponse<WalletDto>.Ok(await _service.GetWalletAsync(GetUser()));

    [HttpGet("wallet/detail")]
    public async Task<ApiResponse<PagedResult<WalletDetailDto>>> WalletDetail([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<WalletDetailDto>>.Ok(await _service.GetWalletDetailAsync(req, GetUser()));

    [HttpGet("records")]
    public async Task<ApiResponse<PagedResult<FeeSettlementRecordDto>>> Records([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<FeeSettlementRecordDto>>.Ok(await _service.GetRecordsAsync(req, GetUser()));

    [HttpPost("manual-trigger")]
    public async Task<ApiResponse<bool>> ManualTrigger([FromBody] ManualTriggerRequest req)
    {
        await _service.ManualTriggerAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("summary")]
    public async Task<ApiResponse<List<SettlementSummaryDto>>> Summary([FromQuery] int? month)
        => ApiResponse<List<SettlementSummaryDto>>.Ok(await _service.GetSummaryAsync(GetUser(), month));

    [HttpGet("export")]
    public async Task<ApiResponse<List<SettlementExportDto>>> Export()
        => ApiResponse<List<SettlementExportDto>>.Ok(await _service.GetExportAsync(GetUser()));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new BizException("未登录");
}
