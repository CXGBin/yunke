using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class EvaluationService : BaseService
{
    public EvaluationService(ISqlSugarClient db) : base(db) { }

    private async Task CheckUltraPackageAsync(long tenantId)
    {
        var org = await Db.Queryable<Organization>().InSingleAsync(tenantId)
            ?? throw new BizException("机构不存在");
        if (!org.CurrentPackageId.HasValue) throw new BizException("评价功能仅Ultra(高级)套餐可用，请升级套餐");
        var pkg = await Db.Queryable<OrgPackage>().InSingleAsync(org.CurrentPackageId.Value)
            ?? throw new BizException("套餐不存在");
        if (pkg.PackageLevel < 2) throw new BizException("评价功能仅Ultra(高级)套餐可用，请升级套餐");
    }

    public async Task<long> CreateAsync(CreateEvaluationRequest req, CurrentUser user)
    {
        await CheckUltraPackageAsync(user.TenantId);

        var schedule = await Db.Queryable<CourseSchedule>()
            .Where(s => s.Id == req.ScheduleId && !s.IsDeleted).FirstAsync()
            ?? throw new BizException("排课记录不存在");

        var existing = await Db.Queryable<CourseEvaluation>()
            .Where(e => e.EvaluatorId == user.UserId && e.ScheduleId == req.ScheduleId && e.CourseId == req.CourseId && !e.IsDeleted).AnyAsync();
        if (existing) throw new BizException("已评价过该课次");

        var eval = new CourseEvaluation
        {
            TenantId = user.TenantId, OrgId = schedule.OrgId, CampusId = schedule.CampusId,
            CourseId = req.CourseId, ScheduleId = req.ScheduleId,
            EvaluatorId = user.UserId, TargetId = schedule.TeacherId, EvalType = 0,
            CourseRating = req.CourseRating, TeacherRating = req.TeacherRating,
            LessonRating = req.LessonRating, Content = req.Content, Tags = req.Tags,
            Images = req.Images, IsAnonymous = req.IsAnonymous, Status = 1,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        return await Db.Insertable(eval).ExecuteReturnBigIdentityAsync();
    }

    public async Task<PagedResult<EvaluationDto>> GetReceivedAsync(PageRequest req, CurrentUser user, int targetType = 0)
    {
        var query = Db.Queryable<CourseEvaluation>()
            .LeftJoin<SysUser>((e, ev) => e.EvaluatorId == ev.Id)
            .LeftJoin<SysUser>((e, ev, t) => e.TargetId == t.Id)
            .LeftJoin<Course>((e, ev, t, c) => e.CourseId == c.Id)
            .Where((e, ev, t, c) => e.TargetId == user.UserId && e.IsDeleted == false && e.Status == 1);
        query = query.OrderByDescending((e, ev, t, c) => e.CreatedAt);
        RefAsync<int> total = 0;
        var list = await query.Select((e, ev, t, c) => new EvaluationDto
        {
            Id = e.Id, CourseId = e.CourseId, CourseName = c.Name, ScheduleId = e.ScheduleId,
            EvaluatorId = e.EvaluatorId, EvaluatorName = e.IsAnonymous ? "匿名用户" : (ev.RealName ?? ev.NickName ?? ""),
            EvaluatorAvatar = e.IsAnonymous ? null : ev.Avatar,
            TargetId = e.TargetId, TargetName = t.RealName ?? t.NickName ?? "",
            EvalType = e.EvalType, CourseRating = e.CourseRating, TeacherRating = e.TeacherRating,
            LessonRating = e.LessonRating, Content = e.Content, Tags = e.Tags, Images = e.Images,
            IsAnonymous = e.IsAnonymous, Status = e.Status, ReplyContent = e.ReplyContent,
            ReplyBy = e.ReplyBy, ReplyAt = e.ReplyAt, IsTop = e.IsTop, CreatedAt = e.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<EvaluationDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<PagedResult<EvaluationDto>> GetByCourseAsync(PageRequest req, long courseId, long tenantId)
    {
        var query = Db.Queryable<CourseEvaluation>()
            .LeftJoin<SysUser>((e, ev) => e.EvaluatorId == ev.Id)
            .LeftJoin<SysUser>((e, ev, t) => e.TargetId == t.Id)
            .LeftJoin<Course>((e, ev, t, c) => e.CourseId == c.Id)
            .Where((e, ev, t, c) => e.CourseId == courseId && e.TenantId == tenantId && e.IsDeleted == false && e.Status == 1);
        query = query.OrderByDescending((e, ev, t, c) => e.IsTop ? 1 : 0).OrderByDescending((e, ev, t, c) => e.CreatedAt);
        RefAsync<int> total = 0;
        var list = await query.Select((e, ev, t, c) => new EvaluationDto
        {
            Id = e.Id, CourseId = e.CourseId, CourseName = c.Name, ScheduleId = e.ScheduleId,
            EvaluatorId = e.EvaluatorId, EvaluatorName = e.IsAnonymous ? "匿名用户" : (ev.RealName ?? ""),
            EvaluatorAvatar = e.IsAnonymous ? null : ev.Avatar,
            TargetId = e.TargetId, TargetName = t.RealName ?? "",
            EvalType = e.EvalType, CourseRating = e.CourseRating, TeacherRating = e.TeacherRating,
            LessonRating = e.LessonRating, Content = e.Content, Tags = e.Tags, Images = e.Images,
            IsAnonymous = e.IsAnonymous, Status = e.Status, ReplyContent = e.ReplyContent,
            ReplyBy = e.ReplyBy, ReplyAt = e.ReplyAt, IsTop = e.IsTop, CreatedAt = e.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<EvaluationDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<PagedResult<EvaluationDto>> GetMyAsync(PageRequest req, CurrentUser user)
    {
        var query = Db.Queryable<CourseEvaluation>()
            .LeftJoin<Course>((e, c) => e.CourseId == c.Id)
            .LeftJoin<SysUser>((e, c, t) => e.TargetId == t.Id)
            .Where((e, c, t) => e.EvaluatorId == user.UserId && e.IsDeleted == false)
            .OrderByDescending((e, c, t) => e.CreatedAt);
        RefAsync<int> total = 0;
        var list = await query.Select((e, c, t) => new EvaluationDto
        {
            Id = e.Id, CourseId = e.CourseId, CourseName = c.Name, ScheduleId = e.ScheduleId,
            EvaluatorId = e.EvaluatorId, TargetId = e.TargetId, TargetName = t.RealName ?? "",
            EvalType = e.EvalType, CourseRating = e.CourseRating, TeacherRating = e.TeacherRating,
            LessonRating = e.LessonRating, Content = e.Content, Tags = e.Tags, IsAnonymous = e.IsAnonymous,
            Status = e.Status, ReplyContent = e.ReplyContent, CreatedAt = e.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<EvaluationDto>(list, total, req.Page, req.PageSize);
    }

    public async Task ReplyAsync(long id, ReplyEvaluationRequest req, CurrentUser user)
    {
        var eval = await Db.Queryable<CourseEvaluation>()
            .Where(e => e.Id == id && !e.IsDeleted).FirstAsync() ?? throw new BizException("评价不存在");
        eval.ReplyContent = req.Content; eval.ReplyBy = user.UserId; eval.ReplyAt = DateTime.Now;
        eval.UpdatedAt = DateTime.Now;
        await Db.Updateable(eval).UpdateColumns(e => new { e.ReplyContent, e.ReplyBy, e.ReplyAt, e.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task HideAsync(long id, CurrentUser user)
    {
        var eval = await Db.Queryable<CourseEvaluation>()
            .Where(e => e.Id == id && e.TenantId == user.TenantId).FirstAsync()
            ?? throw new BizException("评价不存在");
        eval.Status = 0; eval.UpdatedAt = DateTime.Now;
        await Db.Updateable(eval).UpdateColumns(e => new { e.Status, e.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task TopAsync(long id, CurrentUser user)
    {
        var eval = await Db.Queryable<CourseEvaluation>()
            .Where(e => e.Id == id && e.TenantId == user.TenantId).FirstAsync()
            ?? throw new BizException("评价不存在");
        eval.IsTop = !eval.IsTop; eval.UpdatedAt = DateTime.Now;
        await Db.Updateable(eval).UpdateColumns(e => new { e.IsTop, e.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task<EvaluationStatisticsDto> GetCourseStatisticsAsync(long courseId, long tenantId)
    {
        var evals = await Db.Queryable<CourseEvaluation>()
            .Where(e => e.CourseId == courseId && e.TenantId == tenantId && e.IsDeleted == false && e.Status == 1).ToListAsync();
        var course = await Db.Queryable<Course>().InSingleAsync(courseId);
        return new EvaluationStatisticsDto
        {
            CourseId = courseId, CourseName = course?.Name ?? "",
            TotalCount = evals.Count,
            AvgCourseRating = evals.Any() ? Math.Round(evals.Where(e => e.CourseRating.HasValue).Average(e => (decimal)e.CourseRating!.Value), 2) : 0,
            AvgTeacherRating = evals.Any() ? Math.Round(evals.Where(e => e.TeacherRating.HasValue).Average(e => (decimal)e.TeacherRating!.Value), 2) : 0,
            AvgLessonRating = evals.Any() ? Math.Round(evals.Where(e => e.LessonRating.HasValue).Average(e => (decimal)e.LessonRating!.Value), 2) : 0,
            RatingDistribution = Enumerable.Range(1, 5).ToDictionary(i => i, i => evals.Count(e => e.CourseRating == i)),
        };
    }

    public async Task<TeacherEvaluationStatisticsDto> GetTeacherStatisticsAsync(long teacherId)
    {
        var evals = await Db.Queryable<CourseEvaluation>()
            .Where(e => e.TargetId == teacherId && e.IsDeleted == false && e.Status == 1).ToListAsync();
        var teacher = await Db.Queryable<SysUser>().InSingleAsync(teacherId);
        return new TeacherEvaluationStatisticsDto
        {
            TeacherId = teacherId, TeacherName = teacher?.RealName ?? "",
            TotalEvaluations = evals.Count,
            AvgRating = evals.Any() ? Math.Round(evals.Where(e => e.TeacherRating.HasValue).Average(e => (decimal)e.TeacherRating!.Value), 2) : 0,
            RatingDistribution = Enumerable.Range(1, 5).ToDictionary(i => i, i => evals.Count(e => e.TeacherRating == i)),
        };
    }

    public async Task<List<EvaluationTagDto>> GetTagsAsync(long tenantId)
    {
        return await Db.Queryable<EvaluationTag>()
            .Where(t => t.TenantId == tenantId && t.Status == 1)
            .Select(t => new EvaluationTagDto { Id = t.Id, Name = t.Name, TagType = t.TagType, SortOrder = t.SortOrder, Status = t.Status })
            .ToListAsync();
    }

    public async Task<PagedResult<EvaluationDto>> GetPageAsync(PageRequest req, long tenantId)
    {
        var query = Db.Queryable<CourseEvaluation>()
            .LeftJoin<SysUser>((e, ev) => e.EvaluatorId == ev.Id)
            .LeftJoin<Course>((e, ev, c) => e.CourseId == c.Id)
            .LeftJoin<SysUser>((e, ev, c, t) => e.TargetId == t.Id)
            .Where((e, ev, c, t) => e.TenantId == tenantId && e.IsDeleted == false);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where((e, ev, c, t) => c.Name!.Contains(req.Keyword!) || (ev.RealName ?? "").Contains(req.Keyword!));
        query = query.OrderByDescending((e, ev, c, t) => e.CreatedAt);
        RefAsync<int> total = 0;
        var list = await query.Select((e, ev, c, t) => new EvaluationDto
        {
            Id = e.Id, CourseId = e.CourseId, CourseName = c.Name, ScheduleId = e.ScheduleId,
            EvaluatorId = e.EvaluatorId, EvaluatorName = ev.RealName ?? ev.NickName ?? "",
            TargetId = e.TargetId, TargetName = t.RealName ?? "",
            EvalType = e.EvalType, CourseRating = e.CourseRating, TeacherRating = e.TeacherRating,
            Content = e.Content, Status = e.Status, IsTop = e.IsTop, CreatedAt = e.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<EvaluationDto>(list, total, req.Page, req.PageSize);
    }
}
