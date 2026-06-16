using System.ComponentModel.DataAnnotations;

namespace YunKeEdu.Core.Models.DTOs;

#region Course
public class CreateCourseRequest
{
    [Required(ErrorMessage = "课程名称不能为空")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public long? CategoryId { get; set; }

    public string? Description { get; set; }

    public string? CoverImage { get; set; }

    [Range(1, 999)]
    public int TotalLessons { get; set; } = 1;

    [Range(1, 999)]
    public int LessonDuration { get; set; } = 45;

    [Range(0, 2)]
    public int Difficulty { get; set; }

    [Range(0, 999999)]
    public decimal OriginalPrice { get; set; }

    [Range(0, 999999)]
    public decimal DiscountPrice { get; set; }

    [Range(1, 9999)]
    public int MaxStudents { get; set; } = 30;

    [Range(1, 9999)]
    public int MinStudents { get; set; } = 1;

    public DateTime? EnrollmentDeadline { get; set; }

    public string? Tags { get; set; }

    [Required]
    public long TeacherId { get; set; }

    [Required]
    public long CampusId { get; set; }

    [Range(0, 1)]
    public int SettlementType { get; set; }

    [Range(0, 999999)]
    public decimal FixedFeePerLesson { get; set; }

    [Range(0, 999999)]
    public decimal StudentCountCommission { get; set; }

    public int SortOrder { get; set; }
    public bool IsRecommend { get; set; }
    public DateTime? ScheduledPublishTime { get; set; }
    public DateTime? ScheduledOfflineTime { get; set; }
}

public class UpdateCourseRequest : CreateCourseRequest { }

public class CourseDto
{
    public long Id { get; set; }
    public string? CourseCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public int TotalLessons { get; set; }
    public int LessonDuration { get; set; }
    public int Difficulty { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountPrice { get; set; }
    public int MaxStudents { get; set; }
    public int MinStudents { get; set; }
    public DateTime? EnrollmentDeadline { get; set; }
    public string? Tags { get; set; }
    public int Status { get; set; }
    public long TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public long CampusId { get; set; }
    public string? CampusName { get; set; }
    public int SettlementType { get; set; }
    public decimal FixedFeePerLesson { get; set; }
    public decimal StudentCountCommission { get; set; }
    public int SortOrder { get; set; }
    public bool IsRecommend { get; set; }
    public int ViewCount { get; set; }
    public long CreatedBy { get; set; }
    public long? OrgId { get; set; }
    public string? OrgName { get; set; }
    public DateTime CreatedAt { get; set; }
}
#endregion

#region CourseCategory
public class CreateCategoryRequest
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    public long ParentId { get; set; }

    public string? Icon { get; set; }

    public int SortOrder { get; set; }
}

public class UpdateCategoryRequest : CreateCategoryRequest { }

public class CategoryTreeNode
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public long ParentId { get; set; }
    public List<CategoryTreeNode> Children { get; set; } = new();
}
#endregion

#region CourseAttachment
public class CreateAttachmentRequest
{
    [Required]
    [StringLength(200)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string FileUrl { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public string? FileType { get; set; }

    public int SortOrder { get; set; }
}

public class AttachmentDto
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? FileType { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
#endregion

#region CoursePackage
public class CreateCoursePackageRequest
{
    [Required]
    [StringLength(200)]
    public string PackageName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? CoverImage { get; set; }

    public long CampusId { get; set; }

    public int SortOrder { get; set; }
    public bool IsRecommend { get; set; }
    public DateTime? ScheduledPublishTime { get; set; }
    public DateTime? ScheduledOfflineTime { get; set; }

    public List<long> CourseIds { get; set; } = new();
}

public class UpdateCoursePackageRequest : CreateCoursePackageRequest { }

public class CoursePackageDto
{
    public long Id { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public decimal TotalPrice { get; set; }
    public int CourseCount { get; set; }
    public int Status { get; set; }
    public int BuyCount { get; set; }
    public int SortOrder { get; set; }
    public bool IsRecommend { get; set; }
    public long? OrgId { get; set; }
    public string? OrgName { get; set; }
    public List<CoursePackageItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CoursePackageItemDto
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int SortOrder { get; set; }
}

public class PurchaseCoursePackageRequest
{
    public string? PayChannel { get; set; }
}

public class MyCoursePackageDto
{
    public long PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string? CoverImage { get; set; }
    public decimal TotalPrice { get; set; }
    public int CourseCount { get; set; }
    public List<long> EnrolledCourseIds { get; set; } = new();
}
#endregion

#region Enrollment
public class CreateEnrollmentRequest
{
    [Required]
    public long CourseId { get; set; }

    public long? ParentId { get; set; }
    public string? Remark { get; set; }
}

public class ManualAddEnrollmentRequest
{
    [Required]
    public long CourseId { get; set; }

    [Required]
    public long StudentId { get; set; }

    public string? Remark { get; set; }
}

public class EnrollmentDto
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MyScheduleDto
{
    public long ScheduleId { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? LessonTitle { get; set; }
    public DateTime LessonDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? CampusName { get; set; }
    public string? TeacherName { get; set; }
    public int Status { get; set; }
    public int? AttendanceStatus { get; set; }
}

public class WaitListDto
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? NotifiedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
#endregion

#region LessonUnit
public class CreateLessonUnitRequest
{
    public int LessonNo { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public int SortOrder { get; set; }
}

public class BatchGenerateLessonRequest
{
    [Required]
    [Range(1, 999)]
    public int Count { get; set; }

    [StringLength(100)]
    public string? TitlePrefix { get; set; }

    [Range(1, 999)]
    public int StartNo { get; set; } = 1;
}

public class UpdateLessonUnitRequest : CreateLessonUnitRequest { }

public class LessonUnitDto
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public int LessonNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
}
#endregion
