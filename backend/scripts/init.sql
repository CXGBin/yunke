-- ============================================================
-- 云科智教 YunKeEdu 数据库初始化脚本
-- 数据库：MSSQL 2019
-- 共计 36 张表（业务表 + 系统表）
-- 生成时间：2026-06-16
-- ============================================================
USE YunKeEdu;
GO

-- ============================================================
-- 1. Organization（机构表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE Organization (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgCode         varchar(32)     NOT NULL,
    Name            nvarchar(200)   NOT NULL,
    Logo            varchar(500)    NULL,
    ContactPerson   nvarchar(50)    NULL,
    ContactPhone    varchar(20)     NULL,
    Address         nvarchar(300)   NULL,
    Province        nvarchar(50)    NULL,
    City            nvarchar(50)     NULL,
    District        nvarchar(50)    NULL,
    Status          tinyint         NOT NULL DEFAULT 1,
    ExpiredAt       datetime2        NULL,
    Description     nvarchar(500)   NULL,
    ThemeConfig     nvarchar(max)   NULL,
    CurrentPackageId bigint          NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    IsDeleted       bit             DEFAULT 0,
    CONSTRAINT UQ_Organization_OrgCode UNIQUE (OrgCode)
);
CREATE INDEX IX_Organization_TenantId ON Organization(TenantId);
CREATE INDEX IX_Organization_Status ON Organization(Status);
--
GO

-- ============================================================
-- 2. Campus（校区表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE Campus (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CampusCode      varchar(32)     NOT NULL,
    Name            nvarchar(200)   NOT NULL,
    IsDefault       bit             DEFAULT 0,
    ContactPerson   nvarchar(50)    NULL,
    ContactPhone    varchar(20)     NULL,
    Address         nvarchar(300)   NULL,
    Longitude       decimal(10,6)   NULL,
    Latitude        decimal(10,6)   NULL,
    Status          tinyint         NOT NULL DEFAULT 1,
    SortOrder       int             DEFAULT 0,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    IsDeleted       bit             DEFAULT 0,
    CONSTRAINT UQ_Campus_CampusCode UNIQUE (CampusCode)
);
CREATE INDEX IX_Campus_TenantId ON Campus(TenantId);
CREATE INDEX IX_Campus_OrgId ON Campus(OrgId);
CREATE INDEX IX_Campus_TenantId_Status ON Campus(TenantId, Status);
--
GO

-- ============================================================
-- 3. SysUser（系统用户表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE SysUser (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NULL,
    CampusId        bigint          NULL,
    UserCode        varchar(64)     NULL,
    UserName        varchar(64)     NOT NULL,
    [Password]      varchar(256)    NULL,
    RealName        nvarchar(50)    NULL,
    NickName        nvarchar(50)    NULL,
    Avatar          varchar(500)    NULL,
    Phone           varchar(20)     NULL,
    Gender          tinyint         DEFAULT 0,
    BirthDate       date            NULL,
    Grade           nvarchar(20)    NULL,
    Role            tinyint         NOT NULL,
    OpenId          varchar(128)    NULL,
    UnionId         varchar(128)    NULL,
    Status          tinyint         NOT NULL DEFAULT 1,
    LastLoginAt     datetime2        NULL,
    LastLoginIp     varchar(50)     NULL,
    PasswordChangedAt datetime2      NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    IsDeleted       bit             DEFAULT 0,
    CONSTRAINT UQ_SysUser_UserCode UNIQUE (UserCode),
    CONSTRAINT UQ_SysUser_UserName UNIQUE (UserName)
);
CREATE INDEX IX_SysUser_TenantId ON SysUser(TenantId);
CREATE INDEX IX_SysUser_OpenId ON SysUser(OpenId);
CREATE INDEX IX_SysUser_Phone ON SysUser(Phone);
CREATE INDEX IX_SysUser_Role ON SysUser(Role);
--
GO

-- ============================================================
-- 4. UserOrgBinding（用户-机构绑定关系表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE UserOrgBinding (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    UserId          bigint          NOT NULL,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CampusId        bigint          NOT NULL,
    Role            tinyint         NOT NULL,
    UserCode        varchar(64)     NULL,
    Status          tinyint         NOT NULL DEFAULT 1,
    BoundAt         datetime2        NOT NULL DEFAULT GETDATE(),
    BoundVia        tinyint         DEFAULT 0,
    InvitationId    bigint          NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_UserOrgBinding_User_Org_Role UNIQUE (UserId, OrgId, Role)
);
CREATE INDEX IX_UserOrgBinding_UserId_OrgId ON UserOrgBinding(UserId, OrgId);
CREATE INDEX IX_UserOrgBinding_TenantId ON UserOrgBinding(TenantId);
--
GO

