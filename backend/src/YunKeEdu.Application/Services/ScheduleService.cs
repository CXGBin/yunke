using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class ScheduleService : BaseService
{
    public ScheduleService(ISqlSugarClient db) : base(db) { }

    public async Task<long> CreateAsync(CreateScheduleRequest req, CurrentUser user)
    {
        var conflict = await CheckConflictInternalAsync(req.CampusId, req.LessonDate, req.StartTime, req.EndTime, null);
        if (conflict) throw new BizException("该时段存在排课冲突");

        var schedule = new CourseSchedule
        {
            TenantId = user.TenantId, OrgId = user.OrgId, CampusId = req.CampusId,
            CourseId = req.CourseId, TeacherId = req.TeacherId,
            LessonDate = req.LessonDate, StartTime = req.StartTime, EndTime = req.EndTime,
            LessonNo = req.LessonNo, LessonTitle = req.LessonTitle, Remark = req.Remark,
            Status = 0, CreatedBy = user.UserId, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        return await Db.Insertable(schedule).ExecuteReturnBigIdentityAsync();
    }

    public async Task<long> CreateRecurrenceAsync(CreateRecurrenceRequest req, CurrentUser user)
    {
        var recurrence = new ScheduleRecurrence
        {
            TenantId = user.TenantId, OrgId = user.OrgId, CourseId = req.CourseId,
            TeacherId = req.TeacherId, WeekDays = req.WeekDays,
            StartTime = req.StartTime, EndTime = req.EndTime,
            StartDate = req.StartDate, EndDate = req.EndDate,
            TotalLessons = req.TotalLessons, GeneratedLessons = 0, Status = 1,
            CreatedBy = user.UserId, CreatedAt = DateTime.Now,
        };
        recurrence.Id = await Db.Insertable(recurrence).ExecuteReturnBigIdentityAsync();
        return recurrence.Id;
    }

    public async Task UpdateAsync(long id, UpdateScheduleRequest req, CurrentUser user)
    {
        var schedule = await Db.Queryable<CourseSchedule>()
            .Where(s => s.Id == id && s.TenantId == user.TenantId && !s.IsDeleted).FirstAsync()
            ?? throw new BizException("排课记录不存在");
        if (schedule.Status == 1) throw new BizException("已发布的排课不可修改");

        var conflict = await CheckConflictInternalAsync(req.CampusId, req.LessonDate, req.StartTime, req.EndTime, id);
        if (conflict) throw new BizException("该时段存在排课冲突");

        var oldData = System.Text.Json.JsonSerializer.Serialize(new { schedule.CampusId, schedule.LessonDate, schedule.StartTime, schedule.EndTime, schedule.TeacherId });
        schedule.CampusId = req.CampusId; schedule.CourseId = req.CourseId; schedule.TeacherId = req.TeacherId;
        schedule.LessonDate = req.LessonDate; schedule.StartTime = req.StartTime; schedule.EndTime = req.EndTime;
        schedule.LessonNo = req.LessonNo; schedule.LessonTitle = req.LessonTitle; schedule.Remark = req.Remark;
        schedule.IsRescheduled = true; schedule.UpdatedAt = DateTime.Now;
        await Db.Updateable(schedule).ExecuteCommandAsync();

        var newData = System.Text.Json.JsonSerializer.Serialize(new { schedule.CampusId, schedule.LessonDate, schedule.StartTime, schedule.EndTime, schedule.TeacherId });
        await Db.Insertable(new ScheduleChangeLog
        {
            TenantId = user.TenantId, OrgId = user.OrgId, ScheduleId = id,
            ChangeType = 1, OldData = oldData, NewData = newData,
            OperatorId = user.UserId, CreatedAt = DateTime.Now,
        }).ExecuteCommandAsync();
    }

    public async Task CancelAsync(long id, CancelScheduleRequest req, CurrentUser user)
    {
        var schedule = await Db.Queryable<CourseSchedule>()
            .Where(s => s.Id == id && s.TenantId == user.TenantId && !s.IsDeleted).FirstAsync()
            ?? throw new BizException("排课记录不存在");
        schedule.Status = 2; schedule.CancelReason = req.CancelReason;
        schedule.UpdatedAt = DateTime.Now;
        await Db.Updateable(schedule).UpdateColumns(s => new { s.Status, s.CancelReason, s.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task PublishAsync(long id, CurrentUser user)
    {
        var schedule = await Db.Queryable<CourseSchedule>()
            .Where(s => s.Id == id && s.TenantId == user.TenantId && !s.IsDeleted).FirstAsync()
            ?? throw new BizException("排课记录不存在");
        schedule.Status = 1; schedule.PublishedAt = DateTime.Now; schedule.UpdatedAt = DateTime.Now;
        await Db.Updateable(schedule).UpdateColumns(s => new { s.Status, s.PublishedAt, s.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task<PagedResult<ScheduleDto>> GetPageAsync(PageRequest req, CurrentUser user)
    {
        var query = Db.Queryable<CourseSchedule>()
            .LeftJoin<Course>((s, c) => s.CourseId == c.Id)
            .LeftJoin<Campus>((s, c, cam) => s.CampusId == cam.Id)
            .LeftJoin<SysUser>((s, c, cam, t) => s.TeacherId == t.Id)
            .Where((s, c, cam, t) => s.IsDeleted == false);
        if (user.Role == 3) query = query.Where((s, c, cam, t) => s.TeacherId == user.UserId);
        else if (user.Role != 1) query = query.Where((s, c, cam, t) => s.TenantId == user.TenantId);
        query = query.OrderBy((s, c, cam, t) => s.LessonDate).OrderBy((s, c, cam, t) => s.StartTime);
        RefAsync<int> total = 0;
        var list = await query.Select((s, c, cam, t) => new ScheduleDto
        {
            Id = s.Id, CourseId = s.CourseId, CourseName = c.Name,
            CampusId = s.CampusId, CampusName = cam.Name,
            TeacherId = s.TeacherId, TeacherName = t.RealName,
            LessonDate = s.LessonDate, StartTime = s.StartTime, EndTime = s.EndTime,
            LessonNo = s.LessonNo, LessonTitle = s.LessonTitle, Remark = s.Remark,
            Status = s.Status, CancelReason = s.CancelReason, IsRescheduled = s.IsRescheduled,
            PublishedAt = s.PublishedAt, CreatedAt = s.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<ScheduleDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<List<CalendarEventDto>> GetCalendarAsync(DateTime startDate, DateTime endDate, CurrentUser user)
    {
        var query = Db.Queryable<CourseSchedule>()
            .LeftJoin<Course>((s, c) => s.CourseId == c.Id)
            .LeftJoin<Campus>((s, c, cam) => s.CampusId == cam.Id)
            .LeftJoin<SysUser>((s, c, cam, t) => s.TeacherId == t.Id)
            .Where((s, c, cam, t) => s.IsDeleted == false && s.LessonDate >= startDate && s.LessonDate <= endDate && s.Status != 2);
        if (user.Role == 3) query = query.Where((s, c, cam, t) => s.TeacherId == user.UserId);
        else if (user.Role != 1) query = query.Where((s, c, cam, t) => s.TenantId == user.TenantId);

        var schedules = await query.Select((s, c, cam, t) => new
        {
            ScheduleId = s.Id, CourseName = c.Name, CampusName = cam.Name, TeacherName = t.RealName,
            LessonDate = s.LessonDate, StartTime = s.StartTime, EndTime = s.EndTime, Status = s.Status,
        }).ToListAsync();

        return schedules.GroupBy(s => s.LessonDate.Date).Select(g => new CalendarEventDto
        {
            Date = g.Key,
            Events = g.Select(s => new CalendarItemDto
            {
                ScheduleId = s.ScheduleId, CourseName = s.CourseName, CampusName = s.CampusName,
                TeacherName = s.TeacherName, StartTime = s.StartTime, EndTime = s.EndTime, Status = s.Status,
            }).ToList(),
        }).OrderBy(e => e.Date).ToList();
    }

    public async Task<ConflictCheckResult> CheckConflictAsync(ConflictCheckRequest req, CurrentUser user)
    {
        var hasConflict = await CheckConflictInternalAsync(req.CampusId, req.LessonDate, req.StartTime, req.EndTime, req.ExcludeScheduleId);
        var conflicts = new List<ScheduleDto>();
        if (hasConflict)
        {
            conflicts = await Db.Queryable<CourseSchedule>()
                .LeftJoin<Course>((s, c) => s.CourseId == c.Id)
                .LeftJoin<Campus>((s, c, cam) => s.CampusId == cam.Id)
                .LeftJoin<SysUser>((s, c, cam, t) => s.TeacherId == t.Id)
                .Where((s, c, cam, t) => s.CampusId == req.CampusId && s.LessonDate == req.LessonDate && s.Status != 2 && !s.IsDeleted
                    && s.StartTime < req.EndTime && s.EndTime > req.StartTime
                    && (req.ExcludeScheduleId == null || s.Id != req.ExcludeScheduleId))
                .Select((s, c, cam, t) => new ScheduleDto
                {
                    Id = s.Id, CourseName = c.Name, CampusName = cam.Name, TeacherName = t.RealName,
                    LessonDate = s.LessonDate, StartTime = s.StartTime, EndTime = s.EndTime, Status = s.Status,
                }).ToListAsync();
        }
        return new ConflictCheckResult { HasConflict = hasConflict, Conflicts = conflicts };
    }

    public async Task<List<ScheduleChangeLogDto>> GetChangeLogsAsync(long scheduleId, long tenantId)
    {
        return await Db.Queryable<ScheduleChangeLog>()
            .LeftJoin<SysUser>((l, u) => l.OperatorId == u.Id)
            .Where((l, u) => l.ScheduleId == scheduleId && l.TenantId == tenantId)
            .OrderBy((l, u) => l.CreatedAt, OrderByType.Desc)
            .Select((l, u) => new ScheduleChangeLogDto
            {
                Id = l.Id, ScheduleId = l.ScheduleId, ChangeType = l.ChangeType,
                OldData = l.OldData, NewData = l.NewData, Reason = l.Reason,
                OperatorId = l.OperatorId, OperatorName = u.RealName, CreatedAt = l.CreatedAt,
            }).ToListAsync();
    }

    private async Task<bool> CheckConflictInternalAsync(long campusId, DateTime date, TimeSpan start, TimeSpan end, long? excludeId)
    {
        return await Db.Queryable<CourseSchedule>()
            .Where(s => s.CampusId == campusId && s.LessonDate == date && s.Status != 2 && !s.IsDeleted
                && s.StartTime < end && s.EndTime > start
                && (excludeId == null || s.Id != excludeId))
            .AnyAsync();
    }
}
