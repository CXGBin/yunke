using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class SettlementService : BaseService
{
    public SettlementService(ISqlSugarClient db) : base(db) { }

    public async Task<SettlementRuleDto> GetRuleAsync(long courseId, long tenantId)
    {
        var course = await Db.Queryable<Course>()
            .Where(c => c.Id == courseId && c.TenantId == tenantId && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("课程不存在");
        return new SettlementRuleDto
        {
            CourseId = course.Id, CourseName = course.Name,
            SettlementType = course.SettlementType, FixedAmount = course.FixedFeePerLesson,
            OriginalPrice = course.DiscountPrice > 0 ? course.DiscountPrice : course.OriginalPrice,
            TotalLessons = course.TotalLessons,
        };
    }

    public async Task<WalletDto> GetWalletAsync(CurrentUser user)
    {
        var wallet = await Db.Queryable<TeacherWallet>()
            .LeftJoin<SysUser>((w, t) => w.TeacherId == t.Id)
            .Where((w, t) => w.TeacherId == user.UserId)
            .Select((w, t) => new WalletDto
            {
                TeacherId = w.TeacherId, TeacherName = t.RealName ?? "",
                Balance = w.Balance, TotalIncome = w.TotalIncome,
                TotalWithdrawn = w.TotalWithdrawn, LastSettlementAt = w.LastSettlementAt,
            }).FirstAsync() ?? new WalletDto { TeacherId = user.UserId };
        return wallet;
    }

    public async Task<PagedResult<WalletDetailDto>> GetWalletDetailAsync(PageRequest req, CurrentUser user)
    {
        var query = Db.Queryable<FeeSettlementRecord>()
            .LeftJoin<Course>((r, c) => r.CourseId == c.Id)
            .Where((r, c) => r.TeacherId == user.UserId)
            .OrderByDescending((r, c) => r.CreatedAt);
        RefAsync<int> total = 0;
        var items = await query.ToPageListAsync(req.Page, req.PageSize, total);
        var dtos = items.Select(r => new WalletDetailDto
        {
            Id = r.Id, CourseId = r.CourseId, CourseName = "",
            ScheduleId = r.ScheduleId, SettlementDate = r.SettlementDate,
            Amount = r.Amount, SettlementType = r.SettlementType,
            StudentCount = r.StudentCount, TriggerType = r.TriggerType,
            Status = r.Status, Remark = r.Remark, CreatedAt = r.CreatedAt,
        }).ToList();
        return new PagedResult<WalletDetailDto>(dtos, total, req.Page, req.PageSize);
    }

    public async Task<PagedResult<FeeSettlementRecordDto>> GetRecordsAsync(PageRequest req, CurrentUser user, long? tenantId = null)
    {
        var query = Db.Queryable<FeeSettlementRecord>()
            .LeftJoin<Course>((r, c) => r.CourseId == c.Id)
            .LeftJoin<SysUser>((r, c, t) => r.TeacherId == t.Id)
            .Where((r, c, t) => true);
        if (tenantId.HasValue) query = query.Where((r, c, t) => r.TenantId == tenantId.Value);
        else if (user.Role == 3) query = query.Where((r, c, t) => r.TeacherId == user.UserId);
        else if (user.Role != 1) query = query.Where((r, c, t) => r.TenantId == user.TenantId);
        query = query.OrderByDescending((r, c, t) => r.CreatedAt);
        RefAsync<int> total = 0;
        var list = await query.Select((r, c, t) => new FeeSettlementRecordDto
        {
            Id = r.Id, CourseId = r.CourseId, CourseName = c.Name,
            ScheduleId = r.ScheduleId, TeacherId = r.TeacherId, TeacherName = t.RealName ?? "",
            Amount = r.Amount, SettlementType = r.SettlementType, StudentCount = r.StudentCount,
            SettlementDate = r.SettlementDate, SettledAt = r.SettledAt,
            TriggerType = r.TriggerType, Status = r.Status, Remark = r.Remark, CreatedAt = r.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<FeeSettlementRecordDto>(list, total, req.Page, req.PageSize);
    }

    public async Task ManualTriggerAsync(ManualTriggerRequest req, CurrentUser user)
    {
        var schedule = await Db.Queryable<CourseSchedule>()
            .Where(s => s.Id == req.ScheduleId && s.TenantId == user.TenantId && !s.IsDeleted).FirstAsync()
            ?? throw new BizException("排课记录不存在");

        var course = await Db.Queryable<Course>().InSingleAsync(schedule.CourseId)
            ?? throw new BizException("课程不存在");

        var studentCount = await Db.Queryable<Attendance>()
            .Where(a => a.ScheduleId == req.ScheduleId && (a.Status == 1 || a.Status == 2)).CountAsync();

        decimal fee;
        if (course.SettlementType == 0)
        {
            fee = course.FixedFeePerLesson + Math.Max(0, studentCount - 1) * course.StudentCountCommission;
        }
        else
        {
            var unitPrice = Math.Floor((course.DiscountPrice > 0 ? course.DiscountPrice : course.OriginalPrice) / Math.Max(1, course.TotalLessons));
            fee = unitPrice + Math.Max(0, studentCount - 1) * course.StudentCountCommission;
        }

        var existing = await Db.Queryable<FeeSettlementRecord>()
            .Where(r => r.ScheduleId == req.ScheduleId && r.TeacherId == schedule.TeacherId).AnyAsync();
        if (existing) throw new BizException("该课次已结算");

        var wallet = await Db.Queryable<TeacherWallet>()
            .Where(w => w.TeacherId == schedule.TeacherId).FirstAsync();
        if (wallet == null)
        {
            wallet = new TeacherWallet
            {
                TenantId = user.TenantId, OrgId = user.OrgId, TeacherId = schedule.TeacherId,
                CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
            };
            wallet.Id = await Db.Insertable(wallet).ExecuteReturnBigIdentityAsync();
        }
        wallet.Balance += fee; wallet.TotalIncome += fee;
        wallet.LastSettlementAt = DateTime.Now; wallet.UpdatedAt = DateTime.Now;
        await Db.Updateable(wallet).ExecuteCommandAsync();

        await Db.Insertable(new FeeSettlementRecord
        {
            TenantId = user.TenantId, OrgId = user.OrgId, CourseId = schedule.CourseId,
            ScheduleId = req.ScheduleId, TeacherId = schedule.TeacherId, WalletId = wallet.Id,
            Amount = fee, SettlementType = course.SettlementType, StudentCount = studentCount,
            SettlementDate = schedule.LessonDate, TriggerType = 1, Status = 1,
            Remark = req.Remark, CreatedAt = DateTime.Now,
        }).ExecuteCommandAsync();
    }

    public async Task<List<SettlementSummaryDto>> GetSummaryAsync(CurrentUser user, int? month = null)
    {
        var query = Db.Queryable<FeeSettlementRecord>()
            .LeftJoin<SysUser>((r, t) => r.TeacherId == t.Id)
            .Where((r, t) => r.TenantId == user.TenantId);
        if (month.HasValue)
        {
            var start = new DateTime(month.Value / 100, month.Value % 100, 1);
            var end = start.AddMonths(1);
            query = query.Where((r, t) => r.SettlementDate >= start && r.SettlementDate < end);
        }
        return await query.GroupBy((r, t) => new { r.TeacherId, TeacherName = t.RealName ?? "" })
            .Select((r, t) => new SettlementSummaryDto
            {
                TeacherId = r.TeacherId, TeacherName = t.RealName ?? "",
                TotalSettlements = SqlFunc.AggregateCount(r.Id),
                TotalAmount = SqlFunc.AggregateSum(r.Amount),
            }).ToListAsync();
    }

    public async Task<List<SettlementExportDto>> GetExportAsync(CurrentUser user)
    {
        return await Db.Queryable<FeeSettlementRecord>()
            .LeftJoin<Course>((r, c) => r.CourseId == c.Id)
            .LeftJoin<SysUser>((r, c, t) => r.TeacherId == t.Id)
            .Where((r, c, t) => r.TenantId == user.TenantId)
            .GroupBy((r, c, t) => new { r.CourseId, CourseName = c.Name, r.TeacherId, TeacherName = t.RealName ?? "" })
            .Select((r, c, t) => new SettlementExportDto
            {
                CourseId = r.CourseId, CourseName = c.Name,
                TeacherId = r.TeacherId, TeacherName = t.RealName ?? "",
                Lessons = SqlFunc.AggregateCount(r.Id),
                TotalAmount = SqlFunc.AggregateSum(r.Amount),
            }).ToListAsync();
    }

    private async Task<PagedResult<TDto>> GetPagedAsync<TDto>(ISugarQueryable<FeeSettlementRecord> query, int page, int pageSize)
        where TDto : class, new()
    {
        RefAsync<int> total = 0;
        var items = await query.ToPageListAsync(page, pageSize, total);
        var dtos = items.Select(e =>
        {
            var dto = new TDto();
            var srcProps = typeof(FeeSettlementRecord).GetProperties();
            var dstProps = typeof(TDto).GetProperties();
            foreach (var sp in srcProps)
            {
                var dp = dstProps.FirstOrDefault(p => p.Name == sp.Name && p.PropertyType == sp.PropertyType);
                if (dp != null && dp.CanWrite) dp.SetValue(dto, sp.GetValue(e));
            }
            return dto;
        }).ToList();
        return new PagedResult<TDto>(dtos, total, page, pageSize);
    }
}
