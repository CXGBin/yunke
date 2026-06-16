-- 权限控制相关表
-- SysMenu: 系统菜单表（树形结构）
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
DELETE FROM SysMenu;

-- 平台管理菜单
INSERT INTO SysMenu (ParentId, MenuType, Name, Path, Component, Icon, SortOrder, Permission, Visible, Status) VALUES
(0, 1, '系统管理', '/system', NULL, 'SettingOutlined', 1, NULL, 1, 1),
(1, 2, '用户管理', '/system/user', 'System/UserManage/index', 'UserOutlined', 1, NULL, 1, 1),
(1, 3, '用户管理-新增', NULL, NULL, NULL, 1, 'sys:user:add', 1, 1),
(1, 3, '用户管理-编辑', NULL, NULL, NULL, 2, 'sys:user:edit', 1, 1),
(1, 3, '用户管理-删除', NULL, NULL, NULL, 3, 'sys:user:delete', 1, 1),
(1, 3, '用户管理-重置密码', NULL, NULL, NULL, 4, 'sys:user:reset', 1, 1),
(0, 1, '机构管理', '/organization', NULL, 'BankOutlined', 2, NULL, 1, 1),
(7, 2, '机构列表', '/organization/list', 'Organization/OrgList/index', 'TeamOutlined', 1, NULL, 1, 1),
(7, 3, '机构-新增', NULL, NULL, NULL, 1, 'org:add', 1, 1),
(7, 3, '机构-编辑', NULL, NULL, NULL, 2, 'org:edit', 1, 1),
(7, 3, '机构-删除', NULL, NULL, NULL, 3, 'org:delete', 1, 1),
(7, 2, '校区管理', '/organization/campus', 'Organization/CampusManage/index', 'HomeOutlined', 2, NULL, 1, 1),
(7, 3, '校区-新增', NULL, NULL, NULL, 1, 'campus:add', 1, 1),
(7, 3, '校区-编辑', NULL, NULL, NULL, 2, 'campus:edit', 1, 1),
(7, 3, '校区-删除', NULL, NULL, NULL, 3, 'campus:delete', 1, 1),
(0, 1, '人员管理', '/people', NULL, 'ContactsOutlined', 3, NULL, 1, 1),
(15, 2, '教师管理', '/organization/teacher', 'Organization/TeacherManage/index', 'SolutionOutlined', 1, NULL, 1, 1),
(15, 3, '教师-新增', NULL, NULL, NULL, 1, 'teacher:add', 1, 1),
(15, 3, '教师-编辑', NULL, NULL, NULL, 2, 'teacher:edit', 1, 1),
(15, 3, '教师-删除', NULL, NULL, NULL, 3, 'teacher:delete', 1, 1),
(15, 2, '学生管理', '/organization/student', 'Organization/StudentManage/index', 'ReadOutlined', 2, NULL, 1, 1),
(15, 3, '学生-新增', NULL, NULL, NULL, 1, 'student:add', 1, 1),
(15, 3, '学生-编辑', NULL, NULL, NULL, 2, 'student:edit', 1, 1),
(15, 3, '学生-删除', NULL, NULL, NULL, 3, 'student:delete', 1, 1),
(15, 2, '家长管理', '/organization/parent', 'Organization/ParentManage/index', 'UserOutlined', 3, NULL, 1, 1),
(0, 1, '课程管理', '/course', NULL, 'BookOutlined', 4, NULL, 1, 1),
(26, 2, '课程分类', '/course/category', 'Course/CourseCategory/index', 'AppstoreOutlined', 1, NULL, 1, 1),
(26, 2, '课时管理', '/course/lesson-unit', 'Course/LessonUnit/index', 'FieldTimeOutlined', 2, NULL, 1, 1),
(26, 2, '课程列表', '/course/list', 'Course/CourseList/index', 'FileTextOutlined', 3, NULL, 1, 1),
(26, 3, '课程-新增', NULL, NULL, NULL, 1, 'course:add', 1, 1),
(26, 3, '课程-编辑', NULL, NULL, NULL, 2, 'course:edit', 1, 1),
(26, 3, '课程-删除', NULL, NULL, NULL, 3, 'course:delete', 1, 1),
(26, 3, '课程-发布', NULL, NULL, NULL, 4, 'course:publish', 1, 1),
(0, 1, '排课管理', '/schedule', NULL, 'CalendarOutlined', 5, NULL, 1, 1),
(0, 1, '考勤管理', '/attendance', NULL, 'CheckCircleOutlined', 6, NULL, 1, 1),
(33, 2, '考勤记录', '/attendance/manage', 'Attendance/AttendanceManage/index', 'ScheduleOutlined', 1, NULL, 1, 1),
(33, 2, '请假管理', '/attendance/leave', 'Attendance/LeaveManage/index', 'FileProtectOutlined', 2, NULL, 1, 1),
(33, 3, '请假-审批', NULL, NULL, NULL, 1, 'leave:approve', 1, 1),
(0, 1, '评价管理', '/evaluation', NULL, 'LikeOutlined', 7, NULL, 1, 1),
(36, 2, '评价标签', '/evaluation/tag', 'Evaluation/EvaluationTag/index', 'TagOutlined', 1, NULL, 1, 1),
(36, 2, '评价列表', '/evaluation/list', 'Evaluation/EvaluationManage/index', 'MessageOutlined', 2, NULL, 1, 1),
(0, 1, '通知管理', '/notification', NULL, 'BellOutlined', 8, NULL, 1, 1),
(39, 2, '通知管理', '/notification/manage', 'Notification/NotificationManage/index', 'NotificationOutlined', 1, NULL, 1, 1),
(0, 1, '结算管理', '/settlement', NULL, 'MoneyCollectOutlined', 9, NULL, 1, 1),
(41, 2, '课程结算', '/settlement/course', 'Settlement/CourseSettlement/index', 'AccountBookOutlined', 1, NULL, 1, 1),
(0, 1, '套餐管理', '/package', NULL, 'ShoppingOutlined', 10, NULL, 1, 1),
(43, 2, '年费套餐', '/package/annual', 'Package/AnnualPackage/index', 'CrownOutlined', 1, NULL, 1, 1),
(43, 2, '机构套餐', '/package/org', 'Package/OrgPackage/index', 'GiftOutlined', 2, NULL, 1, 1),
(43, 2, '课程套餐', '/package/course', 'Package/CoursePackage/index', 'AppstoreAddOutlined', 3, NULL, 1, 1),
(0, 1, '统计分析', '/statistics', NULL, 'BarChartOutlined', 11, NULL, 1, 1),
(47, 2, '数据看板', '/dashboard', 'Dashboard/index', 'DashboardOutlined', 1, NULL, 1, 1),
(0, 1, '系统配置', '/sysconfig', NULL, 'ToolOutlined', 12, NULL, 1, 1),
(49, 2, '系统配置', '/sysconfig/index', 'System/SysConfig/index', 'ControlOutlined', 1, NULL, 1, 1),
(49, 2, '菜单管理', '/sysconfig/menu', 'System/MenuManage/index', 'MenuOutlined', 2, NULL, 1, 1),
(49, 2, '角色管理', '/sysconfig/role', 'System/RoleManage/index', 'SafetyOutlined', 3, NULL, 1, 1);
GO

