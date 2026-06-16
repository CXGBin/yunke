
using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class TeacherService : BaseService
{
    public TeacherService(ISqlSugarClient db) : base(db) { }

    public async Task<PagedResult<TeacherDto>> GetPageAsync(PageRequest req, long tenantId)
    {
        var query = Db.Queryable<SysUser>()
            .LeftJoin<UserOrgBinding>((u, b) => u.Id == b.UserId && b.TenantId == tenantId)
            .Where((u, b) => u.IsDeleted == false && b.Role == 3 && b.Status == 1);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where((u, b) => u.RealName!.Contains(req.Keyword!) || u.Phone!.Contains(req.Keyword!));
        query = query.OrderBy((u, b) => u.Id, OrderByType.Desc);
        RefAsync<int> total = 0;
        var list = await query.Select((u, b) => new TeacherDto
        {
            Id = u.Id, UserCode = b.UserCode, UserName = u.UserName, RealName = u.RealName,
            NickName = u.NickName, Avatar = u.Avatar, Phone = u.Phone, Gender = u.Gender,
            Role = u.Role, OrgId = b.OrgId, CampusId = b.CampusId, Status = u.Status, CreatedAt = u.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<TeacherDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<TeacherDto> GetByIdAsync(long id, long tenantId)
    {
        var dto = await Db.Queryable<SysUser>()
            .LeftJoin<UserOrgBinding>((u, b) => u.Id == b.UserId && b.TenantId == tenantId)
            .Where((u, b) => u.Id == id && u.IsDeleted == false && b.Role == 3 && b.Status == 1)
            .Select((u, b) => new TeacherDto
            {
                Id = u.Id, UserCode = b.UserCode, UserName = u.UserName, RealName = u.RealName,
                NickName = u.NickName, Avatar = u.Avatar, Phone = u.Phone, Gender = u.Gender,
                Role = u.Role, OrgId = b.OrgId, CampusId = b.CampusId, Status = u.Status, CreatedAt = u.CreatedAt,
            }).FirstAsync() ?? throw new BizException("教师不存在");
        return dto;
    }

    public async Task<long> CreateAsync(CreateTeacherRequest req, CurrentUser user)
    {
        await CheckMaxTeacherCountAsync(user.TenantId);

        var exists = await Db.Queryable<SysUser>()
            .Where(u => u.Phone == req.Phone && !u.IsDeleted).FirstAsync();
        long userId;
        if (exists != null)
        {
            userId = exists.Id;
        }
        else
        {
            var newUser = new SysUser
            {
                UserName = $"teacher_{req.Phone}", Password = string.Empty, RealName = req.RealName,
                Phone = req.Phone, Avatar = req.Avatar, Gender = req.Gender switch { "男" => 1, "女" => 2, _ => 0 },
                Role = 0, Status = 1, TenantId = user.TenantId, OrgId = user.OrgId,
                CampusId = req.CampusId, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
            };
            userId = await Db.Insertable(newUser).ExecuteReturnBigIdentityAsync();
        }

        var userCode = $"{DateTime.Now:yyyyMMdd}{new Random().Next(100, 999)}";
        var binding = new UserOrgBinding
        {
            UserId = userId, TenantId = user.TenantId, OrgId = user.OrgId,
            CampusId = req.CampusId, Role = 3, UserCode = userCode,
            Status = 1, BoundAt = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        await Db.Insertable(binding).ExecuteCommandAsync();
        return userId;
    }

    public async Task UpdateAsync(long id, UpdateTeacherRequest req, CurrentUser user)
    {
        var u = await Db.Queryable<SysUser>().InSingleAsync(id) ?? throw new BizException("教师不存在");
        u.RealName = req.RealName; u.Phone = req.Phone; u.Avatar = req.Avatar;
        u.Gender = req.Gender switch { "男" => 1, "女" => 2, _ => 0 };
        u.UpdatedAt = DateTime.Now;
        await Db.Updateable(u).ExecuteCommandAsync();

        var binding = await Db.Queryable<UserOrgBinding>()
            .Where(b => b.UserId == id && b.TenantId == user.TenantId && b.Role == 3).FirstAsync();
        if (binding != null)
        {
            binding.CampusId = req.CampusId;
            binding.UpdatedAt = DateTime.Now;
            await Db.Updateable(binding).UpdateColumns(b => new { b.CampusId, b.UpdatedAt }).ExecuteCommandAsync();
        }
    }

    public async Task UpdateStatusAsync(long id, int status)
    {
        var u = await Db.Queryable<SysUser>().InSingleAsync(id) ?? throw new BizException("教师不存在");
        u.Status = status; u.UpdatedAt = DateTime.Now;
        await Db.Updateable(u).UpdateColumns(x => new { x.Status, x.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task<List<TeacherDto>> GetPublicListAsync(long tenantId)
    {
        return await Db.Queryable<SysUser>()
            .LeftJoin<UserOrgBinding>((u, b) => u.Id == b.UserId && b.TenantId == tenantId)
            .Where((u, b) => u.IsDeleted == false && b.Role == 3 && b.Status == 1 && u.Status == 1)
            .Select((u, b) => new TeacherDto
            {
                Id = u.Id, RealName = u.RealName, Avatar = u.Avatar, NickName = u.NickName, Status = u.Status,
            }).ToListAsync();
    }

    private async Task CheckMaxTeacherCountAsync(long tenantId)
    {
        var org = await Db.Queryable<Organization>().InSingleAsync(tenantId);
        if (org == null || !org.CurrentPackageId.HasValue) return;
        var pkg = await Db.Queryable<OrgPackage>().InSingleAsync(org.CurrentPackageId.Value);
        if (pkg == null || pkg.MaxTeacherCount == -1) return;
        var count = await Db.Queryable<UserOrgBinding>()
            .Where(b => b.TenantId == tenantId && b.Role == 3 && b.Status == 1).CountAsync();
        if (count >= pkg.MaxTeacherCount)
            throw new BizException($"当前套餐最多允许{pkg.MaxTeacherCount}名教师");
    }
}
