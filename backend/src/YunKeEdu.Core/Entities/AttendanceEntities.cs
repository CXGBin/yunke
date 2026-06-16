using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>签到记录表</summary>
[SugarTable("Attendance")]
public class Attendance
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long? CampusId { get; set; }
    public long ScheduleId { get; set; }
    public long CourseId { get; set; }
    public long StudentId { get; set; }
    public int Status { get; set; }
    public DateTime? SignInTime { get; set; }
    public int? SignMethod { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
    public long? OperatorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>请假记录表</summary>
[SugarTable("LeaveRequest")]
public class LeaveRequest
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long StudentId { get; set; }
    public long CourseId { get; set; }
    public long? ScheduleId { get; set; }
    public int LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [SugarColumn(Length = 500, IsNullable = false)]
    public string Reason { get; set; } = string.Empty;
    public int Status { get; set; }
    public long ApplicantId { get; set; }
    public long? PreReviewerId { get; set; }
    public DateTime? PreReviewedAt { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? PreReviewRemark { get; set; }
    public long? ApproverId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ApproveRemark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>签到二维码表（预留）</summary>
[SugarTable("SignInQRCode")]
public class SignInQRCode
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long? CampusId { get; set; }
    public long? ScheduleId { get; set; }
    [SugarColumn(Length = 128, IsNullable = false)]
    public string Token { get; set; } = string.Empty;
    public DateTime? ExpiredAt { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
