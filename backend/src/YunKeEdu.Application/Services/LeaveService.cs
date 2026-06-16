using SqlSugar;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Exceptions;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;

namespace YunKeEdu.Application.Services;

public class LeaveService : BaseService
{
    public LeaveService(ISqlSugarClient db) : base(db) { }

    public async Task<long> CreateAsync(CreateLeaveRequest req, CurrentUser user)
    {
        var enroll = await Db.Queryable<CourseEnrollment>()
            .Where(e => e.CourseId == req.CourseId && e.StudentId == user.UserId && e.Status == 1).FirstAsync()
            ?? throw new BizException("未选该课程，不可请假");

        var leave = new LeaveRequest
        {
            TenantId = user.TenantId, OrgId = enroll.OrgId,
            StudentId = user.UserId, CourseId = req.CourseId,
            ScheduleId = req.ScheduleId, LeaveType = req.LeaveType,
            StartDate = req.StartDate, EndDate = req.EndDate,
            Reason = req.Reason, Status = 0, ApplicantId = user.UserId,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
        };
        return await Db.Insertable(leave).ExecuteReturnBigIdentityAsync();
    }

    public async Task<List<LeaveRequestDto>> GetMyListAsync(CurrentUser user)
    {
        return await Db.Queryable<LeaveRequest>()
            .LeftJoin<Course>((l, c) => l.CourseId == c.Id)
            .LeftJoin<SysUser>((l, c, s) => l.StudentId == s.Id)
            .Where((l, c, s) => l.StudentId == user.UserId)
            .OrderByDescending((l, c, s) => l.CreatedAt)
            .Select((l, c, s) => new LeaveRequestDto
            {
                Id = l.Id, StudentId = l.StudentId, StudentName = s.RealName ?? "",
                CourseId = l.CourseId, CourseName = c.Name, ScheduleId = l.ScheduleId,
                LeaveType = l.LeaveType, StartDate = l.StartDate, EndDate = l.EndDate,
                Reason = l.Reason, Status = l.Status, ApplicantId = l.ApplicantId,
                PreReviewerId = l.PreReviewerId, PreReviewedAt = l.PreReviewedAt,
                PreReviewRemark = l.PreReviewRemark, ApproverId = l.ApproverId,
                ApprovedAt = l.ApprovedAt, ApproveRemark = l.ApproveRemark, CreatedAt = l.CreatedAt,
            }).ToListAsync();
    }

    public async Task PreReviewAsync(long id, PreReviewRequest req, CurrentUser user)
    {
        var leave = await Db.Queryable<LeaveRequest>()
            .Where(l => l.Id == id && l.TenantId == user.TenantId).FirstAsync()
            ?? throw new BizException("请假记录不存在");
        if (leave.Status != 0) throw new BizException("当前状态不可预审");
        leave.PreReviewerId = user.UserId; leave.PreReviewedAt = DateTime.Now;
        leave.PreReviewRemark = req.Remark;
        leave.Status = req.Approve ? 1 : 3;
        leave.UpdatedAt = DateTime.Now;
        if (req.Approve) await Db.Updateable(leave).ExecuteCommandAsync();
        else await Db.Updateable(leave).ExecuteCommandAsync();
    }

    public async Task ApproveAsync(long id, ApproveLeaveRequest req, CurrentUser user)
    {
        var leave = await Db.Queryable<LeaveRequest>()
            .Where(l => l.Id == id && l.TenantId == user.TenantId).FirstAsync()
            ?? throw new BizException("请假记录不存在");
        if (leave.Status != 1) throw new BizException("当前状态不可审批");
        leave.ApproverId = user.UserId; leave.ApprovedAt = DateTime.Now;
        leave.ApproveRemark = req.Remark;
        leave.Status = req.Approve ? 2 : 3;
        leave.UpdatedAt = DateTime.Now;
        await Db.Updateable(leave).ExecuteCommandAsync();
    }

    public async Task<PagedResult<LeaveRequestDto>> GetPageAsync(PageRequest req, CurrentUser user)
    {
        var query = Db.Queryable<LeaveRequest>()
            .LeftJoin<Course>((l, c) => l.CourseId == c.Id)
            .LeftJoin<SysUser>((l, c, s) => l.StudentId == s.Id)
            .LeftJoin<SysUser>((l, c, s, pr) => l.PreReviewerId == pr.Id)
            .LeftJoin<SysUser>((l, c, s, pr, ap) => l.ApproverId == ap.Id)
            .Where((l, c, s, pr, ap) => l.TenantId == user.TenantId);
        if (user.Role == 3) query = query.Where((l, c, s, pr, ap) => l.PreReviewerId == null);
        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where((l, c, s, pr, ap) => s.RealName!.Contains(req.Keyword!));
        query = query.OrderByDescending((l, c, s, pr, ap) => l.CreatedAt);
        RefAsync<int> total = 0;
        var list = await query.Select((l, c, s, pr, ap) => new LeaveRequestDto
        {
            Id = l.Id, StudentId = l.StudentId, StudentName = s.RealName ?? "",
            CourseId = l.CourseId, CourseName = c.Name, ScheduleId = l.ScheduleId,
            LeaveType = l.LeaveType, StartDate = l.StartDate, EndDate = l.EndDate,
            Reason = l.Reason, Status = l.Status, ApplicantId = l.ApplicantId,
            PreReviewerId = l.PreReviewerId, PreReviewerName = pr.RealName,
            PreReviewedAt = l.PreReviewedAt, PreReviewRemark = l.PreReviewRemark,
            ApproverId = l.ApproverId, ApproverName = ap.RealName,
            ApprovedAt = l.ApprovedAt, ApproveRemark = l.ApproveRemark, CreatedAt = l.CreatedAt,
        }).ToPageListAsync(req.Page, req.PageSize, total);
        return new PagedResult<LeaveRequestDto>(list, total, req.Page, req.PageSize);
    }
}
