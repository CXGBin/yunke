using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;
using SqlSugar;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/organization")]
public class OrganizationController : ControllerBase
{
    private readonly OrganizationService _service;
    private readonly ISqlSugarClient _db;
    public OrganizationController(OrganizationService service, ISqlSugarClient db) { _service = service; _db = db; }

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<OrgDto>>> Page([FromQuery] PageRequest req, [FromQuery] int? status)
        => ApiResponse<PagedResult<OrgDto>>.Ok(await _service.GetPageAsync(req, status));

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

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        var org = await _service.GetByIdAsync(id) ?? throw new Core.Exceptions.BizException("机构不存在");
        await _db.Updateable<Core.Entities.Organization>().SetColumns(o => new Core.Entities.Organization { IsDeleted = true, UpdatedAt = DateTime.Now }).Where(o => o.Id == id).ExecuteCommandAsync();
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
