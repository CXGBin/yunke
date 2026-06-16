# 4. 签到模块

## 4.1 模块概述

签到模块管理学生每节课的出勤情况。支持多种签到方式，教师可在PC端或小程序端操作签到，学生端可查看签到状态。

## 4.2 功能清单

### 4.2.1 签到方式

| 方式 | 说明 | 操作角色 |
|------|------|---------|
| 教师手动签到 | 教师在课堂上逐个/批量标记出勤 | 教师 |
| 学生扫码签到 | 教师生成签到二维码，学生扫码签到 | 学生 |
| 学生定位签到 | 基于GPS定位签到（预留，第一版不实现） | 学生 |
| 一键全到 | 教师一键将全班标记为已到 | 教师 |

> ⚠️ **待确认**：第一版实现哪些签到方式？建议先实现教师手动签到 + 一键全到，扫码签到作为第二期。

### 4.2.2 签到状态

| 状态 | 编码 | 说明 |
|------|------|------|
| 未签到 | 0 | 课次开始但学生未操作 |
| 已到 | 1 | 正常出勤 |
| 迟到 | 2 | 迟到（教师标记或超时扫码） |
| 缺勤 | 3 | 未到且未请假 |
| 请假 | 4 | 课前请假 |

### 4.2.3 请假管理

| 功能点 | 说明 |
|--------|------|
| 学生请假 | 提前请假，填写请假原因 |
| 家长代请假 | 家长代孩子提交请假 |
| 请假审批 | 机构管理员/教师可审批请假 |
| 请假记录 | 请假历史列表 |

> ⚠️ **待确认**：请假是否需要审批流程？还是学生直接提交即生效？当前设计为直接生效，但预留审批字段。

### 4.2.4 签到管理（教师）

- 按课次查看签到状态
- 修改签到状态（如迟到改为已到）
- 补签（课后补录）
- 签到备注

### 4.2.5 签到统计

| 维度 | 说明 |
|------|------|
| 学生出勤率 | 某学生某课程的出勤率 |
| 课程出勤率 | 某课程整体出勤率 |
| 机构出勤率 | 机构整体出勤率（机构管理员） |
| 月度出勤报表 | 按月统计出勤数据 |

### 4.2.6 签到通知

- 上课前提醒（上课前30分钟）
- 缺勤通知（课后发送给家长）
- 请假确认通知

## 4.3 业务流程

```
课次开始前30分钟 → 上课提醒推送
      │
      ▼
课次开始 → 教师发起签到
      │
      ├─ 一键全到 → 全班标记已到
      ├─ 手动签到 → 逐个标记状态
      └─ 扫码签到 → 学生扫码自动标记
      │
      ▼
课次结束后 → 未签到学生标记为缺勤
      │
      ▼
缺勤通知 → 推送给家长
```

```
学生请假流程：
学生/家长提交请假 → 记录请假信息
      │
      ▼
签到时自动标记为"请假"状态（非缺勤）
      │
      ▼
不影响出勤率计算
```

## 4.4 角色权限矩阵

| 操作 | 学生 | 家长 | 教师 | 机构管理员 | 平台管理员 |
|------|:----:|:----:|:----:|:---------:|:---------:|
| 发起签到 | ❌ | ❌ | ✅ | ✅ | ✅ |
| 扫码签到 | ✅ | ❌ | ❌ | ❌ | ❌ |
| 修改签到状态 | ❌ | ❌ | ✅ | ✅ | ✅ |
| 补签 | ❌ | ❌ | ✅ | ✅ | ✅ |
| 提交请假 | ✅ | ✅ | ❌ | ❌ | ❌ |
| 审批请假 | ❌ | ❌ | ✅(预留) | ✅ | ✅ |
| 查看签到记录 | ✅(自己) | ✅(孩子) | ✅(自己课程) | ✅(本机构) | ✅ |
| 查看出勤统计 | ✅(自己) | ✅(孩子) | ✅(自己课程) | ✅(本机构) | ✅ |

