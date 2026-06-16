using System.ComponentModel.DataAnnotations;

namespace YunKeEdu.Core.Models.DTOs;

public class CreateScheduleRequest
{
    [Required]
    public long CourseId { get; set; }

    [Required]
    public long CampusId { get; set; }

    [Required]
    public long TeacherId { get; set; }

    [Required]
    public DateTime LessonDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public int? LessonNo { get; set; }
    public string? LessonTitle { get; set; }
    public string? Remark { get; set; }
}

public class UpdateScheduleRequest : CreateScheduleRequest { }

public class CreateRecurrenceRequest
{
    [Required]
    public long CourseId { get; set; }

    [Required]
    public long CampusId { get; set; }

    [Required]
    public long TeacherId { get; set; }

    [Required]
    public string WeekDays { get; set; } = string.Empty;

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [Range(1, 999)]
    public int TotalLessons { get; set; }

    public string? Remark { get; set; }
}

public class ScheduleDto
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public long CampusId { get; set; }
    public string? CampusName { get; set; }
    public long TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public DateTime LessonDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int? LessonNo { get; set; }
    public string? LessonTitle { get; set; }
    public string? Remark { get; set; }
    public int Status { get; set; }
    public string? CancelReason { get; set; }
    public bool IsRescheduled { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CancelScheduleRequest
{
    public string? CancelReason { get; set; }
}

public class CalendarEventDto
{
    public DateTime Date { get; set; }
    public List<CalendarItemDto> Events { get; set; } = new();
}

public class CalendarItemDto
{
    public long ScheduleId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? CampusName { get; set; }
    public string? TeacherName { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Status { get; set; }
}

public class ConflictCheckRequest
{
    [Required]
    public long CampusId { get; set; }

    [Required]
    public DateTime LessonDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public long? ExcludeScheduleId { get; set; }
    public long? ExcludeTeacherId { get; set; }
}

public class ConflictCheckResult
{
    public bool HasConflict { get; set; }
    public List<ScheduleDto> Conflicts { get; set; } = new();
}

public class ScheduleChangeLogDto
{
    public long Id { get; set; }
    public long ScheduleId { get; set; }
    public int ChangeType { get; set; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
    public string? Reason { get; set; }
    public long OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public DateTime CreatedAt { get; set; }
}
