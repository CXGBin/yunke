using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class StatisticsService : BaseService
{
    public StatisticsService(ISqlSugarClient db) : base(db) { }

    public async Task<OrgDashboardDto> GetOrgDashboardAsync(CurrentUser user)
    {
        var tenantId = user.TenantId;
        var students = await Db.Queryable<UserOrgBinding>().Where(b => b.TenantId == tenantId && b.Role == 4 && b.Status == 1).CountAsync();
        var teachers = await Db.Queryable<UserOrgBinding>().Where(b => b.TenantId == tenantId && b.Role == 3 && b.Status == 1).CountAsync();
        var courses = await Db.Queryable<Course>().Where(c => c.TenantId == tenantId && !c.IsDeleted).CountAsync();
        var activeCourses = await Db.Queryable<Course>().Where(c => c.TenantId == tenantId && c.Status == 1 && !c.IsDeleted).CountAsync();
        var today = DateTime.Today;
        var todayEnrollments = await Db.Queryable<CourseEnrollment>().Where(e => e.TenantId == tenantId && e.EnrolledAt >= today).CountAsync();
        var pendingLeaves = await Db.Queryable<LeaveRequest>().Where(l => l.TenantId == tenantId && l.Status == 0).CountAsync();
        var todaySchedules = await Db.Queryable<CourseSchedule>().Where(s => s.TenantId == tenantId && s.LessonDate == today && !s.IsDeleted).CountAsync();

        return new OrgDashboardDto
        {
            TotalStudents = students, TotalTeachers = teachers, TotalCourses = courses,
            ActiveCourses = activeCourses, TodayEnrollments = todayEnrollments,
            PendingLeaves = pendingLeaves, TodaySchedules = todaySchedules,
        };
    }

    public async Task<PlatformDashboardDto> GetPlatformDashboardAsync()
    {
        var orgs = await Db.Queryable<Organization>().Where(o => !o.IsDeleted).CountAsync();
        var activeOrgs = await Db.Queryable<Organization>().Where(o => !o.IsDeleted && o.Status == 1).CountAsync();
        var students = await Db.Queryable<UserOrgBinding>().Where(b => b.Role == 4 && b.Status == 1).CountAsync();
        var teachers = await Db.Queryable<UserOrgBinding>().Where(b => b.Role == 3 && b.Status == 1).CountAsync();
        var courses = await Db.Queryable<Course>().Where(c => !c.IsDeleted).CountAsync();

        return new PlatformDashboardDto
        {
            TotalOrgs = orgs, ActiveOrgs = activeOrgs, TotalStudents = students,
            TotalTeachers = teachers, TotalCourses = courses,
        };
    }

    public async Task<AttendanceAnalysisDto> GetAttendanceAnalysisAsync(CurrentUser user, DateTime? startDate = null, DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.Today.AddDays(-30);
        var end = endDate ?? DateTime.Today;
        var schedules = await Db.Queryable<CourseSchedule>()
            .Where(s => s.TenantId == user.TenantId && s.LessonDate >= start && s.LessonDate <= end && !s.IsDeleted && s.Status != 2)
            .Select(s => s.Id).ToListAsync();
        var total = await Db.Queryable<Attendance>().Where(a => schedules.Contains(a.ScheduleId)).CountAsync();
        var present = await Db.Queryable<Attendance>().Where(a => schedules.Contains(a.ScheduleId) && (a.Status == 1 || a.Status == 2)).CountAsync();
        return new AttendanceAnalysisDto
        {
            OverallRate = total > 0 ? Math.Round((decimal)present * 100 / total, 2) : 0,
        };
    }

    public async Task<EnrollmentAnalysisDto> GetEnrollmentAnalysisAsync(CurrentUser user)
    {
        var total = await Db.Queryable<CourseEnrollment>().Where(e => e.TenantId == user.TenantId && e.Status == 1).CountAsync();
        var courses = await Db.Queryable<Course>().Where(c => c.TenantId == user.TenantId && c.Status == 1 && !c.IsDeleted).ToListAsync();
        var byCourse = courses.Select(c => new CourseEnrollmentDto
        {
            CourseId = c.Id, CourseName = c.Name, MaxStudents = c.MaxStudents,
            Enrolled = 0, FillRate = 0,
        }).ToList();
        foreach (var item in byCourse)
        {
            item.Enrolled = await Db.Queryable<CourseEnrollment>()
                .Where(e => e.CourseId == item.CourseId && e.Status == 1).CountAsync();
            item.FillRate = item.MaxStudents > 0 ? Math.Round((decimal)item.Enrolled * 100 / item.MaxStudents, 2) : 0;
        }
        return new EnrollmentAnalysisDto { TotalEnrollments = total, ActiveEnrollments = total, ByCourse = byCourse };
    }

    public async Task<SatisfactionAnalysisDto> GetSatisfactionAnalysisAsync(CurrentUser user)
    {
        var evals = await Db.Queryable<CourseEvaluation>()
            .Where(e => e.TenantId == user.TenantId && e.IsDeleted == false && e.Status == 1).ToListAsync();
        var avg = evals.Where(e => e.CourseRating.HasValue).Any()
            ? Math.Round(evals.Where(e => e.CourseRating.HasValue).Average(e => (decimal)e.CourseRating!.Value), 2) : 0;
        return new SatisfactionAnalysisDto
        {
            OverallRating = avg, TotalEvaluations = evals.Count,
            RatingDistribution = Enumerable.Range(1, 5).ToDictionary(i => i, i => evals.Count(e => e.CourseRating == i)),
        };
    }

    public async Task<MyReportDto> GetMyReportAsync(CurrentUser user)
    {
        var enrollments = await Db.Queryable<CourseEnrollment>()
            .Where(e => e.StudentId == user.UserId && e.Status == 1).ToListAsync();
        var courses = new List<MyCourseReportDto>();
        foreach (var e in enrollments)
        {
            var lessons = await Db.Queryable<LessonUnit>()
                .Where(l => l.CourseId == e.CourseId && !l.IsDeleted).CountAsync();
            var attended = await Db.Queryable<CourseSchedule>()
                .LeftJoin<Attendance>((s, a) => s.Id == a.ScheduleId && a.StudentId == user.UserId)
                .Where((s, a) => s.CourseId == e.CourseId && !s.IsDeleted && (a.Status == 1 || a.Status == 2))
                .CountAsync();
            var course = await Db.Queryable<Course>().InSingleAsync(e.CourseId);
            courses.Add(new MyCourseReportDto
            {
                CourseId = e.CourseId, CourseName = course?.Name ?? "",
                CompletedLessons = attended, TotalLessons = lessons,
                AttendanceRate = lessons > 0 ? Math.Round((decimal)attended * 100 / lessons, 2) : 0,
            });
        }
        var totalLessons = courses.Sum(c => c.TotalLessons);
        var completedLessons = courses.Sum(c => c.CompletedLessons);
        return new MyReportDto
        {
            EnrolledCourses = enrollments.Count, CompletedLessons = completedLessons, TotalLessons = totalLessons,
            AttendanceRate = totalLessons > 0 ? Math.Round((decimal)completedLessons * 100 / totalLessons, 2) : 0,
            Courses = courses,
        };
    }

    public async Task<TeacherReportDto> GetTeacherReportAsync(CurrentUser user)
    {
        var lessons = await Db.Queryable<CourseSchedule>()
            .Where(s => s.TeacherId == user.UserId && !s.IsDeleted && s.Status != 2).CountAsync();
        var completed = await Db.Queryable<CourseSchedule>()
            .Where(s => s.TeacherId == user.UserId && s.LessonDate < DateTime.Now && !s.IsDeleted && s.Status != 2).CountAsync();
        var totalStudents = await Db.Queryable<CourseEnrollment>()
            .LeftJoin<CourseSchedule>((e, s) => e.CourseId == s.CourseId)
            .Where((e, s) => s.TeacherId == user.UserId && e.Status == 1)
            .Select((e, s) => e.StudentId).Distinct().CountAsync();
        var wallet = await Db.Queryable<TeacherWallet>().Where(w => w.TeacherId == user.UserId).FirstAsync();
        var sysUser = await Db.Queryable<SysUser>().InSingleAsync(user.UserId);
        return new TeacherReportDto
        {
            TeacherId = user.UserId, TeacherName = sysUser?.RealName ?? "",
            TotalLessons = lessons, CompletedLessons = completed, TotalStudents = totalStudents,
            TotalIncome = wallet?.TotalIncome ?? 0, MonthlyIncome = wallet?.TotalIncome ?? 0,
        };
    }

    public async Task<RevenueDto> GetRevenueAsync(CurrentUser user, int? year = null)
    {
        var y = year ?? DateTime.Now.Year;
        var subs = await Db.Queryable<OrgSubscription>()
            .Where(s => s.PayStatus == 1 && s.CreatedAt.Year == y && (user.Role == 1 || s.TenantId == user.TenantId))
            .ToListAsync();
        return new RevenueDto
        {
            TotalRevenue = subs.Sum(s => s.Amount),
            MonthlyTrend = subs.GroupBy(s => s.CreatedAt.ToString("yyyy-MM")).Select(g => new MonthlyRevenueDto
            {
                Month = g.Key, Revenue = g.Sum(s => s.Amount),
            }).OrderBy(m => m.Month).ToList(),
        };
    }

    public async Task<RevenueDto> GetRevenueExpenseAsync(CurrentUser user, int? year = null)
    {
        var y = year ?? DateTime.Now.Year;
        var records = await Db.Queryable<FeeSettlementRecord>()
            .Where(r => r.CreatedAt.Year == y && (user.Role == 1 || r.TenantId == user.TenantId)).ToListAsync();
        return new RevenueDto
        {
            TotalRevenue = records.Sum(r => -r.Amount),
            MonthlyTrend = records.GroupBy(r => r.SettlementDate.ToString("yyyy-MM")).Select(g => new MonthlyRevenueDto
            {
                Month = g.Key, Expense = g.Sum(r => r.Amount),
            }).OrderBy(m => m.Month).ToList(),
        };
    }

    public async Task<RevenueSummaryDto> GetRevenueSummaryAsync(CurrentUser user, int? year = null)
    {
        var y = year ?? DateTime.Now.Year;
        var income = await GetRevenueAsync(user, y);
        var expense = await GetRevenueExpenseAsync(user, y);
        var months = Enumerable.Range(1, 12).Select(m => new MonthlyRevenueDto
        {
            Month = $"{y}-{m:D2}",
        }).ToList();
        foreach (var month in months)
        {
            var inc = income.MonthlyTrend.FirstOrDefault(t => t.Month == month.Month);
            var exp = expense.MonthlyTrend.FirstOrDefault(t => t.Month == month.Month);
            month.Revenue = inc?.Revenue ?? 0;
            month.Expense = exp?.Expense ?? 0;
            month.NetIncome = month.Revenue - month.Expense;
        }
        return new RevenueSummaryDto
        {
            TotalIncome = income.TotalRevenue, TotalExpense = Math.Abs(expense.TotalRevenue),
            NetIncome = income.TotalRevenue - Math.Abs(expense.TotalRevenue), Trend = months,
        };
    }

    public async Task<OrgReportDto> GetOrgReportAsync(CurrentUser user)
    {
        var tenantId = user.TenantId;
        var students = await Db.Queryable<UserOrgBinding>().Where(b => b.TenantId == tenantId && b.Role == 4 && b.Status == 1).CountAsync();
        var newStudents = await Db.Queryable<UserOrgBinding>()
            .Where(b => b.TenantId == tenantId && b.Role == 4 && b.Status == 1 && b.BoundAt >= DateTime.Now.AddMonths(-1)).CountAsync();
        var courses = await Db.Queryable<Course>().Where(c => c.TenantId == tenantId && !c.IsDeleted).CountAsync();
        var enrollments = await Db.Queryable<CourseEnrollment>().Where(e => e.TenantId == tenantId && e.Status == 1).CountAsync();
        var teachers = await Db.Queryable<UserOrgBinding>().Where(b => b.TenantId == tenantId && b.Role == 3 && b.Status == 1).CountAsync();
        var walletTotal = await Db.Queryable<TeacherWallet>().Where(w => w.TenantId == tenantId).SumAsync(w => w.TotalIncome);

        return new OrgReportDto
        {
            TotalStudents = students, NewStudentsThisMonth = newStudents,
            TotalCourses = courses, TotalEnrollments = enrollments,
            ActiveTeachers = teachers, TeacherTotalIncome = walletTotal,
        };
    }

    public async Task<LessonConsumptionStudentDto> GetLessonConsumptionStudentAsync(long studentId)
    {
        var sysUser = await Db.Queryable<SysUser>().InSingleAsync(studentId);
        var enrollments = await Db.Queryable<CourseEnrollment>()
            .Where(e => e.StudentId == studentId && e.Status == 1).ToListAsync();
        var courses = new List<LessonConsumptionCourseDto>();
        foreach (var e in enrollments)
        {
            var course = await Db.Queryable<Course>().InSingleAsync(e.CourseId);
            var total = await Db.Queryable<LessonUnit>().Where(l => l.CourseId == e.CourseId && !l.IsDeleted).CountAsync();
            var consumed = await Db.Queryable<CourseSchedule>()
                .LeftJoin<Attendance>((s, a) => s.Id == a.ScheduleId && a.StudentId == studentId)
                .Where((s, a) => s.CourseId == e.CourseId && !s.IsDeleted && (a.Status == 1 || a.Status == 2))
                .CountAsync();
            courses.Add(new LessonConsumptionCourseDto
            {
                CourseId = e.CourseId, CourseName = course?.Name ?? "",
                TotalLessons = total, ConsumedLessons = consumed,
                RemainingLessons = Math.Max(0, total - consumed),
                Progress = total > 0 ? Math.Round((decimal)consumed * 100 / total, 2) : 0,
            });
        }
        return new LessonConsumptionStudentDto
        {
            StudentId = studentId, StudentName = sysUser?.RealName ?? "", Courses = courses,
        };
    }

    public async Task<List<LessonConsumptionCourseDto>> GetLessonConsumptionCourseAsync(long courseId, long tenantId)
    {
        var course = await Db.Queryable<Course>().InSingleAsync(courseId);
        var total = await Db.Queryable<LessonUnit>().Where(l => l.CourseId == courseId && !l.IsDeleted).CountAsync();
        var enrolledStudents = await Db.Queryable<CourseEnrollment>()
            .Where(e => e.CourseId == courseId && e.Status == 1).Select(e => e.StudentId).ToListAsync();

        var result = new List<LessonConsumptionCourseDto>();
        foreach (var sid in enrolledStudents.Take(20))
        {
            var consumed = await Db.Queryable<CourseSchedule>()
                .LeftJoin<Attendance>((s, a) => s.Id == a.ScheduleId && a.StudentId == sid)
                .Where((s, a) => s.CourseId == courseId && !s.IsDeleted && (a.Status == 1 || a.Status == 2))
                .CountAsync();
            var su = await Db.Queryable<SysUser>().InSingleAsync(sid);
            result.Add(new LessonConsumptionCourseDto
            {
                CourseId = courseId, CourseName = course?.Name ?? su?.RealName ?? $"学员{sid}",
                TotalLessons = total, ConsumedLessons = consumed,
                RemainingLessons = Math.Max(0, total - consumed),
                Progress = total > 0 ? Math.Round((decimal)consumed * 100 / total, 2) : 0,
            });
        }
        return result;
    }

    public async Task<LessonConsumptionOrgDto> GetLessonConsumptionOrgAsync(CurrentUser user)
    {
        var courses = await Db.Queryable<Course>().Where(c => c.TenantId == user.TenantId && !c.IsDeleted).ToListAsync();
        var topCourses = new List<LessonConsumptionCourseDto>();
        foreach (var c in courses.OrderByDescending(c => c.TotalLessons).Take(10))
        {
            var total = c.TotalLessons;
            var consumed = await Db.Queryable<CourseSchedule>()
                .Where(s => s.CourseId == c.Id && !s.IsDeleted && s.Status != 2).CountAsync();
            topCourses.Add(new LessonConsumptionCourseDto
            {
                CourseId = c.Id, CourseName = c.Name, TotalLessons = total,
                ConsumedLessons = consumed, RemainingLessons = Math.Max(0, total - consumed),
                Progress = total > 0 ? Math.Round((decimal)consumed * 100 / total, 2) : 0,
            });
        }
        var tTotal = topCourses.Sum(c => c.TotalLessons);
        var tConsumed = topCourses.Sum(c => c.ConsumedLessons);
        return new LessonConsumptionOrgDto
        {
            TotalCourses = courses.Count, TotalLessons = tTotal, ConsumedLessons = tConsumed,
            RemainingLessons = Math.Max(0, tTotal - tConsumed),
            OverallProgress = tTotal > 0 ? Math.Round((decimal)tConsumed * 100 / tTotal, 2) : 0,
            TopCourses = topCourses,
        };
    }

    public async Task<OrgOverviewDto> GetPlatformOverviewAsync()
    {
        var orgCount = await Db.Queryable<Organization>().Where(o => !o.IsDeleted).CountAsync();
        var studentCount = await Db.Queryable<SysUser>().Where(u => !u.IsDeleted && u.Role == 4).CountAsync();
        var teacherCount = await Db.Queryable<SysUser>().Where(u => !u.IsDeleted && u.Role == 2).CountAsync();
        var courseCount = await Db.Queryable<Course>().Where(c => !c.IsDeleted).CountAsync();
        return new OrgOverviewDto
        {
            TotalOrgs = orgCount, TotalStudents = studentCount,
            TotalTeachers = teacherCount, TotalCourses = courseCount,
            TotalRevenue = 0,
        };
    }
}
