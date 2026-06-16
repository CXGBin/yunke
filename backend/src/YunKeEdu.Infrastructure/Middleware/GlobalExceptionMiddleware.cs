using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;

namespace YunKeEdu.Infrastructure.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (BizException ex)
        {
            _logger.LogWarning("业务异常: Code={Code}, Message={Message}", ex.Code, ex.Message);
            await WriteResponse(context, ex.Code, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("未授权: {Message}", ex.Message);
            await WriteResponse(context, 401, "未授权，请先登录");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "系统异常: {Message}", ex.Message);
            await WriteResponse(context, 500, "服务器内部错误");
        }
    }

    private static async Task WriteResponse(HttpContext context, int code, string message)
    {
        context.Response.StatusCode = code <= 0 ? 200 : (code >= 400 ? code : 500);
        context.Response.ContentType = "application/json; charset=utf-8";
        var json = JsonSerializer.Serialize(ApiResponse<object>.Fail(code, message),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalException(this IApplicationBuilder builder)
        => builder.UseMiddleware<GlobalExceptionMiddleware>();
}
