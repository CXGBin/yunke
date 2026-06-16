
using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class OrgPackageService : BaseService
{
    public OrgPackageService(ISqlSugarClient db) : base(db) { }

    public async Task<PagedResult<PackageDto>> GetPageAsync(PageRequest req)
    {
        var query = Db.Queryable<OrgPackage>().Where(p => !p.IsDeleted);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where(p => p.PackageName.Contains(req.Keyword!) || p.PackageCode.Contains(req.Keyword!));
        query = query.OrderBy(p => p.PackageLevel).OrderBy(p => p.SortOrder);
        RefAsync<int> total = 0;
        var entities = await query.ToPageListAsync(req.Page, req.PageSize, total);
        var dtos = entities.Select(MapToDto).ToList();
        foreach (var dto in dtos) await LoadFeaturesAsync(dto);
        return new PagedResult<PackageDto>(dtos, total, req.Page, req.PageSize);
    }

    public async Task<PackageDto> GetByIdAsync(long id)
    {
        var pkg = await Db.Queryable<OrgPackage>().Where(p => p.Id == id && !p.IsDeleted).FirstAsync()
            ?? throw new BizException("套餐不存在");
        var dto = MapToDto(pkg);
        await LoadFeaturesAsync(dto);
        return dto;
    }

    public async Task<long> CreateAsync(CreatePackageRequest req)
    {
        if (await Db.Queryable<OrgPackage>().Where(p => p.PackageCode == req.PackageCode && !p.IsDeleted).AnyAsync())
            throw new BizException("套餐编码已存在");
        if (await Db.Queryable<OrgPackage>().Where(p => p.PackageLevel == req.PackageLevel && !p.IsDeleted).AnyAsync())
            throw new BizException("该等级套餐已存在");

        var pkg = new OrgPackage
        {
            PackageName = req.PackageName, PackageCode = req.PackageCode, PackageLevel = req.PackageLevel,
            Price = req.Price, Description = req.Description, Images = req.Images,
            MaxCampusCount = req.MaxCampusCount, MaxTeacherCount = req.MaxTeacherCount,
            MaxStudentCount = req.MaxStudentCount, MaxNotificationTypes = req.MaxNotificationTypes,
            MaxPushChannels = req.MaxPushChannels, AnalyticsDimensions = req.AnalyticsDimensions,
            SortOrder = req.SortOrder, Status = 1,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        return await Db.Insertable(pkg).ExecuteReturnBigIdentityAsync();
    }

    public async Task UpdateAsync(long id, UpdatePackageRequest req)
    {
        var pkg = await Db.Queryable<OrgPackage>().InSingleAsync(id) ?? throw new BizException("套餐不存在");
        pkg.PackageName = req.PackageName; pkg.Price = req.Price;
        pkg.Description = req.Description; pkg.Images = req.Images;
        pkg.MaxCampusCount = req.MaxCampusCount; pkg.MaxTeacherCount = req.MaxTeacherCount;
        pkg.MaxStudentCount = req.MaxStudentCount; pkg.MaxNotificationTypes = req.MaxNotificationTypes;
        pkg.MaxPushChannels = req.MaxPushChannels; pkg.AnalyticsDimensions = req.AnalyticsDimensions;
        pkg.SortOrder = req.SortOrder; pkg.UpdatedAt = DateTime.Now;
        await Db.Updateable(pkg).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var pkg = await Db.Queryable<OrgPackage>().InSingleAsync(id) ?? throw new BizException("套餐不存在");
        pkg.IsDeleted = true; pkg.UpdatedAt = DateTime.Now;
        await Db.Updateable(pkg).UpdateColumns(p => new { p.IsDeleted, p.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task AddFeatureAsync(long packageId, AddFeatureRequest req)
    {
        var exists = await Db.Queryable<OrgPackageFeature>()
            .Where(f => f.PackageId == packageId && f.FeatureCode == req.FeatureCode).AnyAsync();
        if (exists) throw new BizException("该功能已添加");

        await Db.Insertable(new OrgPackageFeature
        {
            PackageId = packageId, FeatureCode = req.FeatureCode, FeatureName = req.FeatureName,
            FeatureGroup = req.FeatureGroup, MinPackageLevel = req.MinPackageLevel,
            SortOrder = req.SortOrder, CreatedAt = DateTime.Now,
        }).ExecuteCommandAsync();
    }

    public async Task RemoveFeatureAsync(long packageId, string featureCode)
    {
        await Db.Deleteable<OrgPackageFeature>()
            .Where(f => f.PackageId == packageId && f.FeatureCode == featureCode).ExecuteCommandAsync();
    }

    public async Task<PackageCompareDto> CompareAsync()
    {
        var packages = await Db.Queryable<OrgPackage>().Where(p => !p.IsDeleted && p.Status == 1)
            .OrderBy(p => p.PackageLevel).ToListAsync();
        var pkgDtos = packages.Select(MapToDto).ToList();

        var features = await Db.Queryable<OrgPackageFeature>()
            .Where(f => packages.Select(p => p.Id).Contains(f.PackageId)).ToListAsync();

        var featureGroups = features.GroupBy(f => f.FeatureCode).Select(g =>
        {
            var first = g.First();
            var enabled = new Dictionary<int, bool>();
            foreach (var pkg in packages)
                enabled[pkg.PackageLevel] = g.Any(f => f.PackageId == pkg.Id);
            return new FeatureCompareItem
            {
                FeatureCode = first.FeatureCode, FeatureName = first.FeatureName,
                FeatureGroup = first.FeatureGroup, PackageEnabled = enabled,
            };
        }).ToList();

        return new PackageCompareDto { Packages = pkgDtos, Features = featureGroups };
    }

    private async Task LoadFeaturesAsync(PackageDto dto)
    {
        dto.Features = await Db.Queryable<OrgPackageFeature>()
            .Where(f => f.PackageId == dto.Id)
            .Select(f => new PackageFeatureDto
            {
                Id = f.Id, PackageId = f.PackageId, FeatureCode = f.FeatureCode,
                FeatureName = f.FeatureName, FeatureGroup = f.FeatureGroup,
                MinPackageLevel = f.MinPackageLevel, SortOrder = f.SortOrder,
            }).ToListAsync();
    }

    private static PackageDto MapToDto(OrgPackage p) => new()
    {
        Id = p.Id, PackageName = p.PackageName, PackageCode = p.PackageCode,
        PackageLevel = p.PackageLevel, Price = p.Price, Description = p.Description,
        Images = p.Images, MaxCampusCount = p.MaxCampusCount, MaxTeacherCount = p.MaxTeacherCount,
        MaxStudentCount = p.MaxStudentCount, MaxNotificationTypes = p.MaxNotificationTypes,
        MaxPushChannels = p.MaxPushChannels, AnalyticsDimensions = p.AnalyticsDimensions,
        SortOrder = p.SortOrder, Status = p.Status, CreatedAt = p.CreatedAt,
    };
}
