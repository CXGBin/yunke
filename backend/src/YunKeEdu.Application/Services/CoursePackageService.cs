using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class CoursePackageService : BaseService
{
    public CoursePackageService(ISqlSugarClient db) : base(db) { }

    public async Task<PagedResult<CoursePackageDto>> GetPageAsync(PageRequest req, CurrentUser user)
    {
        var query = Db.Queryable<CoursePackage>()
            .LeftJoin<Organization>((p, o) => p.TenantId == o.Id && o.TenantId == o.Id)
            .Where((p, o) => p.IsDeleted == false);
        if (user.Role != 1)
            query = query.Where((p, o) => p.TenantId == user.TenantId);
        if (user.Role == 4 || user.Role == 5)
            query = query.Where((p, o) => p.Status == 1);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where((p, o) => p.PackageName.Contains(req.Keyword!));
        query = query.OrderBy((p, o) => p.SortOrder).OrderByDescending((p, o) => p.CreatedAt);
        RefAsync<int> total = 0;
        var list = await query.Select((p, o) => new CoursePackageDto
        {
            Id = p.Id, PackageName = p.PackageName, Description = p.Description, CoverImage = p.CoverImage,
            TotalPrice = p.TotalPrice, CourseCount = p.CourseCount, Status = p.Status,
            BuyCount = p.BuyCount, SortOrder = p.SortOrder, IsRecommend = p.IsRecommend,
            OrgId = p.OrgId, OrgName = o.Name, CreatedAt = p.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<CoursePackageDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<CoursePackageDto> GetByIdAsync(long id, CurrentUser user)
    {
        var pkg = await Db.Queryable<CoursePackage>()
            .LeftJoin<Organization>((p, o) => p.TenantId == o.Id && o.TenantId == o.Id)
            .Where((p, o) => p.Id == id && p.IsDeleted == false)
            .Select((p, o) => new CoursePackageDto
            {
                Id = p.Id, PackageName = p.PackageName, Description = p.Description, CoverImage = p.CoverImage,
                TotalPrice = p.TotalPrice, CourseCount = p.CourseCount, Status = p.Status,
                BuyCount = p.BuyCount, SortOrder = p.SortOrder, IsRecommend = p.IsRecommend,
                OrgId = p.OrgId, OrgName = o.Name, CreatedAt = p.CreatedAt,
            }).FirstAsync() ?? throw new BizException("课程套餐不存在");

        pkg.Items = await Db.Queryable<CoursePackageItem>()
            .Where(i => i.PackageId == id).OrderBy(i => i.SortOrder)
            .Select(i => new CoursePackageItemDto
            {
                Id = i.Id, CourseId = i.CourseId, CourseName = i.CourseName,
                UnitPrice = i.UnitPrice, SortOrder = i.SortOrder,
            }).ToListAsync();
        return pkg;
    }

    public async Task<long> CreateAsync(CreateCoursePackageRequest req, CurrentUser user)
    {
        var courses = await Db.Queryable<Course>()
            .Where(c => req.CourseIds.Contains(c.Id) && c.TenantId == user.TenantId && c.Status == 1 && !c.IsDeleted)
            .ToListAsync();
        var totalPrice = courses.Sum(c => c.DiscountPrice > 0 ? c.DiscountPrice : c.OriginalPrice);

        var pkg = new CoursePackage
        {
            TenantId = user.TenantId, OrgId = user.OrgId, CampusId = req.CampusId,
            PackageName = req.PackageName, Description = req.Description,
            CoverImage = req.CoverImage, TotalPrice = totalPrice,
            CourseCount = courses.Count, Status = 1, SortOrder = req.SortOrder,
            IsRecommend = req.IsRecommend,
            ScheduledPublishTime = req.ScheduledPublishTime, ScheduledOfflineTime = req.ScheduledOfflineTime,
            CreatedBy = user.UserId, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        pkg.Id = await Db.Insertable(pkg).ExecuteReturnBigIdentityAsync();

        foreach (var course in courses)
        {
            await Db.Insertable(new CoursePackageItem
            {
                TenantId = user.TenantId, PackageId = pkg.Id, CourseId = course.Id,
                CourseName = course.Name, UnitPrice = course.DiscountPrice > 0 ? course.DiscountPrice : course.OriginalPrice,
                SortOrder = req.CourseIds.IndexOf(course.Id), CreatedAt = DateTime.Now,
            }).ExecuteCommandAsync();
        }
        return pkg.Id;
    }

    public async Task UpdateAsync(long id, UpdateCoursePackageRequest req, CurrentUser user)
    {
        var pkg = await Db.Queryable<CoursePackage>()
            .Where(p => p.Id == id && p.TenantId == user.TenantId && !p.IsDeleted).FirstAsync()
            ?? throw new BizException("课程套餐不存在");
        pkg.PackageName = req.PackageName; pkg.Description = req.Description;
        pkg.CoverImage = req.CoverImage; pkg.CampusId = req.CampusId;
        pkg.SortOrder = req.SortOrder; pkg.IsRecommend = req.IsRecommend;
        pkg.ScheduledPublishTime = req.ScheduledPublishTime;
        pkg.ScheduledOfflineTime = req.ScheduledOfflineTime;
        pkg.UpdatedAt = DateTime.Now;
        await Db.Updateable(pkg).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(long id, CurrentUser user)
    {
        var pkg = await Db.Queryable<CoursePackage>()
            .Where(p => p.Id == id && p.TenantId == user.TenantId && !p.IsDeleted).FirstAsync()
            ?? throw new BizException("课程套餐不存在");
        if (pkg.Status != 0) throw new BizException("仅草稿套餐可删除");
        pkg.IsDeleted = true; pkg.UpdatedAt = DateTime.Now;
        await Db.Updateable(pkg).UpdateColumns(p => new { p.IsDeleted, p.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task PublishAsync(long id, CurrentUser user)
    {
        var pkg = await Db.Queryable<CoursePackage>()
            .Where(p => p.Id == id && p.TenantId == user.TenantId && !p.IsDeleted).FirstAsync()
            ?? throw new BizException("课程套餐不存在");
        pkg.Status = 1; pkg.UpdatedAt = DateTime.Now;
        await Db.Updateable(pkg).UpdateColumns(p => new { p.Status, p.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task OfflineAsync(long id, CurrentUser user)
    {
        var pkg = await Db.Queryable<CoursePackage>()
            .Where(p => p.Id == id && p.TenantId == user.TenantId && !p.IsDeleted).FirstAsync()
            ?? throw new BizException("课程套餐不存在");
        pkg.Status = 2; pkg.UpdatedAt = DateTime.Now;
        await Db.Updateable(pkg).UpdateColumns(p => new { p.Status, p.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task AddCourseAsync(long packageId, long courseId, CurrentUser user)
    {
        var exists = await Db.Queryable<CoursePackageItem>()
            .Where(i => i.PackageId == packageId && i.CourseId == courseId).AnyAsync();
        if (exists) throw new BizException("该课程已在套餐中");

        var course = await Db.Queryable<Course>()
            .Where(c => c.Id == courseId && c.TenantId == user.TenantId && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("课程不存在");

        await Db.Insertable(new CoursePackageItem
        {
            TenantId = user.TenantId, PackageId = packageId, CourseId = courseId,
            CourseName = course.Name, UnitPrice = course.DiscountPrice > 0 ? course.DiscountPrice : course.OriginalPrice,
            CreatedAt = DateTime.Now,
        }).ExecuteCommandAsync();

        var pkg = await Db.Queryable<CoursePackage>().InSingleAsync(packageId);
        if (pkg != null)
        {
            pkg.CourseCount = await Db.Queryable<CoursePackageItem>()
                .Where(i => i.PackageId == packageId).CountAsync();
            pkg.TotalPrice = await Db.Queryable<CoursePackageItem>()
                .Where(i => i.PackageId == packageId).SumAsync(i => i.UnitPrice);
            pkg.UpdatedAt = DateTime.Now;
            await Db.Updateable(pkg).ExecuteCommandAsync();
        }
    }

    public async Task RemoveCourseAsync(long packageId, long courseId, CurrentUser user)
    {
        await Db.Deleteable<CoursePackageItem>()
            .Where(i => i.PackageId == packageId && i.CourseId == courseId).ExecuteCommandAsync();

        var pkg = await Db.Queryable<CoursePackage>().InSingleAsync(packageId);
        if (pkg != null)
        {
            pkg.CourseCount = await Db.Queryable<CoursePackageItem>()
                .Where(i => i.PackageId == packageId).CountAsync();
            pkg.TotalPrice = await Db.Queryable<CoursePackageItem>()
                .Where(i => i.PackageId == packageId).SumAsync(i => i.UnitPrice);
            pkg.UpdatedAt = DateTime.Now;
            await Db.Updateable(pkg).ExecuteCommandAsync();
        }
    }

    public async Task<List<CourseDto>> GetAvailableCoursesAsync(CurrentUser user)
    {
        return await Db.Queryable<Course>()
            .Where(c => c.TenantId == user.TenantId && c.Status == 1 && !c.IsDeleted)
            .Select(c => new CourseDto { Id = c.Id, Name = c.Name, OriginalPrice = c.OriginalPrice, DiscountPrice = c.DiscountPrice })
            .ToListAsync();
    }
}
