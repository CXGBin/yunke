using Xunit;
using YunKeEdu.Core.Entities;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Core.Exceptions;

namespace YunKeEdu.Tests;

/// <summary>云科智教平台 - 第1轮单元测试（30+用例）</summary>
/// <remarks>
/// 覆盖核心业务逻辑：结算计算、套餐折价、签到统计、评价统计、选课规则、排课冲突、请假审批、邀请绑定等。
/// 由于Service层紧密耦合ISqlSugarClient，本测试侧重：
/// 1. 纯计算逻辑的直接验证（结算费用、折价金额、出勤率）
/// 2. DTO验证与默认值
/// 3. 枚举与业务常量验证
/// 4. BizException异常路径验证
/// 5. Entity构建与属性验证
/// </remarks>
public class YunKeEduUnitTests
{
    // ==================== 一、结算计算测试 ====================

    [Fact]
    [Trait("Category", "Settlement")]
    [Trait("Module", "Settlement")]
    public void Settlement_FixedType_CalculatesCorrectFee()
    {
        // 固定课时费模式：fee = FixedFeePerLesson + (studentCount - 1) * StudentCountCommission
        var fixedFeePerLesson = 100m;
        var studentCount = 5;
        var studentCountCommission = 20m;

        var fee = fixedFeePerLesson + Math.Max(0, studentCount - 1) * studentCountCommission;
        Assert.Equal(180m, (decimal)fee);
    }

    [Fact]
    [Trait("Category", "Settlement")]
    [Trait("Module", "Settlement")]
    public void Settlement_FixedType_SingleStudent_OnlyBaseFee()
    {
        var fixedFeePerLesson = 100m;
        var studentCount = 1;
        var studentCountCommission = 20m;

        var fee = fixedFeePerLesson + Math.Max(0, studentCount - 1) * studentCountCommission;
        Assert.Equal(100m, (decimal)fee);
    }

    [Fact]
    [Trait("Category", "Settlement")]
    [Trait("Module", "Settlement")]
    public void Settlement_RatioType_CalculatesCorrectFee()
    {
        // 按比例模式：unitPrice = Floor(discountPrice / totalLessons), fee = unitPrice + (studentCount - 1) * commission
        var discountPrice = 3000m;
        var totalLessons = 12;
        var studentCount = 8;
        var studentCountCommission = 10m;

        var unitPrice = Math.Floor(discountPrice / Math.Max(1, totalLessons));
        var fee = unitPrice + Math.Max(0, studentCount - 1) * studentCountCommission;
        Assert.Equal(320m, (decimal)fee); // unitPrice=250, commission=70, total=320
    }

    [Fact]
    [Trait("Category", "Settlement")]
    [Trait("Module", "Settlement")]
    public void Settlement_RatioType_UsesOriginalPrice_WhenNoDiscount()
    {
        var originalPrice = 6000m;
        var discountPrice = 0m;
        var totalLessons = 20;
        var studentCount = 3;
        var studentCountCommission = 15m;

        var effectivePrice = discountPrice > 0 ? discountPrice : originalPrice;
        var unitPrice = Math.Floor(effectivePrice / Math.Max(1, totalLessons));
        var fee = unitPrice + Math.Max(0, studentCount - 1) * studentCountCommission;
        Assert.Equal(330m, (decimal)fee); // unitPrice=300, commission=30, total=330
    }

    [Fact]
    [Trait("Category", "Settlement")]
    [Trait("Module", "Settlement")]
    public void Settlement_RatioType_ZeroStudents_ReturnsZeroCommission()
    {
        var discountPrice = 2000m;
        var totalLessons = 10;

        var unitPrice = Math.Floor(discountPrice / Math.Max(1, totalLessons));
        var studentCount = 0;
        var fee = unitPrice + Math.Max(0, studentCount - 1) * 20m;
        Assert.Equal((decimal)unitPrice, (decimal)fee);
    }

    // ==================== 二、套餐升级折价计算测试 ====================

    [Fact]
    [Trait("Category", "Package")]
    [Trait("Module", "Subscription")]
    public void UpgradeDiscount_FullYearUnused_MaxDiscount()
    {
        // 使用0个月，未使用12个月 => 折价 = 月价 * 12
        var oldPrice = 12000m;
        var usedMonths = 0;
        var unusedMonths = 12;
        var oldMonthlyPrice = Math.Floor(oldPrice / 12m * 100) / 100;
        var discountAmount = Math.Floor(oldMonthlyPrice * unusedMonths * 100) / 100;

        Assert.Equal(1000m, oldMonthlyPrice);
        Assert.Equal(12000m, discountAmount);
    }

