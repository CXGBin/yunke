using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class ParentService : BaseService
{
    public ParentService(ISqlSugarClient db) : base(db) { }

    public async Task<PagedResult<ParentDto>> GetPageAsync(PageRequest req, long tenantId)
    {
        var query = Db.Queryable<SysUser>()
            .LeftJoin<UserOrgBinding>((u, b) => u.Id == b.UserId && b.TenantId == tenantId)
            .LeftJoin<ParentStudentRelation>((u, b, r) => b.UserId == r.ParentId)
            .Where((u, b, r) => u.IsDeleted == false && b.Role == 5 && b.Status == 1);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where((u, b, r) => u.Phone!.Contains(req.Keyword!) || u.RealName!.Contains(req.Keyword!));
        query = query.GroupBy((u, b, r) => new { u.Id, u.UserName, u.RealName, u.Avatar, u.Phone, u.CreatedAt });
        RefAsync<int> total = 0;
        var list = await query.Select((u, b, r) => new ParentDto
        {
            Id = u.Id, UserName = u.UserName, RealName = u.RealName,
            Avatar = u.Avatar, Phone = u.Phone,
            ChildrenCount = SqlFunc.AggregateCount(r.ParentId),
            CreatedAt = u.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<ParentDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<ParentDetailDto> GetByIdAsync(long id, long tenantId)
    {
        var parent = await Db.Queryable<SysUser>()
            .LeftJoin<UserOrgBinding>((u, b) => u.Id == b.UserId && b.TenantId == tenantId)
            .Where((u, b) => u.Id == id && u.IsDeleted == false && b.Role == 5)
            .FirstAsync() ?? throw new BizException("家长不存在");

        var children = await Db.Queryable<ParentStudentRelation>()
            .LeftJoin<SysUser>((r, s) => r.StudentId == s.Id)
            .Where((r, s) => r.ParentId == id && r.Status == 1)
            .Select((r, s) => new ChildInfo
            {
                RelationId = r.Id, StudentId = s.Id, StudentName = s.RealName ?? s.NickName ?? "",
                StudentAvatar = s.Avatar, RelationType = r.RelationType, IsPrimary = r.IsPrimary,
            }).ToListAsync();

        return new ParentDetailDto
        {
            Id = parent.Id, UserName = parent.UserName, RealName = parent.RealName,
            Avatar = parent.Avatar, Phone = parent.Phone, Children = children,
            CreatedAt = parent.CreatedAt,
        };
    }

    public async Task BindStudentAsync(BindStudentRequest req, CurrentUser user)
    {
        var rel = await Db.Queryable<ParentStudentRelation>()
            .Where(r => r.ParentId == req.ParentId && r.StudentId == req.StudentId && r.Status == 1).FirstAsync();
        if (rel != null) throw new BizException("该关联已存在");

        var parentCount = await Db.Queryable<ParentStudentRelation>()
            .Where(r => r.ParentId == req.ParentId && r.Status == 1).CountAsync();
        if (parentCount >= 10) throw new BizException("一个家长最多关联10个孩子");

        var studentCount = await Db.Queryable<ParentStudentRelation>()
            .Where(r => r.StudentId == req.StudentId && r.Status == 1).CountAsync();
        if (studentCount >= 5) throw new BizException("一个学生最多关联5名家长");

        await Db.Insertable(new ParentStudentRelation
        {
            TenantId = user.TenantId, OrgId = user.OrgId, ParentId = req.ParentId,
            StudentId = req.StudentId, RelationType = req.RelationType, IsPrimary = req.IsPrimary,
            Status = 1, ConfirmedBy = user.UserId, ConfirmedAt = DateTime.Now,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        }).ExecuteCommandAsync();
    }

    public async Task UnbindAsync(long relationId, CurrentUser user)
    {
        var rel = await Db.Queryable<ParentStudentRelation>().InSingleAsync(relationId)
            ?? throw new BizException("关联记录不存在");
        rel.Status = 0; rel.UpdatedAt = DateTime.Now;
        await Db.Updateable(rel).UpdateColumns(r => new { r.Status, r.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task LinkStudentAsync(LinkStudentRequest req, CurrentUser user)
    {
        if (user.Role != 5) throw new BizException("仅家长可关联学生");
        var binding = await Db.Queryable<UserOrgBinding>()
            .Where(b => b.UserId == user.UserId && b.Status == 1).FirstAsync();
        if (binding == null) throw new BizException("您尚未绑定任何机构");

        var studentBinding = await Db.Queryable<UserOrgBinding>()
            .Where(b => b.UserCode == req.StudentUserCode && b.Role == 4 && b.Status == 1 && b.TenantId == binding.TenantId)
            .FirstAsync() ?? throw new BizException("未找到该学生");

        var exists = await Db.Queryable<ParentStudentRelation>()
            .Where(r => r.ParentId == user.UserId && r.StudentId == studentBinding.UserId && r.Status == 1).AnyAsync();
        if (exists) throw new BizException("已关联该学生，请勿重复操作");

        await Db.Insertable(new ParentStudentRelation
        {
            TenantId = binding.TenantId, OrgId = binding.OrgId,
            ParentId = user.UserId, StudentId = studentBinding.UserId,
            RelationType = 4, Status = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        }).ExecuteCommandAsync();
    }

    public async Task<List<ChildInfo>> GetMyChildrenAsync(CurrentUser user)
    {
        return await Db.Queryable<ParentStudentRelation>()
            .LeftJoin<SysUser>((r, s) => r.StudentId == s.Id)
            .Where((r, s) => r.ParentId == user.UserId && r.Status == 1)
            .Select((r, s) => new ChildInfo
            {
                RelationId = r.Id, StudentId = s.Id, StudentName = s.RealName ?? s.NickName ?? "",
                StudentAvatar = s.Avatar, RelationType = r.RelationType, IsPrimary = r.IsPrimary,
            }).ToListAsync();
    }

    public async Task<List<ParentDto>> GetMyParentsAsync(CurrentUser user)
    {
        return await Db.Queryable<ParentStudentRelation>()
            .LeftJoin<SysUser>((r, p) => r.ParentId == p.Id)
            .Where((r, p) => r.StudentId == user.UserId && r.Status == 1)
            .Select((r, p) => new ParentDto
            {
                Id = p.Id, UserName = p.UserName, RealName = p.RealName,
                Avatar = p.Avatar, Phone = p.Phone, CreatedAt = p.CreatedAt,
            }).ToListAsync();
    }
}
