using System.ComponentModel.DataAnnotations;

namespace YunKeEdu.Core.Models.DTOs;

public class CreateLeaveRequest
{
    [Required]
    public long CourseId { get; set; }

    public long? ScheduleId { get; set; }

    [Required]
    [Range(0, 3)]
    public int LeaveType { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;
}

public class PreReviewRequest
{
    [Required]
    public bool Approve { get; set; }

    public string? Remark { get; set; }
}

public class ApproveLeaveRequest
{
    [Required]
    public bool Approve { get; set; }

    public string? Remark { get; set; }
}

public class LeaveRequestDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public long? ScheduleId { get; set; }
    public int LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int Status { get; set; }
    public long ApplicantId { get; set; }
    public long? PreReviewerId { get; set; }
    public string? PreReviewerName { get; set; }
    public DateTime? PreReviewedAt { get; set; }
    public string? PreReviewRemark { get; set; }
    public long? ApproverId { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApproveRemark { get; set; }
    public DateTime CreatedAt { get; set; }
}
