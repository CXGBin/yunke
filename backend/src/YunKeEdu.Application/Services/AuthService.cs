
using Microsoft.AspNetCore.Http;
using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Enums;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Infrastructure.JWT;
using BCrypt.Net;

namespace YunKeEdu.Application.Services;

public class AuthService : BaseService
{
    private readonly IJwtHelper _jwtHelper;
    public AuthService(ISqlSugarClient db, IJwtHelper jwtHelper) : base(db) => _jwtHelper = jwtHelper;

    public async Task<LoginResponse> LoginAsync(LoginRequest req, HttpContext ctx)
    {
        var user = await Db.Queryable<SysUser>()
            .Where(u => u.Phone == req.Phone && !u.IsDeleted)
            .FirstAsync()
            ?? throw new BizException("用户名或密码错误");

        if (user.Status != 1) throw new BizException("账号已被停用");
        if (string.IsNullOrEmpty(user.Password) || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
            throw new BizException("用户名或密码错误");

        if (user.Role != (int)RoleEnum.PlatformAdmin)
            throw new BizException("仅平台管理员可通过账号密码登录");

        user.LastLoginAt = DateTime.Now;
        user.LastLoginIp = ctx.Connection.RemoteIpAddress?.ToString();
        await Db.Updateable(user).UpdateColumns(u => new { u.LastLoginAt, u.LastLoginIp }).ExecuteCommandAsync();

        return await BuildLoginResponseAsync(user);
    }

    public async Task<LoginResponse> WxLoginAsync(WxLoginRequest req)
    {
        // TODO: 接入微信SDK获取OpenId，暂时用Code模拟
        var user = await Db.Queryable<SysUser>()
            .Where(u => u.OpenId == req.Code && !u.IsDeleted)
            .FirstAsync();

        if (user == null)
        {
            user = new SysUser
            {
                UserName = $"wx_{Guid.NewGuid():N}[..8]",
                Password = string.Empty,
                Role = (int)RoleEnum.NoRole,
                NickName = req.NickName ?? "微信用户",
                Avatar = req.Avatar,
                OpenId = req.Code,
                Status = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            user.Id = await Db.Insertable(user).ExecuteReturnBigIdentityAsync();
        }

        user.LastLoginAt = DateTime.Now;
        await Db.Updateable(user).UpdateColumns(u => new { u.LastLoginAt }).ExecuteCommandAsync();

        return await BuildLoginResponseAsync(user);
    }

    public async Task BindPhoneAsync(BindPhoneRequest req, CurrentUser current)
    {
        var user = await Db.Queryable<SysUser>().InSingleAsync(current.UserId)
            ?? throw new BizException("用户不存在");
        var exists = await Db.Queryable<SysUser>().Where(u => u.Phone == req.Phone && u.Id != current.UserId && !u.IsDeleted).AnyAsync();
        if (exists) throw new BizException("该手机号已被其他账号绑定");
        user.Phone = req.Phone;
        user.UpdatedAt = DateTime.Now;
        await Db.Updateable(user).UpdateColumns(u => new { u.Phone, u.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task<LoginResponse> RegisterOrgAsync(RegisterOrgRequest req, CurrentUser current)
    {
        if (current.Role != (int)RoleEnum.NoRole)
            throw new BizException("仅无角色用户可注册机构");

        var orgCode = $"YK{DateTime.Now:yyyyMMdd}{new Random().Next(100, 999)}";
        var org = new Organization
        {
            OrgCode = orgCode, Name = req.OrgName, ContactPerson = req.ContactPerson,
            ContactPhone = req.ContactPhone, Address = req.Address,
            Province = req.Province, City = req.City, District = req.District,
            Status = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        org.Id = await Db.Insertable(org).ExecuteReturnBigIdentityAsync();
        org.TenantId = org.Id;
        await Db.Updateable(org).UpdateColumns(o => new { o.TenantId }).ExecuteCommandAsync();

        var campusCode = $"{orgCode}-DEFAULT";
        var campus = new Campus
        {
            TenantId = org.Id, OrgId = org.Id, CampusCode = campusCode,
            Name = $"{req.OrgName}默认校区", IsDefault = true, Status = 1,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        await Db.Insertable(campus).ExecuteCommandAsync();

        var orgConfig = new OrgConfig
        {
            TenantId = org.Id, OrgId = org.Id, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        await Db.Insertable(orgConfig).ExecuteCommandAsync();

        var user = await Db.Queryable<SysUser>().InSingleAsync(current.UserId)
            ?? throw new BizException("用户不存在");
        user.Role = (int)RoleEnum.OrgAdmin;
        user.TenantId = org.Id;
        user.OrgId = org.Id;
        user.CampusId = campus.Id;
        user.RealName = req.ContactPerson;
        user.Phone = req.ContactPhone;
        user.UpdatedAt = DateTime.Now;
        await Db.Updateable(user).UpdateColumns(u => new { u.Role, u.TenantId, u.OrgId, u.CampusId, u.RealName, u.Phone, u.UpdatedAt }).ExecuteCommandAsync();

        var binding = new UserOrgBinding
        {
            UserId = current.UserId, TenantId = org.Id, OrgId = org.Id, CampusId = campus.Id,
            Role = (int)RoleEnum.OrgAdmin, Status = 1, BoundVia = 2,
            BoundAt = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        await Db.Insertable(binding).ExecuteCommandAsync();

        return await BuildLoginResponseAsync(user);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest req, CurrentUser current)
    {
        var user = await Db.Queryable<SysUser>().InSingleAsync(current.UserId)
            ?? throw new BizException("用户不存在");
        if (string.IsNullOrEmpty(user.Password) || !BCrypt.Net.BCrypt.Verify(req.OldPassword, user.Password))
            throw new BizException("旧密码不正确");
        user.Password = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        user.PasswordChangedAt = DateTime.Now;
        user.UpdatedAt = DateTime.Now;
        await Db.Updateable(user).UpdateColumns(u => new { u.Password, u.PasswordChangedAt, u.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task<UserInfoDto> GetUserInfoAsync(CurrentUser current)
    {
        var user = await Db.Queryable<SysUser>().InSingleAsync(current.UserId)
            ?? throw new BizException("用户不存在");

        var bindings = await Db.Queryable<UserOrgBinding>()
            .Where(b => b.UserId == current.UserId && b.Status == 1)
            .ToListAsync();

        var orgIds = bindings.Select(b => b.OrgId).Distinct().ToList();
        var orgs = orgIds.Any()
            ? await Db.Queryable<Organization>().Where(o => orgIds.Contains(o.Id) && !o.IsDeleted).ToListAsync()
            : new List<Organization>();

        var orgDict = orgs.ToDictionary(o => o.Id);
        var campuses = bindings.Select(b => b.CampusId).Distinct().ToList();
        var campusList = campuses.Any()
            ? await Db.Queryable<Campus>().Where(c => campuses.Contains(c.Id) && !c.IsDeleted).ToListAsync()
            : new List<Campus>();
        var campusDict = campusList.ToDictionary(c => c.Id);

        return new UserInfoDto
        {
            UserId = user.Id, Phone = user.Phone, RealName = user.RealName,
            NickName = user.NickName, Avatar = user.Avatar,
            Role = user.Role, TenantId = user.TenantId, OrgId = user.OrgId ?? 0,
            Bindings = bindings.Select(b => new UserOrgInfo
            {
                OrgId = b.OrgId, OrgName = orgDict.TryGetValue(b.OrgId, out var o) ? o.Name : "",
                CampusId = b.CampusId, CampusName = campusDict.TryGetValue(b.CampusId, out var c) ? c.Name : "",
                Role = b.Role, UserCode = b.UserCode,
            }).ToList(),
        };
    }

    private async Task<LoginResponse> BuildLoginResponseAsync(SysUser user)
    {
        var current = new CurrentUser
        {
            UserId = user.Id, UserName = user.Phone, Role = user.Role,
            TenantId = user.TenantId, OrgId = user.OrgId ?? 0,
        };
        var token = _jwtHelper.GenerateToken(current);
        var userInfo = await GetUserInfoAsync(current);
        return new LoginResponse { Token = token, UserInfo = userInfo };
    }
}
