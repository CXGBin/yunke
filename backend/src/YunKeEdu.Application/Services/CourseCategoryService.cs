using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class CourseCategoryService : BaseService
{
    public CourseCategoryService(ISqlSugarClient db) : base(db) { }

    public async Task<List<CategoryTreeNode>> GetTreeAsync(long tenantId)
    {
        var list = await Db.Queryable<CourseCategory>()
            .Where(c => c.TenantId == tenantId && !c.IsDeleted)
            .OrderBy(c => c.SortOrder).ToListAsync();
        return BuildTree(list, 0);
    }

    public async Task<long> CreateAsync(CreateCategoryRequest req, CurrentUser user)
    {
        var cat = new CourseCategory
        {
            TenantId = user.TenantId, OrgId = user.OrgId, ParentId = req.ParentId,
            Name = req.Name, Icon = req.Icon, SortOrder = req.SortOrder,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        return await Db.Insertable(cat).ExecuteReturnBigIdentityAsync();
    }

    public async Task UpdateAsync(long id, UpdateCategoryRequest req, CurrentUser user)
    {
        var cat = await Db.Queryable<CourseCategory>()
            .Where(c => c.Id == id && c.TenantId == user.TenantId && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("分类不存在");
        cat.Name = req.Name; cat.Icon = req.Icon; cat.SortOrder = req.SortOrder;
        cat.UpdatedAt = DateTime.Now;
        await Db.Updateable(cat).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(long id, CurrentUser user)
    {
        var hasChildren = await Db.Queryable<CourseCategory>()
            .Where(c => c.ParentId == id && c.TenantId == user.TenantId && !c.IsDeleted).AnyAsync();
        if (hasChildren) throw new BizException("存在子分类，不可删除");

        var hasCourse = await Db.Queryable<Course>()
            .Where(c => c.CategoryId == id && c.TenantId == user.TenantId && !c.IsDeleted).AnyAsync();
        if (hasCourse) throw new BizException("该分类下有课程，不可删除");

        var cat = await Db.Queryable<CourseCategory>()
            .Where(c => c.Id == id && c.TenantId == user.TenantId).FirstAsync()
            ?? throw new BizException("分类不存在");
        cat.IsDeleted = true; cat.UpdatedAt = DateTime.Now;
        await Db.Updateable(cat).UpdateColumns(c => new { c.IsDeleted, c.UpdatedAt }).ExecuteCommandAsync();
    }

    private static List<CategoryTreeNode> BuildTree(List<CourseCategory> list, long parentId)
    {
        return list.Where(c => c.ParentId == parentId).Select(c => new CategoryTreeNode
        {
            Id = c.Id, Name = c.Name, Icon = c.Icon, SortOrder = c.SortOrder,
            ParentId = c.ParentId, Children = BuildTree(list, c.Id),
        }).ToList();
    }
}
