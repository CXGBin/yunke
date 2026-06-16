
using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class OrgSubscriptionService : BaseService
{
    public OrgSubscriptionService(ISqlSugarClient db) : base(db) { }

    public async Task<SubscriptionDto> PurchaseAsync(PurchaseRequest req, CurrentUser user)
    {
        var pkg = await Db.Queryable<OrgPackage>().InSingleAsync(req.PackageId)
            ?? throw new BizException("套餐不存在");
        if (pkg.Status != 1) throw new BizException("该套餐已停用");

        var now = DateTime.Now;
        var sub = new OrgSubscription
        {
            TenantId = user.TenantId, OrgId = user.OrgId, PackageId = req.PackageId,
            StartDate = now, EndDate = now.AddYears(1), Amount = req.Amount,
            PayStatus = req.Amount > 0 ? 0 : 1,
            PayTime = req.Amount > 0 ? null : now,
            PayChannel = req.PayChannel, SubscriptionType = 0, Remark = req.Remark,
            CreatedAt = now, UpdatedAt = now,
        };
        sub.Id = await Db.Insertable(sub).ExecuteReturnBigIdentityAsync();

        if (sub.PayStatus == 1) await ActivateSubscriptionAsync(sub, pkg);
        return await MapToDtoAsync(sub);
    }

    public async Task<SubscriptionDto> RenewAsync(RenewRequest req, CurrentUser user)
    {
        var current = await Db.Queryable<OrgSubscription>()
            .Where(s => s.TenantId == user.TenantId && s.PayStatus == 1)
            .OrderByDescending(s => s.EndDate).FirstAsync()
            ?? throw new BizException("无有效订阅，请先购买套餐");

        var pkg = await Db.Queryable<OrgPackage>().InSingleAsync(current.PackageId)
            ?? throw new BizException("套餐不存在");

        var startDate = current.EndDate > DateTime.Now ? current.EndDate : DateTime.Now;
        var sub = new OrgSubscription
        {
            TenantId = user.TenantId, OrgId = user.OrgId, PackageId = current.PackageId,
            StartDate = startDate, EndDate = startDate.AddYears(1), Amount = pkg.Price,
            PayStatus = 0, PayChannel = req.PayChannel, SubscriptionType = 1,
            Remark = req.Remark, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        sub.Id = await Db.Insertable(sub).ExecuteReturnBigIdentityAsync();
        return await MapToDtoAsync(sub);
    }

    public async Task<UpgradeOrderDto> UpgradeAsync(UpgradeRequest req, CurrentUser user)
    {
        var currentSub = await Db.Queryable<OrgSubscription>()
            .Where(s => s.TenantId == user.TenantId && s.PayStatus == 1)
            .OrderByDescending(s => s.EndDate).FirstAsync()
            ?? throw new BizException("无有效订阅，无法升级");

        var oldPkg = await Db.Queryable<OrgPackage>().InSingleAsync(currentSub.PackageId)
            ?? throw new BizException("当前套餐不存在");

        var newPkg = await Db.Queryable<OrgPackage>().InSingleAsync(req.NewPackageId)
            ?? throw new BizException("目标套餐不存在");

        if (newPkg.PackageLevel <= oldPkg.PackageLevel)
            throw new BizException("只能升级到更高等级的套餐");

        var usedMonths = (int)Math.Ceiling((DateTime.Now - currentSub.StartDate).TotalDays / 30);
        if (usedMonths > 12) usedMonths = 12;
        var unusedMonths = Math.Max(0, 12 - usedMonths);
        var oldMonthlyPrice = Math.Floor(oldPkg.Price / 12m * 100) / 100;
        var discountAmount = oldMonthlyPrice * unusedMonths;
        discountAmount = Math.Floor(discountAmount * 100) / 100;
        var payAmount = newPkg.Price - discountAmount;
        if (payAmount < 0) payAmount = 0;

        var order = new PackageUpgradeOrder
        {
            TenantId = user.TenantId, OrgId = user.OrgId,
            OldSubscriptionId = currentSub.Id, OldPackageId = oldPkg.Id,
            NewPackageId = newPkg.Id,
            OldPackagePrice = oldPkg.Price, NewPackagePrice = newPkg.Price,
            UsedMonths = usedMonths, UnusedMonths = unusedMonths,
            OldMonthlyPrice = oldMonthlyPrice, DiscountAmount = discountAmount,
            PayAmount = payAmount, PayStatus = 0, PayChannel = req.PayChannel,
            Remark = req.Remark, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        order.Id = await Db.Insertable(order).ExecuteReturnBigIdentityAsync();
        return await MapUpgradeOrderDtoAsync(order, oldPkg, newPkg);
    }

    public async Task<SubscriptionDto?> GetCurrentAsync(CurrentUser user)
    {
        var sub = await Db.Queryable<OrgSubscription>()
            .Where(s => s.TenantId == user.TenantId && s.PayStatus == 1)
            .OrderByDescending(s => s.EndDate).FirstAsync();
        if (sub == null) return null;
        return await MapToDtoAsync(sub);
    }

    public async Task<PagedResult<SubscriptionDto>> GetHistoryAsync(PageRequest req, long? tenantId = null)
    {
        var query = Db.Queryable<OrgSubscription>();
        if (tenantId.HasValue)
            query = query.Where(s => s.TenantId == tenantId.Value);
        query = query.OrderByDescending(s => s.CreatedAt);
        RefAsync<int> total = 0;
        var items = await query.ToPageListAsync(req.Page, req.PageSize, total);
        var dtos = new List<SubscriptionDto>();
        foreach (var item in items) dtos.Add(await MapToDtoAsync(item));
        return new PagedResult<SubscriptionDto>(dtos, total, req.Page, req.PageSize);
    }

    public async Task<UpgradeOrderDto> GetUpgradeDetailAsync(long id)
    {
        var order = await Db.Queryable<PackageUpgradeOrder>().InSingleAsync(id)
            ?? throw new BizException("升级订单不存在");
        var oldPkg = await Db.Queryable<OrgPackage>().InSingleAsync(order.OldPackageId) ?? new OrgPackage();
        var newPkg = await Db.Queryable<OrgPackage>().InSingleAsync(order.NewPackageId) ?? new OrgPackage();
        return await MapUpgradeOrderDtoAsync(order, oldPkg, newPkg);
    }

    public async Task PayUpgradeAsync(long id)
    {
        var order = await Db.Queryable<PackageUpgradeOrder>().InSingleAsync(id)
            ?? throw new BizException("升级订单不存在");
        if (order.PayStatus != 0) throw new BizException("订单状态无效");

        order.PayStatus = 1; order.PayTime = DateTime.Now; order.UpdatedAt = DateTime.Now;
        await Db.Updateable(order).ExecuteCommandAsync();

        var newPkg = await Db.Queryable<OrgPackage>().InSingleAsync(order.NewPackageId) ?? throw new BizException("套餐不存在");
        var now = DateTime.Now;
        var newSub = new OrgSubscription
        {
            TenantId = order.TenantId, OrgId = order.OrgId, PackageId = order.NewPackageId,
            StartDate = now, EndDate = now.AddYears(1), Amount = order.PayAmount,
            PayStatus = 1, PayTime = now, SubscriptionType = 2,
            PreSubscriptionId = order.OldSubscriptionId,
            CreatedAt = now, UpdatedAt = now,
        };
        newSub.Id = await Db.Insertable(newSub).ExecuteReturnBigIdentityAsync();
        order.NewSubscriptionId = newSub.Id;
        await Db.Updateable(order).UpdateColumns(o => new { o.NewSubscriptionId }).ExecuteCommandAsync();

        await ActivateSubscriptionAsync(newSub, newPkg);

        var oldSub = await Db.Queryable<OrgSubscription>().InSingleAsync(order.OldSubscriptionId);
        if (oldSub != null) oldSub.EndDate = now;
    }

    private async Task ActivateSubscriptionAsync(OrgSubscription sub, OrgPackage pkg)
    {
        var org = await Db.Queryable<Organization>().InSingleAsync(sub.OrgId);
        if (org != null)
        {
            org.CurrentPackageId = pkg.Id; org.UpdatedAt = DateTime.Now;
            await Db.Updateable(org).UpdateColumns(o => new { o.CurrentPackageId, o.UpdatedAt }).ExecuteCommandAsync();
        }
    }

    private async Task<SubscriptionDto> MapToDtoAsync(OrgSubscription s)
    {
        var pkg = await Db.Queryable<OrgPackage>().InSingleAsync(s.PackageId);
        return new SubscriptionDto
        {
            Id = s.Id, OrgId = s.OrgId, PackageId = s.PackageId,
            PackageName = pkg?.PackageName ?? "", PackageLevel = pkg?.PackageLevel ?? 0,
            StartDate = s.StartDate, EndDate = s.EndDate, Amount = s.Amount,
            PayStatus = s.PayStatus, PayTime = s.PayTime, SubscriptionType = s.SubscriptionType,
            PreSubscriptionId = s.PreSubscriptionId, Remark = s.Remark,
            RemainingDays = Math.Max(0, (int)(s.EndDate - DateTime.Now).TotalDays),
            CreatedAt = s.CreatedAt,
        };
    }

    private async Task<UpgradeOrderDto> MapUpgradeOrderDtoAsync(PackageUpgradeOrder o, OrgPackage oldPkg, OrgPackage newPkg)
    {
        return new UpgradeOrderDto
        {
            Id = o.Id, OrgId = o.OrgId, OldPackageId = o.OldPackageId,
            OldPackageName = oldPkg.PackageName, NewPackageId = o.NewPackageId,
            NewPackageName = newPkg.PackageName, OldPackagePrice = o.OldPackagePrice,
            NewPackagePrice = o.NewPackagePrice, UsedMonths = o.UsedMonths,
            UnusedMonths = o.UnusedMonths, OldMonthlyPrice = o.OldMonthlyPrice,
            DiscountAmount = o.DiscountAmount, PayAmount = o.PayAmount,
            PayStatus = o.PayStatus, PayTime = o.PayTime, Remark = o.Remark,
            CreatedAt = o.CreatedAt,
        };
    }
}
