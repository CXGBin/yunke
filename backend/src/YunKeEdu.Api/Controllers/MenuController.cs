// YunKeEdu.Api - 权限管理 Controller
using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Application.Services;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Api.Controllers;

/// <summary>菜单管理</summary>
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IPermissionService _svc;
    public MenuController(IPermissionService svc) => _svc = svc;

    [HttpGet("tree")]
    public async Task<ApiResponse<List<MenuDto>>> GetTree() => ApiResponse<List<MenuDto>>.Ok(await _svc.GetMenuTreeAsync());

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateMenuDto dto) => ApiResponse<long>.Ok(await _svc.CreateMenuAsync(dto));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] CreateMenuDto dto)
    {
        await _svc.UpdateMenuAsync(id, dto);
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _svc.DeleteMenuAsync(id);
        return ApiResponse<bool>.Ok(true);
    }
}

/// <summary>角色管理</summary>
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IPermissionService _svc;
    public RoleController(IPermissionService svc) => _svc = svc;

    [HttpGet("list")]
    public async Task<ApiResponse<PagedResult<RoleDto>>> GetList([FromQuery] PageRequest page) => ApiResponse<PagedResult<RoleDto>>.Ok(await _svc.GetRolesAsync(page));

    [HttpGet("{id}")]
    public async Task<ApiResponse<RoleDto>> Get(long id) => ApiResponse<RoleDto>.Ok(await _svc.GetRoleAsync(id));

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateRoleDto dto) => ApiResponse<long>.Ok(await _svc.CreateRoleAsync(dto));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] CreateRoleDto dto)
    {
        await _svc.UpdateRoleAsync(id, dto);
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _svc.DeleteRoleAsync(id);
        return ApiResponse<bool>.Ok(true);
    }
}
