// YunKeEdu.Core - 权限控制 DTOs
namespace YunKeEdu.Core.Models.DTOs;

/// <summary>菜单树节点DTO</summary>
public class MenuDto
{
    public long Id { get; set; }
    public long ParentId { get; set; }
    public int MenuType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public string? Permission { get; set; }
    public string? BtnType { get; set; }
    public int Visible { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public List<MenuDto> Children { get; set; } = new();
}

/// <summary>创建/更新菜单请求</summary>
public class CreateMenuDto
{
    public long ParentId { get; set; }
    public int MenuType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public string? Permission { get; set; }
    public string? BtnType { get; set; }
    public int Visible { get; set; } = 1;
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
}

/// <summary>角色DTO</summary>
public class RoleDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? RoleCode { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public int DataScope { get; set; }
    public List<long> MenuIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>创建/更新角色请求</summary>
public class CreateRoleDto
{
    public string RoleName { get; set; } = string.Empty;
    public string? RoleCode { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
    public int DataScope { get; set; } = 1;
    public List<long> MenuIds { get; set; } = new();
}

/// <summary>用户权限信息（登录时返回）</summary>
public class UserPermissionDto
{
    /// <summary>角色列表</summary>
    public List<RoleDto> Roles { get; set; } = new();
    /// <summary>菜单树（用于动态渲染）</summary>
    public List<MenuDto> Menus { get; set; } = new();
    /// <summary>权限码集合（按钮级控制）</summary>
    public List<string> Permissions { get; set; } = new();
}
