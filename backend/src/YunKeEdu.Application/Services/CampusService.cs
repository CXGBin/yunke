using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class CampusService : BaseService
{
    public CampusService(ISqlSugarClient db) : base(db) { }

    public async Task<List<CampusDto>> GetListAsync(long tenantId)
    {
        var list = await Db.Queryable<Campus>()
            .Where(c => c.TenantId == tenantId && !c.IsDeleted)
            .OrderBy(c => c.SortOrder).ToListAsync();
        return list.Select(c => MapToDto(c)).ToList();
    }

    public async Task<CampusDto> GetByIdAsync(long id, long tenantId)
    {
        var c = await Db.Queryable<Campus>()
            .Where(x => x.Id == id && x.TenantId == tenantId && !x.IsDeleted).FirstAsync()
            ?? throw new BizException("校区不存在");
        return MapToDto(c);
    }

    public async Task<long> CreateAsync(CreateCampusRequest req, CurrentUser user)
    {
        await CheckMaxCampusCountAsync(user.TenantId);

        var code = $"{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(10, 99)}";
        var campus = new Campus
        {
            TenantId = user.TenantId, OrgId = user.OrgId, CampusCode = code,
            Name = req.Name, ContactPerson = req.ContactPerson, ContactPhone = req.ContactPhone,
            Address = req.Address, Longitude = req.Longitude, Latitude = req.Latitude,
            SortOrder = req.SortOrder, Status = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        return await Db.Insertable(campus).ExecuteReturnBigIdentityAsync();
    }

    public async Task UpdateAsync(long id, UpdateCampusRequest req, CurrentUser user)
    {
        var campus = await Db.Queryable<Campus>()
            .Where(x => x.Id == id && x.TenantId == user.TenantId && !x.IsDeleted).FirstAsync()
            ?? throw new BizException("校区不存在");
        if (campus.IsDefault) throw new BizException("默认校区不可修改");
        campus.Name = req.Name; campus.ContactPerson = req.ContactPerson;
        campus.ContactPhone = req.ContactPhone; campus.Address = req.Address;
        campus.Longitude = req.Longitude; campus.Latitude = req.Latitude;
        campus.SortOrder = req.SortOrder; campus.UpdatedAt = DateTime.Now;
        await Db.Updateable(campus).ExecuteCommandAsync();
    }

    public async Task UpdateStatusAsync(long id, int status, CurrentUser user)
    {
        var campus = await Db.Queryable<Campus>()
            .Where(x => x.Id == id && x.TenantId == user.TenantId && !x.IsDeleted).FirstAsync()
            ?? throw new BizException("校区不存在");
        if (campus.IsDefault) throw new BizException("默认校区不可停用");
        campus.Status = status; campus.UpdatedAt = DateTime.Now;
        await Db.Updateable(campus).UpdateColumns(c => new { c.Status, c.UpdatedAt }).ExecuteCommandAsync();
    }

    private async Task CheckMaxCampusCountAsync(long tenantId)
    {
        var org = await Db.Queryable<Organization>().InSingleAsync(tenantId);
        if (org == null || !org.CurrentPackageId.HasValue) return;
        var pkg = await Db.Queryable<OrgPackage>().InSingleAsync(org.CurrentPackageId.Value);
        if (pkg == null || pkg.MaxCampusCount == -1) return;
        var count = await Db.Queryable<Campus>()
            .Where(c => c.TenantId == tenantId && !c.IsDeleted).CountAsync();
        if (count >= pkg.MaxCampusCount)
            throw new BizException($"当前套餐最多允许{pkg.MaxCampusCount}个校区");
    }

    private static CampusDto MapToDto(Campus c) => new()
    {
        Id = c.Id, OrgId = c.OrgId, CampusCode = c.CampusCode, Name = c.Name,
        IsDefault = c.IsDefault, ContactPerson = c.ContactPerson, ContactPhone = c.ContactPhone,
        Address = c.Address, Longitude = c.Longitude, Latitude = c.Latitude,
        Status = c.Status, SortOrder = c.SortOrder, CreatedAt = c.CreatedAt,
    };
}