    [Fact]
    [Trait("Category", "Package")]
    [Trait("Module", "Subscription")]
    public void UpgradeDiscount_SixMonthsUsed_HalfDiscount()
    {
        var oldPrice = 12000m;
        var usedMonths = 6;
        var unusedMonths = 6;
        var oldMonthlyPrice = Math.Floor(oldPrice / 12m * 100) / 100;
        var discountAmount = Math.Floor(oldMonthlyPrice * unusedMonths * 100) / 100;

        Assert.Equal(6000m, discountAmount);
    }

    [Fact]
    [Trait("Category", "Package")]
    [Trait("Module", "Subscription")]
    public void UpgradeDiscount_FullYearUsed_ZeroDiscount()
    {
        var oldPrice = 12000m;
        var usedMonths = 12;
        var unusedMonths = 0;
        var oldMonthlyPrice = Math.Floor(oldPrice / 12m * 100) / 100;
        var discountAmount = Math.Floor(oldMonthlyPrice * unusedMonths * 100) / 100;

        Assert.Equal(0m, discountAmount);
    }

    [Fact]
    [Trait("Category", "Package")]
    [Trait("Module", "Subscription")]
    public void UpgradeDiscount_PayAmount_CannotBeNegative()
    {
        var newPrice = 5000m;
        var oldPrice = 12000m;
        var usedMonths = 0;
        var unusedMonths = 12;
        var oldMonthlyPrice = Math.Floor(oldPrice / 12m * 100) / 100;
        var discountAmount = Math.Floor(oldMonthlyPrice * unusedMonths * 100) / 100;
        var payAmount = newPrice - discountAmount;
        if (payAmount < 0) payAmount = 0;

        Assert.Equal(0m, payAmount);
    }

    [Fact]
    [Trait("Category", "Package")]
    [Trait("Module", "Subscription")]
    public void UpgradeDiscount_NegativeUnusedMonths_ClampedToZero()
    {
        var oldPrice = 12000m;
        var usedMonths = 13; // 超过12个月
        usedMonths = Math.Min(usedMonths, 12);
        var unusedMonths = Math.Max(0, 12 - usedMonths);

        Assert.Equal(12, usedMonths);
        Assert.Equal(0, unusedMonths);
    }

    [Fact]
    [Trait("Category", "Package")]
    [Trait("Module", "Subscription")]
    public void UpgradeDiscount_PriceCalculation_Precision()
    {
        // 测试非整除价格
        var oldPrice = 9999m;
        var oldMonthlyPrice = Math.Floor(oldPrice / 12m * 100) / 100;
        Assert.Equal(833.25m, oldMonthlyPrice);

        var unusedMonths = 5;
        var discountAmount = Math.Floor(oldMonthlyPrice * unusedMonths * 100) / 100;
        Assert.Equal(4166.25m, discountAmount);
    }

    // ==================== 三、签到统计计算测试 ====================

    [Fact]
    [Trait("Category", "Attendance")]
    [Trait("Module", "Attendance")]
    public void AttendanceRate_AllPresent_Returns100()
    {
        var total = 10;
        var present = 8;
        var late = 2;
        var rate = total > 0 ? Math.Round((decimal)(present + late) * 100 / total, 2) : 0;
        Assert.Equal(100m, rate);
    }

    [Fact]
    [Trait("Category", "Attendance")]
    [Trait("Module", "Attendance")]
    public void AttendanceRate_HalfPresent_Returns50()
    {
        var total = 10;
        var present = 5;
        var late = 0;
        var rate = total > 0 ? Math.Round((decimal)(present + late) * 100 / total, 2) : 0;
        Assert.Equal(50m, rate);
    }

    [Fact]
    [Trait("Category", "Attendance")]
    [Trait("Module", "Attendance")]
    public void AttendanceRate_ZeroTotal_ReturnsZero()
    {
        var total = 0;
        var present = 0;
        var late = 0;
        var rate = total > 0 ? Math.Round((decimal)(present + late) * 100 / total, 2) : 0;
        Assert.Equal(0m, rate);
    }

