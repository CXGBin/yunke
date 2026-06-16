// YunKeEdu.Application - 权限控制 Service
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using SqlSugar;
using Microsoft.Extensions.Logging;

namespace YunKeEdu.Application.Services;

public interface IPermissionService
{
    /// <summary>获取菜单树（全部）</summary>
    Task<List<MenuDto>> GetMenuTreeAsync();
    /// <summary>创建菜单</summary>
    Task<long> CreateMenuAsync(CreateMenuDto dto);
    /// <summary>更新菜单</summary>
    Task UpdateMenuAsync(long id, CreateMenuDto dto);
    /// <summary>删除菜单</summary>
    Task DeleteMenuAsync(long id);
    /// <summary>获取角色列表</summary>
    Task<PagedResult<RoleDto>> GetRolesAsync(PageRequest page);
    /// <summary>获取角色详情</summary>
    Task<RoleDto> GetRoleAsync(long id);
    /// <summary>创建角色</summary>
    Task<long> CreateRoleAsync(CreateRoleDto dto);
    /// <summary>更新角色（含菜单分配）</summary>
    Task UpdateRoleAsync(long id, CreateRoleDto dto);
    /// <summary>删除角色</summary>
    Task DeleteRoleAsync(long id);
    /// <summary>获取用户权限信息（角色+菜单树+权限码）</summary>
    Task<UserPermissionDto> GetUserPermissionsAsync(long userId, long tenantId);
}

public class PermissionService : BaseService, IPermissionService
{
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(ISqlSugarClient db, ILogger<PermissionService> logger) : base(db)
    {
        _logger = logger;
    }

    public async Task<List<MenuDto>> GetMenuTreeAsync()
    {
        var menus = await Db.Queryable<SysMenu>()
            .Where(m => m.Status == 1)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        return BuildMenuTree(menus, 0);
    }

    public async Task<long> CreateMenuAsync(CreateMenuDto dto)
    {
        var menu = new SysMenu
        {
            ParentId = dto.ParentId,
            MenuType = dto.MenuType,
            Name = dto.Name,
            Path = dto.Path,
            Component = dto.Component,
            Icon = dto.Icon,
            SortOrder = dto.SortOrder,
            Permission = dto.Permission,
            BtnType = dto.BtnType,
            Visible = dto.Visible,
            Status = dto.Status,
            Description = dto.Description,
        };
        await Db.Insertable(menu).ExecuteCommandAsync();
        return menu.Id;
    }

    public async Task UpdateMenuAsync(long id, CreateMenuDto dto)
    {
        var menu = await Db.Queryable<SysMenu>().FirstAsync(m => m.Id == id)
            ?? throw new BizException("菜单不存在");

        menu.ParentId = dto.ParentId;
        menu.MenuType = dto.MenuType;
        menu.Name = dto.Name;
        menu.Path = dto.Path;
        menu.Component = dto.Component;
        menu.Icon = dto.Icon;
        menu.SortOrder = dto.SortOrder;
        menu.Permission = dto.Permission;
        menu.BtnType = dto.BtnType;
        menu.Visible = dto.Visible;
        menu.Status = dto.Status;
        menu.Description = dto.Description;
        menu.UpdatedAt = DateTime.Now;
        await Db.Updateable(menu).ExecuteCommandAsync();
    }

    public async Task DeleteMenuAsync(long id)
    {
        var hasChildren = await Db.Queryable<SysMenu>().AnyAsync(m => m.ParentId == id);
        if (hasChildren) throw new BizException("存在子菜单，不允许删除");
        await Db.Deleteable<SysMenu>().Where(m => m.Id == id).ExecuteCommandAsync();
    }

    public async Task<PagedResult<RoleDto>> GetRolesAsync(PageRequest page)
    {
        var query = Db.Queryable<SysRole>();
        if (!string.IsNullOrWhiteSpace(page.Keyword))
            query = query.Where(r => r.RoleName.Contains(page.Keyword) || r.RoleCode!.Contains(page.Keyword));

        var total = await query.CountAsync();
        var list = await query.OrderBy(r => r.SortOrder)
            .ToPageListAsync(page.Page, page.PageSize);

        var dtos = list.Select(r => new RoleDto
        {
            Id = r.Id, TenantId = r.TenantId, RoleName = r.RoleName,
            RoleCode = r.RoleCode, Description = r.Description,
            SortOrder = r.SortOrder, Status = r.Status, DataScope = r.DataScope,
            CreatedAt = r.CreatedAt, UpdatedAt = r.UpdatedAt,
        }).ToList();

        return new PagedResult<RoleDto>(dtos, total, page.Page, page.PageSize);
    }

    public async Task<RoleDto> GetRoleAsync(long id)
    {
        var role = await Db.Queryable<SysRole>().FirstAsync(r => r.Id == id)
            ?? throw new BizException("角色不存在");

        var menuIds = await Db.Queryable<SysRoleMenu>()
            .Where(rm => rm.RoleId == id)
            .Select(rm => rm.MenuId)
            .ToListAsync();

        return new RoleDto
        {
            Id = role.Id, TenantId = role.TenantId, RoleName = role.RoleName,
            RoleCode = role.RoleCode, Description = role.Description,
            SortOrder = role.SortOrder, Status = role.Status, DataScope = role.DataScope,
            MenuIds = menuIds,
            CreatedAt = role.CreatedAt, UpdatedAt = role.UpdatedAt,
        };
    }

