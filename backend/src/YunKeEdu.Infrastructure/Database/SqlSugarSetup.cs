// YunKeEdu.Infrastructure - 数据库/SqlSugar配置
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using YunKeEdu.Core.Entities;

namespace YunKeEdu.Infrastructure.Database;

/// <summary>
/// SqlSugar数据库配置（连接串 + 多租户全局过滤器）
/// </summary>
public static class SqlSugarSetup
{
    /// <summary>
    /// 注册SqlSugar为Scope
    /// </summary>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("数据库连接串未配置");

        services.AddScoped<ISqlSugarClient>(provider =>
        {
            var sqlSugar = new SqlSugarScope(new ConnectionConfig
            {
                DbType = DbType.SqlServer,
                ConnectionString = connectionString,
                IsAutoCloseConnection = true,
            },
            db =>
            {
                // 多租户全局过滤器：TenantId<=0(平台管理员)时跳过
                db.QueryFilter.AddTableFilter<BaseEntity>(t => TenantContext.CurrentTenantId <= 0 || (t.TenantId == TenantContext.CurrentTenantId && t.IsDeleted == false));
                db.QueryFilter.AddTableFilter<Attendance>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<CourseAttachment>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<CourseEnrollment>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<CourseFeeSettlement>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<CoursePackageItem>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<EvaluationReply>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<EvaluationTag>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<FeeSettlementRecord>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<LeaveRequest>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<NotificationConfig>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<NotificationLog>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<NotificationTemplate>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<OrgConfig>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<OrgSubscription>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<PackageUpgradeOrder>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<ScheduleChangeLog>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<ScheduleRecurrence>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<SignInQRCode>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<StatisticsCourseSnapshot>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<StatisticsDailySnapshot>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<TeacherWallet>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<UserOrgBinding>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
                db.QueryFilter.AddTableFilter<WaitList>(t => TenantContext.CurrentTenantId <= 0 || t.TenantId == TenantContext.CurrentTenantId);
            });

            return sqlSugar;
        });

        return services;
    }
}

/// <summary>租户上下文（供SqlSugar全局过滤器使用）</summary>
public static class TenantContext
{
    private static readonly AsyncLocal<long> _tenantId = new();

    /// <summary>当前租户ID</summary>
    public static long CurrentTenantId
    {
        get => _tenantId.Value;
        set => _tenantId.Value = value;
    }
}
