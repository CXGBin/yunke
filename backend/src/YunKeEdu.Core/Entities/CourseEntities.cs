using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>课程表</summary>
[SugarTable("Course")]
public class Course : BaseEntity
{
    public long? OrgId { get; set; }
    public long CampusId { get; set; }
    [SugarColumn(Length = 32, IsNullable = true)]
    public string? CourseCode { get; set; }
    [SugarColumn(Length = 200, IsNullable = false)]
    public string Name { get; set; } = string.Empty;
    public long? CategoryId { get; set; }
    [SugarColumn(Length = -1, IsNullable = true, ColumnDataType = "nvarchar(max)")]
    public string? Description { get; set; }
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? CoverImage { get; set; }
    public int TotalLessons { get; set; } = 1;
    public int LessonDuration { get; set; } = 45;
    public int Difficulty { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountPrice { get; set; }
    public int MaxStudents { get; set; } = 30;
    public int MinStudents { get; set; } = 1;
    public DateTime? EnrollmentDeadline { get; set; }
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Tags { get; set; }
    public int Status { get; set; }
    public long TeacherId { get; set; }
    public int SettlementType { get; set; }
    public decimal FixedFeePerLesson { get; set; }
    public decimal StudentCountCommission { get; set; }
    public int SortOrder { get; set; }
    public bool IsRecommend { get; set; }
    public DateTime? ScheduledPublishTime { get; set; }
    public DateTime? ScheduledOfflineTime { get; set; }
    public int ViewCount { get; set; }
    public long CreatedBy { get; set; }
}

/// <summary>课程分类表</summary>
[SugarTable("CourseCategory")]
public class CourseCategory : BaseEntity
{
    public long OrgId { get; set; }
    public long ParentId { get; set; }
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Name { get; set; } = string.Empty;
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>课程附件表</summary>
[SugarTable("CourseAttachment")]
public class CourseAttachment
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long CourseId { get; set; }
    [SugarColumn(Length = 200, IsNullable = false)]
    public string FileName { get; set; } = string.Empty;
    [SugarColumn(Length = 500, IsNullable = false)]
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? FileType { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>课程套餐表</summary>
[SugarTable("CoursePackage")]
public class CoursePackage : BaseEntity
{
    public long OrgId { get; set; }
    public long CampusId { get; set; }
    [SugarColumn(Length = 200, IsNullable = false)]
    public string PackageName { get; set; } = string.Empty;
    [SugarColumn(Length = -1, IsNullable = true, ColumnDataType = "nvarchar(max)")]
    public string? Description { get; set; }
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? CoverImage { get; set; }
    public decimal TotalPrice { get; set; }
    public int CourseCount { get; set; }
    public int Status { get; set; }
    public int BuyCount { get; set; }
    public int SortOrder { get; set; }
    public bool IsRecommend { get; set; }
    public DateTime? ScheduledPublishTime { get; set; }
    public DateTime? ScheduledOfflineTime { get; set; }
    public long CreatedBy { get; set; }
}

/// <summary>套餐-课程关联表</summary>
[SugarTable("CoursePackageItem")]
public class CoursePackageItem
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long PackageId { get; set; }
    public long CourseId { get; set; }
    [SugarColumn(Length = 200, IsNullable = false)]
    public string CourseName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
