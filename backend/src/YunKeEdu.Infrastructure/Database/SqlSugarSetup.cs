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
                // 多租户全局过滤器：自动追加 WHERE TenantId = @CurrentTenantId AND IsDeleted = 0
                db.QueryFilter.AddTableFilter<BaseEntity>(t => t.TenantId == TenantContext.CurrentTenantId && t.IsDeleted == false);
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
