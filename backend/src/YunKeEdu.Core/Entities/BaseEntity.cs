// YunKeEdu.Core - 实体基类（所有业务表共用字段）
using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>
/// 实体基类（包含Id, TenantId, IsDeleted, CreatedAt, UpdatedAt）
/// </summary>
public abstract class BaseEntity
{
    /// <summary>主键</summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>租户ID（机构ID）</summary>
    public long TenantId { get; set; }

    /// <summary>软删除标记</summary>
    public bool IsDeleted { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>更新时间</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
