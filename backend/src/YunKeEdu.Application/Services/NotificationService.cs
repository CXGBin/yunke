using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class NotificationService : BaseService
{
    public NotificationService(ISqlSugarClient db) : base(db) { }

    public async Task<PagedResult<NotificationTemplateDto>> GetTemplatePageAsync(PageRequest req, long? tenantId = null)
    {
        var query = Db.Queryable<NotificationTemplate>().Where(t => true);
        if (tenantId.HasValue) query = query.Where(t => t.TenantId == tenantId.Value);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where(t => t.TemplateName.Contains(req.Keyword!) || t.TemplateCode.Contains(req.Keyword!));
        query = query.OrderBy(t => t.Id, OrderByType.Desc);
        RefAsync<int> total = 0;
        var list = await query.Select(t => new NotificationTemplateDto
        {
            Id = t.Id, OrgId = t.OrgId, TemplateName = t.TemplateName, TemplateCode = t.TemplateCode,
            NotifyType = t.NotifyType, Channel = t.Channel, TitleTemplate = t.TitleTemplate,
            ContentTemplate = t.ContentTemplate, Variables = t.Variables, Status = t.Status, CreatedAt = t.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<NotificationTemplateDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<long> CreateTemplateAsync(CreateNotificationTemplateRequest req, CurrentUser user)
    {
        var template = new NotificationTemplate
        {
            TenantId = user.TenantId, OrgId = user.OrgId,
            TemplateName = req.TemplateName, TemplateCode = req.TemplateCode,
            NotifyType = req.NotifyType, Channel = req.Channel,
            TitleTemplate = req.TitleTemplate, ContentTemplate = req.ContentTemplate,
            Variables = req.Variables, Status = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        return await Db.Insertable(template).ExecuteReturnBigIdentityAsync();
    }

    public async Task UpdateTemplateAsync(long id, UpdateNotificationTemplateRequest req, CurrentUser user)
    {
        var template = await Db.Queryable<NotificationTemplate>().InSingleAsync(id)
            ?? throw new BizException("模板不存在");
        template.TemplateName = req.TemplateName; template.TemplateCode = req.TemplateCode;
        template.NotifyType = req.NotifyType; template.Channel = req.Channel;
        template.TitleTemplate = req.TitleTemplate; template.ContentTemplate = req.ContentTemplate;
        template.Variables = req.Variables; template.UpdatedAt = DateTime.Now;
        await Db.Updateable(template).ExecuteCommandAsync();
    }

    public async Task DeleteTemplateAsync(long id)
    {
        await Db.Deleteable<NotificationTemplate>().Where(t => t.Id == id).ExecuteCommandAsync();
    }

    public async Task<NotificationConfigDto> GetConfigAsync(CurrentUser user)
    {
        var config = await Db.Queryable<NotificationConfig>()
            .Where(c => c.TenantId == user.TenantId).FirstAsync();
        if (config == null)
        {
            config = new NotificationConfig { TenantId = user.TenantId, OrgId = user.OrgId, ReminderMinutes = 30, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            config.Id = await Db.Insertable(config).ExecuteReturnBigIdentityAsync();
        }
        return new NotificationConfigDto
        {
            Id = config.Id, NotifyTypes = config.NotifyTypes, ReminderMinutes = config.ReminderMinutes,
            QuietHoursStart = config.QuietHoursStart, QuietHoursEnd = config.QuietHoursEnd,
            IsQuietEnabled = config.IsQuietEnabled,
        };
    }

    public async Task UpdateConfigAsync(UpdateNotificationConfigRequest req, CurrentUser user)
    {
        var config = await Db.Queryable<NotificationConfig>()
            .Where(c => c.TenantId == user.TenantId).FirstAsync()
            ?? throw new BizException("通知配置不存在");
        if (req.NotifyTypes != null) config.NotifyTypes = req.NotifyTypes;
        config.ReminderMinutes = req.ReminderMinutes;
        config.QuietHoursStart = req.QuietHoursStart;
        config.QuietHoursEnd = req.QuietHoursEnd;
        config.IsQuietEnabled = req.IsQuietEnabled;
        config.UpdatedAt = DateTime.Now;
        await Db.Updateable(config).ExecuteCommandAsync();
    }

    public async Task<PagedResult<NotificationLogDto>> GetMyListAsync(PageRequest req, CurrentUser user)
    {
        var query = Db.Queryable<NotificationLog>()
            .LeftJoin<SysUser>((l, u) => l.RecipientId == u.Id)
            .Where((l, u) => l.RecipientId == user.UserId)
            .OrderByDescending((l, u) => l.CreatedAt);
        RefAsync<int> total = 0;
        var list = await query.Select((l, u) => new NotificationLogDto
        {
            Id = l.Id, RecipientId = l.RecipientId, RecipientName = u.RealName ?? u.NickName ?? "",
            NotifyType = l.NotifyType, Channel = l.Channel, Title = l.Title, Content = l.Content,
            IsRead = l.IsRead, ReadAt = l.ReadAt, RelateType = l.RelateType, RelateId = l.RelateId,
            SendStatus = l.SendStatus, CreatedAt = l.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<NotificationLogDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<int> GetUnreadCountAsync(CurrentUser user)
    {
        return await Db.Queryable<NotificationLog>()
            .Where(l => l.RecipientId == user.UserId && l.IsRead == false).CountAsync();
    }

    public async Task MarkReadAsync(long id, CurrentUser user)
    {
        var log = await Db.Queryable<NotificationLog>()
            .Where(l => l.Id == id && l.RecipientId == user.UserId).FirstAsync()
            ?? throw new BizException("消息不存在");
        log.IsRead = true; log.ReadAt = DateTime.Now;
        await Db.Updateable(log).UpdateColumns(l => new { l.IsRead, l.ReadAt }).ExecuteCommandAsync();
    }

    public async Task MarkAllReadAsync(CurrentUser user)
    {
        await Db.Updateable<NotificationLog>()
            .SetColumns(l => new NotificationLog { IsRead = true, ReadAt = DateTime.Now })
            .Where(l => l.RecipientId == user.UserId && l.IsRead == false)
            .ExecuteCommandAsync();
    }

    public async Task<long> SendAsync(SendNotificationRequest req, CurrentUser user)
    {
        var log = new NotificationLog
        {
            TenantId = user.TenantId, OrgId = user.OrgId, RecipientId = req.RecipientId,
            NotifyType = req.NotifyType, Channel = req.Channel, Title = req.Title,
            Content = req.Content, RelateType = req.RelateType, RelateId = req.RelateId,
            SendStatus = 1, SendTime = DateTime.Now, CreatedAt = DateTime.Now,
        };
        return await Db.Insertable(log).ExecuteReturnBigIdentityAsync();
    }
}