## 4.5 数据库表设计

### Attendance（签到记录表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| CampusId | bigint | 校区ID（学生所属校区） |
| ScheduleId | bigint FK NOT NULL | 排课ID（课次） |
| CourseId | bigint FK NOT NULL | 课程ID |
| StudentId | bigint FK NOT NULL | 学生ID |
| Status | tinyint NOT NULL DEFAULT 0 | 0未签到 1已到 2迟到 3缺勤 4请假 |
| SignInTime | datetime2 | 签到时间 |
| SignMethod | tinyint | 0手动 1扫码 2定位 3一键全到 |
| Remark | nvarchar(200) | 备注 |
| OperatorId | bigint | 操作人ID（教师手动签到时） |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |

**唯一约束**：(ScheduleId, StudentId) 保证同一课次同一学生只有一条记录

### LeaveRequest（请假记录表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| StudentId | bigint FK NOT NULL | 学生ID |
| CourseId | bigint FK NOT NULL | 课程ID |
| ScheduleId | bigint FK | 关联课次（可选，单次请假） |
| LeaveType | tinyint DEFAULT 0 | 0事假 1病假 2其他 |
| StartDate | date NOT NULL | 请假开始日期 |
| EndDate | date NOT NULL | 请假结束日期 |
| Reason | nvarchar(500) NOT NULL | 请假原因 |
| Status | tinyint DEFAULT 1 | 0待审批 1已通过 2已驳回 3已取消 |
| ApplicantId | bigint NOT NULL | 申请人ID（学生或家长） |
| ApproverId | bigint | 审批人ID |
| ApprovedAt | datetime2 | 审批时间 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |

### SignInQRCode（签到二维码表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| ScheduleId | bigint FK NOT NULL | 排课ID |
| QRCodeToken | varchar(64) UNIQUE NOT NULL | 二维码令牌 |
| ExpiresAt | datetime2 NOT NULL | 过期时间 |
| MaxDistance | int DEFAULT 100 | 最大签到距离（米），预留 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |

## 4.6 API接口规划

### 签到 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/attendance/sign-in | 手动签到/修改状态 | 教师/机构管理员 |
| POST | /api/attendance/sign-all | 一键全到 | 教师 |
| POST | /api/attendance/scan-sign | 扫码签到 | 学生 |
| GET | /api/attendance/schedule/{scheduleId} | 某课次签到列表 | 教师/机构管理员 |
| GET | /api/attendance/my-records | 我的签到记录 | 学生/家长 |
| GET | /api/attendance/statistics/student | 学生出勤统计 | 学生/家长 |
| GET | /api/attendance/statistics/course/{courseId} | 课程出勤统计 | 教师/机构管理员 |
| POST | /api/attendance/qrcode/generate | 生成签到二维码 | 教师 |
| GET | /api/attendance/qrcode/validate/{token} | 验证二维码 | 学生 |

### 请假 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/leave | 提交请假 | 学生/家长 |
| GET | /api/leave/my-list | 我的请假记录 | 学生/家长 |
| PUT | /api/leave/{id}/approve | 审批请假 | 机构管理员 |
| GET | /api/leave/page | 请假分页列表 | 机构管理员 |

## 4.7 页面规划

### 小程序端页面

| 页面 | 说明 |
|------|------|
| 扫码签到 | 扫描二维码完成签到 |
| 签到结果 | 签到成功/失败提示 |
| 我的签到 | 签到记录列表 |
| 出勤统计 | 出勤率图表 |
| 提交请假 | 请假表单 |
| 请假记录 | 请假历史 |

### PC端页面

| 页面 | 路由 | 说明 |
|------|------|------|
| 签到管理 | /attendance/list | 按课程/日期查看签到 |
| 签到操作 | /attendance/operate/:scheduleId | 单课次签到面板 |
| 出勤统计 | /attendance/statistics | 出勤率多维统计 |
| 请假管理 | /attendance/leave | 请假审批列表 |
