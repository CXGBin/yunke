using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class LessonUnitService : BaseService
{
    public LessonUnitService(ISqlSugarClient db) : base(db) { }

    public async Task<List<LessonUnitDto>> GetByCourseAsync(long courseId, CurrentUser user)
    {
        return await Db.Queryable<LessonUnit>()
            .Where(l => l.CourseId == courseId && l.TenantId == user.TenantId && !l.IsDeleted)
            .OrderBy(l => l.LessonNo).Select(l => new LessonUnitDto
            {
                Id = l.Id, CourseId = l.CourseId, LessonNo = l.LessonNo,
                Title = l.Title, Description = l.Description,
                SortOrder = l.SortOrder, Status = l.Status,
            }).ToListAsync();
    }

    public async Task BatchGenerateAsync(long courseId, BatchGenerateLessonRequest req, CurrentUser user)
    {
        var course = await Db.Queryable<Course>()
            .Where(c => c.Id == courseId && c.TenantId == user.TenantId && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("课程不存在");

        var prefix = req.TitlePrefix ?? course.Name;
        var units = new List<LessonUnit>();
        for (int i = 0; i < req.Count; i++)
        {
            units.Add(new LessonUnit
            {
                TenantId = user.TenantId, OrgId = user.OrgId, CourseId = courseId,
                LessonNo = req.StartNo + i, Title = $"{prefix} 第{req.StartNo + i}课",
                SortOrder = i, Status = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
            });
        }
        await Db.Insertable(units).ExecuteCommandAsync();
    }

    public async Task UpdateAsync(long id, UpdateLessonUnitRequest req, CurrentUser user)
    {
        var unit = await Db.Queryable<LessonUnit>()
            .Where(l => l.Id == id && l.TenantId == user.TenantId && !l.IsDeleted).FirstAsync()
            ?? throw new BizException("课节不存在");
        unit.LessonNo = req.LessonNo; unit.Title = req.Title;
        unit.Description = req.Description; unit.SortOrder = req.SortOrder;
        unit.UpdatedAt = DateTime.Now;
        await Db.Updateable(unit).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(long id, CurrentUser user)
    {
        var unit = await Db.Queryable<LessonUnit>()
            .Where(l => l.Id == id && l.TenantId == user.TenantId).FirstAsync()
            ?? throw new BizException("课节不存在");
        unit.IsDeleted = true; unit.UpdatedAt = DateTime.Now;
        await Db.Updateable(unit).UpdateColumns(l => new { l.IsDeleted, l.UpdatedAt }).ExecuteCommandAsync();
    }
}
