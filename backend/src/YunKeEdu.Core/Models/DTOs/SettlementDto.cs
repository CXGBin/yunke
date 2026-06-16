using System.ComponentModel.DataAnnotations;

namespace YunKeEdu.Core.Models.DTOs;

public class SettlementRuleDto
{
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SettlementType { get; set; }
    public decimal FixedAmount { get; set; }
    public decimal OriginalPrice { get; set; }
    public int TotalLessons { get; set; }
}

public class WalletDto
{
    public long TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public DateTime? LastSettlementAt { get; set; }
}

public class WalletDetailDto
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public long ScheduleId { get; set; }
    public DateTime SettlementDate { get; set; }
    public decimal Amount { get; set; }
    public int SettlementType { get; set; }
    public int StudentCount { get; set; }
    public int TriggerType { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FeeSettlementRecordDto
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public long ScheduleId { get; set; }
    public long TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int SettlementType { get; set; }
    public int StudentCount { get; set; }
    public DateTime SettlementDate { get; set; }
    public DateTime SettledAt { get; set; }
    public int TriggerType { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ManualTriggerRequest
{
    [Required]
    public long ScheduleId { get; set; }

    public string? Remark { get; set; }
}

public class SettlementSummaryDto
{
    public long TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int TotalSettlements { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal MonthlyAmount { get; set; }
}

public class SettlementExportDto
{
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public long TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int Lessons { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal MonthlyAmount { get; set; }
}
