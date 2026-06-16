using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class CourseAttachmentService : BaseService
{
    public CourseAttachmentService(ISqlSugarClient db) : base(db) { }

    public async Task<long> CreateAsync(long courseId, CreateAttachmentRequest req, CurrentUser user)
    {
        var course = await Db.Queryable<Course>()
            .Where(c => c.Id == courseId && c.TenantId == user.TenantId && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("课程不存在");

        var count = await Db.Queryable<CourseAttachment>()
            .Where(a => a.CourseId == courseId && a.TenantId == user.TenantId).CountAsync();
        if (count >= 10) throw new BizException("最多上传10个附件");

        var att = new CourseAttachment
        {
            TenantId = user.TenantId, CourseId = courseId,
            FileName = req.FileName, FileUrl = req.FileUrl,
            FileSize = req.FileSize, FileType = req.FileType, SortOrder = req.SortOrder,
            CreatedAt = DateTime.Now,
        };
        return await Db.Insertable(att).ExecuteReturnBigIdentityAsync();
    }

    public async Task DeleteAsync(long id, CurrentUser user)
    {
        var att = await Db.Queryable<CourseAttachment>()
            .Where(a => a.Id == id && a.TenantId == user.TenantId).FirstAsync()
            ?? throw new BizException("附件不存在");
        await Db.Deleteable<CourseAttachment>().Where(a => a.Id == id).ExecuteCommandAsync();
    }
}
