using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class AttendanceService : BaseService
{
    public AttendanceService(ISqlSugarClient db) : base(db) { }

    public async Task SignInAsync(SignInRequest req, CurrentUser user)
    {
        var schedule = await Db.Queryable<CourseSchedule>()
            .Where(s => s.Id == req.ScheduleId && !s.IsDeleted).FirstAsync()
            ?? throw new BizException("排课记录不存在");

        var existing = await Db.Queryable<Attendance>()
            .Where(a => a.ScheduleId == req.ScheduleId && a.StudentId == req.StudentId).FirstAsync();
        if (existing != null)
        {
            existing.Status = req.Status; existing.SignInTime = DateTime.Now;
            existing.SignMethod = req.SignMethod; existing.Remark = req.Remark;
            existing.OperatorId = user.UserId; existing.UpdatedAt = DateTime.Now;
            await Db.Updateable(existing).ExecuteCommandAsync();
        }
        else
        {
            await Db.Insertable(new Attendance
            {
                TenantId = schedule.TenantId, OrgId = schedule.OrgId, CampusId = schedule.CampusId,
                ScheduleId = req.ScheduleId, CourseId = schedule.CourseId, StudentId = req.StudentId,
                Status = req.Status, SignInTime = DateTime.Now, SignMethod = req.SignMethod,
                Remark = req.Remark, OperatorId = user.UserId,
                CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
            }).ExecuteCommandAsync();
        }
    }

    public async Task SignAllAsync(SignAllRequest req, CurrentUser user)
    {
        var schedule = await Db.Queryable<CourseSchedule>()
            .Where(s => s.Id == req.ScheduleId && s.TeacherId == user.UserId && !s.IsDeleted).FirstAsync()
            ?? throw new BizException("排课记录不存在");

        var enrolledStudents = await Db.Queryable<CourseEnrollment>()
            .Where(e => e.CourseId == schedule.CourseId && e.Status == 1)
            .Select(e => e.StudentId).ToListAsync();

        var existing = await Db.Queryable<Attendance>()
            .Where(a => a.ScheduleId == req.ScheduleId).Select(a => a.StudentId).ToListAsync();

        var newAttendances = enrolledStudents.Where(s => !existing.Contains(s)).Select(studentId => new Attendance
        {
            TenantId = schedule.TenantId, OrgId = schedule.OrgId, CampusId = schedule.CampusId,
            ScheduleId = req.ScheduleId, CourseId = schedule.CourseId, StudentId = studentId,
            Status = req.Status, SignInTime = DateTime.Now, OperatorId = user.UserId,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        }).ToList();

        if (newAttendances.Any())
            await Db.Insertable(newAttendances).ExecuteCommandAsync();

        if (existing.Any())
        {
            await Db.Updateable<Attendance>()
                .SetColumns(a => new Attendance { Status = req.Status, SignInTime = DateTime.Now, UpdatedAt = DateTime.Now })
                .Where(a => a.ScheduleId == req.ScheduleId).ExecuteCommandAsync();
        }
    }

    public async Task<List<AttendanceDto>> GetByScheduleAsync(long scheduleId, long tenantId)
    {
        return await Db.Queryable<Attendance>()
            .LeftJoin<SysUser>((a, s) => a.StudentId == s.Id)
            .LeftJoin<Course>((a, s, c) => a.CourseId == c.Id)
            .Where((a, s, c) => a.ScheduleId == scheduleId && a.TenantId == tenantId)
            .OrderBy((a, s, c) => a.StudentId)
            .Select((a, s, c) => new AttendanceDto
            {
                Id = a.Id, ScheduleId = a.ScheduleId, CourseId = a.CourseId, CourseName = c.Name,
                StudentId = a.StudentId, StudentName = s.RealName ?? s.NickName ?? "",
                Status = a.Status, SignInTime = a.SignInTime, SignMethod = a.SignMethod,
                Remark = a.Remark, CreatedAt = a.CreatedAt,
            }).ToListAsync();
    }

    public async Task<List<AttendanceDto>> GetMyRecordsAsync(CurrentUser user, int? limit = null)
    {
        var query = Db.Queryable<Attendance>()
            .LeftJoin<Course>((a, c) => a.CourseId == c.Id)
            .Where((a, c) => a.StudentId == user.UserId)
            .OrderByDescending((a, c) => a.CreatedAt);
        if (limit.HasValue) query = query.Take(limit.Value);
        return await query.Select((a, c) => new AttendanceDto
        {
            Id = a.Id, ScheduleId = a.ScheduleId, CourseId = a.CourseId, CourseName = c.Name,
            StudentId = a.StudentId, Status = a.Status, SignInTime = a.SignInTime,
            CreatedAt = a.CreatedAt,
        }).ToListAsync();
    }

    public async Task<object> GetStudentStatisticsAsync(CurrentUser user)
    {
        var total = await Db.Queryable<Attendance>()
            .Where(a => a.StudentId == user.UserId).CountAsync();
        var present = await Db.Queryable<Attendance>()
            .Where(a => a.StudentId == user.UserId && a.Status == 1).CountAsync();
        var late = await Db.Queryable<Attendance>()
            .Where(a => a.StudentId == user.UserId && a.Status == 2).CountAsync();
        var rate = total > 0 ? Math.Round((decimal)(present + late) * 100 / total, 2) : 0;
        return new { Total = total, Present = present, Late = late, Absent = total - present - late, AttendanceRate = rate };
    }

    public async Task<object> GetCourseStatisticsAsync(long courseId, long tenantId)
    {
        var schedules = await Db.Queryable<CourseSchedule>()
            .Where(s => s.CourseId == courseId && s.TenantId == tenantId && !s.IsDeleted && s.Status != 2)
            .Select(s => s.Id).ToListAsync();

        var totalAttendance = await Db.Queryable<Attendance>()
            .Where(a => schedules.Contains(a.ScheduleId)).CountAsync();
        var present = await Db.Queryable<Attendance>()
            .Where(a => schedules.Contains(a.ScheduleId) && (a.Status == 1 || a.Status == 2)).CountAsync();
        var rate = totalAttendance > 0 ? Math.Round((decimal)present * 100 / totalAttendance, 2) : 0;
        return new { TotalSchedules = schedules.Count, TotalAttendance = totalAttendance, PresentCount = present, AttendanceRate = rate };
    }

    public async Task<PagedResult<object>> GetPagedListAsync(PageRequest req, long tenantId)
    {
        var query = Db.Queryable<Attendance>().Where(a => a.TenantId == tenantId);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.CreatedAt)
            .ToPageListAsync(req.Page, req.PageSize);
        var dtos = items.Select(a => (object)new { a.Id, a.ScheduleId, a.StudentId, a.Status, a.SignMethod, a.Remark, a.CreatedAt }).ToList();
        return new PagedResult<object>(dtos, total, req.Page, req.PageSize);
    }

}