-- ============================================================
-- 5. InvitationRecord（邀请记录表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE InvitationRecord (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CampusId        bigint          NOT NULL,
    InviterId       bigint          NOT NULL,
    InviteCode      varchar(32)     NOT NULL,
    InvitedRole     tinyint         NOT NULL,
    InvitedName     nvarchar(50)    NULL,
    InvitedPhone    varchar(20)     NULL,
    Status          tinyint         NOT NULL DEFAULT 0,
    ExpiresAt       datetime2        NOT NULL,
    UsedBy          bigint          NULL,
    UsedAt          datetime2        NULL,
    Remark          nvarchar(200)   NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_InvitationRecord_InviteCode UNIQUE (InviteCode)
);
CREATE INDEX IX_InvitationRecord_InvitedPhone ON InvitationRecord(InvitedPhone);
CREATE INDEX IX_InvitationRecord_OrgId_Status ON InvitationRecord(OrgId, Status);
CREATE INDEX IX_InvitationRecord_TenantId ON InvitationRecord(TenantId);
--
GO

-- ============================================================
-- 6. ParentStudentRelation（家长-学生关联表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE ParentStudentRelation (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    ParentId        bigint          NOT NULL,
    StudentId       bigint          NOT NULL,
    RelationType    tinyint         DEFAULT 0,
    IsPrimary       bit             DEFAULT 0,
    Status          tinyint         DEFAULT 1,
    ConfirmedBy     bigint          NULL,
    ConfirmedAt     datetime2        NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_ParentStudentRelation_Parent_Student UNIQUE (ParentId, StudentId)
);
CREATE INDEX IX_ParentStudentRelation_ParentId ON ParentStudentRelation(ParentId);
CREATE INDEX IX_ParentStudentRelation_StudentId ON ParentStudentRelation(StudentId);
CREATE INDEX IX_ParentStudentRelation_TenantId ON ParentStudentRelation(TenantId);
--
GO

-- ============================================================
-- 7. OrgConfig（机构配置表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE OrgConfig (
    Id                          bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId                    bigint          NOT NULL,
    OrgId                       bigint          NOT NULL,
    FreeRefundDays              int             DEFAULT 3,
    SignInMethods               varchar(100)    DEFAULT '0,3',
    AttendanceTimeout           int             DEFAULT 15,
    EnableEvaluationReview      bit             DEFAULT 0,
    EnableLeaveApproval         bit             DEFAULT 0,
    EnableTeacherPreReview      bit             DEFAULT 0,
    WaitlistExpireHours         int             DEFAULT 24,
    MaxStudentsPerParent         int             DEFAULT 10,
    MaxParentsPerStudent        int             DEFAULT 5,
    MaxCoursesPerStudent         int             DEFAULT 20,
    InvitationExpireDays         int             DEFAULT 7,
    ThemeConfig                 nvarchar(max)   NULL,
    CreatedAt                   datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt                   datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_OrgConfig_OrgId UNIQUE (OrgId)
);
--
GO

-- ============================================================
-- 8. SysConfig（系统全局配置表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE SysConfig (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    ConfigKey       varchar(100)    NOT NULL,
    ConfigValue     nvarchar(max)   NOT NULL,
    ConfigGroup     varchar(50)     NULL,
    Description     nvarchar(200)   NULL,
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_SysConfig_ConfigKey UNIQUE (ConfigKey)
);
--
GO

