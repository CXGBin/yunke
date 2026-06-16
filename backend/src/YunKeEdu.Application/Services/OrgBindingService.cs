using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class OrgBindingService : BaseService
{
    public OrgBindingService(ISqlSugarClient db) : base(db) { }

    public async Task<List<UserOrgInfo>> GetMyOrgsAsync(CurrentUser user)
    {
        if (user.Role == 0) throw new BizException("无角色用户无绑定机构");

        var bindings = await Db.Queryable<UserOrgBinding>()
            .Where(b => b.UserId == user.UserId && b.Status == 1).ToListAsync();
        if (!bindings.Any()) return new List<UserOrgInfo>();

        var orgIds = bindings.Select(b => b.OrgId).Distinct().ToList();
        var orgs = await Db.Queryable<Organization>().Where(o => orgIds.Contains(o.Id) && !o.IsDeleted).ToListAsync();
        var orgDict = orgs.ToDictionary(o => o.Id);

        var campusIds = bindings.Select(b => b.CampusId).Distinct().ToList();
        var campuses = await Db.Queryable<Campus>().Where(c => campusIds.Contains(c.Id) && !c.IsDeleted).ToListAsync();
        var campusDict = campuses.ToDictionary(c => c.Id);

        return bindings.Select(b => new UserOrgInfo
        {
            OrgId = b.OrgId,
            OrgName = orgDict.TryGetValue(b.OrgId, out var o) ? o.Name : "",
            CampusId = b.CampusId,
            CampusName = campusDict.TryGetValue(b.CampusId, out var c) ? c.Name : "",
            Role = b.Role, UserCode = b.UserCode,
        }).ToList();
    }

    public async Task<UserOrgInfo> GetDetailAsync(long orgId, CurrentUser user)
    {
        var binding = await Db.Queryable<UserOrgBinding>()
            .Where(b => b.UserId == user.UserId && b.OrgId == orgId && b.Status == 1).FirstAsync()
            ?? throw new BizException("未绑定该机构");

        var org = await Db.Queryable<Organization>().Where(o => o.Id == orgId && !o.IsDeleted).FirstAsync()
            ?? throw new BizException("机构不存在");

        var campus = await Db.Queryable<Campus>().InSingleAsync(binding.CampusId);

        return new UserOrgInfo
        {
            OrgId = org.Id, OrgName = org.Name, CampusId = binding.CampusId,
            CampusName = campus?.Name ?? "", Role = binding.Role, UserCode = binding.UserCode,
        };
    }
}
