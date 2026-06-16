// YunKeEdu.Api - Program.cs 入口文件
using Hangfire;
using SqlSugar;
using YunKeEdu.Application.Services;
using YunKeEdu.Core.Entities;
using YunKeEdu.Infrastructure.Database;
using YunKeEdu.Infrastructure.Hangfire;
using YunKeEdu.Infrastructure.JWT;
using YunKeEdu.Infrastructure.Middleware;
using YunKeEdu.Infrastructure.Redis;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. 注册服务
// ============================================================

// SqlSugar ORM（含多租户全局过滤器）
builder.Services.AddSqlSugar(builder.Configuration);

// Redis缓存
builder.Services.AddRedis(builder.Configuration);

// JWT认证
builder.Services.AddJwtAuthentication(builder.Configuration);

// Hangfire定时任务
builder.Services.AddHangfireService(builder.Configuration);

// 控制器（注册HttpContextAccessor）
builder.Services.AddHttpContextAccessor();

// 注册所有Service
var serviceAssembly = typeof(BaseService).Assembly;
var serviceTypes = serviceAssembly.GetTypes()
    .Where(t => !t.IsAbstract && !t.IsGenericType && t.IsClass && t.Name.EndsWith("Service") && t != typeof(BaseService));
foreach (var st in serviceTypes)
{
    var interfaces = st.GetInterfaces().Where(i => i.Name.EndsWith("Service"));
    if (interfaces.Any())
        foreach (var i in interfaces) builder.Services.AddScoped(i, st);
    else
        builder.Services.AddScoped(st);
}

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = false;
    });

// Swagger/OpenAPI文档
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "云科智教 API",
        Version = "v1",
        Description = "云科智教教育SaaS课程管理平台后端接口文档"
    });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT授权，格式：Bearer {token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// 跨域配置
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// ============================================================
// 2. 启动时自动建表 + 种子数据
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
    // CodeFirst建表（数据库已通过init.sql创建，启动时不再自动迁移）
    // db.CodeFirst.InitTables(/* 已通过scripts/init.sql手动创建 */);
    // 种子数据
    await DbSeed.SeedAsync(db);
}

// ============================================================
// 3. 配置中间件管道
// ============================================================

app.UseGlobalException();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "云科智教 API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();
app.MapControllers();

app.Run();

public partial class Program { }
