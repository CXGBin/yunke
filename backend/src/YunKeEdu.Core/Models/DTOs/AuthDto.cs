using System.ComponentModel.DataAnnotations;

namespace YunKeEdu.Core.Models.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "用户名不能为空")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}

public class WxLoginRequest
{
    [Required(ErrorMessage = "code不能为空")]
    public string Code { get; set; } = string.Empty;

    public string? NickName { get; set; }
    public string? Avatar { get; set; }
}

public class BindPhoneRequest
{
    [Required(ErrorMessage = "手机号不能为空")]
    [Phone(ErrorMessage = "手机号格式不正确")]
    public string Phone { get; set; } = string.Empty;
}

public class RegisterOrgRequest
{
    [Required(ErrorMessage = "机构名称不能为空")]
    [StringLength(200)]
    public string OrgName { get; set; } = string.Empty;

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
}

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "旧密码不能为空")]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "新密码不能为空")]
    [MinLength(6, ErrorMessage = "新密码至少6位")]
    public string NewPassword { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserInfoDto UserInfo { get; set; } = new();
}

public class UserInfoDto
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? RealName { get; set; }
    public string? NickName { get; set; }
    public string? Avatar { get; set; }
    public int Role { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public List<UserOrgInfo> Bindings { get; set; } = new();
}

public class UserOrgInfo
{
    public long OrgId { get; set; }
    public string OrgName { get; set; } = string.Empty;
    public long CampusId { get; set; }
    public string CampusName { get; set; } = string.Empty;
    public int Role { get; set; }
    public string? UserCode { get; set; }
}

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
