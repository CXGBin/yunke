# 云科智教教育SaaS课程管理平台 — 前后端类型一致性专项审查报告

> 审查时间：2026-06-16 23:51 (Asia/Shanghai)  
> 审查范围：前端 `frontend/src/services/*.ts` + `typings.d.ts` ↔ 后端 `backend/src/YunKeEdu.Api/Controllers/*.cs` + `DTOs/*.cs` + `Enums/*.cs`  
> 后端序列化：CamelCase（`JsonNamingPolicy.CamelCase`）  
> 后端分页模型：`PageRequest { Page, PageSize, Keyword }` → `PagedResult<T> { Items, Total, Page, PageSize }`

---

## 📊 审查摘要

| 严重等级 | 数量 | 说明 |
|---------|------|------|
| **P0（致命）** | 8 | 会导致接口调用400/500错误、功能完全失效 |
| **P1（严重）** | 14 | 类型不匹配可能导致数据异常或运行时报错 |
| **P2（建议）** | 12 | 类型缺失、不够严谨但不影响核心功能 |

---

## P0 — 致命问题（必须立即修复）

### P0-1: MenuController / RoleController 缺少 [ApiController] 属性

- **文件**: `backend/src/YunKeEdu.Api/Controllers/MenuController.cs:10,38`
- **问题**: `MenuController` 和 `RoleController` 均未标注 `[ApiController]`，且未全局注册 `[ApiController]` 约定（Program.cs 未见 `AddControllers(opt => opt.SuppressModelStateInvalidFilter = false)` 等全局配置）
- **影响**: 
  - 无 `[ApiController]` 时，`[FromBody]` 参数如未正确绑定不会返回400而是返回415或不触发模型验证
  - `[FromQuery] PageRequest` 不会自动绑定复杂查询参数
  - 无自动 `[Authorize]` 级别的模型验证响应
- **修复**: 为两个Controller添加 `[ApiController]` 属性

### P0-2: 前端 SysUser.gender 类型为 number，后端 UserController 详情返回无 Gender 字段

- **文件**: `frontend/typings.d.ts:176` / `backend/src/YunKeEdu.Api/Controllers/UserController.cs`
- **后端 GET `/api/users/list` 返回**: 匿名对象无 `gender` 字段
- **后端 GET `/api/users/{id}` 返回**: 匿名对象包含 `gender`，类型为 `int`
- **前端 API.SysUser.gender 声明**: `gender: number`（非可选）
- **影响**: 列表页绑定 gender 时取到 undefined，可能显示异常；应为可选类型
- **修复**: 前端 SysUser.gender 改为 `gender?: number`

### P0-3: 前端 OrgPackageParams 缺少 enableEvaluation 字段

- **文件**: `frontend/typings.d.ts:256` / `backend/src/YunKeEdu.Core/Models/DTOs/PackageDto.cs`
- **后端 CreatePackageRequest**: `bool EnableEvaluation` (默认 false)
- **前端 API.OrgPackageParams**: 无 `enableEvaluation` 字段
- **影响**: 创建/更新套餐时无法设置评价功能开关，始终默认 false，后端保存值与前端意图不一致
- **修复**: 前端 OrgPackageParams 添加 `enableEvaluation?: boolean`

### P0-4: 前端 batchGenerateLessons 传递 courseId 为 query string，后端为路由参数

- **文件**: `frontend/src/services/course.ts:93-96`
- **前端**: `POST /api/lesson-unit/batch-generate?courseId=${courseId}`（query string 传 number）
- **后端**: `[HttpPost("batch-generate")] public async Task BatchGenerate(long courseId, [FromBody] BatchGenerateLessonRequest req)`
- **问题**: 后端 `courseId` 未标注 `[FromQuery]`，默认从路由模板绑定，但路由模板为 `api/lesson-unit/batch-generate` 无 `{courseId}` 参数位 → courseId 永远为 0
- **影响**: 批量生成课时功能完全失效，courseId=0 会导致后续逻辑异常
- **修复**: 后端方法参数添加 `[FromQuery] long courseId`