    public async Task<long> CreateRoleAsync(CreateRoleDto dto)
    {
        var role = new SysRole
        {
            TenantId = 0, // 平台级角色
            RoleName = dto.RoleName,
            RoleCode = dto.RoleCode,
            Description = dto.Description,
            SortOrder = dto.SortOrder,
            Status = dto.Status,
            DataScope = dto.DataScope,
        };
        await Db.Insertable(role).ExecuteCommandAsync();

        if (dto.MenuIds.Count > 0)
        {
            var roleMenus = dto.MenuIds.Select(mid => new SysRoleMenu { RoleId = role.Id, MenuId = mid }).ToList();
            await Db.Insertable(roleMenus).ExecuteCommandAsync();
        }
        return role.Id;
    }

    public async Task UpdateRoleAsync(long id, CreateRoleDto dto)
    {
        var role = await Db.Queryable<SysRole>().FirstAsync(r => r.Id == id)
            ?? throw new BizException("角色不存在");

        role.RoleName = dto.RoleName;
        role.RoleCode = dto.RoleCode;
        role.Description = dto.Description;
        role.SortOrder = dto.SortOrder;
        role.Status = dto.Status;
        role.DataScope = dto.DataScope;
        role.UpdatedAt = DateTime.Now;
        await Db.Updateable(role).ExecuteCommandAsync();

        // 更新角色菜单关联（先删后加）
        await Db.Deleteable<SysRoleMenu>().Where(rm => rm.RoleId == id).ExecuteCommandAsync();
        if (dto.MenuIds.Count > 0)
        {
            var roleMenus = dto.MenuIds.Select(mid => new SysRoleMenu { RoleId = id, MenuId = mid }).ToList();
            await Db.Insertable(roleMenus).ExecuteCommandAsync();
        }
    }

    public async Task DeleteRoleAsync(long id)
    {
        await Db.Ado.BeginTranAsync();
        try
        {
            await Db.Deleteable<SysRoleMenu>().Where(rm => rm.RoleId == id).ExecuteCommandAsync();
            await Db.Deleteable<SysUserRole>().Where(ur => ur.RoleId == id).ExecuteCommandAsync();
            await Db.Deleteable<SysRole>().Where(r => r.Id == id).ExecuteCommandAsync();
            await Db.Ado.CommitTranAsync();
        }
        catch
        {
            await Db.Ado.RollbackTranAsync();
            throw;
        }
    }

    public async Task<UserPermissionDto> GetUserPermissionsAsync(long userId, long tenantId)
    {
        // 获取用户角色列表
        var userRoles = await Db.Queryable<SysUserRole>()
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        // 也检查旧版 Role 字段（兼容现有数据）
        var user = await Db.Queryable<SysUser>().FirstAsync(u => u.Id == userId);
        if (user != null && user.Role > 0 && !roleIds.Contains(user.Role))
        {
            roleIds.Add(user.Role);
        }

        if (roleIds.Count == 0)
            return new UserPermissionDto();

        // 获取角色详情
        var roles = await Db.Queryable<SysRole>()
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync();

        // 获取角色关联的菜单ID
        var menuIds = await Db.Queryable<SysRoleMenu>()
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Select(rm => rm.MenuId)
            .Distinct()
            .ToListAsync();

        // 获取菜单树
        var menus = await Db.Queryable<SysMenu>()
            .Where(m => m.Status == 1 && menuIds.Contains(m.Id))
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        // 获取所有权限码
        var permissions = menus
            .Where(m => !string.IsNullOrEmpty(m.Permission))
            .Select(m => m.Permission!)
            .Distinct()
            .ToList();

        return new UserPermissionDto
        {
            Roles = roles.Select(r => new RoleDto
            {
                Id = r.Id, TenantId = r.TenantId, RoleName = r.RoleName,
                RoleCode = r.RoleCode, Status = r.Status,
            }).ToList(),
            Menus = BuildMenuTree(menus, 0),
            Permissions = permissions,
        };
    }

    private static List<MenuDto> BuildMenuTree(List<SysMenu> allMenus, long parentId)
    {
        return allMenus
            .Where(m => m.ParentId == parentId)
            .Select(m => new MenuDto
            {
                Id = m.Id, ParentId = m.ParentId, MenuType = m.MenuType,
                Name = m.Name, Path = m.Path, Component = m.Component,
                Icon = m.Icon, SortOrder = m.SortOrder, Permission = m.Permission,
                BtnType = m.BtnType, Visible = m.Visible, Status = m.Status,
                Description = m.Description,
                Children = BuildMenuTree(allMenus, m.Id),
            })
            .OrderBy(m => m.SortOrder)
            .ToList();
    }
}