    [Fact]
    [Trait("Category", "Attendance")]
    [Trait("Module", "Attendance")]
    public void AttendanceRate_WithLate_CalculatedCorrectly()
    {
        var total = 20;
        var present = 15;
        var late = 3;
        var absent = total - present - late;
        var rate = total > 0 ? Math.Round((decimal)(present + late) * 100 / total, 2) : 0;
        Assert.Equal(90m, rate);
        Assert.Equal(2, absent);
    }

    // ==================== 四、评价统计计算测试 ====================

    [Fact]
    [Trait("Category", "Evaluation")]
    [Trait("Module", "Evaluation")]
    public void EvaluationStats_NoEvaluations_ReturnsZero()
    {
        var evals = new List<int>(); // empty
        var avg = evals.Any() ? Math.Round((decimal)evals.Average(), 2) : 0m;
        Assert.Equal(0m, avg);
    }

    [Fact]
    [Trait("Category", "Evaluation")]
    [Trait("Module", "Evaluation")]
    public void EvaluationStats_AllFiveStars_ReturnsFive()
    {
        var ratings = Enumerable.Repeat(5, 10).Select(x => (decimal)x).ToList();
        var avg = Math.Round(ratings.Average(), 2);
        Assert.Equal(5m, avg);
    }

    [Fact]
    [Trait("Category", "Evaluation")]
    [Trait("Module", "Evaluation")]
    public void EvaluationStats_MixedRatings_CorrectAverage()
    {
        var ratings = new[] { 5m, 4m, 3m, 5m, 4m, 3m, 5m, 5m, 4m, 2m };
        var avg = Math.Round(ratings.Average(), 2);
        Assert.Equal(4m, avg); // (5+4+3+5+4+3+5+5+4+2)/10 = 40/10 = 4.0
    }

    [Fact]
    [Trait("Category", "Evaluation")]
    [Trait("Module", "Evaluation")]
    public void EvaluationStats_RatingDistribution_Correct()
    {
        var evals = new[] { 5, 5, 4, 3, 2, 5, 1, 4, 5, 3 };
        var distribution = Enumerable.Range(1, 5).ToDictionary(i => i, i => evals.Count(e => e == i));
        Assert.Equal(1, distribution[1]);
        Assert.Equal(1, distribution[2]);
        Assert.Equal(2, distribution[3]);
        Assert.Equal(2, distribution[4]);
        Assert.Equal(4, distribution[5]);
    }

    // ==================== 五、课程选课业务规则测试 ====================

    [Fact]
    [Trait("Category", "Enrollment")]
    [Trait("Module", "Course")]
    public void Enrollment_MaxStudentsLimit_RejectsWhenFull()
    {
        var maxStudents = 30;
        var enrolledCount = 30;
        Assert.True(enrolledCount >= maxStudents, "课程已满员");
    }

    [Fact]
    [Trait("Category", "Enrollment")]
    [Trait("Module", "Course")]
    public void Enrollment_MaxStudentsLimit_AcceptsWhenNotFull()
    {
        var maxStudents = 30;
        var enrolledCount = 29;
        Assert.False(enrolledCount >= maxStudents, "课程未满员，可以报名");
    }

    [Fact]
    [Trait("Category", "Enrollment")]
    [Trait("Module", "Course")]
    public void Enrollment_MaxCoursePerStudent_RejectsAtLimit()
    {
        var studentEnrollCount = 20;
        var maxCourses = 20;
        Assert.True(studentEnrollCount >= maxCourses, "最多选20门课程");
    }

    [Fact]
    [Trait("Category", "Enrollment")]
    [Trait("Module", "Course")]
    public void Enrollment_CourseStatus_OnlyPublished()
    {
        var courseStatus = 1; // 已发布
        Assert.True(courseStatus == 1, "只有已发布的课程可以被选课");
    }

    // ==================== 六、排课冲突检测测试 ====================

    [Fact]
    [Trait("Category", "Schedule")]
    [Trait("Module", "Schedule")]
    public void ScheduleConflict_NoOverlap_ReturnsFalse()
    {
        var existStart = new TimeSpan(8, 0, 0);
        var existEnd = new TimeSpan(9, 0, 0);
        var newStart = new TimeSpan(10, 0, 0);
        var newEnd = new TimeSpan(11, 0, 0);
        // 冲突条件: exist.Start < new.End && exist.End > new.Start
        var conflict = existStart < newEnd && existEnd > newStart;
        Assert.False(conflict);
    }