### P0-5: 前端 Enrollment.courseStudents 传递 courseId 为 query string，后端为 [FromQuery] 无默认值

- **文件**: `frontend/src/services/enrollment.ts:32`
- **前端**: `GET /api/enrollment/course-students?courseId=xxx`（query string）
- **后端**: `[HttpGet("course-students")] public async Task CourseStudents([FromQuery] PageRequest req, [FromQuery] long courseId)` — courseId 为 `long` 非可空
- **问题**: 后端 `courseId` 是必填 `long`，前端传递 `number` 类型 → 类型一致，此条实际 **无问题**，降级为P2（建议标注前端类型为 `courseId: number` 明确必填）
- **重新评估**: ~~P0→P2~~ 前端传递正确，降级

### P0-5 (修正): 前端 sysConfig.ts 引用未定义函数 getSysConfigs / createSysConfig

- **文件**: `frontend/src/services/sysConfig.ts:28-29`
- **代码**: `export const getConfigPage = getSysConfigs;` / `export const createConfig = createSysConfig;`
- **问题**: `getSysConfigs` 和 `createSysConfig` 函数在文件中未定义，会导致模块加载时报 ReferenceError
- **影响**: sysConfig 模块加载崩溃，影响所有依赖此模块的页面
- **修复**: 将 `getConfigPage` 指向 `getSysConfigList`，删除或实现 `createSysConfig`（后端无 create config 接口）

### P0-6: 前端 getEnrollmentPage 调用不存在的后端接口

- **文件**: `frontend/src/services/enrollment.ts:53-55`
- **前端**: `GET /api/enrollment/page`
- **后端**: EnrollmentController 无 `page` 端点（只有 `POST /api/enrollment`, `GET my-courses`, `GET my-schedule`, `GET course-students`）
- **影响**: 404 错误
- **修复**: 后端 EnrollmentController 添加 `[HttpGet("page")]` 端点，或前端移除此无效调用

### P0-7: 前端 getAttendancePage 调用不存在的后端接口

