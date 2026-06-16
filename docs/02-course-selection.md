# 2. 选课模块

## 2.1 模块概述

学生/家长浏览已上架课程并进行选课（报名），支持退课。系统管理选课名额、Waitlist候补、个人课表。

## 2.2 功能清单

### 2.2.1 课程浏览（学生/家长）

- 首页推荐课程（机构管理员标记的推荐课程）
- 按分类浏览
- 按条件筛选：价格区间、级别、年龄段、上课时间
- 搜索课程名称
- 查看课程详情（含已选人数/剩余名额、教师简介、课程大纲）

### 2.2.2 选课/报名（学生/家长）

| 功能点 | 说明 |
|--------|------|
| 报名 | 点击报名，确认课程信息后提交 |
| 名额校验 | 校验剩余名额，满员则进入Waitlist |
| 重复校验 | 同一课程不可重复报名 |
| 时间冲突校验 | 校验与已选课程的时间是否冲突 |
| 年龄校验 | 校验学生年龄是否符合课程适用范围 |
| 报名确认 | 报名成功后展示选课凭证/二维码 |

> ⚠️ **待确认**：免费课程是否也需要报名流程？还是直接加入？当前设计为所有课程统一走报名流程。

### 2.2.3 退课（学生/家长）

- 已选课程可申请退课
- 退课规则：
  - 开课前 N 天可免费退课（N 可由机构配置）
  - 超过免费退课期限则不可退课
  - 退课后名额自动释放给 Waitlist 第一位
- 退课记录留存

### 2.2.4 Waitlist 候补（学生/家长）

- 课程满员时，学生可加入候补名单
- 有人退课/机构扩容时，自动按候补顺序通知
- 学生可主动取消候补

### 2.2.5 我的课表（学生/家长/教师）

| 角色 | 说明 |
|------|------|
| 学生 | 查看已选课程的排课时间表（日/周/月视图） |
| 家长 | 查看关联孩子的课表 |
| 教师 | 查看自己授课的课程排课表 |

### 2.2.6 选课管理（机构管理员）

- 查看某课程的所有选课学生列表
- 手动添加/移除选课学生
- 调整课程最大人数
- 导出选课名单
- 查看待缴费学生（预留，如后续对接支付）

## 2.3 业务流程

```
学生浏览课程 → 查看详情
      │
      ▼
点击报名
      │
  ┌───┼───┐
  │   │   │
  有名额  名额满  时间冲突
  │       │       │
  ▼       ▼       ▼
报名成功  Waitlist  提示冲突
  │       │
  ▼       ▼
展示凭证  加入候补
          │
          ▼(有人退课)
       自动通知 → 确认报名
```

```
学生申请退课
      │
      ▼
校验退课期限
      │
  ┌───┴───┐
  │       │
 可退     不可退
  │       │
  ▼       ▼
退课成功  提示无法退课
  │
  ▼
释放名额 → 通知Waitlist首位
```

## 2.4 角色权限矩阵

| 操作 | 学生 | 家长 | 教师 | 机构管理员 | 平台管理员 |
|------|:----:|:----:|:----:|:---------:|:---------:|
| 浏览课程 | ✅ | ✅ | ✅ | ✅ | ✅ |
| 报名选课 | ✅ | ✅(代孩子) | ❌ | ✅(手动添加) | ✅ |
| 退课 | ✅ | ✅(代孩子) | ❌ | ✅(手动移除) | ✅ |
| 加入Waitlist | ✅ | ✅ | ❌ | ❌ | ❌ |
| 查看课表 | ✅ | ✅ | ✅ | ✅ | ✅ |
| 管理选课名单 | ❌ | ❌ | ❌ | ✅ | ✅ |
| 导出选课名单 | ❌ | ❌ | ❌ | ✅ | ✅ |

## 2.5 数据库表设计

### CourseEnrollment（选课/报名表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| CampusId | bigint | 校区ID（学生所属校区） |
| CourseId | bigint FK NOT NULL | 课程ID |
| StudentId | bigint FK NOT NULL | 学生ID |
| ParentId | bigint | 家长ID（代报名时记录） |
| Status | tinyint NOT NULL DEFAULT 0 | 0已报名 1已退课 2已完成 |
| EnrolledAt | datetime2 NOT NULL | 报名时间 |
| RefundedAt | datetime2 | 退课时间 |
| RefundReason | nvarchar(200) | 退课原因 |
| Remark | nvarchar(200) | 备注 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |

### WaitList（候补表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| CampusId | bigint | 校区ID |
| CourseId | bigint FK NOT NULL | 课程ID |
| StudentId | bigint FK NOT NULL | 学生ID |
| Status | tinyint NOT NULL DEFAULT 0 | 0候补中 1已通知 2已确认 3已取消 4已过期 |
| JoinedAt | datetime2 NOT NULL | 加入候补时间 |
| NotifiedAt | datetime2 | 通知时间 |
| ExpiresAt | datetime2 | 通知过期时间（超时未确认自动取消） |
| CreatedAt | datetime2 NOT NULL | 创建时间 |

**唯一约束**：(CourseId, StudentId) + Status IN (0,1) 保证同一课程不会重复候补

### CourseSchedule（排课计划表 — 与排课模块共用）

> 见排课模块设计，选课模块只读取

## 2.6 API接口规划

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/enrollment | 选课报名 | 学生/家长 |
| DELETE | /api/enrollment/{id} | 退课 | 学生/家长 |
| GET | /api/enrollment/my-courses | 我的已选课程 | 学生/家长 |
| GET | /api/enrollment/my-schedule | 我的课表 | 学生/家长/教师 |
| GET | /api/enrollment/course-students | 某课程的选课学生列表 | 机构管理员/教师 |
| POST | /api/enrollment/manual-add | 手动添加选课 | 机构管理员 |
| DELETE | /api/enrollment/manual-remove/{id} | 手动移除选课 | 机构管理员 |
| POST | /api/waitlist/join | 加入候补 | 学生/家长 |
| DELETE | /api/waitlist/{id} | 取消候补 | 学生/家长 |
| GET | /api/waitlist/my-list | 我的候补列表 | 学生/家长 |
| GET | /api/enrollment/export | 导出选课名单 | 机构管理员 |

## 2.7 页面规划

### 小程序端页面

| 页面 | 说明 |
|------|------|
| 课程首页 | 推荐+分类+列表（见课程模块） |
| 课程详情 | 课程信息+报名按钮+剩余名额 |
| 确认报名页 | 确认课程信息+时间冲突提示 |
| 报名成功页 | 展示选课信息+添加到课表 |
| 我的课表 | 日/周视图课表 |
| 我的课程 | 已选课程列表（支持退课） |
| 候补列表 | 候补中的课程+状态 |
| 退课确认页 | 退课原因填写+确认 |

### PC端页面

| 页面 | 路由 | 说明 |
|------|------|------|
| 选课管理 | /enrollment/list | 按课程查看选课学生 |
| 选课详情 | /enrollment/detail/:courseId | 某课程选课详情 |
| 候补管理 | /enrollment/waitlist | 候补名单管理 |
| 导出选课 | /enrollment/export | 选课名单导出 |

> ⚠️ **待确认**：退课是否需要审批？还是学生在规定时间内可自行退课？当前设计为学生在免费退课期内可直接退课。
