using System.ComponentModel.DataAnnotations;

namespace YunKeEdu.Core.Models.DTOs;

public class CreateEvaluationRequest
{
    [Required]
    public long CourseId { get; set; }

    [Required]
    public long ScheduleId { get; set; }

    [Required]
    [Range(1, 5)]
    public int CourseRating { get; set; }

    [Range(1, 5)]
    public int? TeacherRating { get; set; }

    [Range(1, 5)]
    public int? LessonRating { get; set; }

    public string? Content { get; set; }

    public string? Tags { get; set; }

    public string? Images { get; set; }
    public bool IsAnonymous { get; set; }
}

public class ReplyEvaluationRequest
{
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    public string? Images { get; set; }
}

public class EvaluationDto
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public long ScheduleId { get; set; }
    public long EvaluatorId { get; set; }
    public string EvaluatorName { get; set; } = string.Empty;
    public string? EvaluatorAvatar { get; set; }
    public long TargetId { get; set; }
    public string TargetName { get; set; } = string.Empty;
    public int EvalType { get; set; }
    public int? CourseRating { get; set; }
    public int? TeacherRating { get; set; }
    public int? LessonRating { get; set; }
    public string? Content { get; set; }
    public string? Tags { get; set; }
    public string? Images { get; set; }
    public bool IsAnonymous { get; set; }
    public int Status { get; set; }
    public string? ReplyContent { get; set; }
    public long? ReplyBy { get; set; }
    public DateTime? ReplyAt { get; set; }
    public bool IsTop { get; set; }
    public List<EvaluationReplyDto> Replies { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class EvaluationReplyDto
{
    public long Id { get; set; }
    public long EvaluationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Images { get; set; }
    public int ReplyType { get; set; }
    public long ReplyById { get; set; }
    public string? ReplyByName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EvaluationStatisticsDto
{
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public decimal AvgCourseRating { get; set; }
    public decimal AvgTeacherRating { get; set; }
    public decimal AvgLessonRating { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; } = new();
}

public class TeacherEvaluationStatisticsDto
{
    public long TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int TotalEvaluations { get; set; }
    public decimal AvgRating { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; } = new();
}
