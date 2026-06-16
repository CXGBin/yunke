using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>每日统计快照表</summary>
[SugarTable("StatisticsDailySnapshot")]
public class StatisticsDailySnapshot
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public DateTime StatDate { get; set; }
    public int NewStudents { get; set; }
    public int ActiveCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public decimal TotalAttendanceRate { get; set; }
    public int TotalEvaluations { get; set; }
    public decimal AvgRating { get; set; }
    public decimal DailyRevenue { get; set; }
    public decimal TeacherFeeExpense { get; set; }
    public decimal NetIncome { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>课程统计快照表</summary>
[SugarTable("StatisticsCourseSnapshot")]
public class StatisticsCourseSnapshot
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long CourseId { get; set; }
    public DateTime StatMonth { get; set; }
    public int EnrollmentCount { get; set; }
    public decimal AttendanceRate { get; set; }
    public decimal AvgRating { get; set; }
    public int EvaluationCount { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public decimal CourseRevenue { get; set; }
    public decimal CourseExpense { get; set; }
    public int ConsumedLessons { get; set; }
    public int RemainingLessons { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
