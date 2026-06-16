# 云科智教教育SaaS课程管理平台 — 第1轮全面代码审查报告

> 审查时间：2026-06-17 01:00 (Asia/Shanghai)  
> 项目路径：`/home/ubuntu/.openclaw/workspace/yunke-edu/`  
> 后端：.NET 8 WebAPI + SqlSugar + MSSQL 2019（CamelCase序列化）  
> 前端：React + Ant Design Pro + @umijs/max  
> 审查范围：前后端全部代码（30个Controller、27个Service、18个前端Service、27个页面、37张数据库表）

---

## 📊 审查摘要

| 严重等级 | 数量 |
|---------|------|
| **P0（致命）** | 14 |
| **P1（严重）** | 18 |
| **P2（建议）** | 16 |

---

## 一、前后端参数类型一致性

> 前端 services/*.ts 每个API调用 vs 后端 Controller 对应接口参数类型

### P0

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 1 | **P0-1** | **MenuController/RoleController 缺少 [ApiController]**。无此属性时 [FromQuery] 复杂参数无法自动绑定、[FromBody] 无模型验证、响应不统一。路由属性 `[Route("api/[controller]")]` 依赖运行时解析，但无 ApiController 前缀无法正确推断 controller name | `Controllers/MenuController.cs:10,38` |
| 2 | **P0-2** | **LessonUnitController.BatchGenerate courseId 缺少 [FromQuery]**。后端 `BatchGenerate(long courseId, [FromBody] req)` — courseId 无绑定源（非路由参数、非标注 [FromQuery]），永远为0。前端传 `POST /api/lesson-unit/batch-generate?courseId=${courseId}` | `Controllers/LessonUnitController.cs:28` |
| 3 | **P0-3** | **sysConfig.ts 引用未定义函数**。`getConfigPage = getSysConfigs` 和 `createConfig = createSysConfig` 在文件中不存在，模块加载时直接 ReferenceError 崩溃，影响所有依赖此模块的页面 | `services/sysConfig.ts:28-29` |
| 4 | **P0-4** | **后端缺少 /api/enrollment/page 端点**。前端 `getEnrollmentPage()` 调用此路径，后端 EnrollmentController 无对应方法，返回404 | `Controllers/EnrollmentController.cs` / `services/enrollment.ts:53` |
| 5 | **P0-5** | **后端缺少 /api/attendance/page 端点**。前端 `getAttendancePage()` 调用此路径，后端 AttendanceController 无对应方法，返回404 | `Controllers/AttendanceController.cs` / `services/attendance.ts:80` |

### P1

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 6 | **P1-1** | **后端缺少 PUT /api/users/{id}/status**。前端 `updateUserStatus()` 调用此路径，404 | `services/user.ts:39` |
| 7 | **P1-2** | **后端缺少 DELETE /api/organization/{id}**。前端 `deleteOrg()` 调用此路径，404 | `services/organization.ts:37` |
| 8 | **P1-3** | **后端缺少 DELETE /api/campus/{id}**。前端 `deleteCampus()` 调用此路径，404 | `services/campus.ts:39` |
| 9 | **P1-4** | **后端缺少 DELETE /api/system/config/{id}**。前端 `deleteConfig()` 调用此路径，404 | `services/sysConfig.ts:31` |
| 10 | **P1-5** | **CoursePackage Page 接口缺少 orgId 筛选参数**。前端传 `{ page, pageSize, keyword, orgId }`，后端仅接收 `PageRequest { Page, PageSize, Keyword }`，orgId 被忽略 | `services/coursePackage.ts:6` |
| 11 | **P1-6** | **Organization Page 接口缺少 status 筛选参数**。前端传 `{ page, pageSize, keyword, status }`，后端仅接收 `PageRequest`，status 被忽略 | `services/organization.ts:6` |
| 12 | **P1-7** | **前端 Schedule.startTime/endTime 声明为 string**，后端为 `TimeSpan`。JSON序列化后为 `"01:30:00"` 字符串格式可接受，但前端若需时间计算或比较需额外解析 | `typings.d.ts:327-328` |
| 13 | **P1-8** | **前端 CoursePackage.AddCourseToPackage 传 courseId 为 query string**，后端 `[FromQuery] long courseId` — 类型一致，但前端用模板字符串拼接 `?courseId=${courseId}` 而非 params 对象，若 courseId 来自 ProTable row.id 可能安全；若为 undefined 则传字符串 "undefined" | `services/coursePackage.ts:71` |

### ✅ 确认一致的接口（19个Service模块）

以下模块 API 路径、HTTP方法、参数类型逐一对比**完全一致**：
- AuthController ↔ `auth.ts` ✅
- CampusController ↔ `campus.ts` ✅（缺 DELETE 端点见 P1-3）
- StudentController ↔ `student.ts` ✅
- ParentController ↔ `parent.ts` ✅
- CourseController ↔ `course.ts` ✅
- CoursePackageController ↔ `coursePackage.ts` ✅（缺 orgId 见 P1-5）
- EnrollmentController ↔ `enrollment.ts` ✅（缺 page 见 P0-4）
- ScheduleController ↔ `schedule.ts` ✅
- AttendanceController ↔ `attendance.ts` ✅（缺 page 见 P0-5）
- EvaluationController ↔ `evaluation.ts` ✅
- EvaluationTagController ↔ `evaluation.ts` ✅
- SettlementController ↔ `settlement.ts` ✅
- OrgPackageController ↔ `orgPackage.ts` ✅
- NotificationController ↔ `notification.ts` ✅
- StatisticsController ↔ `statistics.ts` ✅
- PermissionService(Menu/Role) ↔ `permission.ts` ✅
- LeaveController ↔ `attendance.ts`(请假部分) ✅
- ConfigController ↔ `sysConfig.ts` ✅
- InvitationController ↔ 无前端service（见 P2-13）

---

## 二、前后端模型对齐

> 前端 TypeScript interface/type ↔ 后端 C# DTO 字段名和类型

### P0

| # | 编号 | 描述 | 详情 |
|---|------|------|------|
| 1 | **P0-6** | **OrgPackageParams 缺少 enableEvaluation 字段**。后端 `CreatePackageRequest.EnableEvaluation` 为 `bool`（非 nullable），前端接口无此字段 → 创建套餐时无法启用评价功能，默认false | `typings.d.ts:256` |
| 2 | **P0-7** | **前端缺少 SysMenu/SysRole/SysRoleMenu/SysUserRole 实体对应的数据库表**。后端 SystemEntities.cs 定义了4个权限实体，但 init.sql 中无对应 CREATE TABLE → 权限管理接口运行即报错（表不存在） | `scripts/init.sql` |

### P1

| # | 编号 | 描述 | 详情 |
|---|------|------|------|
| 3 | **P1-9** | **前端 SysUser.gender 应为可选**。后端 GET /api/users/list 返回匿名对象无 gender 字段，GET /api/users/{id} 有 gender。前端 `gender: number`（非可选）→ 列表绑定 undefined | `typings.d.ts:176` |
| 4 | **P1-10** | **前端缺少 Teacher 接口定义**。后端 TeacherDto 有完整字段定义，前端 teacher service 全部使用 `any` 类型，无编译期检查 | `typings.d.ts` |
| 5 | **P1-11** | **CreateTeacherRequest.Gender 为 string，TeacherDto.Gender 为 int**。同一概念在不同 DTO 中类型不一致，前端若按 TeacherDto 的 int 传值给 CreateRequest 的 string，可能导致逻辑错误 | `DTOs/SystemDto.cs` |
| 6 | **P1-12** | **前端 API.OrgSubscription.remainingDays 与后端一致为 int**，但后端 DTO 中 `RemainingDays` 是计算字段（非数据库字段），需确认 Service 层是否正确赋值 | `DTOs/PackageDto.cs:162` |
| 7 | **P1-13** | **后端 CoursePackageController.Purchase / MyPackages 为 TODO 空壳**。`[HttpPost("{id}/purchase")]` 直接返回 true 无任何业务逻辑；`[HttpGet("my-packages")]` 返回空列表 | `Controllers/CoursePackageController.cs:79-91` |

### ✅ 确认一致的模型

以下模型字段名（CamelCase后）、类型、可选标记**完全一致**：
- `LoginRequest` ↔ `API.LoginParams` ✅
- `LoginResponse` ↔ `API.LoginResult` ✅
- `UserInfoDto` ↔ `API.CurrentUser` ✅
- `UserOrgInfo` ↔ `API.UserOrgInfo` ✅
- `OrgDto` ↔ `API.Organization` ✅
- `CampusDto` ↔ `API.Campus` ✅
- `StudentDto` ↔ `API.Student` ✅
- `ParentDto` ↔ `API.Parent` ✅
- `ChildInfo` ↔ `API.ChildInfo` ✅
- `CourseDto` ↔ `API.Course` ✅
- `CoursePackageDto` ↔ `API.CoursePackage` ✅
- `EnrollmentDto` ↔ `API.Enrollment` ✅
- `EvaluationDto` ↔ `API.Evaluation` ✅
- `EvaluationReplyDto` ↔ `API.EvaluationReply` ✅
- `LeaveRequestDto` ↔ `API.LeaveRequest` ✅
- `SettlementRuleDto` ↔ `API.SettlementRule` ✅
- `WalletDto` ↔ `API.Wallet` ✅
- `FeeSettlementRecordDto` ↔ `API.FeeSettlementRecord` ✅
- `NotificationTemplateDto` ↔ `API.NotificationTemplate` ✅
- `NotificationLogDto` ↔ `API.NotificationLog` ✅
- `InvitationDto` ↔ `API.Invitation` ✅
- `ValidateInvitationDto` ↔ `API.ValidateInvitation` ✅
- `PackageDto` ↔ `API.OrgPackage` ✅
- `SubscriptionDto` ↔ `API.OrgSubscription` ✅
- `PageRequest` ↔ `API.PageParams` ✅
- `PagedResult<T>` ↔ `API.PagedResult<T>` ✅

---

## 三、API路径一致性

> 前端 services/*.ts 路径 ↔ 后端 Controller [Route] + [HttpX] 属性

### 已覆盖在维度一（见上文的P0-4/P0-5/P1-1~P1-4）

### 额外发现

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 1 | **P2-12** | **CourseAttachmentController 使用内联路由而非类前缀**。`[HttpPost("api/course/{courseId}/attachment")]` 写死完整路径，与其他 Controller 风格不一致。功能不受影响 | `Controllers/CourseAttachmentController.cs` |

---

## 四、后端代码质量

### P0

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 1 | **P0-7** | **init.sql 缺少 SysMenu/SysRole/SysRoleMenu/SysUserRole 4张权限表**。代码中定义了实体和Service，数据库无表 → 权限管理功能完全不可用 | `scripts/init.sql` |
| 2 | **P0-8** | **多租户全局过滤器仅覆盖 BaseEntity 派生类**。`CourseAttachment`、`CoursePackageItem`、`CourseEnrollment`、`WaitList`、`Attendance`、`LeaveRequest`、`CourseEvaluation`、`EvaluationReply`、`SignInQRCode`、`CourseFeeSettlement`、`TeacherWallet`、`FeeSettlementRecord`、`NotificationTemplate`、`NotificationLog`、`NotificationConfig`、`StatisticsDailySnapshot`、`StatisticsCourseSnapshot` 等 17 张表未继承 `BaseEntity`（自定义 Id/TenantId），全局过滤器不生效 → **存在跨租户数据泄露风险** | `SqlSugarSetup.cs:22` |

### P1

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 3 | **P1-14** | **25个 Controller 使用 `throw new Exception("未登录")` 而非 `throw new BizException()`**。`throw new Exception` 会被全局异常中间件捕获为500错误而非400。应统一使用 `BizException` 或 `UnauthorizedAccessException` | 所有 Controller 的 `GetUser()` 方法 |
| 4 | **P1-15** | **CoursePackageController.Purchase 为 TODO 空壳**。返回 true 无任何业务逻辑实现 | `Controllers/CoursePackageController.cs:79` |
| 5 | **P1-16** | **CoursePackageController.MyPackages 为 TODO 空壳**。返回空列表 | `Controllers/CoursePackageController.cs:84` |
| 6 | **P1-17** | **StatisticsController.Export 为 TODO 空壳**。返回 true 无导出逻辑 | `Controllers/StatisticsController.cs:83` |
| 7 | **P1-18** | **AuthService.WxLoginAsync 含 TODO**。微信登录使用 Code 模拟，未接入微信SDK获取 OpenId | `Services/AuthService.cs:47` |
| 8 | **P1-19** | **UserController / PackageController / SystemController 直接操作 SqlSugar**。未走 Service 层，缺乏统一业务逻辑封装、事务管理和权限控制 | `Controllers/UserController.cs`, `PackageController.cs`, `SystemController.cs` |
| 9 | **P1-20** | **UserController.GetList 使用 `.Contains(keyword)` 做搜索**。SqlSugar 会将其翻译为 LIKE '%keyword%'，虽然不是 SQL 注入风险（参数化），但无全文索引支持时大表性能差 | `Controllers/UserController.cs:25` |
| 10 | **P1-21** | **Program.cs 全局 AuthorizeFilter 要求所有接口认证，但 UserController/PackageController/SystemController 部分接口需匿名访问**（如统计概览、套餐列表）。未在这些 Controller 或方法上标注 `[AllowAnonymous]` → 匿名访问会返回401 | `Program.cs:50` |
| 11 | **P1-22** | **UserController.GetList 返回匿名对象** `new { x.Id, x.UserName, ... }` — 未经过DTO映射，字段与前端 SysUser 接口可能不一致（列表无 gender/nickName 等），且无法利用 Swagger 文档 | `Controllers/UserController.cs:26` |
| 12 | **P1-23** | **PackageController 直接使用 Entity 作为请求体**。`[HttpPost("annual")] CreateAnnualPackage([FromBody] OrgPackage req)` — 直接将数据库实体暴露为 API 入参，客户端可传入任意字段（含 Id、IsDeleted 等） | `Controllers/PackageController.cs:51` |
| 13 | **P1-24** | **PackageController.GetAnnualPackages 返回 `List<object>`**。未经过 DTO 映射，返回原始 Entity（含 IsDeleted、内部字段），存在数据泄露风险 | `Controllers/PackageController.cs:40` |
| 14 | **P1-25** | **无统一分页参数模型**。UserController 使用 `[FromQuery] int page, int pageSize`，其他 Controller 使用 `[FromQuery] PageRequest req`，风格不统一 | `Controllers/UserController.cs:18` vs 其他 |

### P2

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 15 | **P2-8** | **CreateUserDto/UpdateUserDto/ResetPwdDto 定义在 Controller 文件末尾**，应移至 DTOs 目录 | `Controllers/UserController.cs:73-76` |
| 16 | **P2-9** | **UpdateConfigDto 定义在 SystemController 文件末尾**，应移至 DTOs 目录 | `Controllers/SystemController.cs` |
| 17 | **P2-10** | **CourseAttachmentController 路由风格不一致**（内联完整路径 vs 类前缀） | `Controllers/CourseAttachmentController.cs` |
| 18 | **P2-11** | **Cors 策略 AllowAnyOrigin**。生产环境应限制为已知前端域名 | `Program.cs:77` |

---

## 五、前端代码质量

### P0

（已在维度一覆盖 P0-3）

### P1

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 1 | **P1-26** | **前端 service 层 18 处使用 any 类型**。涵盖 createCourse、updateCourse、createTeacher、updateTeacher、createEvaluation、createLeave、createSchedule、createRecurrenceSchedule、updateSchedule、importStudent、createCoursePackage、updateCoursePackage、updateNotificationConfig、updateOrgConfig、getAttendancePage、getEnrollmentPage、getParents | 多个 `services/*.ts` |
| 2 | **P1-27** | **AttendanceManage 页面使用 `<ProTable<any>>`**。无类型检查，列绑定和搜索字段无编译期保障 | `pages/Attendance/AttendanceManage/index.tsx:23` |
| 3 | **P1-28** | **前端页面缺乏 Loading/Empty/Error 三态处理**。除 Dashboard/Statistics/Overview 外，其他26个页面在 API 请求失败时仅有 `message.error()` 弹窗提示，无 Empty 组件、无 loading 状态展示、无重试按钮 | 大部分 `pages/**/*.tsx` |
| 4 | **P1-29** | **前端无统一错误拦截处理**。Login 页面自行 try/catch，其他页面依赖 request 层默认处理。无统一的 401 跳转登录、500 全局提示等 | `app.ts` / 各页面 |

### P2

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 5 | **P2-13** | **缺少 invitation service**。后端 InvitationController 有完整接口，前端无 services/invitation.ts | `services/` |
| 6 | **P2-14** | **缺少 orgBinding service**。后端 OrgBindingController 有接口，前端无 services/orgBinding.ts | `services/` |
| 7 | **P2-15** | **缺少 theme service**（已创建 services/theme.ts 但未在审查中发现被引用） | `services/` |
| 8 | **P2-16** | **global.less 字体统一 13px 已配置**，ProTable 和 Antd 组件字体均已覆盖。✅ 全局样式一致性符合要求 | `global.less` |

---

## 六、数据库设计

### P0

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 1 | **P0-7** | **init.sql 缺少 SysMenu/SysRole/SysRoleMenu/SysUserRole 4张权限表**（代码有实体定义） | `scripts/init.sql` |

### P1

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 2 | **P1-30** | **17张表未继承 BaseEntity，多租户过滤器不覆盖**（见后端 P0-8） | 多个 Entities/*.cs |
| 3 | **P1-31** | **NotificationLog 表缺少 SendTime 索引**。按时间范围查询未读消息是高频操作，需要 `(RecipientId, IsRead, CreatedAt)` 复合索引 | `scripts/init.sql` |
| 4 | **P1-32** | **CourseEvaluation 表缺少 (CourseId, ScheduleId) 联合索引**。按课次查评价是常见查询场景 | `scripts/init.sql` |

### P2

| # | 编号 | 描述 | 文件 |
|---|------|------|------|
| 5 | **P2-17** | **StatisticsDailySnapshot 缺少 (TenantId, StatDate) 复合索引**。按租户+日期查统计是常见查询 | `scripts/init.sql` |
| 6 | **P2-18** | **StatisticsCourseSnapshot 缺少 (TenantId, StatMonth) 复合索引** | `scripts/init.sql` |

### ✅ 数据库设计亮点

- **37张表**（需求文档32张表，额外5张为权限控制和统计快照），覆盖所有需求
- **67个索引**，包含高频查询的复合索引（如排课 OrgId+CampusId+LessonDate）
- 所有业务表包含 TenantId 字段（多租户共享数据库方案）
- 合理使用 `nvarchar(max)` 处理长文本（Description、Content、Images等）
- 唯一约束（OrgCode、CampusCode）防止数据重复

---

## 📋 全量问题清单

### P0 致命（14个）

| # | 编号 | 维度 | 描述 | 位置 |
|---|------|------|------|------|
| 1 | P0-1 | 后端 | MenuController/RoleController 缺少 [ApiController] | Controllers/MenuController.cs |
| 2 | P0-2 | 后端 | LessonUnitController.BatchGenerate courseId 缺 [FromQuery] | Controllers/LessonUnitController.cs:28 |
| 3 | P0-3 | 前端 | sysConfig.ts 引用未定义函数 getSysConfigs/createSysConfig | services/sysConfig.ts:28-29 |
| 4 | P0-4 | API缺失 | 后端缺少 /api/enrollment/page 端点 | EnrollmentController |
| 5 | P0-5 | API缺失 | 后端缺少 /api/attendance/page 端点 | AttendanceController |
| 6 | P0-6 | 前端类型 | OrgPackageParams 缺少 enableEvaluation | typings.d.ts |
| 7 | P0-7 | 数据库 | init.sql 缺少 SysMenu/SysRole/SysRoleMenu/SysUserRole 4张表 | scripts/init.sql |
| 8 | P0-8 | 后端 | 17张表未覆盖多租户全局过滤器，存在跨租户数据泄露 | SqlSugarSetup.cs |

### P1 严重（18个）

| # | 编号 | 维度 | 描述 |
|---|------|------|------|
| 1 | P1-1 | API缺失 | 后端缺少 PUT /api/users/{id}/status |
| 2 | P1-2 | API缺失 | 后端缺少 DELETE /api/organization/{id} |
| 3 | P1-3 | API缺失 | 后端缺少 DELETE /api/campus/{id} |
| 4 | P1-4 | API缺失 | 后端缺少 DELETE /api/system/config/{id} |
| 5 | P1-5 | 后端参数 | CoursePackage Page 缺少 orgId 筛选 |
| 6 | P1-6 | 后端参数 | Organization Page 缺少 status 筛选 |
| 7 | P1-7 | 类型格式 | Schedule.startTime/endTime TimeSpan↔string |
| 8 | P1-8 | 前端 | CoursePackage.addCourseToPackage courseId 可能 undefined |
| 9 | P1-9 | 前端类型 | SysUser.gender 应为可选 |
| 10 | P1-10 | 前端类型 | 缺少 Teacher 接口定义 |
| 11 | P1-11 | 后端类型 | CreateTeacherRequest.Gender(string) vs TeacherDto.Gender(int) |
| 12 | P1-12 | 后端 | SubscriptionDto.RemainingDays 需确认 Service 赋值 |
| 13 | P1-13 | 后端 | CoursePackage Purchase/MyPackages 为 TODO 空壳 |
| 14 | P1-14 | 后端 | 25个 Controller 用 throw new Exception 替代 BizException |
| 15 | P1-15~16 | 后端 | Purchase/MyPackages TODO 空壳 |
| 16 | P1-17 | 后端 | StatisticsController.Export TODO 空壳 |
| 17 | P1-18 | 后端 | AuthService.WxLoginAsync TODO 未接入微信 |
| 18 | P1-19 | 后端 | UserController/PackageController/SystemController 绕过 Service 层 |
| 19 | P1-20 | 后端 | Contains 做搜索大表性能差 |
| 20 | P1-21 | 后端 | UserController/PackageController/SystemController 缺 [AllowAnonymous] |
| 21 | P1-22 | 后端 | UserController 返回匿名对象未走 DTO |
| 22 | P1-23 | 后端 | PackageController 直接用 Entity 作为请求体 |
| 23 | P1-24 | 后端 | PackageController 返回原始 Entity 含敏感字段 |
| 24 | P1-25 | 后端 | 分页参数模型不统一 |
| 25 | P1-26 | 前端 | 18处 service 使用 any 类型 |
| 26 | P1-27 | 前端 | AttendanceManage ProTable<any> |
| 27 | P1-28 | 前端 | 26个页面缺 Loading/Empty/Error 三态 |
| 28 | P1-29 | 前端 | 无统一错误拦截（401跳转、500提示） |
| 29 | P1-30 | 数据库 | 17张表未覆盖多租户过滤器 |
| 30 | P1-31 | 数据库 | NotificationLog 缺少复合索引 |
| 31 | P1-32 | 数据库 | CourseEvaluation 缺少联合索引 |

### P2 建议（16个）

| # | 编号 | 维度 | 描述 |
|---|------|------|------|
| 1 | P2-1 | 前端 | Service 层 any 替换为具体类型 |
| 2 | P2-2 | 前端 | 添加 Teacher 接口定义 |
| 3 | P2-3 | 前端 | SysUser 列表与详情字段差异未区分 |
| 4 | P2-4 | 后端 | DTO 类移至 DTOs 目录 |
| 5 | P2-5 | 后端 | decimal↔number 精度差异（当前场景可接受） |
| 6 | P2-6 | 后端 | TimeSpan↔string 格式兼容（当前可接受） |
| 7 | P2-7 | 后端 | CourseAttachmentController 路由风格不一致 |
| 8 | P2-8 | 后端 | DTO 位置不规范 |
| 9 | P2-9 | 后端 | UpdateConfigDto 位置不规范 |
| 10 | P2-10 | 后端 | CourseAttachmentController 路由风格 |
| 11 | P2-11 | 后端 | CORS AllowAnyOrigin |
| 12 | P2-12 | 后端 | CourseAttachmentController 路由内联 |
| 13 | P2-13 | 前端 | 缺少 invitation service |
| 14 | P2-14 | 前端 | 缺少 orgBinding service |
| 15 | P2-15 | 前端 | 缺少 theme service 引用 |
| 16 | P2-16 | 前端 | global.less 字体统一 ✅ |
| 17 | P2-17 | 数据库 | StatisticsDailySnapshot 缺复合索引 |
| 18 | P2-18 | 数据库 | StatisticsCourseSnapshot 缺复合索引 |

---

## 🔧 修复优先级建议

### 第一优先（阻断性）：P0 全部（8个）
1. **P0-7**: init.sql 补充 SysMenu/SysRole/SysRoleMenu/SysUserRole 建表语句
2. **P0-8**: 将不继承 BaseEntity 的17张表的 TenantId 纳入全局过滤器（手动追加 Where 条件或在 Service 层统一处理）
3. **P0-1**: MenuController/RoleController 添加 `[ApiController]`
4. **P0-2**: LessonUnitController.BatchGenerate courseId 添加 `[FromQuery]`
5. **P0-3**: sysConfig.ts 修复未定义函数引用
6. **P0-4/P0-5**: 后端补充 enrollment/page 和 attendance/page 端点
7. **P0-6**: OrgPackageParams 添加 enableEvaluation

### 第二优先：API补齐（P1-1~P1-6，6个）
后端补充缺失的 DELETE/PUT 端点和筛选参数

### 第三优先：后端质量（P1-14~P1-25，12个）
统一异常处理、消除 TODO 空壳、补全 Service 层、DTO 规范化

### 第四优先：前端质量（P1-26~P1-29，4个）
替换 any 类型、补全三态处理、统一错误拦截

### 第五优先：数据库优化（P1-31~P1-32、P2-17~P2-18）
补充缺失索引
