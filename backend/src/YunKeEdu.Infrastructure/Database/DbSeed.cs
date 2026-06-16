using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Enums;

namespace YunKeEdu.Infrastructure.Database;

public static class DbSeed
{
    public static async Task SeedAsync(ISqlSugarClient db)
    {
        // 种子：4级套餐
        if (!await db.Queryable<OrgPackage>().AnyAsync())
        {
            var now = DateTime.Now;
            var packages = new List<OrgPackage>
            {
                new() { PackageName="Plus", PackageCode="PLUS", PackageLevel=0, Price=2999, MaxCampusCount=1, MaxTeacherCount=5, MaxStudentCount=50, MaxNotificationTypes=3, MaxPushChannels=1, AnalyticsDimensions="basic", SortOrder=1, Status=1, CreatedAt=now, UpdatedAt=now },
                new() { PackageName="Pro", PackageCode="PRO", PackageLevel=1, Price=9999, MaxCampusCount=3, MaxTeacherCount=20, MaxStudentCount=200, MaxNotificationTypes=6, MaxPushChannels=2, AnalyticsDimensions="standard", SortOrder=2, Status=1, CreatedAt=now, UpdatedAt=now },
                new() { PackageName="Ultra", PackageCode="ULTRA", PackageLevel=2, Price=29999, MaxCampusCount=10, MaxTeacherCount=100, MaxStudentCount=1000, MaxNotificationTypes=12, MaxPushChannels=3, AnalyticsDimensions="advanced", SortOrder=3, Status=1, CreatedAt=now, UpdatedAt=now },
                new() { PackageName="Ultimate", PackageCode="ULTIMATE", PackageLevel=3, Price=99999, MaxCampusCount=int.MaxValue, MaxTeacherCount=int.MaxValue, MaxStudentCount=int.MaxValue, MaxNotificationTypes=12, MaxPushChannels=3, AnalyticsDimensions="full", SortOrder=4, Status=1, CreatedAt=now, UpdatedAt=now },
            };
            await db.Insertable(packages).ExecuteCommandAsync();
        }

        // 种子：平台管理员账号
        if (!await db.Queryable<SysUser>().AnyAsync(u => u.Role == (int)RoleEnum.PlatformAdmin))
        {
            var admin = new SysUser
            {
                TenantId = 0, OrgId = null, UserName = "admin", RealName = "平台管理员",
                Phone = "13000000000", Role = (int)RoleEnum.PlatformAdmin, Status = 1,
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
            };
            await db.Insertable(admin).ExecuteCommandAsync();
        }
    }
}
