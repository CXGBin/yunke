# 3. 排课模块

## 3.1 模块概述

排课模块管理课程的具体上课时间和教室分配。机构管理员/教师创建排课计划，系统自动检测冲突，学生端同步展示课表。

## 3.2 功能清单

### 3.2.1 教室管理（机构管理员）

| 功能点 | 说明 |
|--------|------|
| 教室CRUD | 教室名称、容量、位置、设备信息 |
| 教室状态 | 启用/停用 |
| 设备标记 | 投影仪、白板、电脑、音响等 |

### 3.2.2 排课创建（教师/机构管理员）

| 功能点 | 说明 |
|--------|------|
| 基本信息 | 关联课程、选择教师、选择教室 |
| 时间设置 | 上课日期、开始时间、结束时间 |
| 循环排课 | 支持按周循环（如每周一、三、五，共8周） |
| 生成预览 | 循环排课前预览生成的课表 |
| 备注信息 | 课次备注（如"第一节：课程介绍"） |

### 3.2.3 冲突检测

| 冲突类型 | 检测规则 |
|----------|---------|
| 教师时间冲突 | 同一教师在同一时段不可上两门课 |
| 教室冲突 | 同一教室在同一时段不可排两门课 |
| 学生冲突 | 同一学生在同一时段不可排两门课（选课校验） |

### 3.2.4 调课（教师/机构管理员）

- 修改单次上课时间/教室
- 批量调课（修改循环排课规则）
- 调课通知：自动通知已选课的学生
- 调课记录留存

### 3.2.5 课表视图

| 角色 | 视图 |
|------|------|
| 教师 | 我的授课日历/周视图 |
| 机构管理员 | 全机构教室排课日历/甘特图 |
| 学生 | 我的课表日历（小程序端，见选课模块） |

### 3.2.6 取消课次（教师/机构管理员）

- 取消单次课次（如节假日调休）
- 取消原因记录
- 自动通知已选课学生
- 预留：取消是否影响课时结算

## 3.3 业务流程

```
创建排课计划
      │
      ▼
设置时间（单次/循环）
      │
      ▼
冲突检测
      │
  ┌───┴───┐
  │       │
  无冲突  有冲突
  │       │
  ▼       ▼
保存排课  提示冲突详情
  │
  ▼
发布课表 → 学生端课表同步更新
  │
  ▼
调课/取消 → 冲突检测 → 保存 → 通知学生
```

## 3.4 角色权限矩阵

| 操作 | 教师 | 机构管理员 | 平台管理员 |
|------|:----:|:---------:|:---------:|
| 查看教室 | ✅ | ✅ | ✅ |
| 管理教室 | ❌ | ✅ | ✅ |
| 创建排课(自己课程) | ✅ | ✅ | ✅ |
| 创建排课(任意课程) | ❌ | ✅ | ✅ |
| 调课(自己课程) | ✅ | ✅ | ✅ |
| 调课(任意课程) | ❌ | ✅ | ✅ |
| 取消课次 | ✅(自己) | ✅ | ✅ |
| 查看全机构排课 | ❌ | ✅ | ✅ |

## 3.5 数据库表设计

### Classroom（教室表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| CampusId | bigint FK NOT NULL | 所属校区ID |
| Name | nvarchar(100) NOT NULL | 教室名称 |
| Capacity | int DEFAULT 30 | 容量（人数） |
| Location | nvarchar(200) | 位置描述（如"A栋3楼301"） |
| Equipment | varchar(500) | 设备，逗号分隔（如"投影仪,白板"） |
| Status | tinyint DEFAULT 1 | 1启用 0停用 |
| SortOrder | int DEFAULT 0 | 排序 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |
| IsDeleted | bit DEFAULT 0 | 软删除 |

