using System.ComponentModel.DataAnnotations;

namespace YunKeEdu.Core.Models.DTOs;

public class SignInRequest
{
    [Required]
    public long ScheduleId { get; set; }

    [Required]
    public long StudentId { get; set; }

    [Required]
    [Range(0, 4)]
    public int Status { get; set; }

    public int? SignMethod { get; set; }
    public string? Remark { get; set; }
}

public class SignAllRequest
{
    [Required]
    public long ScheduleId { get; set; }

    [Range(0, 4)]
    public int Status { get; set; } = 1;

    public string? Remark { get; set; }
}

public class AttendanceDto
{
    public long Id { get; set; }
    public long ScheduleId { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime? SignInTime { get; set; }
    public int? SignMethod { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}
