using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;


namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ISqlSugarClient _db;

    public UserController(ISqlSugarClient db) => _db = db;

    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<object>>> GetList([FromQuery] PageRequest req, [FromQuery] long? orgId = null, [FromQuery] int? role = null)
    {
        var q = _db.Queryable<SysUser>().Where(x => !x.IsDeleted);
        if (orgId.HasValue) q = q.Where(x => x.OrgId == orgId);
        if (role.HasValue) q = q.Where(x => x.Role == role);
        if (!string.IsNullOrWhiteSpace(req.Keyword)) q = q.Where(x => x.UserName.Contains(req.Keyword!) || x.RealName!.Contains(req.Keyword!) || x.Phone!.Contains(req.Keyword!));
        RefAsync<int> total = 0;
        var items = await q.OrderBy(x => x.Id).ToPageListAsync(req.Page, req.PageSize, total);
        return ApiResponse<PagedResult<object>>.Ok(new PagedResult<object>(items.Select(x => (object)new { x.Id, x.UserName, x.RealName, x.Phone, x.Avatar, x.Role, x.OrgId, x.CampusId, x.Status }).ToList(), total, req.Page, req.PageSize));
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<object>> GetDetail(long id)
    {
        var u = await _db.Queryable<SysUser>().Where(x => x.Id == id && !x.IsDeleted).FirstAsync();
        if (u == null) throw new BizException("用户不存在");
        return ApiResponse<object>.Ok(new { u.Id, u.UserName, u.RealName, u.NickName, u.Phone, u.Avatar, u.Gender, u.BirthDate, u.Grade, u.Role, u.OrgId, u.CampusId, u.Status });
    }

    [HttpPost]
    public async Task<ApiResponse<bool>> Create([FromBody] CreateUserDto req)
    {
        var user = new SysUser
        {
            TenantId = req.OrgId ?? 0, OrgId = req.OrgId, CampusId = req.CampusId,
            UserName = req.UserName ?? req.Phone!, RealName = req.RealName, Phone = req.Phone,
            Role = req.Role, Status = 1, Password = BCrypt.Net.BCrypt.HashPassword(req.Password ?? "123456"),
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
        };
        await _db.Insertable(user).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateUserDto req)
    {
        var exists = await _db.Queryable<SysUser>().Where(x => x.Id == id && !x.IsDeleted).AnyAsync();
        if (!exists) throw new BizException("用户不存在");
        var user = new SysUser { Id = id, RealName = req.RealName, NickName = req.NickName, Avatar = req.Avatar, Gender = req.Gender ?? 0, BirthDate = req.BirthDate, Grade = req.Grade, Phone = req.Phone, Status = req.Status ?? 1, UpdatedAt = DateTime.Now };
        await _db.Updateable(user).IgnoreColumns(x => new { x.TenantId, x.OrgId, x.CampusId, x.UserName, x.UserCode, x.Password, x.Role, x.OpenId, x.UnionId, x.LastLoginAt, x.LastLoginIp, x.PasswordChangedAt, x.CreatedAt, x.IsDeleted }).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _db.Updateable<SysUser>().SetColumns(x => x.IsDeleted == true).Where(x => x.Id == id).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(long id, [FromBody] UpdateStatusDto req)
    {
        var user = await _db.Queryable<SysUser>().FirstAsync(u => u.Id == id)
            ?? throw new BizException("用户不存在");
        user.Status = req.Status;
        user.UpdatedAt = DateTime.Now;
        await _db.Updateable(user).UpdateColumns(u => new { u.Status, u.UpdatedAt }).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("{id}/reset-password")]
    public async Task<ApiResponse<bool>> ResetPassword(long id, [FromBody] ResetPwdDto req)
    {
        var u = await _db.Queryable<SysUser>().Where(x => x.Id == id && !x.IsDeleted).FirstAsync();
        if (u == null) throw new BizException("用户不存在");
        u.Password = BCrypt.Net.BCrypt.HashPassword(req.NewPassword ?? "123456");
        u.PasswordChangedAt = DateTime.Now;
        await _db.Updateable(u).UpdateColumns(x => new { x.Password, x.PasswordChangedAt }).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }
}

public class CreateUserDto { public string? UserName { get; set; } public string? RealName { get; set; } public string? Phone { get; set; } public string? Password { get; set; } public int Role { get; set; } public long? OrgId { get; set; } public long? CampusId { get; set; } }
public class UpdateStatusDto { public int Status { get; set; } }

public class UpdateUserDto { public string? RealName { get; set; } public string? NickName { get; set; } public string? Avatar { get; set; } public string? Phone { get; set; } public int? Gender { get; set; } public DateTime? BirthDate { get; set; } public string? Grade { get; set; } public int? Status { get; set; } }
public class ResetPwdDto { public string? NewPassword { get; set; } }
