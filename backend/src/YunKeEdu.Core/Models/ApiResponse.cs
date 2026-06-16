// YunKeEdu.Core - 统一响应模型
namespace YunKeEdu.Core.Models;

/// <summary>统一响应结构</summary>
public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = "success";
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T? data = default, string message = "success")
        => new() { Code = 0, Message = message, Data = data };

    public static ApiResponse<T> Fail(int code, string message)
        => new() { Code = code, Message = message, Data = default };

    public static ApiResponse<PagedResult<T>> OkPage(List<T> items, int total, int page, int pageSize)
        => new() { Code = 0, Message = "success", Data = new PagedResult<T>(items, total, page, pageSize) };
}

/// <summary>分页结果</summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public PagedResult(List<T> items, int total, int page, int pageSize)
    {
        Items = items; Total = total; Page = page; PageSize = pageSize;
    }
}
