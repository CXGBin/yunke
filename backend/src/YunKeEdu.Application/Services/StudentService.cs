using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class StudentService : BaseService
{
    public StudentService(ISqlSugarClient db) : base(db) { }

    public async Task<PagedResult<StudentDto>> GetPageAsync(PageRequest req, long tenantId)
    {
        var query = Db.Queryable<SysUser>()
            .LeftJoin<UserOrgBinding>((u, b) => u.Id == b.UserId && b.TenantId == tenantId)
            .LeftJoin<Organization>((u, b, o) => b.OrgId == o.Id)
            .Where((u, b, o) => u.IsDeleted == false && b.Role == 4 && b.Status == 1);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where((u, b, o) => u.RealName!.Contains(req.Keyword!) || u.Phone!.Contains(req.Keyword!));
        query = query.OrderBy((u, b, o) => u.Id, OrderByType.Desc);
        RefAsync<int> total = 0;
        var list = await query.Select((u, b, o) => new StudentDto
        {
            Id = u.Id, UserCode = b.UserCode, UserName = u.UserName, RealName = u.RealName,
            Avatar = u.Avatar, Phone = u.Phone, Gender = u.Gender, Grade = u.Grade,
            Status = u.Status, OrgId = b.OrgId, CampusId = b.CampusId,
            OrgName = o.Name, CreatedAt = u.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<StudentDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<StudentDto> GetByIdAsync(long id, long tenantId)
    {
        return await Db.Queryable<SysUser>()
            .LeftJoin<UserOrgBinding>((u, b) => u.Id == b.UserId && b.TenantId == tenantId)
            .LeftJoin<Organization>((u, b, o) => b.OrgId == o.Id)
            .Where((u, b, o) => u.Id == id && u.IsDeleted == false && b.Role == 4)
            .Select((u, b, o) => new StudentDto
            {
                Id = u.Id, UserCode = b.UserCode, UserName = u.UserName, RealName = u.RealName,
                Avatar = u.Avatar, Phone = u.Phone, Gender = u.Gender, Grade = u.Grade,
                Status = u.Status, OrgId = b.OrgId, CampusId = b.CampusId,
                OrgName = o.Name, CreatedAt = u.CreatedAt,
            }).FirstAsync() ?? throw new BizException("学生不存在");
    }

    public async Task<bool> ImportAsync(StudentImportRequest req, CurrentUser user)
    {
        await CheckMaxStudentCountAsync(user.TenantId);

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
                UserName = $"student_{req.Phone}", RealName = req.RealName, Phone = req.Phone,
                Gender = req.Gender, Grade = req.Grade, Role = 0, Status = 1,
                TenantId = user.TenantId, OrgId = user.OrgId, CampusId = req.CampusId,
                CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
            };
            userId = await Db.Insertable(newUser).ExecuteReturnBigIdentityAsync();
        }

        var userCode = $"S{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
        var binding = new UserOrgBinding
        {
            UserId = userId, TenantId = user.TenantId, OrgId = user.OrgId,
            CampusId = req.CampusId, Role = 4, UserCode = userCode,
            Status = 1, BoundAt = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        await Db.Insertable(binding).ExecuteCommandAsync();
        return true;
    }

    private async Task CheckMaxStudentCountAsync(long tenantId)
    {
        var org = await Db.Queryable<Organization>().InSingleAsync(tenantId);
        if (org == null || !org.CurrentPackageId.HasValue) return;
        var pkg = await Db.Queryable<OrgPackage>().InSingleAsync(org.CurrentPackageId.Value);
        if (pkg == null || pkg.MaxStudentCount == -1) return;
        var count = await Db.Queryable<UserOrgBinding>()
            .Where(b => b.TenantId == tenantId && b.Role == 4 && b.Status == 1).CountAsync();
        if (count >= pkg.MaxStudentCount)
            throw new BizException($"当前套餐最多允许{pkg.MaxStudentCount}名学生");
    }
}
