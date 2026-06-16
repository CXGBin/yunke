using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class EvaluationTagService : BaseService
{
    public EvaluationTagService(ISqlSugarClient db) : base(db) { }

    public async Task<List<EvaluationTagDto>> GetListAsync(long tenantId)
    {
        var list = await Db.Queryable<EvaluationTag>()
            .Where(t => t.TenantId == tenantId && t.Status == 1)
            .OrderBy(t => t.SortOrder).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<long> CreateAsync(CreateEvaluationTagRequest req, CurrentUser user)
    {
        var tag = new EvaluationTag
        {
            TenantId = user.TenantId, OrgId = user.OrgId,
            Name = req.Name, TagType = req.TagType, SortOrder = req.SortOrder, Status = 1,
            CreatedAt = DateTime.Now,
        };
        return await Db.Insertable(tag).ExecuteReturnBigIdentityAsync();
    }

    public async Task DeleteAsync(long id, long tenantId)
    {
        await Db.Deleteable<EvaluationTag>()
            .Where(t => t.Id == id && t.TenantId == tenantId).ExecuteCommandAsync();
    }

    private static EvaluationTagDto MapToDto(EvaluationTag t) => new()
    {
        Id = t.Id, Name = t.Name, TagType = t.TagType, SortOrder = t.SortOrder,
        Status = t.Status, CreatedAt = t.CreatedAt,
    };
}