- **文件**: `frontend/src/services/attendance.ts:80-81`
- **前端**: `GET /api/attendance/page`
- **后端**: AttendanceController 无 `page` 端点（只有 sign-in, sign-all, schedule/{id}, my-records, statistics/*）
- **影响**: 404 错误
- **修复**: 后端添加签到记录分页查询接口，或前端移除此无效调用

### P0-8: 前端 user.ts 的 getParents 别名指向 parent/page 但 parent 模块无分页端点

- **文件**: `frontend/src/services/user.ts:41`
- **前端**: `export async function getParents(params: any) { return request<API.PagedResult<any>>("/api/parent/page", ...) }`
- **后端**: ParentController **有** `[HttpGet("page")]` — 已确认存在
- **重新评估**: ~~P0→移除~~ 后端接口存在，路径一致，此条实际无问题

---

## P1 — 严重问题

### P1-1: 前端 CourseService 多处使用 `any` 类型，丧失类型安全

- **文件**: `frontend/src/services/course.ts:56,61` (createCourse/updateCourse), `teacher.ts:22,27` (createTeacher/updateTeacher), `evaluation.ts:5` (createEvaluation), `attendance.ts:50` (createLeave), `coursePackage.ts:28,33` (createCoursePackage/updateCoursePackage)
- **问题**: createCourse、updateCourse、createTeacher、updateTeacher、createEvaluation、createLeave 等函数的 data 参数均声明为 `any`
- **影响**: 无法在编译期检测参数名/类型错误，实际传递了错误字段名时无任何提示
- **修复**: 将 `any` 替换为具体类型（如 `API.CourseParams`、`API.TeacherParams` 等）

### P1-2: 前端 CoursePageParams 缺少 keyword 字段

- **文件**: `frontend/typings.d.ts:302-305` / `frontend/src/services/course.ts:9`
- **后端 PageRequest**: `{ Page, PageSize, Keyword }`
- **前端 CoursePageParams**: `{ page?, pageSize?, status?, orgId?, categoryId? }` — 无 keyword
- **前端实际调用**: `getCoursePage` 传递的 params 可能包含 keyword（因为继承了 PageParams）
- **影响**: 如果前端搜索框传递了 keyword 参数但类型定义未声明，TypeScript 编译器可能警告
- **修复**: `CoursePageParams` 已继承 `PageParams`，PageParams 包含 keyword → 实际无问题，降级为 **P2**（建议确认所有 PageParams 继承链）

### P1-2 (修正): 前端 TeacherDto 缺少 introduction 字段

- **文件**: `frontend/typings.d.ts` 无 `Teacher` 接口定义
- **后端 TeacherDto**: `{ Id, UserCode, UserName, RealName, NickName, Avatar, Phone, Gender, Role, OrgId?, CampusId?, Status, CreatedAt }`
- **前端**: 教师列表返回 `PagedResult<any>` — 完全无类型
- **影响**: 页面绑定字段名可能拼写错误
- **修复**: 前端添加 `Teacher` 接口定义，teacher service 替换 `any`

### P1-3: 后端 OrgSubscription.status 返回 int，前端 createdAt 为 string 但后端为 DateTime

- **文件**: `frontend/typings.d.ts:284` / `backend/src/YunKeEdu.Core/Models/DTOs/PackageDto.cs`
- **后端 SubscriptionDto**: `int RemainingDays`、`int PayStatus`、`int SubscriptionType`
- **前端 OrgSubscription**: 全部 `number` → 类型一致
- **后端 DateTime 序列化**: CamelCase 下 DateTime 默认序列化为 `2026-06-16T15:33:38` 格式字符串
- **前端 startDate/endDate**: `string` → 可正确接收
- **评估**: DateTime ↔ string 在 JSON 层面兼容，此条降级为 **P2**

### P1-3 (修正): 后端 ScheduleDto.StartTime/EndTime 为 TimeSpan，前端声明为 string

- **文件**: `frontend/typings.d.ts:327` / `backend/src/YunKeEdu.Core/Models/DTOs/ScheduleDto.cs`
- **后端**: `TimeSpan StartTime`、`TimeSpan EndTime`
- **JSON 序列化**: TimeSpan 默认序列化为 `"01:30:00"` 格式字符串
- **前端**: `startTime: string`、`endTime: string`
- **影响**: JSON 层面兼容，但前端如需时间计算需解析字符串
- **修复**: 无需修改类型，但建议前端页面处理时注意格式

### P1-4: 前端 OrgPackageParams.price 类型为 number，后端为 decimal

- **文件**: `frontend/typings.d.ts:247` / `backend/src/YunKeEdu.Core/Models/DTOs/PackageDto.cs`
- **后端**: `decimal Price`（精度28位）
- **前端**: `price: number`（JS 双精度浮点，约15位有效数字）
- **影响**: 大额价格（>9999999999999.99）可能丢失精度；教育SaaS场景价格通常不会超出
- **修复**: 当前场景可接受，但建议后端金额字段统一使用 `long`（分）或前端使用字符串

### P1-5: 前端 SettlementRule.fixedAmount 字段名正确，但 FeeSettlementRecord.settledAt 应为 string?

- **文件**: `frontend/typings.d.ts:405` / `backend/src/YunKeEdu.Core/Models/DTOs/SettlementDto.cs`
- **后端 FeeSettlementRecordDto**: `DateTime SettledAt`（非可空）
- **前端 FeeSettlementRecord.settledAt**: `string`
- **影响**: DateTime → JSON string，类型兼容
- **评估**: 无需修改，降级为 **P2**

### P1-5 (修正): 后端 ScheduleController.Calendar 参数为 DateTime，前端传 string

- **文件**: `frontend/src/services/schedule.ts:50`
- **前端**: `getScheduleCalendar(params: { startDate: string; endDate: string })`
- **后端**: `Calendar([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)`
- **影响**: .NET 模型绑定器可自动将 `2026-06-16` 格式字符串转为 DateTime。但若前端传 `06/16/2026` 格式（美式）则解析失败
- **修复**: 确保前端统一传 ISO 格式 `YYYY-MM-DD`

### P1-6: 后端 ConflictCheckRequest.StartTime/EndTime 为 TimeSpan，前端传 string

- **文件**: `frontend/src/services/schedule.ts:55-59`
- **前端**: `checkScheduleConflict(params: { ..., startTime: string; endTime: string; ... })`
- **后端**: `TimeSpan StartTime`、`TimeSpan EndTime`（query string [FromQuery]）
- **影响**: TimeSpan 从 query string 绑定时，需要标准格式如 `01:30:00`
- **修复**: 确保前端传 HH:mm:ss 格式

### P1-7: 后端 AttendanceController 无 page 端点，前端 getAttendancePage 将404

- **已合并至 P0-7**

### P1-7 (修正): 前端 Schedule.startTime/endTime 为 string，后端为 TimeSpan

- **已合并至 P1-3(修正)**

### P1-8: 后端 StatisticsController.export 缺少 type/year 参数的类型定义

- **文件**: `frontend/src/services/statistics.ts:72-76`
- **后端**: `[HttpGet("export")] Export([FromQuery] string? type, [FromQuery] int? year)`
- **前端**: `exportStatistics(type?: string, year?: number)` — 类型一致
- **评估**: 无问题，移除

### P1-8 (修正): 前端 updateUserStatus 调用不存在的后端接口

- **文件**: `frontend/src/services/user.ts:39`
- **前端**: `PUT /api/users/{id}/status`
- **后端**: UserController 无 `PUT {id}/status` 端点（只有 GET list, GET {id}, POST, PUT {id}, DELETE {id}, POST reset-password）
- **影响**: 404 错误，用户状态更新功能失效
- **修复**: 后端 UserController 添加 `PUT {id}/status` 端点

### P1-9: 前端 deleteOrg 调用不存在的后端接口

- **文件**: `frontend/src/services/organization.ts:37`
- **前端**: `DELETE /api/organization/{id}`
- **后端**: OrganizationController 无 `[HttpDelete]` 端点（只有 GET page, GET {id}, POST, PUT {id}, PUT {id}/status）
- **影响**: 404 错误，机构删除功能失效
- **修复**: 后端添加 DELETE 端点

### P1-10: 前端 deleteCampus 调用不存在的后端接口

- **文件**: `frontend/src/services/campus.ts:39`
- **前端**: `DELETE /api/campus/{id}`
- **后端**: CampusController 无 `[HttpDelete]` 端点
- **影响**: 404 错误，校区删除功能失效
- **修复**: 后端添加 DELETE 端点

### P1-11: 前端 deleteConfig 调用 SystemController 但 ConfigController 路径不同

- **文件**: `frontend/src/services/sysConfig.ts:31`
- **前端**: `DELETE /api/system/config/${id}`
- **后端 ConfigController**: 无 DELETE 端点（只有 GET org/sys, PUT org/sys）
- **后端 SystemController**: 无 DELETE config 端点
- **影响**: 404 错误
- **修复**: 后端添加 DELETE 端点或前端移除此调用

### P1-12: 前端 OrgPackageParams 缺少 enableEvaluation 和部分必填字段验证

- **已合并至 P0-3**

### P1-12 (修正): 前端 getCoursePackagePage 传递 keyword/orgId 但后端 PageRequest 无这些字段

- **文件**: `frontend/src/services/coursePackage.ts:6`
- **前端**: `getCoursePackagePage(params: API.PageParams & { keyword?: string; orgId?: number })`
- **后端**: `[HttpGet("page")] Page([FromQuery] PageRequest req)` — PageRequest 只有 `{ Page, PageSize, Keyword }`
- **影响**: `orgId` 作为额外 query string 传到后端，后端不读取该参数 → 机构过滤无效
- **修复**: 后端 CoursePackageController.Page 方法添加 `[FromQuery] long? orgId` 参数

### P1-13: 前端 getOrgPage 传递 status 参数，后端 PageRequest 无 status 字段

- **文件**: `frontend/src/services/organization.ts:6`
- **前端**: `getOrgPage(params: API.PageParams & { keyword?: string; status?: number })`
- **后端**: `Page([FromQuery] PageRequest req)` — PageRequest 只有 `{ Page, PageSize, Keyword }`
- **影响**: status 作为额外 query string 被忽略 → 状态筛选无效
- **修复**: 后端 OrganizationController.Page 添加 `[FromQuery] int? status` 参数

### P1-14: 后端 UserController.GetList 使用 page/pageSize 而非 PageRequest

- **文件**: `backend/src/YunKeEdu.Api/Controllers/UserController.cs:18`
- **后端**: `[HttpGet("list")] GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20, ...)`
- **前端**: `getUserList(params: API.UserPageParams)` — `UserPageParams { page?, pageSize?, orgId?, role?, keyword? }`
- **问题**: 后端使用独立参数而非 PageRequest 模型，但参数名和类型一致 → **实际无问题**
- **评估**: 命名风格不统一但不影响功能，降级为 **P2**

---

## P2 — 建议改进

### P2-1: 前端 Service 层大量使用 `any` 类型

- **文件**: `teacher.ts` (createTeacher/updateTeacher 返回 any), `course.ts` (createCourse/updateCourse), `evaluation.ts` (createEvaluation), `coursePackage.ts`, `attendance.ts` (createLeave)
- **影响**: 丧失类型检查能力
- **修复**: 定义对应的 Params 接口替换 any

### P2-2: 前端缺少 Teacher 接口定义

- **文件**: `frontend/typings.d.ts`
- **后端 TeacherDto**: 完整字段定义
- **前端**: 无 `Teacher` 接口，teacher service 全部使用 `any`
- **修复**: 添加 `Teacher` interface

### P2-3: 前端 CoursePageParams 已继承 PageParams，但 PageParams.keyword 可能被 course 的 status/orgId/categoryId 参数覆盖

- **评估**: TypeScript 继承链正确，PageParams.keyword 与扩展字段不冲突
- **无需修改**

### P2-4: 后端 UserController.CreateUserDto 定义在 Controller 文件中，应移至 DTOs

- **文件**: `backend/src/YunKeEdu.Api/Controllers/UserController.cs:73-76`
- **问题**: DTO 类定义在 Controller 文件末尾，不符合分层架构规范
- **修复**: 移至 `YunKeEdu.Core/Models/DTOs/SystemDto.cs`

### P2-5: 前端 SysUser 接口与后端 GET /api/users/list 返回匿名对象不完全匹配

- **后端 list 返回**: `{ Id, UserName, RealName, Phone, Avatar, Role, OrgId, CampusId, Status }`
- **前端 SysUser**: 额外包含 `nickName?, gender, ...`
- **影响**: 列表返回无 nickName/gender 等字段，前端绑定这些字段时为 undefined
- **修复**: 区分 SysUserListItem 和 SysUserDetail，或前端标记为可选

### P2-6: 前端 EvaluationReply.replyById 字段名正确但后端为 ReplyById（CamelCase序列化后一致）

- **已确认一致，无需修改**

### P2-7: 后端 CourseAttachmentController 无 [Route] 前缀，使用内联路由

- **文件**: `backend/src/YunKeEdu.Api/Controllers/CourseAttachmentController.cs`
- **后端路由**: `[HttpPost("api/course/{courseId}/attachment")]` — 内联完整路径
- **问题**: 与其他 Controller 使用 `[Route("api/xxx")]` 前缀的风格不一致，但不影响功能
- **修复**: 添加 `[Route("api")]` 前缀并简化方法路由

### P2-8: 前端缺少 Invitation Service

- **文件**: 前端无 `services/invitation.ts`
- **后端**: InvitationController 存在完整的邀请 CRUD 接口
- **影响**: PC端管理后台无法使用邀请功能
- **修复**: 创建 `services/invitation.ts` 并封装所有邀请接口

### P2-9: 前端缺少 Theme Service

- **文件**: 前端无 `services/theme.ts`
- **后端**: ThemeController 存在
- **影响**: PC端无法管理主题
- **修复**: 创建 `services/theme.ts`

### P2-10: 前端缺少 OrgBinding Service

- **文件**: 前端无 `services/orgBinding.ts`
- **后端**: OrgBindingController 存在 (GET my-orgs, GET detail/{orgId})
- **影响**: 无法查看/切换用户绑定的机构
- **修复**: 创建 `services/orgBinding.ts`

### P2-11: 后端 CreateTeacherRequest.Gender 为 string，但 TeacherDto.Gender 为 int

- **文件**: `backend/src/YunKeEdu.Core/Models/DTOs/SystemDto.cs`
- **CreateTeacherRequest**: `string? Gender`
- **TeacherDto**: `int Gender`
- **问题**: 同一概念在不同 DTO 中类型不一致（string vs int）
- **影响**: 前端若按 TeacherDto 的 int 类型传值给 CreateTeacherRequest 的 string，可能导致验证失败或数据错误
- **修复**: 统一为 `int` 或 `string`，推荐 `int`

### P2-12: 前端 Schedule.isRescheduled 后端为 bool，前端为 boolean → 类型一致

- **已确认一致，无需修改**

---

## 📋 问题汇总清单

| 编号 | 等级 | 类别 | 描述 | 文件 |
|------|------|------|------|------|
| P0-1 | 致命 | 后端 | MenuController/RoleController 缺少 [ApiController] | Controllers/MenuController.cs |
| P0-2 | 致命 | 前端类型 | SysUser.gender 应为可选 | typings.d.ts |
| P0-3 | 致命 | 前端类型 | OrgPackageParams 缺少 enableEvaluation | typings.d.ts |
| P0-4 | 致命 | 后端 | LessonUnitController.BatchGenerate courseId 缺少 [FromQuery] | Controllers/LessonUnitController.cs |
| P0-5 | 致命 | 前端 | sysConfig.ts 引用未定义函数 getSysConfigs/createSysConfig | services/sysConfig.ts |
| P0-6 | 致命 | API缺失 | 后端缺少 /api/enrollment/page 端点 | Controllers/EnrollmentController.cs |
| P0-7 | 致命 | API缺失 | 后端缺少 /api/attendance/page 端点 | Controllers/AttendanceController.cs |
| P1-1 | 严重 | 前端类型 | 多处 service data 参数使用 any | 多个 services/*.ts |
| P1-2 | 严重 | 前端类型 | 缺少 Teacher 接口定义 | typings.d.ts |
| P1-4 | 严重 | 类型精度 | decimal ↔ number 精度差异 | typings.d.ts / DTOs |
| P1-5 | 严重 | 类型格式 | TimeSpan ↔ string 日期格式兼容 | typings.d.ts |
| P1-6 | 严重 | 类型格式 | TimeSpan query string 绑定需标准格式 | services/schedule.ts |
| P1-8 | 严重 | API缺失 | 后端缺少 PUT /api/users/{id}/status 端点 | Controllers/UserController.cs |
| P1-9 | 严重 | API缺失 | 后端缺少 DELETE /api/organization/{id} 端点 | Controllers/OrganizationController.cs |
| P1-10 | 严重 | API缺失 | 后端缺少 DELETE /api/campus/{id} 端点 | Controllers/CampusController.cs |
| P1-11 | 严重 | API缺失 | 后端缺少 DELETE /api/system/config/{id} 端点 | Controllers/ConfigController.cs |
| P1-12 | 严重 | 后端参数 | CoursePackage Page 缺少 orgId 筛选参数 | Controllers/CoursePackageController.cs |
| P1-13 | 严重 | 后端参数 | Organization Page 缺少 status 筛选参数 | Controllers/OrganizationController.cs |
| P1-14 | 严重 | 代码规范 | UserController 使用独立分页参数而非 PageRequest | Controllers/UserController.cs |
| P2-1 | 建议 | 前端类型 | service 层大量 any 替换为具体类型 | 多个 services/*.ts |
| P2-2 | 建议 | 前端类型 | 缺少 Teacher 接口定义 | typings.d.ts |
| P2-4 | 建议 | 代码规范 | CreateUserDto 定义在 Controller 中应移至 DTOs | Controllers/UserController.cs |
| P2-5 | 建议 | 前端类型 | SysUser 列表与详情字段差异未区分 | typings.d.ts |
| P2-7 | 建议 | 代码规范 | CourseAttachmentController 路由风格不一致 | Controllers/CourseAttachmentController.cs |
| P2-8 | 建议 | 前端缺失 | 缺少 invitation service | services/ |
| P2-9 | 建议 | 前端缺失 | 缺少 theme service | services/ |
| P2-10 | 建议 | 前端缺失 | 缺少 orgBinding service | services/ |
| P2-11 | 建议 | 后端类型 | CreateTeacherRequest.Gender 为 string 但 DTO 为 int | DTOs/SystemDto.cs |

---

## ✅ 确认无问题的模块

以下接口经逐一对比确认前后端**完全一致**：

| 模块 | 状态 |
|------|------|
| AuthController ↔ services/auth.ts | ✅ 路径/参数/返回类型完全一致 |
| TeacherController ↔ services/teacher.ts | ✅ 路径/方法一致（类型待补） |
| StudentController ↔ services/student.ts | ✅ 完全一致 |
| ParentController ↔ services/parent.ts | ✅ 完全一致 |
| CourseController ↔ services/course.ts | ✅ 路径/方法一致 |
| CoursePackageController ↔ services/coursePackage.ts | ✅ 路径/方法一致 |
| EnrollmentController ↔ services/enrollment.ts | ✅ 路径/方法一致（缺 page） |
| ScheduleController ↔ services/schedule.ts | ✅ 路径/方法一致 |
| AttendanceController ↔ services/attendance.ts | ✅ 路径/方法一致（缺 page） |
| EvaluationController ↔ services/evaluation.ts | ✅ 完全一致 |
| EvaluationTagController ↔ services/evaluation.ts | ✅ 完全一致 |
| SettlementController ↔ services/settlement.ts | ✅ 完全一致 |
| NotificationController ↔ services/notification.ts | ✅ 完全一致 |
| StatisticsController ↔ services/statistics.ts | ✅ 完全一致 |
| OrgPackageController ↔ services/orgPackage.ts | ✅ 路径/方法一致 |
| PermissionController (Menu/Role) ↔ services/permission.ts | ✅ 路径一致 |

---

## 🔧 修复优先级建议

1. **第一优先**：修复 P0-1（MenuController/RoleController 添加 [ApiController]）、P0-4（courseId [FromQuery]）、P0-5（sysConfig 未定义引用）
2. **第二优先**：补齐后端缺失的 API 端点（P0-6/P0-7/P1-8~P1-11）
3. **第三优先**：修复前端类型定义（P0-2/P0-3、P1-1~P1-2）
4. **第四优先**：后端参数扩展（P1-12/P1-13）
5. **第五优先**：前端 Service 类型补全（P2-1~P2-2）及缺失 Service 创建（P2-8~P2-10）
