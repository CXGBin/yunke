using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;


namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/system")]
public class SystemController : ControllerBase
{
    private readonly ISqlSugarClient _db;

    public SystemController(ISqlSugarClient db) => _db = db;

    [HttpGet("config")]
    public async Task<ApiResponse<List<object>>> GetConfigs([FromQuery] string? group = null)
    {
        var q = _db.Queryable<SysConfig>();
        if (!string.IsNullOrEmpty(group)) q = q.Where(x => x.ConfigGroup == group);
        var items = await q.ToListAsync();
        return ApiResponse<List<object>>.Ok(items.Select(x => (object)x).ToList());
    }

    [HttpPut("config")]
    public async Task<ApiResponse<bool>> UpdateConfig([FromBody] UpdateConfigDto dto)
    {
        var exists = await _db.Queryable<SysConfig>().Where(x => x.ConfigKey == dto.ConfigKey).FirstAsync();
        if (exists == null) throw new BizException("配置项不存在");
        exists.ConfigValue = dto.ConfigValue; exists.UpdatedAt = DateTime.Now;
        await _db.Updateable(exists).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("config/{id}")]
    public async Task<ApiResponse<bool>> DeleteConfig(long id)
    {
        var config = await _db.Queryable<Core.Entities.SysConfig>().InSingleAsync(id)
            ?? throw new Core.Exceptions.BizException("配置不存在");
        await _db.Deleteable<Core.Entities.SysConfig>().Where(c => c.Id == id).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("org-config/{orgId}")]
    public async Task<ApiResponse<object>> GetOrgConfig(long orgId)
    {
        var c = await _db.Queryable<OrgConfig>().Where(x => x.OrgId == orgId).FirstAsync();
        return ApiResponse<object>.Ok(c);
    }

    [HttpPut("org-config/{orgId}")]
    public async Task<ApiResponse<bool>> UpdateOrgConfig(long orgId, [FromBody] OrgConfig req)
    {
        var existing = await _db.Queryable<OrgConfig>().Where(x => x.OrgId == orgId).FirstAsync();
        if (existing == null) { req.OrgId = orgId; req.TenantId = orgId; req.CreatedAt = DateTime.Now; req.UpdatedAt = DateTime.Now; await _db.Insertable(req).ExecuteCommandAsync(); }
        else { req.Id = existing.Id; req.UpdatedAt = DateTime.Now; await _db.Updateable(req).IgnoreColumns(x => new { x.TenantId, x.OrgId, x.CreatedAt }).ExecuteCommandAsync(); }
        return ApiResponse<bool>.Ok(true);
    }

    // 家长-学生关系
    [HttpGet("parent-student")]
    public async Task<ApiResponse<List<object>>> GetParentStudentRelations([FromQuery] long? parentId = null, [FromQuery] long? studentId = null)
    {
        var q = _db.Queryable<ParentStudentRelation>().Where(x => !x.IsDeleted && x.Status == 1);
        if (parentId.HasValue) q = q.Where(x => x.ParentId == parentId);
        if (studentId.HasValue) q = q.Where(x => x.StudentId == studentId);
        var items = await q.ToListAsync();
        return ApiResponse<List<object>>.Ok(items.Select(x => (object)x).ToList());
    }

    [HttpPost("parent-student")]
    public async Task<ApiResponse<bool>> AddParentStudentRelation([FromBody] ParentStudentRelation req)
    {
        req.CreatedAt = DateTime.Now; req.UpdatedAt = DateTime.Now; req.Status = 1;
        await _db.Insertable(req).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }
}

