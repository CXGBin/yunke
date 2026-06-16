using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class CourseService : BaseService
{
    public CourseService(ISqlSugarClient db) : base(db) { }

    public async Task<PagedResult<CourseDto>> GetPageAsync(PageRequest req, CurrentUser user)
    {
        var query = Db.Queryable<Course>()
            .LeftJoin<CourseCategory>((c, cat) => c.CategoryId == cat.Id)
            .LeftJoin<SysUser>((c, cat, t) => c.TeacherId == t.Id)
            .LeftJoin<Campus>((c, cat, t, cam) => c.CampusId == cam.Id)
            .LeftJoin<Organization>((c, cat, t, cam, org) => c.TenantId == org.Id && org.TenantId == org.Id);

        if (user.Role != 1)
            query = query.Where((c, cat, t, cam, org) => c.TenantId == user.TenantId && c.IsDeleted == false);
        else
            query = query.Where((c, cat, t, cam, org) => c.IsDeleted == false);

        if (user.Role == 3)
            query = query.Where((c, cat, t, cam, org) => c.TeacherId == user.UserId);
        if (user.Role == 4 || user.Role == 5)
            query = query.Where((c, cat, t, cam, org) => c.Status == 1);

        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where((c, cat, t, cam, org) => c.Name.Contains(req.Keyword!));

        query = query.OrderBy((c, cat, t, cam, org) => c.SortOrder).OrderByDescending((c, cat, t, cam, org) => c.CreatedAt);
        RefAsync<int> total = 0;
        var list = await query.Select((c, cat, t, cam, org) => new CourseDto
        {
            Id = c.Id, CourseCode = c.CourseCode, Name = c.Name, CategoryId = c.CategoryId,
            CategoryName = cat.Name, Description = c.Description, CoverImage = c.CoverImage,
            TotalLessons = c.TotalLessons, LessonDuration = c.LessonDuration, Difficulty = c.Difficulty,
            OriginalPrice = c.OriginalPrice, DiscountPrice = c.DiscountPrice,
            MaxStudents = c.MaxStudents, MinStudents = c.MinStudents,
            EnrollmentDeadline = c.EnrollmentDeadline, Tags = c.Tags, Status = c.Status,
            TeacherId = c.TeacherId, TeacherName = t.RealName,
            CampusId = c.CampusId, CampusName = cam.Name,
            SettlementType = c.SettlementType, FixedFeePerLesson = c.FixedFeePerLesson,
            StudentCountCommission = c.StudentCountCommission,
            SortOrder = c.SortOrder, IsRecommend = c.IsRecommend, ViewCount = c.ViewCount,
            CreatedBy = c.CreatedBy, OrgId = c.OrgId, OrgName = org.Name, CreatedAt = c.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<CourseDto>(list, total, req.Page, req.PageSize);
    }

    public async Task<CourseDto> GetByIdAsync(long id, CurrentUser user)
    {
        var course = await Db.Queryable<Course>()
            .LeftJoin<CourseCategory>((c, cat) => c.CategoryId == cat.Id)
            .LeftJoin<SysUser>((c, cat, t) => c.TeacherId == t.Id)
            .LeftJoin<Campus>((c, cat, t, cam) => c.CampusId == cam.Id)
            .LeftJoin<Organization>((c, cat, t, cam, org) => c.TenantId == org.Id && org.TenantId == org.Id)
            .Where((c, cat, t, cam, org) => c.Id == id && c.IsDeleted == false)
            .Select((c, cat, t, cam, org) => new CourseDto
            {
                Id = c.Id, CourseCode = c.CourseCode, Name = c.Name, CategoryId = c.CategoryId,
                CategoryName = cat.Name, Description = c.Description, CoverImage = c.CoverImage,
                TotalLessons = c.TotalLessons, LessonDuration = c.LessonDuration, Difficulty = c.Difficulty,
                OriginalPrice = c.OriginalPrice, DiscountPrice = c.DiscountPrice,
                MaxStudents = c.MaxStudents, MinStudents = c.MinStudents,
                EnrollmentDeadline = c.EnrollmentDeadline, Tags = c.Tags, Status = c.Status,
                TeacherId = c.TeacherId, TeacherName = t.RealName,
                CampusId = c.CampusId, CampusName = cam.Name,
                SettlementType = c.SettlementType, FixedFeePerLesson = c.FixedFeePerLesson,
                StudentCountCommission = c.StudentCountCommission,
                SortOrder = c.SortOrder, IsRecommend = c.IsRecommend, ViewCount = c.ViewCount,
                CreatedBy = c.CreatedBy, OrgId = c.OrgId, OrgName = org.Name, CreatedAt = c.CreatedAt,
            }).FirstAsync() ?? throw new BizException("课程不存在");
        return course;
    }

    public async Task<long> CreateAsync(CreateCourseRequest req, CurrentUser user)
    {
        var code = $"CRS{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(10, 99)}";
        var course = new Course
        {
            TenantId = user.TenantId, OrgId = user.OrgId, CampusId = req.CampusId,
            CourseCode = code, Name = req.Name, CategoryId = req.CategoryId,
            Description = req.Description, CoverImage = req.CoverImage,
            TotalLessons = req.TotalLessons, LessonDuration = req.LessonDuration,
            Difficulty = req.Difficulty, OriginalPrice = req.OriginalPrice,
            DiscountPrice = req.DiscountPrice, MaxStudents = req.MaxStudents,
            MinStudents = req.MinStudents, EnrollmentDeadline = req.EnrollmentDeadline,
            Tags = req.Tags, Status = 1, TeacherId = req.TeacherId,
            SettlementType = req.SettlementType, FixedFeePerLesson = req.FixedFeePerLesson,
            StudentCountCommission = req.StudentCountCommission,
            SortOrder = req.SortOrder, IsRecommend = req.IsRecommend,
            ScheduledPublishTime = req.ScheduledPublishTime, ScheduledOfflineTime = req.ScheduledOfflineTime,
            CreatedBy = user.UserId, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        return await Db.Insertable(course).ExecuteReturnBigIdentityAsync();
    }

    public async Task UpdateAsync(long id, UpdateCourseRequest req, CurrentUser user)
    {
        var course = await Db.Queryable<Course>()
            .Where(c => c.Id == id && c.TenantId == user.TenantId && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("课程不存在");
        course.Name = req.Name; course.CategoryId = req.CategoryId;
        course.Description = req.Description; course.CoverImage = req.CoverImage;
        course.TotalLessons = req.TotalLessons; course.LessonDuration = req.LessonDuration;
        course.Difficulty = req.Difficulty; course.MaxStudents = req.MaxStudents;
        course.MinStudents = req.MinStudents; course.EnrollmentDeadline = req.EnrollmentDeadline;
        course.Tags = req.Tags; course.TeacherId = req.TeacherId;
        course.CampusId = req.CampusId;
        course.SettlementType = req.SettlementType;
        course.FixedFeePerLesson = req.FixedFeePerLesson;
        course.StudentCountCommission = req.StudentCountCommission;
        course.SortOrder = req.SortOrder; course.IsRecommend = req.IsRecommend;
        course.ScheduledPublishTime = req.ScheduledPublishTime;
        course.ScheduledOfflineTime = req.ScheduledOfflineTime;
        course.UpdatedAt = DateTime.Now;
        await Db.Updateable(course).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(long id, CurrentUser user)
    {
        var course = await Db.Queryable<Course>()
            .Where(c => c.Id == id && c.TenantId == user.TenantId && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("课程不存在");
        if (course.Status != 0) throw new BizException("仅草稿课程可删除");
        course.IsDeleted = true; course.UpdatedAt = DateTime.Now;
        await Db.Updateable(course).UpdateColumns(c => new { c.IsDeleted, c.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task PublishAsync(long id, CurrentUser user)
    {
        var course = await Db.Queryable<Course>()
            .Where(c => c.Id == id && c.TenantId == user.TenantId && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("课程不存在");
        course.Status = 1; course.UpdatedAt = DateTime.Now;
        await Db.Updateable(course).UpdateColumns(c => new { c.Status, c.UpdatedAt }).ExecuteCommandAsync();
    }

    public async Task OfflineAsync(long id, CurrentUser user)
    {
        var course = await Db.Queryable<Course>()
            .Where(c => c.Id == id && c.TenantId == user.TenantId && !c.IsDeleted).FirstAsync()
            ?? throw new BizException("课程不存在");
        course.Status = 2; course.UpdatedAt = DateTime.Now;
        await Db.Updateable(course).UpdateColumns(c => new { c.Status, c.UpdatedAt }).ExecuteCommandAsync();
    }
}
