using System.ComponentModel.DataAnnotations;

namespace YunKeEdu.Core.Models.DTOs;

#region Organization
public class CreateOrgRequest
{
    [Required(ErrorMessage = "机构名称不能为空")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Logo { get; set; }

    [StringLength(50)]
    public string? ContactPerson { get; set; }

    [StringLength(20)]
    public string? ContactPhone { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? Province { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(50)]
    public string? District { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}

public class UpdateOrgRequest : CreateOrgRequest { }

public class OrgDto
{
    public long Id { get; set; }
    public string OrgCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public int Status { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public string? Description { get; set; }
    public long? CurrentPackageId { get; set; }
    public DateTime CreatedAt { get; set; }
}
#endregion

#region Campus
public class CreateCampusRequest
{
    [Required(ErrorMessage = "校区名称不能为空")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? ContactPerson { get; set; }

    [StringLength(20)]
    public string? ContactPhone { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }

    public int SortOrder { get; set; }
}

public class UpdateCampusRequest : CreateCampusRequest { }

public class CampusDto
{
    public long Id { get; set; }
    public long OrgId { get; set; }
    public string CampusCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public int Status { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
#endregion

#region Teacher
public class CreateTeacherRequest
{
    [Required(ErrorMessage = "姓名不能为空")]
    [StringLength(50)]
    public string RealName { get; set; } = string.Empty;

    [Required(ErrorMessage = "手机号不能为空")]
    [Phone(ErrorMessage = "手机号格式不正确")]
    public string Phone { get; set; } = string.Empty;

    public long CampusId { get; set; }

    [StringLength(500)]
    public string? Avatar { get; set; }

    public int Gender { get; set; }

    [StringLength(500)]
    public string? Introduction { get; set; }
}

public class UpdateTeacherRequest : CreateTeacherRequest { }

public class TeacherDto
{
    public long Id { get; set; }
    public string? UserCode { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? RealName { get; set; }
    public string? NickName { get; set; }
    public string? Avatar { get; set; }
    public string? Phone { get; set; }
    public int Gender { get; set; }
    public int Role { get; set; }
    public long? OrgId { get; set; }
    public long? CampusId { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateStatusRequest
{
    [Required(ErrorMessage = "状态值不能为空")]
    public int Status { get; set; }
}
#endregion

#region Student
public class StudentImportRequest
{
    [Required(ErrorMessage = "姓名不能为空")]
    [StringLength(50)]
    public string RealName { get; set; } = string.Empty;

    [Required(ErrorMessage = "手机号不能为空")]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    public long CampusId { get; set; }

    [StringLength(20)]
    public string? Grade { get; set; }

    public int Gender { get; set; }

    [StringLength(50)]
    public string? ParentName { get; set; }

    [StringLength(20)]
    public string? ParentPhone { get; set; }
}

public class StudentDto
{
    public long Id { get; set; }
    public string? UserCode { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? RealName { get; set; }
    public string? Avatar { get; set; }
    public string? Phone { get; set; }
    public int Gender { get; set; }
    public string? Grade { get; set; }
    public int Status { get; set; }
    public long? OrgId { get; set; }
    public long? CampusId { get; set; }
    public string? OrgName { get; set; }
    public DateTime CreatedAt { get; set; }
}
#endregion

#region Parent
public class BindStudentRequest
{
    [Required(ErrorMessage = "家长ID不能为空")]
    public long ParentId { get; set; }

    [Required(ErrorMessage = "学生ID不能为空")]
    public long StudentId { get; set; }

    [Range(0, 4)]
    public int RelationType { get; set; }

    public bool IsPrimary { get; set; }
}

public class LinkStudentRequest
{
    [Required(ErrorMessage = "学生UserCode不能为空")]
    public string StudentUserCode { get; set; } = string.Empty;
}

public class ConfirmLinkRequest
{
    [Required]
    public long RelationId { get; set; }
    public bool Accept { get; set; }
}

public class ParentDto
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? RealName { get; set; }
    public string? Avatar { get; set; }
    public string? Phone { get; set; }
    public int ChildrenCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ParentDetailDto : ParentDto
{
    public List<ChildInfo> Children { get; set; } = new();
}

public class ChildInfo
{
    public long RelationId { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? StudentAvatar { get; set; }
    public string? Grade { get; set; }
    public int RelationType { get; set; }
    public bool IsPrimary { get; set; }
}
#endregion

#region Invitation
public class GenerateInvitationRequest
{
    [Required]
    public int InvitedRole { get; set; }

    [Required(ErrorMessage = "被邀请人姓名不能为空")]
    [StringLength(50)]
    public string InvitedName { get; set; } = string.Empty;

    [Required(ErrorMessage = "被邀请人手机号不能为空")]
    [Phone]
    public string InvitedPhone { get; set; } = string.Empty;

    public long CampusId { get; set; }

    [StringLength(200)]
    public string? Remark { get; set; }
}

public class AcceptInvitationRequest
{
    [Required]
    public string InviteCode { get; set; } = string.Empty;
}

public class InvitationDto
{
    public long Id { get; set; }
    public string InviteCode { get; set; } = string.Empty;
    public int InvitedRole { get; set; }
    public string? InvitedName { get; set; }
    public string? InvitedPhone { get; set; }
    public int Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public long? UsedBy { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ValidateInvitationDto
{
    public bool Valid { get; set; }
    public string? InviteCode { get; set; }
    public int InvitedRole { get; set; }
    public string? OrgName { get; set; }
    public string? CampusName { get; set; }
    public string? InvitedName { get; set; }
}
#endregion

#region Config
public class UpdateOrgConfigRequest
{
    public int FreeRefundDays { get; set; } = 3;
    public string? SignInMethods { get; set; }
    public int AttendanceTimeout { get; set; } = 15;
    public bool EnableLeaveApproval { get; set; }
    public bool EnableTeacherPreReview { get; set; }
    public int WaitlistExpireHours { get; set; } = 24;
    public int MaxStudentsPerParent { get; set; } = 10;
    public int MaxParentsPerStudent { get; set; } = 5;
    public int MaxCoursesPerStudent { get; set; } = 20;
    public int InvitationExpireDays { get; set; } = 7;
}

public class OrgConfigDto
{
    public long Id { get; set; }
    public int FreeRefundDays { get; set; }
    public string? SignInMethods { get; set; }
    public int AttendanceTimeout { get; set; }
    public bool EnableEvaluationReview { get; set; }
    public bool EnableLeaveApproval { get; set; }
    public bool EnableTeacherPreReview { get; set; }
    public int WaitlistExpireHours { get; set; }
    public int MaxStudentsPerParent { get; set; }
    public int MaxParentsPerStudent { get; set; }
    public int MaxCoursesPerStudent { get; set; }
    public int InvitationExpireDays { get; set; }
    public string? ThemeConfig { get; set; }
}

public class UpdateSysConfigRequest
{
    [Required]
    public string ConfigKey { get; set; } = string.Empty;

    [Required]
    public string ConfigValue { get; set; } = string.Empty;

    public string? ConfigGroup { get; set; }
    public string? Description { get; set; }
}

public class SysConfigDto
{
    public long Id { get; set; }
    public string ConfigKey { get; set; } = string.Empty;
    public string ConfigValue { get; set; } = string.Empty;
    public string? ConfigGroup { get; set; }
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
}
#endregion

#region Theme
public class UpdateOrgThemeRequest
{
    [Required]
    public string ThemeConfig { get; set; } = string.Empty;
}

public class SwitchThemeRequest
{
    [Required]
    public string ThemeId { get; set; } = string.Empty;
}

public class ThemeDto
{
    public string ThemeId { get; set; } = string.Empty;
    public string ThemeName { get; set; } = string.Empty;
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? ButtonColor { get; set; }
    public string? BackgroundColor { get; set; }
}

public class EvaluationTagDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TagType { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEvaluationTagRequest
{
    [Required(ErrorMessage = "标签名称不能为空")]
    [StringLength(20)]
    public string Name { get; set; } = string.Empty;

    public int TagType { get; set; }
    public int SortOrder { get; set; }
}
#endregion


/// <summary>更新系统配置请求</summary>
public class UpdateConfigDto { public string ConfigKey { get; set; } = ""; public string ConfigValue { get; set; } = ""; }
