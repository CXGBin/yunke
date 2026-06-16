using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class ConfigService : BaseService
{
    public ConfigService(ISqlSugarClient db) : base(db) { }

    public async Task<OrgConfigDto> GetOrgConfigAsync(CurrentUser user)
    {
        var config = await Db.Queryable<OrgConfig>()
            .Where(c => c.TenantId == user.TenantId).FirstAsync();
        if (config == null) throw new BizException("机构配置不存在");
        return MapToDto(config);
    }

    public async Task UpdateOrgConfigAsync(UpdateOrgConfigRequest req, CurrentUser user)
    {
        var config = await Db.Queryable<OrgConfig>()
            .Where(c => c.TenantId == user.TenantId).FirstAsync()
            ?? throw new BizException("机构配置不存在");
        config.FreeRefundDays = req.FreeRefundDays;
        if (req.SignInMethods != null) config.SignInMethods = req.SignInMethods;
        config.AttendanceTimeout = req.AttendanceTimeout;
        config.EnableLeaveApproval = req.EnableLeaveApproval;
        config.EnableTeacherPreReview = req.EnableTeacherPreReview;
        config.WaitlistExpireHours = req.WaitlistExpireHours;
        config.MaxStudentsPerParent = req.MaxStudentsPerParent;
        config.MaxParentsPerStudent = req.MaxParentsPerStudent;
        config.MaxCoursesPerStudent = req.MaxCoursesPerStudent;
        config.InvitationExpireDays = req.InvitationExpireDays;
        config.UpdatedAt = DateTime.Now;
        await Db.Updateable(config).ExecuteCommandAsync();
    }

    public async Task<List<SysConfigDto>> GetSysConfigAsync(string? group = null)
    {
        var query = Db.Queryable<SysConfig>();
        if (!string.IsNullOrWhiteSpace(group))
            query = query.Where(c => c.ConfigGroup == group);
        var list = await query.OrderBy(c => c.Id).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task UpdateSysConfigAsync(UpdateSysConfigRequest req)
    {
        var config = await Db.Queryable<SysConfig>()
            .Where(c => c.ConfigKey == req.ConfigKey).FirstAsync();
        if (config == null)
        {
            config = new SysConfig
            {
                ConfigKey = req.ConfigKey, ConfigValue = req.ConfigValue,
                ConfigGroup = req.ConfigGroup, Description = req.Description,
            };
            await Db.Insertable(config).ExecuteCommandAsync();
        }
        else
        {
            config.ConfigValue = req.ConfigValue;
            config.ConfigGroup = req.ConfigGroup;
            config.Description = req.Description;
            config.UpdatedAt = DateTime.Now;
            await Db.Updateable(config).ExecuteCommandAsync();
        }
    }

    private static OrgConfigDto MapToDto(OrgConfig c) => new()
    {
        Id = c.Id, FreeRefundDays = c.FreeRefundDays, SignInMethods = c.SignInMethods,
        AttendanceTimeout = c.AttendanceTimeout, EnableEvaluationReview = c.EnableEvaluationReview,
        EnableLeaveApproval = c.EnableLeaveApproval, EnableTeacherPreReview = c.EnableTeacherPreReview,
        WaitlistExpireHours = c.WaitlistExpireHours, MaxStudentsPerParent = c.MaxStudentsPerParent,
        MaxParentsPerStudent = c.MaxParentsPerStudent, MaxCoursesPerStudent = c.MaxCoursesPerStudent,
        InvitationExpireDays = c.InvitationExpireDays, ThemeConfig = c.ThemeConfig,
    };

    private static SysConfigDto MapToDto(SysConfig c) => new()
    {
        Id = c.Id, ConfigKey = c.ConfigKey, ConfigValue = c.ConfigValue,
        ConfigGroup = c.ConfigGroup, Description = c.Description, UpdatedAt = c.UpdatedAt,
    };
}