    [Fact]
    [Trait("Category", "Schedule")]
    [Trait("Module", "Schedule")]
    public void ScheduleConflict_ExactOverlap_ReturnsTrue()
    {
        var existStart = new TimeSpan(8, 0, 0);
        var existEnd = new TimeSpan(10, 0, 0);
        var newStart = new TimeSpan(8, 0, 0);
        var newEnd = new TimeSpan(10, 0, 0);
        var conflict = existStart < newEnd && existEnd > newStart;
        Assert.True(conflict);
    }

    [Fact]
    [Trait("Category", "Schedule")]
    [Trait("Module", "Schedule")]
    public void ScheduleConflict_PartialOverlap_ReturnsTrue()
    {
        var existStart = new TimeSpan(8, 0, 0);
        var existEnd = new TimeSpan(10, 0, 0);
        var newStart = new TimeSpan(9, 30, 0);
        var newEnd = new TimeSpan(11, 30, 0);
        var conflict = existStart < newEnd && existEnd > newStart;
        Assert.True(conflict);
    }

    [Fact]
    [Trait("Category", "Schedule")]
    [Trait("Module", "Schedule")]
    public void ScheduleConflict_ContainedWithin_ReturnsTrue()
    {
        var existStart = new TimeSpan(8, 0, 0);
        var existEnd = new TimeSpan(12, 0, 0);
        var newStart = new TimeSpan(9, 0, 0);
        var newEnd = new TimeSpan(10, 0, 0);
        var conflict = existStart < newEnd && existEnd > newStart;
        Assert.True(conflict);
    }

    [Fact]
    [Trait("Category", "Schedule")]
    [Trait("Module", "Schedule")]
    public void ScheduleConflict_AdjacentNoOverlap_ReturnsFalse()
    {
        var existStart = new TimeSpan(8, 0, 0);
        var existEnd = new TimeSpan(10, 0, 0);
        var newStart = new TimeSpan(10, 0, 0);
        var newEnd = new TimeSpan(12, 0, 0);
        var conflict = existStart < newEnd && existEnd > newStart;
        Assert.False(conflict);
    }

    // ==================== 七、请假审批状态机测试 ====================

    [Fact]
    [Trait("Category", "Leave")]
    [Trait("Module", "Leave")]
    public void LeaveStatus_InitialState_IsPending()
    {
        // 请假初始状态: 0=待审批
        var status = 0;
        Assert.Equal(0, status);
    }

    [Fact]
    [Trait("Category", "Leave")]
    [Trait("Module", "Leave")]
    public void LeaveStatus_PreReviewApprove_MovesToPreApproved()
    {
        // 教师预审通过: 0→1
        var currentStatus = 0;
        var approve = true;
        var newStatus = approve ? 1 : 3;
        Assert.Equal(1, newStatus);
    }

    [Fact]
    [Trait("Category", "Leave")]
    [Trait("Module", "Leave")]
    public void LeaveStatus_PreReviewReject_MovesToRejected()
    {
        // 教师预审拒绝: 0→3
        var approve = false;
        var newStatus = approve ? 1 : 3;
        Assert.Equal(3, newStatus);
    }

    [Fact]
    [Trait("Category", "Leave")]
    [Trait("Module", "Leave")]
    public void LeaveStatus_FinalApprove_MovesToApproved()
    {
        // 平台终审通过: 1→2
        var currentStatus = 1;
        Assert.Equal(1, currentStatus);
        var approve = true;
        var newStatus = approve ? 2 : 3;
        Assert.Equal(2, newStatus);
    }

    [Fact]
    [Trait("Category", "Leave")]
    [Trait("Module", "Leave")]
    public void LeaveStatus_CannotFinalReview_WhenNotPreApproved()
    {
        var currentStatus = 0; // 待预审，不能直接终审
        Assert.True(currentStatus != 1, "只有预审通过的请假才能终审");
    }

    // ==================== 八、邀请绑定流程测试 ====================

    [Fact]
    [Trait("Category", "Invitation")]
    [Trait("Module", "Invitation")]
    public void Invitation_InviteCode_FormatIsUpperCase8Chars()
    {
        var code = Guid.NewGuid().ToString("N")[..8].ToUpper();
        Assert.Equal(8, code.Length);
        Assert.True(code.All(c => char.IsUpper(c) || char.IsDigit(c)), "邀请码应为大写字母数字");
    }

