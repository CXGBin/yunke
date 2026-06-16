using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>选课/报名表</summary>
[SugarTable("CourseEnrollment")]
public class CourseEnrollment
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long? CampusId { get; set; }
    public long CourseId { get; set; }
    public long StudentId { get; set; }
    public long? ParentId { get; set; }
    public int Status { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.Now;
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>候补表</summary>
[SugarTable("WaitList")]
public class WaitList
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long? CampusId { get; set; }
    public long CourseId { get; set; }
    public long StudentId { get; set; }
    public int Status { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.Now;
    public DateTime? NotifiedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
