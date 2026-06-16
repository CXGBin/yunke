using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class ThemeService : BaseService
{
    public ThemeService(ISqlSugarClient db) : base(db) { }

    public async Task<ThemeDto?> GetCurrentThemeAsync(CurrentUser user)
    {
        var org = await Db.Queryable<Organization>().InSingleAsync(user.TenantId);
        if (org?.ThemeConfig == null) return null;
        try { return System.Text.Json.JsonSerializer.Deserialize<ThemeDto>(org.ThemeConfig); }
        catch { return null; }
    }

    public async Task<List<ThemeDto>> GetThemeListAsync()
    {
        return new List<ThemeDto>
        {
            new() { ThemeId = "blue", ThemeName = "蓝色科技风", PrimaryColor = "#1890ff", SecondaryColor = "#40a9ff", ButtonColor = "#1890ff", BackgroundColor = "#f0f5ff" },
            new() { ThemeId = "green", ThemeName = "绿色清新风", PrimaryColor = "#52c41a", SecondaryColor = "#73d13d", ButtonColor = "#52c41a", BackgroundColor = "#f6ffed" },
            new() { ThemeId = "purple", ThemeName = "紫色优雅风", PrimaryColor = "#722ed1", SecondaryColor = "#9254de", ButtonColor = "#722ed1", BackgroundColor = "#f9f0ff" },
            new() { ThemeId = "orange", ThemeName = "橙色活力风", PrimaryColor = "#fa8c16", SecondaryColor = "#ffa940", ButtonColor = "#fa8c16", BackgroundColor = "#fff7e6" },
        };
    }

    public async Task UpdateOrgThemeAsync(UpdateOrgThemeRequest req, CurrentUser user)
    {
        var org = await Db.Queryable<Organization>().InSingleAsync(user.TenantId)
            ?? throw new BizException("机构不存在");
        org.ThemeConfig = req.ThemeConfig; org.UpdatedAt = DateTime.Now;
        await Db.Updateable(org).UpdateColumns(o => new { o.ThemeConfig, o.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task SwitchThemeAsync(SwitchThemeRequest req, CurrentUser user)
    {
        var themes = await GetThemeListAsync();
        var theme = themes.FirstOrDefault(t => t.ThemeId == req.ThemeId) ?? throw new BizException("主题不存在");
        var json = System.Text.Json.JsonSerializer.Serialize(theme);
        var org = await Db.Queryable<Organization>().InSingleAsync(user.TenantId)
            ?? throw new BizException("机构不存在");
        org.ThemeConfig = json; org.UpdatedAt = DateTime.Now;
        await Db.Updateable(org).UpdateColumns(o => new { o.ThemeConfig, o.UpdatedAt }).ExecuteCommandAsync();
    }
}
