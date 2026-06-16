using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using YunKeEdu.Core.Models;

namespace YunKeEdu.Infrastructure.JWT;

public static class JwtSetup
{
    public const string SectionName = "Jwt";

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = configuration.GetSection(SectionName).Get<JwtConfig>()
            ?? throw new InvalidOperationException("JWT配置缺失");

        services.AddSingleton(jwtConfig);
        services.AddSingleton<IJwtHelper, JwtHelper>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var claims = context.Principal?.Claims;
                    if (claims != null)
                    {
                        var userId = long.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                        var userName = claims.First(c => c.Type == ClaimTypes.Name).Value;
                        var role = int.Parse(claims.First(c => c.Type == ClaimTypes.Role).Value);
                        var tenantId = long.Parse(claims.First(c => c.Type == "TenantId").Value);
                        var orgId = long.Parse(claims.First(c => c.Type == "OrgId").Value);

                        context.HttpContext.Items["CurrentUser"] = new CurrentUser
                        {
                            UserId = userId, UserName = userName, Role = role,
                            TenantId = tenantId, OrgId = orgId
                        };

                        Database.TenantContext.CurrentTenantId = tenantId;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}

public class JwtConfig
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "YunKeEdu";
    public string Audience { get; set; } = "YunKeEdu";
    public int ExpireMinutes { get; set; } = 1440;
}

public interface IJwtHelper
{
    string GenerateToken(CurrentUser user);
}

public class JwtHelper : IJwtHelper
{
    private readonly JwtConfig _config;

    public JwtHelper(JwtConfig config) => _config = config;

    public string GenerateToken(CurrentUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("TenantId", user.TenantId.ToString()),
            new Claim("OrgId", user.OrgId.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _config.Issuer, audience: _config.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_config.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
