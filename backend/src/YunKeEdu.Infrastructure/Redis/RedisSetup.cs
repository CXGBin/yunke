// YunKeEdu.Infrastructure - Redis配置
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace YunKeEdu.Infrastructure.Redis;

/// <summary>
/// Redis连接配置与注册
/// </summary>
public static class RedisSetup
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(connectionString));
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        return services;
    }
}

/// <summary>Redis缓存服务接口</summary>
public interface IRedisCacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}

/// <summary>Redis缓存服务实现</summary>
public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis) => _db = redis.GetDatabase();

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }
        catch (Exception ex) { Console.WriteLine($"Redis SetAsync 异常: {ex.Message}"); }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var json = await _db.StringGetAsync(key);
            return json.IsNullOrEmpty ? default : System.Text.Json.JsonSerializer.Deserialize<T>(json!);
        }
        catch (Exception ex) { Console.WriteLine($"Redis GetAsync 异常: {ex.Message}"); return default; }
    }

    public async Task RemoveAsync(string key)
    {
        try { await _db.KeyDeleteAsync(key); }
        catch (Exception ex) { Console.WriteLine($"Redis RemoveAsync 异常: {ex.Message}"); }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try { return await _db.KeyExistsAsync(key); }
        catch (Exception ex) { Console.WriteLine($"Redis ExistsAsync 异常: {ex.Message}"); return false; }
    }
}
