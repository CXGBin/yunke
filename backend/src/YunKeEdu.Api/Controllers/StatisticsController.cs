using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly StatisticsService _service;
    public StatisticsController(StatisticsService service) => _service = service;

    [HttpGet("dashboard/org")]
    public async Task<ApiResponse<OrgDashboardDto>> OrgDashboard()
        => ApiResponse<OrgDashboardDto>.Ok(await _service.GetOrgDashboardAsync(GetUser()));

    [HttpGet("dashboard/platform")]
    public async Task<ApiResponse<PlatformDashboardDto>> PlatformDashboard()
        => ApiResponse<PlatformDashboardDto>.Ok(await _service.GetPlatformDashboardAsync());

    [HttpGet("attendance")]
    public async Task<ApiResponse<AttendanceAnalysisDto>> Attendance([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        => ApiResponse<AttendanceAnalysisDto>.Ok(await _service.GetAttendanceAnalysisAsync(GetUser(), startDate, endDate));

    [HttpGet("enrollment")]
    public async Task<ApiResponse<EnrollmentAnalysisDto>> Enrollment()
        => ApiResponse<EnrollmentAnalysisDto>.Ok(await _service.GetEnrollmentAnalysisAsync(GetUser()));

    [HttpGet("satisfaction")]
    public async Task<ApiResponse<SatisfactionAnalysisDto>> Satisfaction()
        => ApiResponse<SatisfactionAnalysisDto>.Ok(await _service.GetSatisfactionAnalysisAsync(GetUser()));

    [HttpGet("my-report")]
    public async Task<ApiResponse<MyReportDto>> MyReport()
        => ApiResponse<MyReportDto>.Ok(await _service.GetMyReportAsync(GetUser()));

    [HttpGet("teacher-report")]
    public async Task<ApiResponse<TeacherReportDto>> TeacherReport()
        => ApiResponse<TeacherReportDto>.Ok(await _service.GetTeacherReportAsync(GetUser()));

    [HttpGet("revenue")]
    public async Task<ApiResponse<RevenueDto>> Revenue([FromQuery] int? year)
        => ApiResponse<RevenueDto>.Ok(await _service.GetRevenueAsync(GetUser(), year));

    [HttpGet("revenue/expense")]
    public async Task<ApiResponse<RevenueDto>> RevenueExpense([FromQuery] int? year)
        => ApiResponse<RevenueDto>.Ok(await _service.GetRevenueExpenseAsync(GetUser(), year));

    [HttpGet("revenue/summary")]
    public async Task<ApiResponse<RevenueSummaryDto>> RevenueSummary([FromQuery] int? year)
        => ApiResponse<RevenueSummaryDto>.Ok(await _service.GetRevenueSummaryAsync(GetUser(), year));

    [HttpGet("org-reports")]
    public async Task<ApiResponse<OrgReportDto>> OrgReports()
        => ApiResponse<OrgReportDto>.Ok(await _service.GetOrgReportAsync(GetUser()));

    [HttpGet("teacher-reports")]
    public async Task<ApiResponse<TeacherReportDto>> TeacherReports()
        => ApiResponse<TeacherReportDto>.Ok(await _service.GetTeacherReportAsync(GetUser()));

    [HttpGet("lesson-consumption/student/{studentId}")]
    public async Task<ApiResponse<LessonConsumptionStudentDto>> LessonConsumptionStudent(long studentId)
        => ApiResponse<LessonConsumptionStudentDto>.Ok(await _service.GetLessonConsumptionStudentAsync(studentId));

    [HttpGet("lesson-consumption/course/{courseId}")]
    public async Task<ApiResponse<List<LessonConsumptionCourseDto>>> LessonConsumptionCourse(long courseId)
        => ApiResponse<List<LessonConsumptionCourseDto>>.Ok(await _service.GetLessonConsumptionCourseAsync(courseId, GetUser().TenantId));

    [HttpGet("lesson-consumption/org")]
    public async Task<ApiResponse<LessonConsumptionOrgDto>> LessonConsumptionOrg()
        => ApiResponse<LessonConsumptionOrgDto>.Ok(await _service.GetLessonConsumptionOrgAsync(GetUser()));

    [HttpGet("export")]
    public async Task<ApiResponse<bool>> Export([FromQuery] string? type, [FromQuery] int? year)
    {
        // TODO: Excel export implementation
        return ApiResponse<bool>.Ok(true);
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
