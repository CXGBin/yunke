using YunKeEdu.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;
using SqlSugar;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/campus")]
public class CampusController : ControllerBase
{
    private readonly CampusService _service;
    private readonly ISqlSugarClient _db;
    public CampusController(CampusService service, ISqlSugarClient db) { _service = service; _db = db; }

    [HttpGet("list")]
    public async Task<ApiResponse<List<CampusDto>>> List()
        => ApiResponse<List<CampusDto>>.Ok(await _service.GetListAsync(GetUser().TenantId));

    [HttpGet("{id}")]
    public async Task<ApiResponse<CampusDto>> Get(long id)
        => ApiResponse<CampusDto>.Ok(await _service.GetByIdAsync(id, GetUser().TenantId));

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateCampusRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req, GetUser()));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateCampusRequest req)
    {
        await _service.UpdateAsync(id, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        var campus = await _service.GetByIdAsync(id, GetUser().TenantId) ?? throw new BizException("校区不存在");
        // 软删除通过直接操作Db完成
        await _db.Updateable<Core.Entities.Campus>().SetColumns(c => new Core.Entities.Campus { IsDeleted = true, UpdatedAt = DateTime.Now }).Where(c => c.Id == id).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(long id, [FromBody] UpdateStatusRequest req)
    {
        await _service.UpdateStatusAsync(id, req.Status, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new BizException("未登录");
}