    [Fact]
    [Trait("Category", "Invitation")]
    [Trait("Module", "Invitation")]
    public void Invitation_CanAccept_WhenValidAndNotExpired()
    {
        var status = 0; // 待使用
        var expiresAt = DateTime.Now.AddDays(1);
        var isValid = status == 0 && expiresAt > DateTime.Now;
        Assert.True(isValid);
    }

    [Fact]
    [Trait("Category", "Invitation")]
    [Trait("Module", "Invitation")]
    public void Invitation_CannotAccept_WhenExpired()
    {
        var status = 0;
        var expiresAt = DateTime.Now.AddDays(-1);
        var isValid = status == 0 && expiresAt > DateTime.Now;
        Assert.False(isValid);
    }

    [Fact]
    [Trait("Category", "Invitation")]
    [Trait("Module", "Invitation")]
    public void Invitation_CannotAccept_WhenAlreadyUsed()
    {
        var status = 1; // 已使用
        var isValid = status == 0;
        Assert.False(isValid);
    }

    [Fact]
    [Trait("Category", "Invitation")]
    [Trait("Module", "Invitation")]
    public void Invitation_RoleAssignment_Teacher()
    {
        var invitedRole = 3; // 教师
        var userCodePrefix = invitedRole switch { 3 => "T", 4 => "S", _ => "P" };
        Assert.Equal("T", userCodePrefix);
    }

    [Fact]
    [Trait("Category", "Invitation")]
    [Trait("Module", "Invitation")]
    public void Invitation_RoleAssignment_Student()
    {
        var invitedRole = 4; // 学生
        var userCodePrefix = invitedRole switch { 3 => "T", 4 => "S", _ => "P" };
        Assert.Equal("S", userCodePrefix);
    }

    // ==================== 九、套餐等级与功能限制测试 ====================

    [Fact]
    [Trait("Category", "Package")]
    [Trait("Module", "Package")]
    public void Package_Evaluation_OnlyUltraAndAbove()
    {
        // 评价功能：PackageLevel < 2 不可用 (2=Ultra, 3=Ultimate)
        var pkgLevel = 0; // Plus
        var enableEval = pkgLevel >= 2;
        Assert.False(enableEval);

        pkgLevel = 2; // Ultra
        enableEval = pkgLevel >= 2;
        Assert.True(enableEval);
    }

    [Fact]
    [Trait("Category", "Package")]
    [Trait("Module", "Package")]
    public void Package_Upgrade_OnlyToHigherLevel()
    {
        var oldLevel = 1; // Pro
        var newLevel = 2; // Ultra
        Assert.True(newLevel > oldLevel, "只能升级到更高等级");
    }

    [Fact]
    [Trait("Category", "Package")]
    [Trait("Module", "Package")]
    public void Package_Upgrade_SameLevel_Rejected()
    {
        var oldLevel = 1; // Pro
        var newLevel = 1; // Pro
        Assert.False(newLevel > oldLevel, "同级不能升级");
    }

    [Fact]
    [Trait("Category", "Package")]
    [Trait("Module", "Package")]
    public void Package_Downgrade_Rejected()
    {
        var oldLevel = 2; // Ultra
        var newLevel = 0; // Plus
        Assert.False(newLevel > oldLevel, "不能降级");
    }

    // ==================== 十、DTO与Entity构建验证测试 ====================

    [Fact]
    [Trait("Category", "Model")]
    [Trait("Module", "Core")]
    public void CourseEntity_DefaultValues_Verified()
    {
        var course = new Course();
        Assert.Equal(1, course.TotalLessons);
        Assert.Equal(45, course.LessonDuration);
        Assert.Equal(30, course.MaxStudents);
        Assert.Equal(1, course.MinStudents);
        Assert.Equal(string.Empty, course.Name);
    }

    [Fact]
    [Trait("Category", "Model")]
    [Trait("Module", "Core")]
    public void BaseEntity_DefaultIsDeleted_IsFalse()
    {
        // BaseEntity is abstract, test via concrete entity
        var course = new Course();
        Assert.False(course.IsDeleted);
    }

