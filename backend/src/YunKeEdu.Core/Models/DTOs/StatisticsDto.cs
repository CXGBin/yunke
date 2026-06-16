namespace YunKeEdu.Core.Models.DTOs;

public class OrgDashboardDto
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalCourses { get; set; }
    public int ActiveCourses { get; set; }
    public int TodayEnrollments { get; set; }
    public decimal TodayAttendanceRate { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int PendingLeaves { get; set; }
    public int TodaySchedules { get; set; }
}

public class PlatformDashboardDto
{
    public int TotalOrgs { get; set; }
    public int ActiveOrgs { get; set; }
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalCourses { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int NewOrgsThisMonth { get; set; }
    public int NewStudentsThisMonth { get; set; }
}

public class AttendanceAnalysisDto
{
    public decimal OverallRate { get; set; }
    public List<DailyAttendanceDto> DailyTrend { get; set; } = new();
    public List<CourseAttendanceDto> ByCourse { get; set; } = new();
}

public class DailyAttendanceDto
{
    public DateTime Date { get; set; }
    public int TotalStudents { get; set; }
    public int PresentCount { get; set; }
    public decimal Rate { get; set; }
}

public class CourseAttendanceDto
{
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public decimal AttendanceRate { get; set; }
}

public class EnrollmentAnalysisDto
{
    public int TotalEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }
    public decimal EnrollmentRate { get; set; }
    public List<CourseEnrollmentDto> ByCourse { get; set; } = new();
}

public class CourseEnrollmentDto
{
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int Enrolled { get; set; }
    public int MaxStudents { get; set; }
    public decimal FillRate { get; set; }
}

public class SatisfactionAnalysisDto
{
    public decimal OverallRating { get; set; }
    public int TotalEvaluations { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; } = new();
    public List<CourseSatisfactionDto> ByCourse { get; set; } = new();
}

public class CourseSatisfactionDto
{
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int EvaluationCount { get; set; }
    public decimal AvgRating { get; set; }
}

public class MyReportDto
{
    public int EnrolledCourses { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public decimal AttendanceRate { get; set; }
    public int MyEvaluations { get; set; }
    public decimal MyAvgRating { get; set; }
    public List<MyCourseReportDto> Courses { get; set; } = new();
}

public class MyCourseReportDto
{
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public decimal AttendanceRate { get; set; }
}

public class TeacherReportDto
{
    public long TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int TotalCourses { get; set; }
    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalStudents { get; set; }
    public decimal AvgAttendanceRate { get; set; }
    public decimal AvgRating { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal MonthlyIncome { get; set; }
}

public class RevenueDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public List<MonthlyRevenueDto> MonthlyTrend { get; set; } = new();
}

public class MonthlyRevenueDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Expense { get; set; }
    public decimal NetIncome { get; set; }
}

public class RevenueSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetIncome { get; set; }
    public List<MonthlyRevenueDto> Trend { get; set; } = new();
}

public class LessonConsumptionStudentDto
{
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public List<LessonConsumptionCourseDto> Courses { get; set; } = new();
}

public class LessonConsumptionCourseDto
{
    public long CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public int ConsumedLessons { get; set; }
    public int RemainingLessons { get; set; }
    public decimal Progress { get; set; }
}

public class LessonConsumptionOrgDto
{
    public int TotalCourses { get; set; }
    public int TotalLessons { get; set; }
    public int ConsumedLessons { get; set; }
    public int RemainingLessons { get; set; }
    public decimal OverallProgress { get; set; }
    public List<LessonConsumptionCourseDto> TopCourses { get; set; } = new();
}

public class OrgReportDto
{
    public int TotalStudents { get; set; }
    public int NewStudentsThisMonth { get; set; }
    public int TotalCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public decimal AttendanceRate { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal TeacherExpense { get; set; }
    public int ActiveTeachers { get; set; }
    public decimal TeacherTotalIncome { get; set; }
    public List<TeacherReportDto> TeacherReports { get; set; } = new();
}
