using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>通知模板表</summary>
[SugarTable("NotificationTemplate")]
public class NotificationTemplate
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? OrgId { get; set; }
    [SugarColumn(Length = 100, IsNullable = false)]
    public string TemplateName { get; set; } = string.Empty;
    [SugarColumn(Length = 64, IsNullable = false)]
    public string TemplateCode { get; set; } = string.Empty;
    public int NotifyType { get; set; }
    public int Channel { get; set; }
    [SugarColumn(Length = 200, IsNullable = false)]
    public string TitleTemplate { get; set; } = string.Empty;
    [SugarColumn(Length = -1, IsNullable = false, ColumnDataType = "nvarchar(max)")]
    public string ContentTemplate { get; set; } = string.Empty;
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Variables { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>通知日志表</summary>
[SugarTable("NotificationLog")]
public class NotificationLog
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long RecipientId { get; set; }
    public int NotifyType { get; set; }
    public int Channel { get; set; }
    [SugarColumn(Length = 200, IsNullable = false)]
    public string Title { get; set; } = string.Empty;
    [SugarColumn(Length = -1, IsNullable = false, ColumnDataType = "nvarchar(max)")]
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    [SugarColumn(Length = 32, IsNullable = true)]
    public string? RelateType { get; set; }
    public long? RelateId { get; set; }
    public int SendStatus { get; set; }
    public DateTime? SendTime { get; set; }
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>通知配置表</summary>
[SugarTable("NotificationConfig")]
public class NotificationConfig
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? NotifyTypes { get; set; }
    public int ReminderMinutes { get; set; } = 30;
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }
    public bool IsQuietEnabled { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
