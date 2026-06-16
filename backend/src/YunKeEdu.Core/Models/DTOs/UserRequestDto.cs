
namespace YunKeEdu.Core.Models.DTOs;

/// <summary>创建用户请求</summary>
public class CreateUserDto
{
    public string? UserName { get; set; }
    public string? RealName { get; set; }
    public string? Phone { get; set; }
    public string? Password { get; set; }
    public int Role { get; set; }
    public long? OrgId { get; set; }
    public long? CampusId { get; set; }
}

/// <summary>更新用户请求</summary>
public class UpdateUserDto
{
    public string? RealName { get; set; }
    public string? NickName { get; set; }
    public string? Avatar { get; set; }
    public string? Phone { get; set; }
    public int? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Grade { get; set; }
    public int? Status { get; set; }
}

/// <summary>重置密码请求</summary>
public class ResetPwdDto { public string? NewPassword { get; set; } }

/// <summary>更新状态请求</summary>
public class UpdateStatusDto { public int Status { get; set; } }
