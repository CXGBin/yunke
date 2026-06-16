using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class EnrollmentService : BaseService
{
    public EnrollmentService(ISqlSugarClient db) : base(db) { }

    public async Task<long> EnrollAsync(CreateEnrollmentRequest req, CurrentUser user)
    {
        var course = await Db.Queryable<Course>()
            .Where(c => c.Id == req.CourseId && c.Status == 1 && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("课程不存在或已下架");

        var enrolledCount = await Db.Queryable<CourseEnrollment>()
            .Where(e => e.CourseId == req.CourseId && e.Status == 1).CountAsync();
        if (enrolledCount >= course.MaxStudents) throw new BizException("课程已满员");

        var existing = await Db.Queryable<CourseEnrollment>()
            .Where(e => e.CourseId == req.CourseId && e.StudentId == user.UserId && e.Status == 1).AnyAsync();
        if (existing) throw new BizException("已选过该课程");

        var studentEnrollCount = await Db.Queryable<CourseEnrollment>()
            .Where(e => e.StudentId == user.UserId && e.Status == 1).CountAsync();
        if (studentEnrollCount >= 20) throw new BizException("最多选20门课程");

        var enroll = new CourseEnrollment
        {
            TenantId = course.TenantId, OrgId = course.OrgId ?? 0, CampusId = course.CampusId,
            CourseId = req.CourseId, StudentId = user.UserId, ParentId = req.ParentId,
            Status = 1, Remark = req.Remark, EnrolledAt = DateTime.Now,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        return await Db.Insertable(enroll).ExecuteReturnBigIdentityAsync();
    }

    public async Task<List<CourseDto>> GetMyCoursesAsync(CurrentUser user)
    {
        var enrollments = await Db.Queryable<CourseEnrollment>()
            .LeftJoin<Course>((e, c) => e.CourseId == c.Id)
            .Where((e, c) => e.StudentId == user.UserId && e.Status == 1)
            .Select((e, c) => new CourseDto
            {
                Id = c.Id, CourseCode = c.CourseCode, Name = c.Name, Description = c.Description,
                CoverImage = c.CoverImage, TotalLessons = c.TotalLessons, LessonDuration = c.LessonDuration,
                OriginalPrice = c.OriginalPrice, DiscountPrice = c.DiscountPrice, Status = c.Status,
                TeacherId = c.TeacherId, CampusId = c.CampusId, OrgId = c.OrgId, CreatedAt = c.CreatedAt,
            }).ToListAsync();
        return enrollments;
    }

    public async Task<List<MyScheduleDto>> GetMyScheduleAsync(CurrentUser user, DateTime? startDate = null, DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.Today;
        var end = endDate ?? start.AddDays(7);

        if (user.Role == 3)
        {
            return await Db.Queryable<CourseSchedule>()
                .LeftJoin<Course>((s, c) => s.CourseId == c.Id)
                .LeftJoin<Campus>((s, c, cam) => s.CampusId == cam.Id)
                .LeftJoin<SysUser>((s, c, cam, t) => s.TeacherId == t.Id)
                .LeftJoin<Attendance>((s, c, cam, t, a) => s.Id == a.ScheduleId && a.StudentId == user.UserId)
                .Where((s, c, cam, t, a) => s.TeacherId == user.UserId && s.LessonDate >= start && s.LessonDate <= end && s.Status != 2 && !s.IsDeleted)
                .Select((s, c, cam, t, a) => new MyScheduleDto
                {
                    ScheduleId = s.Id, CourseId = s.CourseId, CourseName = c.Name,
                    LessonTitle = s.LessonTitle, LessonDate = s.LessonDate,
                    StartTime = s.StartTime, EndTime = s.EndTime,
                    CampusName = cam.Name, TeacherName = t.RealName,
                    Status = s.Status, AttendanceStatus = a.Status,
                }).ToListAsync();
        }

        var myCourseIds = await Db.Queryable<CourseEnrollment>()
            .Where(e => e.StudentId == user.UserId && e.Status == 1)
            .Select(e => e.CourseId).ToListAsync();

        return await Db.Queryable<CourseSchedule>()
            .LeftJoin<Course>((s, c) => s.CourseId == c.Id)
            .LeftJoin<Campus>((s, c, cam) => s.CampusId == cam.Id)
            .LeftJoin<SysUser>((s, c, cam, t) => s.TeacherId == t.Id)
            .LeftJoin<Attendance>((s, c, cam, t, a) => s.Id == a.ScheduleId && a.StudentId == user.UserId)
            .Where((s, c, cam, t, a) => myCourseIds.Contains(s.CourseId) && s.LessonDate >= start && s.LessonDate <= end && s.Status != 2 && !s.IsDeleted)
            .Select((s, c, cam, t, a) => new MyScheduleDto
            {
                ScheduleId = s.Id, CourseId = s.CourseId, CourseName = c.Name,
                LessonTitle = s.LessonTitle, LessonDate = s.LessonDate,
                StartTime = s.StartTime, EndTime = s.EndTime,
                CampusName = cam.Name, TeacherName = t.RealName,
                Status = s.Status, AttendanceStatus = a.Status,
            }).ToListAsync();
    }

    public async Task<PagedResult<EnrollmentDto>> GetCourseStudentsAsync(PageRequest req, long courseId, long tenantId)
    {
        var query = Db.Queryable<CourseEnrollment>()
            .LeftJoin<SysUser>((e, s) => e.StudentId == s.Id)
            .LeftJoin<Course>((e, s, c) => e.CourseId == c.Id)
            .Where((e, s, c) => e.CourseId == courseId && e.TenantId == tenantId && e.Status == 1);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where((e, s, c) => s.RealName!.Contains(req.Keyword!));
        query = query.OrderBy((e, s, c) => e.EnrolledAt, OrderByType.Desc);
        RefAsync<int> total = 0;
        var list = await query.Select((e, s, c) => new EnrollmentDto
        {
            Id = e.Id, CourseId = e.CourseId, CourseName = c.Name,
            StudentId = e.StudentId, StudentName = s.RealName ?? s.NickName ?? "",
            Status = e.Status, EnrolledAt = e.EnrolledAt, CreatedAt = e.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<EnrollmentDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<long> ManualAddAsync(ManualAddEnrollmentRequest req, CurrentUser user)
    {
        var course = await Db.Queryable<Course>()
            .Where(c => c.Id == req.CourseId && c.TenantId == user.TenantId && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("课程不存在");
        var enroll = new CourseEnrollment
        {
            TenantId = user.TenantId, OrgId = user.OrgId, CampusId = course.CampusId,
            CourseId = req.CourseId, StudentId = req.StudentId, Status = 1,
            Remark = req.Remark, EnrolledAt = DateTime.Now,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        return await Db.Insertable(enroll).ExecuteReturnBigIdentityAsync();
    }

    public async Task ManualRemoveAsync(long id, CurrentUser user)
    {
        var enroll = await Db.Queryable<CourseEnrollment>()
            .Where(e => e.Id == id && e.TenantId == user.TenantId).FirstAsync()
            ?? throw new BizException("选课记录不存在");
        enroll.Status = 0; enroll.UpdatedAt = DateTime.Now;
        await Db.Updateable(enroll).UpdateColumns(e => new { e.Status, e.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task<long> JoinWaitlistAsync(long courseId, CurrentUser user)
    {
        var existing = await Db.Queryable<WaitList>()
            .Where(w => w.CourseId == courseId && w.StudentId == user.UserId && w.Status == 1).AnyAsync();
        if (existing) throw new BizException("已在候补列表中");

        var wait = new WaitList
        {
            TenantId = user.TenantId, OrgId = user.OrgId, CourseId = courseId,
            StudentId = user.UserId, Status = 1, JoinedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddHours(24), CreatedAt = DateTime.Now,
        };
        return await Db.Insertable(wait).ExecuteReturnBigIdentityAsync();
    }

    public async Task CancelWaitlistAsync(long id, CurrentUser user)
    {
        var wait = await Db.Queryable<WaitList>()
            .Where(w => w.Id == id && w.StudentId == user.UserId).FirstAsync()
            ?? throw new BizException("候补记录不存在");
        wait.Status = 0; await Db.Updateable(wait).UpdateColumns(w => new { w.Status }).ExecuteCommandAsync();
    }

    public async Task<List<WaitListDto>> GetMyWaitlistAsync(CurrentUser user)
    {
        return await Db.Queryable<WaitList>()
            .LeftJoin<Course>((w, c) => w.CourseId == c.Id)
            .Where((w, c) => w.StudentId == user.UserId && w.Status == 1)
            .Select((w, c) => new WaitListDto
            {
                Id = w.Id, CourseId = w.CourseId, CourseName = c.Name,
                Status = w.Status, JoinedAt = w.JoinedAt, NotifiedAt = w.NotifiedAt, ExpiresAt = w.ExpiresAt,
            }).ToListAsync();
    }
}
