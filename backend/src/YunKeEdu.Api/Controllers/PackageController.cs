using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;


namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/packages")]
public class PackageController : ControllerBase
{
    private readonly ISqlSugarClient _db;

    public PackageController(ISqlSugarClient db) => _db = db;

    // 年费套餐
    [HttpGet("annual/list")]
    public async Task<ApiResponse<List<object>>> GetAnnualPackages()
    {
        var items = await _db.Queryable<OrgPackage>().Where(x => !x.IsDeleted).OrderBy(x => x.PackageLevel).ToListAsync();
        return ApiResponse<List<object>>.Ok(items.Select(x => (object)x).ToList());
    }

    [HttpGet("annual/{id}")]
    public async Task<ApiResponse<object>> GetAnnualPackage(long id)
    {
        var p = await _db.Queryable<OrgPackage>().Where(x => x.Id == id && !x.IsDeleted).FirstAsync();
        if (p == null) throw new BizException("套餐不存在");
        var features = await _db.Queryable<OrgPackageFeature>().Where(x => x.PackageId == id).ToListAsync();
        return ApiResponse<object>.Ok(new { package = p, features });
    }

    [HttpPost("annual")]
    public async Task<ApiResponse<bool>> CreateAnnualPackage([FromBody] OrgPackage req)
    {
        req.CreatedAt = DateTime.Now; req.UpdatedAt = DateTime.Now;
        await _db.Insertable(req).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("annual/{id}")]
    public async Task<ApiResponse<bool>> UpdateAnnualPackage(long id, [FromBody] OrgPackage req)
    {
        req.Id = id; req.UpdatedAt = DateTime.Now;
        await _db.Updateable(req).IgnoreColumns(x => new { x.PackageCode, x.CreatedAt, x.IsDeleted }).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("annual/{id}")]
    public async Task<ApiResponse<bool>> DeleteAnnualPackage(long id)
    {
        await _db.Updateable<OrgPackage>().SetColumns(x => x.IsDeleted == true).Where(x => x.Id == id).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    // 套餐功能
    [HttpPost("annual/{packageId}/features")]
    public async Task<ApiResponse<bool>> SetFeatures(long packageId, [FromBody] List<OrgPackageFeature> features)
    {
        await _db.Deleteable<OrgPackageFeature>().Where(x => x.PackageId == packageId).ExecuteCommandAsync();
        foreach (var f in features) { f.Id = 0; f.PackageId = packageId; f.CreatedAt = DateTime.Now; }
        await _db.Insertable(features).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    // 机构订阅
    [HttpGet("subscriptions")]
    public async Task<ApiResponse<PagedResult<object>>> GetSubscriptions([FromQuery] long? orgId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var q = _db.Queryable<OrgSubscription>();
        if (orgId.HasValue) q = q.Where(x => x.OrgId == orgId);
        RefAsync<int> total = 0;
        var items = await q.OrderByDescending(x => x.CreatedAt).ToPageListAsync(page, pageSize, total);
        return ApiResponse<PagedResult<object>>.Ok(new PagedResult<object>(items.Select(x => (object)x).ToList(), total, page, pageSize));
    }

    [HttpPost("subscriptions")]
    public async Task<ApiResponse<bool>> CreateSubscription([FromBody] OrgSubscription req)
    {
        req.CreatedAt = DateTime.Now; req.UpdatedAt = DateTime.Now;
        await _db.Insertable(req).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    // 升级订单
    [HttpPost("upgrade")]
    public async Task<ApiResponse<bool>> CreateUpgradeOrder([FromBody] PackageUpgradeOrder req)
    {
        req.CreatedAt = DateTime.Now; req.UpdatedAt = DateTime.Now;
        await _db.Insertable(req).ExecuteCommandAsync();
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("upgrade/list")]
    public async Task<ApiResponse<PagedResult<object>>> GetUpgradeOrders([FromQuery] long? orgId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var q = _db.Queryable<PackageUpgradeOrder>();
        if (orgId.HasValue) q = q.Where(x => x.OrgId == orgId);
        RefAsync<int> total = 0;
        var items = await q.OrderByDescending(x => x.CreatedAt).ToPageListAsync(page, pageSize, total);
        return ApiResponse<PagedResult<object>>.Ok(new PagedResult<object>(items.Select(x => (object)x).ToList(), total, page, pageSize));
    }
}