-- ===== 种子数据：角色 =====
-- 超级管理员角色
INSERT INTO SysRole (TenantId, RoleName, RoleCode, Description, SortOrder, Status, DataScope)
VALUES (0, N'超级管理员', 'SUPER_ADMIN', N'平台超级管理员，拥有所有权限', 1, 1, 0);

-- 平台运营角色
INSERT INTO SysRole (TenantId, RoleName, RoleCode, Description, SortOrder, Status, DataScope)
VALUES (0, N'平台运营', 'PLATFORM_OPERATOR', N'平台运营人员，管理机构和套餐', 2, 1, 0);

-- 机构管理员角色
INSERT INTO SysRole (TenantId, RoleName, RoleCode, Description, SortOrder, Status, DataScope)
VALUES (0, N'机构管理员', 'ORG_ADMIN', N'教育机构管理员，管理本机构所有业务', 3, 1, 1);

-- 教师角色
INSERT INTO SysRole (TenantId, RoleName, RoleCode, Description, SortOrder, Status, DataScope)
VALUES (0, N'教师', 'TEACHER', N'教师角色，管理课程和考勤', 4, 1, 2);

-- 将超级管理员关联所有菜单
DECLARE @SuperAdminId BIGINT = SCOPE_IDENTITY() - 3;
INSERT INTO SysRoleMenu (RoleId, MenuId)
SELECT @SuperAdminId, Id FROM SysMenu WHERE Status = 1;

-- 将平台运营关联部分菜单（排除系统配置下的菜单管理和角色管理）
DECLARE @PlatformOpId BIGINT = @SuperAdminId + 1;
INSERT INTO SysRoleMenu (RoleId, MenuId)
SELECT @PlatformOpId, Id FROM SysMenu WHERE Status = 1 AND Id NOT IN (
    SELECT sm.Id FROM SysMenu sm
    INNER JOIN SysMenu parent ON sm.ParentId = parent.Id
    WHERE parent.Name = N'系统配置' AND sm.MenuType IN (2, 3)
);
GO

-- 更新平台管理员的SysUser关联超级管理员角色
DECLARE @AdminUserId BIGINT = (SELECT TOP 1 Id FROM SysUser WHERE Phone = '13000000000');
DECLARE @SuperRoleId BIGINT = (SELECT TOP 1 Id FROM SysRole WHERE RoleCode = 'SUPER_ADMIN');
IF @AdminUserId IS NOT NULL AND @SuperRoleId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM SysUserRole WHERE UserId = @AdminUserId AND RoleId = @SuperRoleId)
    BEGIN
        INSERT INTO SysUserRole (UserId, RoleId, TenantId) VALUES (@AdminUserId, @SuperRoleId, 0);
    END
END
GO

PRINT '权限控制表和种子数据创建完成';