### CourseSchedule（排课计划表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| CampusId | bigint FK NOT NULL | 所属校区ID |
| CourseId | bigint FK NOT NULL | 课程ID |
| TeacherId | bigint FK NOT NULL | 教师ID |
| ClassroomId | bigint FK NOT NULL | 教室ID |
| LessonDate | date NOT NULL | 上课日期 |
| StartTime | time NOT NULL | 开始时间 |
| EndTime | time NOT NULL | 结束时间 |
| LessonNo | int | 第几课次 |
| LessonTitle | nvarchar(200) | 课次标题（如"第一章"） |
| Remark | nvarchar(500) | 备注 |
| Status | tinyint NOT NULL DEFAULT 0 | 0计划中 1已发布 2已取消 |
| CancelReason | nvarchar(200) | 取消原因 |
| IsRescheduled | bit DEFAULT 0 | 是否调课（标记调课记录） |
| OriginalScheduleId | bigint | 原排课ID（调课时记录） |
| CreatedBy | bigint NOT NULL | 创建人ID |
| PublishedAt | datetime2 | 发布时间 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |
| IsDeleted | bit DEFAULT 0 | 软删除 |

### ScheduleRecurrence（循环排课规则表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| CourseId | bigint FK NOT NULL | 课程ID |
| TeacherId | bigint FK NOT NULL | 教师ID |
| ClassroomId | bigint FK NOT NULL | 教室ID |
| WeekDays | varchar(20) NOT NULL | 周几，如"1,3,5" |
| StartTime | time NOT NULL | 开始时间 |
| EndTime | time NOT NULL | 结束时间 |
| StartDate | date NOT NULL | 开始日期 |
| EndDate | date NOT NULL | 结束日期 |
| TotalLessons | int NOT NULL | 总课次 |
| GeneratedLessons | int DEFAULT 0 | 已生成课次数 |
| Status | tinyint DEFAULT 0 | 0生成中 1已完成 2已取消 |
| CreatedBy | bigint NOT NULL | 创建人ID |
| CreatedAt | datetime2 NOT NULL | 创建时间 |

### ScheduleChangeLog（排课变更记录表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| ScheduleId | bigint FK NOT NULL | 排课ID |
| ChangeType | tinyint NOT NULL | 0新建 1修改 2取消 3恢复 |
| OldData | nvarchar(max) | 变更前数据（JSON） |
| NewData | nvarchar(max) | 变更后数据（JSON） |
| Reason | nvarchar(200) | 变更原因 |
| OperatorId | bigint NOT NULL | 操作人ID |
| CreatedAt | datetime2 NOT NULL | 创建时间 |

## 3.6 API接口规划

### 教室管理 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/classroom/list | 教室列表 | 教师/机构管理员 |
| POST | /api/classroom | 新增教室 | 机构管理员 |
| PUT | /api/classroom/{id} | 编辑教室 | 机构管理员 |
| DELETE | /api/classroom/{id} | 删除教室 | 机构管理员 |

### 排课管理 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/schedule | 创建单次排课 | 教师/机构管理员 |
| POST | /api/schedule/recurrence | 创建循环排课 | 教师/机构管理员 |
| PUT | /api/schedule/{id} | 修改排课 | 教师/机构管理员 |
| POST | /api/schedule/{id}/cancel | 取消课次 | 教师/机构管理员 |
| POST | /api/schedule/{id}/publish | 发布课表 | 机构管理员 |
| GET | /api/schedule/page | 排课分页列表 | 教师/机构管理员 |
| GET | /api/schedule/calendar | 排课日历数据（按月/周） | 教师/机构管理员/学生 |
| GET | /api/schedule/check-conflict | 冲突检测 | 教师/机构管理员 |
| GET | /api/schedule/change-log/{scheduleId} | 排课变更记录 | 教师/机构管理员 |

## 3.7 页面规划

### PC端页面

| 页面 | 路由 | 说明 |
|------|------|------|
| 教室管理 | /classroom/list | 教室列表+CRUD |
| 排课管理 | /schedule/list | 排课列表+筛选 |
| 创建排课 | /schedule/create | 单次/循环排课表单 |
| 排课日历 | /schedule/calendar | 月/周视图日历，支持拖拽（预留） |
| 冲突检测 | /schedule/conflict | 排课冲突提示面板 |
| 排课变更记录 | /schedule/log | 排课变更历史 |

### 小程序端页面

| 页面 | 说明 |
|------|------|
| 课表 | 日/周视图展示已选课程排课 |
| 课次详情 | 单次上课时间、教室、教师信息 |

> ⚠️ **待确认**：排课是否需要支持拖拽调整？这会增加前端复杂度，当前设计预留但第一版不实现拖拽。
> ⚠️ **待确认**：是否需要课节概念（如上午第1节、第2节）？还是自由时间？当前设计为自由时间设置。
