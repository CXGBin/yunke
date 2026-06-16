using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/notification")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _service;
    public NotificationController(NotificationService service) => _service = service;

    [HttpGet("template/page")]
    public async Task<ApiResponse<PagedResult<NotificationTemplateDto>>> TemplatePage([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<NotificationTemplateDto>>.Ok(await _service.GetTemplatePageAsync(req, GetUser().Role == 1 ? null : GetUser().TenantId));

    [HttpPost("template")]
    public async Task<ApiResponse<long>> CreateTemplate([FromBody] CreateNotificationTemplateRequest req)
        => ApiResponse<long>.Ok(await _service.CreateTemplateAsync(req, GetUser()));

    [HttpPut("template/{id}")]
    public async Task<ApiResponse<bool>> UpdateTemplate(long id, [FromBody] UpdateNotificationTemplateRequest req)
    {
        await _service.UpdateTemplateAsync(id, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("template/{id}")]
    public async Task<ApiResponse<bool>> DeleteTemplate(long id)
    {
        await _service.DeleteTemplateAsync(id);
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("config")]
    public async Task<ApiResponse<NotificationConfigDto>> GetConfig()
        => ApiResponse<NotificationConfigDto>.Ok(await _service.GetConfigAsync(GetUser()));

    [HttpPut("config")]
    public async Task<ApiResponse<bool>> UpdateConfig([FromBody] UpdateNotificationConfigRequest req)
    {
        await _service.UpdateConfigAsync(req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("my-list")]
    public async Task<ApiResponse<PagedResult<NotificationLogDto>>> MyList([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<NotificationLogDto>>.Ok(await _service.GetMyListAsync(req, GetUser()));

    [HttpGet("my-unread-count")]
    public async Task<ApiResponse<int>> UnreadCount()
        => ApiResponse<int>.Ok(await _service.GetUnreadCountAsync(GetUser()));

    [HttpPut("{id}/read")]
    public async Task<ApiResponse<bool>> MarkRead(long id)
    {
        await _service.MarkReadAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("read-all")]
    public async Task<ApiResponse<bool>> ReadAll()
    {
        await _service.MarkAllReadAsync(GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("send")]
    public async Task<ApiResponse<long>> Send([FromBody] SendNotificationRequest req)
        => ApiResponse<long>.Ok(await _service.SendAsync(req, GetUser()));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
