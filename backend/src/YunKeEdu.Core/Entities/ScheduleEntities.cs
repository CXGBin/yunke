using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>课节定义表（简化版）</summary>
[SugarTable("LessonUnit")]
public class LessonUnit : BaseEntity
{
    public long OrgId { get; set; }
    public long CourseId { get; set; }
    public int LessonNo { get; set; }
    [SugarColumn(Length = 200, IsNullable = false)]
    public string Title { get; set; } = string.Empty;
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
}

/// <summary>排课计划表</summary>
[SugarTable("CourseSchedule")]
public class CourseSchedule : BaseEntity
{
    public long OrgId { get; set; }
    public long CampusId { get; set; }
    public long CourseId { get; set; }
    public long TeacherId { get; set; }
    public DateTime LessonDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int? LessonNo { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? LessonTitle { get; set; }
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
    public int Status { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? CancelReason { get; set; }
    public bool IsRescheduled { get; set; }
    public long? OriginalScheduleId { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
}

/// <summary>循环排课规则表</summary>
[SugarTable("ScheduleRecurrence")]
public class ScheduleRecurrence
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long CourseId { get; set; }
    public long TeacherId { get; set; }
    [SugarColumn(Length = 20, IsNullable = false)]
    public string WeekDays { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalLessons { get; set; }
    public int GeneratedLessons { get; set; }
    public int Status { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>排课变更记录表</summary>
[SugarTable("ScheduleChangeLog")]
public class ScheduleChangeLog
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long ScheduleId { get; set; }
    public int ChangeType { get; set; }
    [SugarColumn(Length = -1, IsNullable = true, ColumnDataType = "nvarchar(max)")]
    public string? OldData { get; set; }
    [SugarColumn(Length = -1, IsNullable = true, ColumnDataType = "nvarchar(max)")]
    public string? NewData { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Reason { get; set; }
    public long OperatorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
