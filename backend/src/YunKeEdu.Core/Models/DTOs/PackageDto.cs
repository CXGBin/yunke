using System.ComponentModel.DataAnnotations;

namespace YunKeEdu.Core.Models.DTOs;

public class CreatePackageRequest
{
    [Required(ErrorMessage = "套餐名称不能为空")]
    [StringLength(50)]
    public string PackageName { get; set; } = string.Empty;

    [Required(ErrorMessage = "套餐编码不能为空")]
    [StringLength(32)]
    public string PackageCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "套餐等级不能为空")]
    [Range(0, 3)]
    public int PackageLevel { get; set; }

    [Required(ErrorMessage = "价格不能为空")]
    [Range(0, 999999)]
    public decimal Price { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public string? Images { get; set; }

    public int MaxCampusCount { get; set; } = 1;
    public int MaxTeacherCount { get; set; } = 5;
    public int MaxStudentCount { get; set; } = 50;
    public int MaxNotificationTypes { get; set; }
    public byte MaxPushChannels { get; set; }
    public string? AnalyticsDimensions { get; set; }
    public bool EnableEvaluation { get; set; }
    public int SortOrder { get; set; }
}

public class UpdatePackageRequest : CreatePackageRequest { }

public class PackageDto
{
    public long Id { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string PackageCode { get; set; } = string.Empty;
    public int PackageLevel { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? Images { get; set; }
    public int MaxCampusCount { get; set; }
    public int MaxTeacherCount { get; set; }
    public int MaxStudentCount { get; set; }
    public int MaxNotificationTypes { get; set; }
    public byte MaxPushChannels { get; set; }
    public string? AnalyticsDimensions { get; set; }
    public bool EnableEvaluation { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public List<PackageFeatureDto> Features { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class PackageFeatureDto
{
    public long Id { get; set; }
    public long PackageId { get; set; }
    public string FeatureCode { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string? FeatureGroup { get; set; }
    public int MinPackageLevel { get; set; }
    public int SortOrder { get; set; }
}

public class AddFeatureRequest
{
    [Required]
    [StringLength(64)]
    public string FeatureCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FeatureName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? FeatureGroup { get; set; }
    public int MinPackageLevel { get; set; }
    public int SortOrder { get; set; }
}

public class PurchaseRequest
{
    [Required]
    public long PackageId { get; set; }

    [Range(0, 999999)]
    public decimal Amount { get; set; }

    public string? PayChannel { get; set; }
    public string? Remark { get; set; }
}

public class RenewRequest
{
    public string? PayChannel { get; set; }
    public string? Remark { get; set; }
}

public class UpgradeRequest
{
    [Required]
    public long NewPackageId { get; set; }

    public string? PayChannel { get; set; }
    public string? Remark { get; set; }
}

public class SubscriptionDto
{
    public long Id { get; set; }
    public long OrgId { get; set; }
    public long PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public int PackageLevel { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public int PayStatus { get; set; }
    public DateTime? PayTime { get; set; }
    public int SubscriptionType { get; set; }
    public long? PreSubscriptionId { get; set; }
    public string? Remark { get; set; }
    public int RemainingDays { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpgradeOrderDto
{
    public long Id { get; set; }
    public long OrgId { get; set; }
    public long OldPackageId { get; set; }
    public string OldPackageName { get; set; } = string.Empty;
    public long NewPackageId { get; set; }
    public string NewPackageName { get; set; } = string.Empty;
    public decimal OldPackagePrice { get; set; }
    public decimal NewPackagePrice { get; set; }
    public int UsedMonths { get; set; }
    public int UnusedMonths { get; set; }
    public decimal OldMonthlyPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PayAmount { get; set; }
    public int PayStatus { get; set; }
    public DateTime? PayTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PackageCompareDto
{
    public List<PackageDto> Packages { get; set; } = new();
    public List<FeatureCompareItem> Features { get; set; } = new();
}

public class FeatureCompareItem
{
    public string FeatureCode { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string? FeatureGroup { get; set; }
    public Dictionary<int, bool> PackageEnabled { get; set; } = new();
}
