using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class InvitationService : BaseService
{
    public InvitationService(ISqlSugarClient db) : base(db) { }

    public async Task<InvitationDto> GenerateAsync(GenerateInvitationRequest req, CurrentUser user)
    {
        var inviteCode = GenerateCode();
        var expireDays = await GetExpireDaysAsync(user.TenantId);

        var record = new InvitationRecord
        {
            TenantId = user.TenantId, OrgId = user.OrgId, CampusId = req.CampusId,
            InviterId = user.UserId, InviteCode = inviteCode, InvitedRole = req.InvitedRole,
            InvitedName = req.InvitedName, InvitedPhone = req.InvitedPhone,
            Status = 0, ExpiresAt = DateTime.Now.AddDays(expireDays), Remark = req.Remark,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        record.Id = await Db.Insertable(record).ExecuteReturnBigIdentityAsync();
        return MapToDto(record);
    }

    public async Task<PagedResult<InvitationDto>> GetPageAsync(PageRequest req, long tenantId)
    {
        var query = (tenantId > 0
            ? Db.Queryable<InvitationRecord>().Where(i => i.TenantId == tenantId)
            : Db.Queryable<InvitationRecord>()).Where(i => !i.IsDeleted);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where(i => i.InvitedName!.Contains(req.Keyword!) || i.InvitedPhone!.Contains(req.Keyword!));
        query = query.OrderBy(i => i.Id, OrderByType.Desc);
        RefAsync<int> total = 0;
        var items = await query.ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<InvitationDto>(items.Select(MapToDto).ToList(), total, req.Page, req.PageSize);
    }

    public async Task CancelAsync(long id, CurrentUser user)
    {
        var record = await Db.Queryable<InvitationRecord>()
            .Where(i => i.Id == id && i.TenantId == user.TenantId && !i.IsDeleted).FirstAsync()
            ?? throw new BizException("邀请记录不存在");
        if (record.Status != 0) throw new BizException("仅待使用的邀请可取消");
        record.Status = 3; record.UpdatedAt = DateTime.Now;
        await Db.Updateable(record).UpdateColumns(i => new { i.Status, i.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task AcceptAsync(AcceptInvitationRequest req, CurrentUser user)
    {
        var record = await Db.Queryable<InvitationRecord>()
            .Where(i => i.InviteCode == req.InviteCode && !i.IsDeleted).FirstAsync()
            ?? throw new BizException("邀请码无效");

        if (record.Status != 0) throw new BizException($"邀请码状态无效：{record.Status}");
        if (record.ExpiresAt < DateTime.Now) throw new BizException("邀请已过期");

        record.Status = 1; record.UsedBy = user.UserId; record.UsedAt = DateTime.Now; record.UpdatedAt = DateTime.Now;
        await Db.Updateable(record).UpdateColumns(i => new { i.Status, i.UsedBy, i.UsedAt, i.UpdatedAt }).ExecuteCommandAsync();

        var sysUser = await Db.Queryable<SysUser>().InSingleAsync(user.UserId) ?? throw new BizException("用户不存在");

        if (sysUser.Role == 0)
            sysUser.Role = record.InvitedRole;
        sysUser.TenantId = record.TenantId;
        sysUser.OrgId = record.OrgId;
        sysUser.CampusId = record.CampusId;
        sysUser.UpdatedAt = DateTime.Now;
        await Db.Updateable(sysUser)
            .UpdateColumns(u => new { u.Role, u.TenantId, u.OrgId, u.CampusId, u.UpdatedAt }).ExecuteCommandAsync();

        var existingBinding = await Db.Queryable<UserOrgBinding>()
            .Where(b => b.UserId == user.UserId && b.OrgId == record.OrgId && b.Role == record.InvitedRole && b.Status == 1).AnyAsync();
        if (!existingBinding)
        {
            var userCode = record.InvitedRole switch
            {
                3 => $"T{DateTime.Now:yyyyMMdd}{new Random().Next(100, 999)}",
                4 => $"S{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}",
                _ => $"P{DateTime.Now:yyyyMMdd}{new Random().Next(100, 999)}",
            };
            await Db.Insertable(new UserOrgBinding
            {
                UserId = user.UserId, TenantId = record.TenantId, OrgId = record.OrgId,
                CampusId = record.CampusId, Role = record.InvitedRole, UserCode = userCode,
                Status = 1, BoundVia = 0, InvitationId = record.Id,
                BoundAt = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
            }).ExecuteCommandAsync();
        }
    }

    public async Task<ValidateInvitationDto> ValidateAsync(string inviteCode)
    {
        var record = await Db.Queryable<InvitationRecord>()
            
            .LeftJoin<Organization>((i, o) => i.OrgId == o.Id)
            .LeftJoin<Campus>((i, o, c) => i.CampusId == c.Id)
            .Where((i, o, c) => i.InviteCode == inviteCode && !i.IsDeleted)
            .Select((i, o, c) => new ValidateInvitationDto
            {
                Valid = i.Status == 0 && i.ExpiresAt > DateTime.Now,
                InviteCode = i.InviteCode, InvitedRole = i.InvitedRole,
                OrgName = o.Name, CampusName = c.Name, InvitedName = i.InvitedName,
            }).FirstAsync() ?? throw new BizException("邀请码无效");
        return record;
    }

    private static async Task<int> GetExpireDaysAsync(long tenantId)
    {
        // Default 7 days
        await Task.CompletedTask;
        return 7;
    }

    private static string GenerateCode() => Guid.NewGuid().ToString("N")[..8].ToUpper();

    private static InvitationDto MapToDto(InvitationRecord r) => new()
    {
        Id = r.Id, InviteCode = r.InviteCode, InvitedRole = r.InvitedRole,
        InvitedName = r.InvitedName, InvitedPhone = r.InvitedPhone,
        Status = r.Status, ExpiresAt = r.ExpiresAt, UsedBy = r.UsedBy,
        UsedAt = r.UsedAt, Remark = r.Remark, CreatedAt = r.CreatedAt,
    };
}
