using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>套餐定义表</summary>
[SugarTable("OrgPackage")]
public class OrgPackage
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    [SugarColumn(Length = 50, IsNullable = false)]
    public string PackageName { get; set; } = string.Empty;
    [SugarColumn(Length = 32, IsNullable = false)]
    public string PackageCode { get; set; } = string.Empty;
    public int PackageLevel { get; set; }
    public decimal Price { get; set; }
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }
    [SugarColumn(Length = 2000, IsNullable = true)]
    public string? Images { get; set; }
    public int MaxCampusCount { get; set; } = 1;
    public int MaxTeacherCount { get; set; } = 5;
    public int MaxStudentCount { get; set; } = 50;
    public int MaxNotificationTypes { get; set; }
    public byte MaxPushChannels { get; set; }
    [SugarColumn(Length = 500, IsNullable = true, DefaultValue = "basic")]
    public string? AnalyticsDimensions { get; set; } = "basic";
    /// <summary>评价功能是否可用（Ultra/Ultimate可用）</summary>
    public bool EnableEvaluation { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; }
}

/// <summary>套餐功能关联表</summary>
[SugarTable("OrgPackageFeature")]
public class OrgPackageFeature
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long PackageId { get; set; }
    [SugarColumn(Length = 64, IsNullable = false)]
    public string FeatureCode { get; set; } = string.Empty;
    [SugarColumn(Length = 100, IsNullable = false)]
    public string FeatureName { get; set; } = string.Empty;
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? FeatureGroup { get; set; }
    public int MinPackageLevel { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>机构订阅记录表</summary>
[SugarTable("OrgSubscription")]
public class OrgSubscription
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long PackageId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public int PayStatus { get; set; }
    public DateTime? PayTime { get; set; }
    [SugarColumn(Length = 32, IsNullable = true)]
    public string? PayChannel { get; set; }
    public int SubscriptionType { get; set; }
    public long? PreSubscriptionId { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>套餐升级订单表</summary>
[SugarTable("PackageUpgradeOrder")]
public class PackageUpgradeOrder
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long OldSubscriptionId { get; set; }
    public long OldPackageId { get; set; }
    public long NewPackageId { get; set; }
    public long? NewSubscriptionId { get; set; }
    public decimal OldPackagePrice { get; set; }
    public decimal NewPackagePrice { get; set; }
    public int UsedMonths { get; set; }
    public int UnusedMonths { get; set; }
    public decimal OldMonthlyPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PayAmount { get; set; }
    public int PayStatus { get; set; }
    public DateTime? PayTime { get; set; }
    [SugarColumn(Length = 32, IsNullable = true)]
    public string? PayChannel { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