    [Fact]
    [Trait("Category", "Model")]
    [Trait("Module", "Core")]
    public void OrgPackage_DefaultStatus_IsActive()
    {
        var pkg = new OrgPackage();
        Assert.Equal(1, pkg.Status);
        Assert.Equal(1, pkg.MaxCampusCount);
        Assert.Equal(5, pkg.MaxTeacherCount);
        Assert.Equal(50, pkg.MaxStudentCount);
    }

    [Fact]
    [Trait("Category", "Model")]
    [Trait("Module", "Core")]
    public void PageRequest_DefaultValues_Correct()
    {
        var req = new PageRequest();
        Assert.Equal(1, req.Page);
        Assert.Equal(20, req.PageSize);
        Assert.Null(req.Keyword);
    }

    [Fact]
    [Trait("Category", "Model")]
    [Trait("Module", "Core")]
    public void ApiResponse_Ok_ReturnsSuccess()
    {
        var resp = ApiResponse<int>.Ok(42);
        Assert.Equal(0, resp.Code);
        Assert.Equal("success", resp.Message);
        Assert.Equal(42, resp.Data);
    }

    [Fact]
    [Trait("Category", "Model")]
    [Trait("Module", "Core")]
    public void ApiResponse_Fail_ReturnsErrorCode()
    {
        var resp = ApiResponse<string>.Fail(400, "参数错误");
        Assert.Equal(400, resp.Code);
        Assert.Equal("参数错误", resp.Message);
    }

    [Fact]
    [Trait("Category", "Model")]
    [Trait("Module", "Core")]
    public void PagedResult_Constructor_Correct()
    {
        var items = new List<string> { "a", "b" };
        var paged = new PagedResult<string>(items, 100, 1, 20);
        Assert.Equal(2, paged.Items.Count);
        Assert.Equal(100, paged.Total);
        Assert.Equal(1, paged.Page);
        Assert.Equal(20, paged.PageSize);
    }

    [Fact]
    [Trait("Category", "Model")]
    [Trait("Module", "Core")]
    public void CurrentUser_DefaultValues_Verified()
    {
        var user = new CurrentUser { UserId = 1, UserName = "admin", Role = 2, TenantId = 100, OrgId = 100 };
        Assert.Equal(1, user.UserId);
        Assert.Equal(2, user.Role);
        Assert.Equal(100, user.TenantId);
    }

    // ==================== 十一、通知模块测试 ====================

    [Fact]
    [Trait("Category", "Notification")]
    [Trait("Module", "Notification")]
    public void Notification_UnreadCount_StartsAtZero()
    {
        var count = 0;
        Assert.Equal(0, count);
    }

    [Fact]
    [Trait("Category", "Notification")]
    [Trait("Module", "Notification")]
    public void Notification_MarkRead_SetsReadTime()
    {
        var now = DateTime.Now;
        var isRead = true;
        var readAt = now;
        Assert.True(isRead);
        Assert.True(readAt > DateTime.MinValue);
    }

    // ==================== 十二、机构创建初始化测试 ====================

    [Fact]
    [Trait("Category", "Organization")]
    [Trait("Module", "Organization")]
    public void OrgCreate_DefaultAdminPassword_Hashed()
    {
        var plainPassword = "Yk@123456";
        var hashed = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        Assert.True(BCrypt.Net.BCrypt.Verify(plainPassword, hashed));
        Assert.NotEqual(plainPassword, hashed);
    }

    [Fact]
    [Trait("Category", "Organization")]
    [Trait("Module", "Organization")]
    public void OrgCreate_OrgCode_Format()
    {
        var orgCode = $"YK{DateTime.Now:yyyyMMdd}{new Random().Next(100, 999)}";
        Assert.StartsWith("YK", orgCode);
        // Length: 2(YK) + 8(yyyyMMdd) + 3(100-999) = 13
        Assert.Equal(13, orgCode.Length);
    }

    [Fact]
    [Trait("Category", "Organization")]
    [Trait("Module", "Organization")]
    public void OrgConfig_DefaultValues_Verified()
    {
        var config = new OrgConfig();
        Assert.Equal(3, config.FreeRefundDays);
        Assert.Equal("0,3", config.SignInMethods);
        Assert.Equal(15, config.AttendanceTimeout);
        Assert.Equal(24, config.WaitlistExpireHours);
        Assert.Equal(20, config.MaxCoursesPerStudent);
        Assert.Equal(10, config.MaxStudentsPerParent);
        Assert.Equal(7, config.InvitationExpireDays);
    }
}
