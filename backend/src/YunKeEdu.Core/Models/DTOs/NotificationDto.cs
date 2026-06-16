using System.ComponentModel.DataAnnotations;

namespace YunKeEdu.Core.Models.DTOs;

public class CreateNotificationTemplateRequest
{
    [Required]
    [StringLength(100)]
    public string TemplateName { get; set; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string TemplateCode { get; set; } = string.Empty;

    [Required]
    public int NotifyType { get; set; }

    [Required]
    public int Channel { get; set; }

    [Required]
    [StringLength(200)]
    public string TitleTemplate { get; set; } = string.Empty;

    [Required]
    public string ContentTemplate { get; set; } = string.Empty;

    public string? Variables { get; set; }
}

public class UpdateNotificationTemplateRequest : CreateNotificationTemplateRequest { }

public class NotificationTemplateDto
{
    public long Id { get; set; }
    public long? OrgId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string TemplateCode { get; set; } = string.Empty;
    public int NotifyType { get; set; }
    public int Channel { get; set; }
    public string TitleTemplate { get; set; } = string.Empty;
    public string ContentTemplate { get; set; } = string.Empty;
    public string? Variables { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateNotificationConfigRequest
{
    public string? NotifyTypes { get; set; }
    public int ReminderMinutes { get; set; } = 30;
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }
    public bool IsQuietEnabled { get; set; }
}

public class NotificationConfigDto
{
    public long Id { get; set; }
    public string? NotifyTypes { get; set; }
    public int ReminderMinutes { get; set; }
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }
    public bool IsQuietEnabled { get; set; }
}

public class NotificationLogDto
{
    public long Id { get; set; }
    public long RecipientId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public int NotifyType { get; set; }
    public int Channel { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? RelateType { get; set; }
    public long? RelateId { get; set; }
    public int SendStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SendNotificationRequest
{
    [Required]
    public long RecipientId { get; set; }

    [Required]
    public int NotifyType { get; set; }

    [Required]
    public int Channel { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? RelateType { get; set; }
    public long? RelateId { get; set; }
}
