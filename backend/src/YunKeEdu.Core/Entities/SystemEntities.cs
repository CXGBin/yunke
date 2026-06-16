using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>机构表</summary>
[SugarTable("Organization")]
public class Organization : BaseEntity
{
    [SugarColumn(Length = 32, IsNullable = false)]
    public string OrgCode { get; set; } = string.Empty;
    [SugarColumn(Length = 200, IsNullable = false)]
    public string Name { get; set; } = string.Empty;
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Logo { get; set; }
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ContactPerson { get; set; }
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? ContactPhone { get; set; }
    [SugarColumn(Length = 300, IsNullable = true)]
    public string? Address { get; set; }
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Province { get; set; }
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? City { get; set; }
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? District { get; set; }
    public int Status { get; set; } = 1;
    public DateTime? ExpiredAt { get; set; }
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }
    [SugarColumn(Length = -1, IsNullable = true, ColumnDataType = "nvarchar(max)")]
    public string? ThemeConfig { get; set; }
    public long? CurrentPackageId { get; set; }
}

/// <summary>校区表</summary>
[SugarTable("Campus")]
public class Campus : BaseEntity
{
    public long OrgId { get; set; }
    [SugarColumn(Length = 32, IsNullable = false)]
    public string CampusCode { get; set; } = string.Empty;
    [SugarColumn(Length = 200, IsNullable = false)]
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ContactPerson { get; set; }
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? ContactPhone { get; set; }
    [SugarColumn(Length = 300, IsNullable = true)]
    public string? Address { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public int Status { get; set; } = 1;
    public int SortOrder { get; set; }
}

/// <summary>系统用户表</summary>
[SugarTable("SysUser")]
public class SysUser : BaseEntity
{
    public long? OrgId { get; set; }
    public long? CampusId { get; set; }
    [SugarColumn(Length = 64, IsNullable = true)]
    public string? UserCode { get; set; }
    [SugarColumn(Length = 64, IsNullable = false)]
    public string UserName { get; set; } = string.Empty;
    [SugarColumn(Length = 256, IsNullable = true)]
    public string? Password { get; set; }
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? RealName { get; set; }
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? NickName { get; set; }
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Avatar { get; set; }
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }
    public int Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Grade { get; set; }
    public int Role { get; set; }
    [SugarColumn(Length = 128, IsNullable = true)]
    public string? OpenId { get; set; }
    [SugarColumn(Length = 128, IsNullable = true)]
    public string? UnionId { get; set; }
    public int Status { get; set; } = 1;
    public DateTime? LastLoginAt { get; set; }
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LastLoginIp { get; set; }
    public DateTime? PasswordChangedAt { get; set; }
}

/// <summary>用户-机构绑定关系表</summary>
[SugarTable("UserOrgBinding")]
public class UserOrgBinding
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long UserId { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long CampusId { get; set; }
    public int Role { get; set; }
    [SugarColumn(Length = 64, IsNullable = true)]
    public string? UserCode { get; set; }
    public int Status { get; set; } = 1;
    public DateTime BoundAt { get; set; } = DateTime.Now;
    public int BoundVia { get; set; }
    public long? InvitationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>邀请记录表</summary>
[SugarTable("InvitationRecord")]
public class InvitationRecord : BaseEntity
{
    public long OrgId { get; set; }
    public long CampusId { get; set; }
    public long InviterId { get; set; }
    [SugarColumn(Length = 32, IsNullable = false)]
    public string InviteCode { get; set; } = string.Empty;
    public int InvitedRole { get; set; }
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? InvitedName { get; set; }
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? InvitedPhone { get; set; }
    public int Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public long? UsedBy { get; set; }
    public DateTime? UsedAt { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
}

/// <summary>家长-学生关联表</summary>
[SugarTable("ParentStudentRelation")]
public class ParentStudentRelation : BaseEntity
{
    public long OrgId { get; set; }
    public long ParentId { get; set; }
    public long StudentId { get; set; }
    public int RelationType { get; set; }
    public bool IsPrimary { get; set; }
    public int Status { get; set; } = 1;
    public long? ConfirmedBy { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

/// <summary>机构配置表</summary>
[SugarTable("OrgConfig")]
public class OrgConfig
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public int FreeRefundDays { get; set; } = 3;
    [SugarColumn(Length = 100, IsNullable = true, DefaultValue = "0,3")]
    public string? SignInMethods { get; set; } = "0,3";
    public int AttendanceTimeout { get; set; } = 15;
    public bool EnableEvaluationReview { get; set; }
    public bool EnableLeaveApproval { get; set; }
    public bool EnableTeacherPreReview { get; set; }
    public int WaitlistExpireHours { get; set; } = 24;
    public int MaxStudentsPerParent { get; set; } = 10;
    public int MaxParentsPerStudent { get; set; } = 5;
    public int MaxCoursesPerStudent { get; set; } = 20;
    public int InvitationExpireDays { get; set; } = 7;
    [SugarColumn(Length = -1, IsNullable = true, ColumnDataType = "nvarchar(max)")]
    public string? ThemeConfig { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>系统全局配置表</summary>
[SugarTable("SysConfig")]
public class SysConfig
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    [SugarColumn(Length = 100, IsNullable = false)]
    public string ConfigKey { get; set; } = string.Empty;
    [SugarColumn(Length = -1, IsNullable = false, ColumnDataType = "nvarchar(max)")]
    public string ConfigValue { get; set; } = string.Empty;
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ConfigGroup { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>评价标签配置表</summary>
[SugarTable("EvaluationTag")]
public class EvaluationTag
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Name { get; set; } = string.Empty;
    public int TagType { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
