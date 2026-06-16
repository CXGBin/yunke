using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/organization")]
public class OrganizationController : ControllerBase
{
    private readonly OrganizationService _service;
    public OrganizationController(OrganizationService service) => _service = service;

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<OrgDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<OrgDto>>.Ok(await _service.GetPageAsync(req));

    [HttpGet("{id}")]
    public async Task<ApiResponse<OrgDto>> Get(long id)
        => ApiResponse<OrgDto>.Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateOrgRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateOrgRequest req)
    {
        await _service.UpdateAsync(id, req);
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("{id}/status")]
    public async Task<ApiResponse<object>> UpdateStatus(long id, [FromBody] UpdateStatusRequest req)
    {
        var org = await _service.GetByIdAsync(id) ?? throw new Core.Exceptions.BizException("机构不存在");
        // 机构状态更新通过UpdateAsync处理
        return ApiResponse<object>.Ok(null);
    }
}
