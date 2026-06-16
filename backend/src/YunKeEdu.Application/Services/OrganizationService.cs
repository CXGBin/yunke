using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class OrganizationService : BaseService
{
    public OrganizationService(ISqlSugarClient db) : base(db) { }

    public async Task<PagedResult<OrgDto>> GetPageAsync(PageRequest req, int? status = null)
    {
        var query = Db.Queryable<Organization>().Where(o => !o.IsDeleted);
        if (status.HasValue) query = query.Where(o => o.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where(o => o.Name.Contains(req.Keyword!) || o.OrgCode.Contains(req.Keyword!));
        query = query.OrderBy(o => o.Id, OrderByType.Desc);
        RefAsync<int> total = 0;
        var items = await query.ToPageListAsync(req.Page, req.PageSize, total);
        var dtos = items.Select(MapToDto).ToList();
        return new PagedResult<OrgDto>(dtos, total, req.Page, req.PageSize);
    }

    public async Task<OrgDto?> GetByIdAsync(long id)
    {
        var org = await Db.Queryable<Organization>().Where(o => o.Id == id && !o.IsDeleted).FirstAsync();
        return org == null ? null : MapToDto(org);
    }

    public async Task<long> CreateAsync(CreateOrgRequest req)
    {
        var orgCode = $"YK{DateTime.Now:yyyyMMdd}{new Random().Next(100, 999)}";
        if (await Db.Queryable<Organization>().Where(o => o.Name == req.Name && !o.IsDeleted).AnyAsync())
            throw new BizException("机构名称已存在");

        var org = new Organization
        {
            OrgCode = orgCode, Name = req.Name, Logo = req.Logo,
            ContactPerson = req.ContactPerson, ContactPhone = req.ContactPhone,
            Address = req.Address, Province = req.Province, City = req.City, District = req.District,
            Description = req.Description, Status = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        org.Id = await Db.Insertable(org).ExecuteReturnBigIdentityAsync();
        org.TenantId = org.Id;
        await Db.Updateable(org).UpdateColumns(o => new { o.TenantId }).ExecuteCommandAsync();

        var campus = new Campus
        {
            TenantId = org.Id, OrgId = org.Id, CampusCode = $"{orgCode}-DEFAULT",
            Name = $"{req.Name}默认校区", IsDefault = true, Status = 1,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        var campusId = await Db.Insertable(campus).ExecuteReturnBigIdentityAsync();

        var orgConfig = new OrgConfig
        {
            TenantId = org.Id, OrgId = org.Id, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        await Db.Insertable(orgConfig).ExecuteCommandAsync();

        var user = new SysUser
        {
            TenantId = org.Id, OrgId = org.Id, CampusId = campusId,
            UserName = $"admin_{orgCode}", Password = BCrypt.Net.BCrypt.HashPassword("Yk@123456"),
            RealName = req.ContactPerson, Phone = req.ContactPhone, Role = 2, Status = 1,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        await Db.Insertable(user).ExecuteCommandAsync();

        var binding = new UserOrgBinding
        {
            UserId = user.Id, TenantId = org.Id, OrgId = org.Id, CampusId = campusId,
            Role = 2, Status = 1, BoundVia = 2, BoundAt = DateTime.Now,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        await Db.Insertable(binding).ExecuteCommandAsync();

        return org.Id;
    }

    public async Task UpdateAsync(long id, UpdateOrgRequest req)
    {
        var org = await Db.Queryable<Organization>().InSingleAsync(id) ?? throw new BizException("机构不存在");
        org.Name = req.Name; org.Logo = req.Logo; org.ContactPerson = req.ContactPerson;
        org.ContactPhone = req.ContactPhone; org.Address = req.Address;
        org.Province = req.Province; org.City = req.City; org.District = req.District;
        org.Description = req.Description; org.UpdatedAt = DateTime.Now;
        await Db.Updateable(org).ExecuteCommandAsync();
    }

    public async Task UpdateStatusAsync(long id, int status)
    {
        var org = await Db.Queryable<Organization>().InSingleAsync(id) ?? throw new BizException("机构不存在");
        org.Status = status; org.UpdatedAt = DateTime.Now;
        await Db.Updateable(org).UpdateColumns(o => new { o.Status, o.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var org = await Db.Queryable<Organization>().InSingleAsync(id) ?? throw new BizException("机构不存在");
        org.IsDeleted = true; org.UpdatedAt = DateTime.Now;
        await Db.Updateable(org).UpdateColumns(o => new { o.IsDeleted, o.UpdatedAt }).ExecuteCommandAsync();
    }

    private static OrgDto MapToDto(Organization org) => new()
    {
        Id = org.Id, OrgCode = org.OrgCode, Name = org.Name, Logo = org.Logo,
        ContactPerson = org.ContactPerson, ContactPhone = org.ContactPhone,
        Address = org.Address, Province = org.Province, City = org.City, District = org.District,
        Status = org.Status, ExpiredAt = org.ExpiredAt, Description = org.Description,
        CurrentPackageId = org.CurrentPackageId, CreatedAt = org.CreatedAt,
    };
}
