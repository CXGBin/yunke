using Microsoft.AspNetCore.Http;
using SqlSugar;
using YunKeEdu.Core.Models;

namespace YunKeEdu.Application.Services;

public abstract class BaseService
{
    protected readonly ISqlSugarClient Db;
    protected BaseService(ISqlSugarClient db) => Db = db;

    protected CurrentUser? GetCurrentUser(HttpContext context)
    {
        if (context.Items.TryGetValue("CurrentUser", out var user) && user is CurrentUser currentUser)
            return currentUser;
        return null;
    }

    protected async Task<PagedResult<T>> GetPagedAsync<T>(ISugarQueryable<T> query, int page, int pageSize) where T : class, new()
    {
        RefAsync<int> total = 0;
        var items = await query.ToPageListAsync(page, pageSize, total);
        return new PagedResult<T>(items, total, page, pageSize);
    }
}