-- ============================================================
-- 9. EvaluationTag（评价标签配置表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE EvaluationTag (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    Name            nvarchar(20)    NOT NULL,
    TagType         tinyint         DEFAULT 0,
    SortOrder       int             DEFAULT 0,
    Status          tinyint         DEFAULT 1,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_EvaluationTag_TenantId ON EvaluationTag(TenantId);
--
GO

-- ============================================================
-- 10. OrgPackage（套餐定义表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE OrgPackage (
    Id                      bigint          IDENTITY(1,1) PRIMARY KEY,
    PackageName             nvarchar(50)    NOT NULL,
    PackageCode             varchar(32)     NOT NULL,
    PackageLevel            tinyint         NOT NULL,
    Price                   decimal(10,2)   NOT NULL DEFAULT 0,
    Description             nvarchar(500)   NULL,
    Images                  varchar(2000)   NULL,
    MaxCampusCount          int             NOT NULL DEFAULT 1,
    MaxTeacherCount         int             NOT NULL DEFAULT 5,
    MaxStudentCount         int             NOT NULL DEFAULT 50,
    MaxNotificationTypes    int             NOT NULL DEFAULT 0,
    MaxPushChannels         tinyint         NOT NULL DEFAULT 0,
    AnalyticsDimensions     varchar(500)    DEFAULT 'basic',
    SortOrder               int             DEFAULT 0,
    Status                  tinyint         NOT NULL DEFAULT 1,
    CreatedAt               datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt               datetime2        NOT NULL DEFAULT GETDATE(),
    IsDeleted               bit             DEFAULT 0,
    CONSTRAINT UQ_OrgPackage_PackageCode UNIQUE (PackageCode),
    CONSTRAINT UQ_OrgPackage_PackageLevel UNIQUE (PackageLevel)
);
CREATE INDEX IX_OrgPackage_Status ON OrgPackage(Status);
--
GO

-- ============================================================
-- 11. OrgPackageFeature（套餐功能关联表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE OrgPackageFeature (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    PackageId       bigint          NOT NULL,
    FeatureCode     varchar(64)     NOT NULL,
    FeatureName     nvarchar(100)   NOT NULL,
    FeatureGroup    varchar(50)     NULL,
    MinPackageLevel tinyint         NOT NULL DEFAULT 0,
    SortOrder       int             DEFAULT 0,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_OrgPackageFeature_Package_Feature UNIQUE (PackageId, FeatureCode)
);
CREATE INDEX IX_OrgPackageFeature_PackageId ON OrgPackageFeature(PackageId);
--
GO

-- ============================================================
-- 12. OrgSubscription（机构订阅记录表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE OrgSubscription (
    Id                  bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId            bigint          NOT NULL,
    OrgId               bigint          NOT NULL,
    PackageId           bigint          NOT NULL,
    StartDate           date            NOT NULL,
    EndDate             date            NOT NULL,
    Amount              decimal(10,2)   NOT NULL,
    PayStatus           tinyint         NOT NULL DEFAULT 0,
    PayTime             datetime2        NULL,
    PayChannel          varchar(32)     NULL,
    SubscriptionType    tinyint         DEFAULT 0,
    PreSubscriptionId    bigint          NULL,
    Remark              nvarchar(200)   NULL,
    CreatedAt           datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt           datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_OrgSubscription_OrgId_Status ON OrgSubscription(OrgId, Status);
--
GO

-- ============================================================
-- 13. PackageUpgradeOrder（套餐升级订单表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE PackageUpgradeOrder (
    Id                  bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId            bigint          NOT NULL,
    OrgId               bigint          NOT NULL,
    OldSubscriptionId    bigint          NOT NULL,
    OldPackageId       bigint          NOT NULL,
    NewPackageId       bigint          NOT NULL,
    NewSubscriptionId    bigint          NULL,
    OldPackagePrice     decimal(10,2)   NOT NULL,
    NewPackagePrice     decimal(10,2)   NOT NULL,
    UsedMonths          int             NOT NULL,
    UnusedMonths        int             NOT NULL,
    OldMonthlyPrice     decimal(10,2)   NOT NULL,
    DiscountAmount      decimal(10,2)   NOT NULL,
    PayAmount           decimal(10,2)   NOT NULL,
    PayStatus           tinyint         NOT NULL DEFAULT 0,
    PayTime             datetime2        NULL,
    PayChannel          varchar(32)     NULL,
    Remark              nvarchar(200)   NULL,
    CreatedAt           datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt           datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_PackageUpgradeOrder_OrgId_PayStatus ON PackageUpgradeOrder(OrgId, PayStatus);
--
GO

-- ============================================================
-- 14. Course（课程表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE Course (
    Id                      bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId                bigint          NOT NULL,
    OrgId                   bigint          NULL,
    CampusId                bigint          NOT NULL,
    CourseCode              varchar(32)     NULL,
    Name                    nvarchar(200)   NOT NULL,
    CategoryId              bigint          NULL,
    Description             nvarchar(max)   NULL,
    CoverImage              varchar(500)    NULL,
    TotalLessons            int             NOT NULL DEFAULT 1,
    LessonDuration          int             NOT NULL DEFAULT 45,
    Difficulty              tinyint         DEFAULT 0,
    OriginalPrice           decimal(10,2)   NOT NULL DEFAULT 0,
    DiscountPrice           decimal(10,2)   DEFAULT 0,
    MaxStudents             int             DEFAULT 30,
    MinStudents             int             DEFAULT 1,
    EnrollmentDeadline      datetime2        NULL,
    Tags                    varchar(500)    NULL,
    Status                  tinyint         NOT NULL DEFAULT 0,
    TeacherId               bigint          NOT NULL,
    SettlementType          tinyint         NOT NULL DEFAULT 0,
    FixedFeePerLesson       decimal(10,2)   DEFAULT 0,
    StudentCountCommission  decimal(10,2)   DEFAULT 0,
    SortOrder               int             DEFAULT 0,
    IsRecommend             bit             DEFAULT 0,
    ScheduledPublishTime    datetime2        NULL,
    ScheduledOfflineTime    datetime2        NULL,
    ViewCount               int             DEFAULT 0,
    CreatedBy               bigint          NOT NULL,
    CreatedAt               datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt               datetime2        NOT NULL DEFAULT GETDATE(),
    IsDeleted               bit             DEFAULT 0,
    CONSTRAINT UQ_Course_CourseCode UNIQUE (CourseCode)
);
CREATE INDEX IX_Course_OrgId_Status ON Course(OrgId, Status);
CREATE INDEX IX_Course_OrgId_TeacherId ON Course(OrgId, TeacherId);
CREATE INDEX IX_Course_OrgId_CampusId ON Course(OrgId, CampusId);
CREATE INDEX IX_Course_TenantId ON Course(TenantId);
--
GO

-- ============================================================
-- 15. CourseCategory（课程分类表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE CourseCategory (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    ParentId        bigint          DEFAULT 0,
    Name            nvarchar(50)    NOT NULL,
    Icon            varchar(200)    NULL,
    SortOrder       int             DEFAULT 0,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    IsDeleted       bit             DEFAULT 0
);
CREATE INDEX IX_CourseCategory_TenantId ON CourseCategory(TenantId);
--
GO

-- ============================================================
-- 16. CourseAttachment（课程附件表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE CourseAttachment (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    CourseId        bigint          NOT NULL,
    FileName        nvarchar(200)   NOT NULL,
    FileUrl         varchar(500)    NOT NULL,
    FileSize        bigint          DEFAULT 0,
    FileType        varchar(50)     NULL,
    SortOrder       int             DEFAULT 0,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_CourseAttachment_CourseId ON CourseAttachment(CourseId);
--
GO

-- ============================================================
-- 17. CoursePackage（课程套餐表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE CoursePackage (
    Id                      bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId                bigint          NOT NULL,
    OrgId                   bigint          NOT NULL,
    CampusId                bigint          NOT NULL,
    PackageName             nvarchar(200)   NOT NULL,
    Description             nvarchar(max)   NULL,
    CoverImage              varchar(500)    NULL,
    TotalPrice              decimal(10,2)   NOT NULL,
    CourseCount             int             NOT NULL,
    Status                  tinyint         NOT NULL DEFAULT 0,
    BuyCount                int             DEFAULT 0,
    SortOrder               int             DEFAULT 0,
    IsRecommend             bit             DEFAULT 0,
    ScheduledPublishTime    datetime2        NULL,
    ScheduledOfflineTime    datetime2        NULL,
    CreatedBy               bigint          NOT NULL,
    CreatedAt               datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt               datetime2        NOT NULL DEFAULT GETDATE(),
    IsDeleted               bit             DEFAULT 0
);
CREATE INDEX IX_CoursePackage_TenantId ON CoursePackage(TenantId);
CREATE INDEX IX_CoursePackage_OrgId ON CoursePackage(OrgId);
--
GO

-- ============================================================
-- 18. CoursePackageItem（套餐-课程关联表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE CoursePackageItem (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    PackageId       bigint          NOT NULL,
    CourseId        bigint          NOT NULL,
    CourseName      nvarchar(200)   NOT NULL,
    UnitPrice       decimal(10,2)   NOT NULL,
    SortOrder       int             DEFAULT 0,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_CoursePackageItem_Package_Course UNIQUE (PackageId, CourseId)
);
CREATE INDEX IX_CoursePackageItem_PackageId ON CoursePackageItem(PackageId);
--
GO

-- ============================================================
-- 19. CourseEnrollment（选课/报名表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE CourseEnrollment (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CampusId        bigint          NULL,
    CourseId        bigint          NOT NULL,
    StudentId       bigint          NOT NULL,
    ParentId        bigint          NULL,
    Status          tinyint         NOT NULL DEFAULT 0,
    EnrolledAt      datetime2        NOT NULL DEFAULT GETDATE(),
    Remark          nvarchar(200)   NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_CourseEnrollment_CourseId_Status ON CourseEnrollment(CourseId, Status);
CREATE INDEX IX_CourseEnrollment_StudentId_Status ON CourseEnrollment(StudentId, Status);
CREATE INDEX IX_CourseEnrollment_TenantId ON CourseEnrollment(TenantId);
--
GO

-- ============================================================
-- 20. WaitList（候补表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE WaitList (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CampusId        bigint          NULL,
    CourseId        bigint          NOT NULL,
    StudentId       bigint          NOT NULL,
    Status          tinyint         NOT NULL DEFAULT 0,
    JoinedAt        datetime2        NOT NULL DEFAULT GETDATE(),
    NotifiedAt      datetime2        NULL,
    ExpiresAt       datetime2        NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_WaitList_CourseId_StudentId ON WaitList(CourseId, StudentId);
CREATE INDEX IX_WaitList_StudentId_Status ON WaitList(StudentId, Status);
--
GO

-- ============================================================
-- 21. LessonUnit（课节定义表，简化版）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE LessonUnit (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CourseId        bigint          NOT NULL,
    LessonNo        int             NOT NULL,
    Title           nvarchar(200)   NOT NULL,
    Description     nvarchar(500)   NULL,
    SortOrder       int             NOT NULL,
    Status          tinyint         DEFAULT 1,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    IsDeleted       bit             DEFAULT 0
);
CREATE INDEX IX_LessonUnit_CourseId ON LessonUnit(CourseId);
CREATE INDEX IX_LessonUnit_TenantId ON LessonUnit(TenantId);
--
GO

-- ============================================================
-- 22. CourseSchedule（排课计划表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE CourseSchedule (
    Id                  bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId            bigint          NOT NULL,
    OrgId               bigint          NOT NULL,
    CampusId            bigint          NOT NULL,
    CourseId            bigint          NOT NULL,
    TeacherId           bigint          NOT NULL,
    LessonDate          date            NOT NULL,
    StartTime           time            NOT NULL,
    EndTime             time            NOT NULL,
    LessonNo            int             NULL,
    LessonTitle         nvarchar(200)   NULL,
    Remark              nvarchar(500)   NULL,
    Status              tinyint         NOT NULL DEFAULT 0,
    CancelReason        nvarchar(200)   NULL,
    IsRescheduled       bit             DEFAULT 0,
    OriginalScheduleId  bigint          NULL,
    CreatedBy           bigint          NOT NULL,
    PublishedAt         datetime2        NULL,
    CreatedAt           datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt           datetime2        NOT NULL DEFAULT GETDATE(),
    IsDeleted           bit             DEFAULT 0
);
CREATE INDEX IX_CourseSchedule_OrgId_CampusId_Date ON CourseSchedule(OrgId, CampusId, LessonDate);
CREATE INDEX IX_CourseSchedule_TeacherId_Date ON CourseSchedule(TeacherId, LessonDate);
CREATE INDEX IX_CourseSchedule_CourseId ON CourseSchedule(CourseId);
CREATE INDEX IX_CourseSchedule_TenantId ON CourseSchedule(TenantId);
--
GO

-- ============================================================
-- 23. ScheduleRecurrence（循环排课规则表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE ScheduleRecurrence (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CourseId        bigint          NOT NULL,
    TeacherId       bigint          NOT NULL,
    WeekDays        varchar(20)     NOT NULL,
    StartTime       time            NOT NULL,
    EndTime         time            NOT NULL,
    StartDate       date            NOT NULL,
    EndDate         date            NOT NULL,
    TotalLessons    int             NOT NULL,
    GeneratedLessons int             DEFAULT 0,
    Status          tinyint         DEFAULT 0,
    CreatedBy       bigint          NOT NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_ScheduleRecurrence_CourseId ON ScheduleRecurrence(CourseId);
CREATE INDEX IX_ScheduleRecurrence_TenantId ON ScheduleRecurrence(TenantId);
--
GO

-- ============================================================
-- 24. ScheduleChangeLog（排课变更记录表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE ScheduleChangeLog (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    ScheduleId      bigint          NOT NULL,
    ChangeType      tinyint         NOT NULL,
    OldData         nvarchar(max)   NULL,
    NewData         nvarchar(max)   NULL,
    Reason          nvarchar(200)   NULL,
    OperatorId      bigint          NOT NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_ScheduleChangeLog_ScheduleId ON ScheduleChangeLog(ScheduleId);
CREATE INDEX IX_ScheduleChangeLog_TenantId ON ScheduleChangeLog(TenantId);
--
GO

-- ============================================================
-- 25. Attendance（签到记录表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE Attendance (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CampusId        bigint          NULL,
    ScheduleId      bigint          NOT NULL,
    CourseId        bigint          NOT NULL,
    StudentId       bigint          NOT NULL,
    Status          tinyint         NOT NULL DEFAULT 0,
    SignInTime      datetime2        NULL,
    SignMethod      tinyint         NULL,
    Remark          nvarchar(200)   NULL,
    OperatorId      bigint          NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Attendance_Schedule_Student UNIQUE (ScheduleId, StudentId)
);
CREATE INDEX IX_Attendance_ScheduleId ON Attendance(ScheduleId);
CREATE INDEX IX_Attendance_StudentId ON Attendance(StudentId);
CREATE INDEX IX_Attendance_TenantId ON Attendance(TenantId);
--
GO

-- ============================================================
-- 26. SignInQRCode（签到二维码表）【预留】
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE SignInQRCode (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CampusId        bigint          NULL,
    ScheduleId      bigint          NULL,
    Token           varchar(128)    NOT NULL,
    ExpiredAt       datetime2        NULL,
    Status          tinyint         DEFAULT 0,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_SignInQRCode_Token ON SignInQRCode(Token);
--
GO

-- ============================================================
-- 27. LeaveRequest（请假记录表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE LeaveRequest (
    Id                  bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId            bigint          NOT NULL,
    OrgId               bigint          NOT NULL,
    StudentId           bigint          NOT NULL,
    CourseId            bigint          NOT NULL,
    ScheduleId          bigint          NULL,
    LeaveType           tinyint         DEFAULT 0,
    StartDate           date            NOT NULL,
    EndDate             date            NOT NULL,
    Reason              nvarchar(500)   NOT NULL,
    Status              tinyint         DEFAULT 1,
    ApplicantId         bigint          NOT NULL,
    PreReviewerId        bigint          NULL,
    PreReviewedAt       datetime2        NULL,
    PreReviewRemark     nvarchar(200)   NULL,
    ApproverId          bigint          NULL,
    ApprovedAt          datetime2        NULL,
    ApproveRemark       nvarchar(200)   NULL,
    CreatedAt           datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt           datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_LeaveRequest_StudentId ON LeaveRequest(StudentId);
CREATE INDEX IX_LeaveRequest_OrgId ON LeaveRequest(OrgId);
CREATE INDEX IX_LeaveRequest_TenantId ON LeaveRequest(TenantId);
--
GO

-- ============================================================
-- 28. CourseEvaluation（课程评价表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE CourseEvaluation (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CampusId        bigint          NULL,
    CourseId        bigint          NOT NULL,
    ScheduleId      bigint          NOT NULL,
    EvaluatorId     bigint          NOT NULL,
    TargetId        bigint          NOT NULL,
    EvalType        tinyint         NOT NULL,
    CourseRating    tinyint         NULL,
    TeacherRating   tinyint         NULL,
    LessonRating    tinyint         NULL,
    Content         nvarchar(max)   NULL,
    Tags            varchar(200)    NULL,
    Images          varchar(1000)   NULL,
    IsAnonymous     bit             DEFAULT 0,
    Status          tinyint         DEFAULT 1,
    ReplyContent    nvarchar(max)   NULL,
    ReplyBy         bigint          NULL,
    ReplyAt         datetime2        NULL,
    IsTop           bit             DEFAULT 0,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_CourseEvaluation_Schedule_Evaluator_Target_EvalType UNIQUE (ScheduleId, EvaluatorId, TargetId, EvalType)
);
CREATE INDEX IX_CourseEvaluation_ScheduleId ON CourseEvaluation(ScheduleId);
CREATE INDEX IX_CourseEvaluation_TargetId_EvalType ON CourseEvaluation(TargetId, EvalType);
CREATE INDEX IX_CourseEvaluation_EvaluatorId ON CourseEvaluation(EvaluatorId);
CREATE INDEX IX_CourseEvaluation_TenantId ON CourseEvaluation(TenantId);
--
GO

-- ============================================================
-- 29. EvaluationReply（追加评价/回复表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE EvaluationReply (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    EvaluationId    bigint          NOT NULL,
    Content         nvarchar(max)   NOT NULL,
    Images          varchar(1000)   NULL,
    ReplyType       tinyint         NOT NULL,
    ReplyById       bigint          NOT NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_EvaluationReply_EvaluationId ON EvaluationReply(EvaluationId);
CREATE INDEX IX_EvaluationReply_TenantId ON EvaluationReply(TenantId);
--
GO

-- ============================================================
-- 30. CourseFeeSettlement（结算规则表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE CourseFeeSettlement (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    CourseId        bigint          NOT NULL,
    SettlementType  tinyint         NOT NULL,
    FixedAmount     decimal(10,2)   DEFAULT 0,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_CourseFeeSettlement_CourseId UNIQUE (CourseId)
);
--
GO

-- ============================================================
-- 31. TeacherWallet（教师钱包/余额表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE TeacherWallet (
    Id                  bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId            bigint          NOT NULL,
    OrgId               bigint          NOT NULL,
    TeacherId           bigint          NOT NULL,
    Balance             decimal(12,2)   NOT NULL DEFAULT 0,
    TotalIncome         decimal(12,2)   NOT NULL DEFAULT 0,
    TotalWithdrawn      decimal(12,2)   NOT NULL DEFAULT 0,
    LastSettlementAt    datetime2        NULL,
    CreatedAt           datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt           datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_TeacherWallet_TenantId_TeacherId UNIQUE (TenantId, TeacherId)
);
--
GO

-- ============================================================
-- 32. FeeSettlementRecord（结算记录表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE FeeSettlementRecord (
    Id                  bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId            bigint          NOT NULL,
    OrgId               bigint          NOT NULL,
    CourseId            bigint          NOT NULL,
    ScheduleId          bigint          NOT NULL,
    TeacherId           bigint          NOT NULL,
    WalletId            bigint          NOT NULL,
    Amount              decimal(10,2)   NOT NULL,
    SettlementType      tinyint         NOT NULL,
    StudentCount        int             DEFAULT 0,
    SettlementDate      date            NOT NULL,
    SettledAt           datetime2        NOT NULL DEFAULT GETDATE(),
    TriggerType         tinyint         NOT NULL,
    Status              tinyint         NOT NULL DEFAULT 1,
    Remark              nvarchar(200)   NULL,
    CreatedAt           datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_FeeSettlementRecord_TeacherId_SettlementDate ON FeeSettlementRecord(TeacherId, SettlementDate);
CREATE INDEX IX_FeeSettlementRecord_CourseId ON FeeSettlementRecord(CourseId);
CREATE INDEX IX_FeeSettlementRecord_TenantId ON FeeSettlementRecord(TenantId);
--
GO

-- ============================================================
-- 33. NotificationTemplate（通知模板表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE NotificationTemplate (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NULL,
    TemplateName    nvarchar(100)   NOT NULL,
    TemplateCode    varchar(64)     NOT NULL,
    NotifyType      tinyint         NOT NULL,
    Channel         tinyint         NOT NULL,
    TitleTemplate   nvarchar(200)   NOT NULL,
    ContentTemplate nvarchar(max)   NOT NULL,
    Variables       varchar(500)    NULL,
    Status          tinyint         DEFAULT 1,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_NotificationTemplate_TemplateCode UNIQUE (TemplateCode)
);
CREATE INDEX IX_NotificationTemplate_TenantId ON NotificationTemplate(TenantId);
--
GO

-- ============================================================
-- 34. NotificationLog（通知日志表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE NotificationLog (
    Id              bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId        bigint          NOT NULL,
    OrgId           bigint          NOT NULL,
    RecipientId     bigint          NOT NULL,
    NotifyType      tinyint         NOT NULL,
    Channel         tinyint         NOT NULL,
    Title           nvarchar(200)   NOT NULL,
    Content         nvarchar(max)   NOT NULL,
    IsRead          bit             DEFAULT 0,
    ReadAt          datetime2        NULL,
    RelateType      varchar(32)     NULL,
    RelateId        bigint          NULL,
    SendStatus      tinyint         NOT NULL DEFAULT 0,
    SendTime        datetime2        NULL,
    ErrorMessage    nvarchar(500)   NULL,
    CreatedAt       datetime2        NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_NotificationLog_RecipientId_IsRead ON NotificationLog(RecipientId, IsRead);
CREATE INDEX IX_NotificationLog_RecipientId_CreatedAt ON NotificationLog(RecipientId, CreatedAt);
CREATE INDEX IX_NotificationLog_TenantId ON NotificationLog(TenantId);
--
GO

-- ============================================================
-- 35. NotificationConfig（通知配置表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE NotificationConfig (
    Id                  bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId            bigint          NOT NULL,
    OrgId               bigint          NOT NULL,
    NotifyTypes         varchar(500)    NULL,
    ReminderMinutes     int             DEFAULT 30,
    QuietHoursStart     time            NULL,
    QuietHoursEnd       time            NULL,
    IsQuietEnabled      bit             DEFAULT 0,
    CreatedAt           datetime2        NOT NULL DEFAULT GETDATE(),
    UpdatedAt           datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_NotificationConfig_OrgId UNIQUE (OrgId)
);
--
GO

-- ============================================================
-- 36. StatisticsDailySnapshot（每日统计快照表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE StatisticsDailySnapshot (
    Id                      bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId                bigint          NOT NULL,
    OrgId                   bigint          NOT NULL,
    StatDate                date            NOT NULL,
    NewStudents             int             DEFAULT 0,
    ActiveCourses           int             DEFAULT 0,
    TotalEnrollments        int             DEFAULT 0,
    TotalAttendanceRate     decimal(5,2)   DEFAULT 0,
    TotalEvaluations        int             DEFAULT 0,
    AvgRating               decimal(3,2)   DEFAULT 0,
    DailyRevenue            decimal(12,2)  DEFAULT 0,
    TeacherFeeExpense        decimal(12,2)  DEFAULT 0,
    NetIncome               decimal(12,2)   DEFAULT 0,
    CreatedAt               datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_StatisticsDailySnapshot_OrgId_Date UNIQUE (OrgId, StatDate)
);
--
GO

-- ============================================================
-- 37. StatisticsCourseSnapshot（课程统计快照表）
-- ============================================================
-- (table will be created if not exists)
--
CREATE TABLE StatisticsCourseSnapshot (
    Id                      bigint          IDENTITY(1,1) PRIMARY KEY,
    TenantId                bigint          NOT NULL,
    OrgId                   bigint          NOT NULL,
    CourseId                bigint          NOT NULL,
    StatMonth               date            NOT NULL,
    EnrollmentCount         int             DEFAULT 0,
    AttendanceRate          decimal(5,2)   DEFAULT 0,
    AvgRating               decimal(3,2)   DEFAULT 0,
    EvaluationCount         int             DEFAULT 0,
    CompletedLessons        int             DEFAULT 0,
    TotalLessons            int             DEFAULT 0,
    CourseRevenue           decimal(12,2)   DEFAULT 0,
    CourseExpense           decimal(12,2)   DEFAULT 0,
    ConsumedLessons         int             DEFAULT 0,
    RemainingLessons        int             DEFAULT 0,
    CreatedAt               datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_StatisticsCourseSnapshot_CourseId_Month UNIQUE (CourseId, StatMonth)
);
CREATE INDEX IX_StatisticsCourseSnapshot_TenantId ON StatisticsCourseSnapshot(TenantId);
--
GO

-- ============================================================
-- 建表脚本执行完毕
-- 共计 36 张业务表 + 系统表
-- ============================================================
PRINT '初始化完成：36 张表已创建';
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'SysMenu')
BEGIN
    CREATE TABLE SysMenu (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        ParentId BIGINT NOT NULL DEFAULT 0,
        MenuType INT NOT NULL DEFAULT 1,       -- 1=目录 2=菜单 3=按钮
        Name NVARCHAR(50) NOT NULL,
        Path NVARCHAR(200) NULL,
        Component NVARCHAR(200) NULL,
        Icon NVARCHAR(50) NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        Permission NVARCHAR(100) NULL,         -- 权限码 如 sys:user:add
        BtnType NVARCHAR(20) NULL,             -- view/edit/delete/add/import/export
        Visible INT NOT NULL DEFAULT 1,       -- 0=隐藏 1=显示
        Status INT NOT NULL DEFAULT 1,         -- 0=禁用 1=启用
        Description NVARCHAR(200) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
    CREATE INDEX IX_SysMenu_ParentId ON SysMenu(ParentId);
END
GO

-- SysRole: 系统角色表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'SysRole')
BEGIN
    CREATE TABLE SysRole (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        TenantId BIGINT NOT NULL DEFAULT 0,
        RoleName NVARCHAR(50) NOT NULL,
        RoleCode NVARCHAR(100) NULL,
        Description NVARCHAR(200) NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        Status INT NOT NULL DEFAULT 1,
        DataScope INT NOT NULL DEFAULT 1,      -- 0=全部 1=本机构 2=本校区 3=仅本人
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
GO

-- SysRoleMenu: 角色-菜单关联表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'SysRoleMenu')
BEGIN
    CREATE TABLE SysRoleMenu (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        RoleId BIGINT NOT NULL,
        MenuId BIGINT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_SysRoleMenu_Role FOREIGN KEY (RoleId) REFERENCES SysRole(Id),
        CONSTRAINT FK_SysRoleMenu_Menu FOREIGN KEY (MenuId) REFERENCES SysMenu(Id)
    );
    CREATE INDEX IX_SysRoleMenu_RoleId ON SysRoleMenu(RoleId);
END
GO

-- SysUserRole: 用户-角色关联表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'SysUserRole')
BEGIN
    CREATE TABLE SysUserRole (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        UserId BIGINT NOT NULL,
        RoleId BIGINT NOT NULL,
        TenantId BIGINT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_SysUserRole_User FOREIGN KEY (UserId) REFERENCES SysUser(Id),
        CONSTRAINT FK_SysUserRole_Role FOREIGN KEY (RoleId) REFERENCES SysRole(Id)
    );
    CREATE INDEX IX_SysUserRole_UserId ON SysUserRole(UserId);
END
GO

-- ===== 种子数据：菜单树 =====
-- 清空旧数据
DELETE FROM SysRoleMenu WHERE RoleId IN (SELECT Id FROM SysRole WHERE TenantId = 0);
DELETE FROM SysRole WHERE TenantId = 0;

-- ========== 补充复合索引（Round1审查 P1-31/P1-32） ==========

-- P1-31: NotificationLog 按收件人+已读状态+时间查询（高频）
CREATE NONCLUSTERED INDEX IX_NotificationLog_Recipient_IsRead_Created
ON NotificationLog(RecipientId, IsRead, CreatedAt);

-- P1-32: CourseEvaluation 按课程+排课查评价
CREATE NONCLUSTERED INDEX IX_CourseEvaluation_CourseId_ScheduleId
ON CourseEvaluation(CourseId, ScheduleId);

-- P2-17: StatisticsDailySnapshot 按租户+日期查统计
CREATE NONCLUSTERED INDEX IX_StatisticsDailySnapshot_TenantId_StatDate
ON StatisticsDailySnapshot(TenantId, StatDate);

-- P2-18: StatisticsCourseSnapshot 按租户+月份查统计
CREATE NONCLUSTERED INDEX IX_StatisticsCourseSnapshot_TenantId_StatMonth
ON StatisticsCourseSnapshot(TenantId, StatMonth);
