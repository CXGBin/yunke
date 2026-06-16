using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>结算规则表（冗余记录）</summary>
[SugarTable("CourseFeeSettlement")]
public class CourseFeeSettlement
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long CourseId { get; set; }
    public int SettlementType { get; set; }
    public decimal FixedAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>教师钱包/余额表</summary>
[SugarTable("TeacherWallet")]
public class TeacherWallet
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long TeacherId { get; set; }
    public decimal Balance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public DateTime? LastSettlementAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>结算记录表</summary>
[SugarTable("FeeSettlementRecord")]
public class FeeSettlementRecord
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
    public long CourseId { get; set; }
    public long ScheduleId { get; set; }
    public long TeacherId { get; set; }
    public long WalletId { get; set; }
    public decimal Amount { get; set; }
    public int SettlementType { get; set; }
    public int StudentCount { get; set; }
    public DateTime SettlementDate { get; set; }
    public DateTime SettledAt { get; set; } = DateTime.Now;
    public int TriggerType { get; set; }
    public int Status { get; set; } = 1;
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
