# 1. 课程发布模块

## 1.1 模块概述

课程发布是整个系统的起点。教师创建课程，机构管理员审核后上架，学生即可浏览和选课。

## 1.2 功能清单

### 1.2.1 课程创建（教师）

| 功能点 | 说明 |
|--------|------|
| 基本信息 | 课程名称、课程编码（自动生成）、课程分类、适用年龄段 |
| 课程描述 | 富文本编辑，支持图片插入 |
| 封面图片 | 上传课程封面，支持裁剪 |
| 课程详情 | 课时数、单课时时长（分钟）、总课时、课程级别（入门/进阶/高级） |
| 价格设置 | 原价、优惠价、是否免费 |
| 招生设置 | 最大人数、最小开班人数、报名截止时间 |
| 附件资料 | 课件上传（PDF/PPT等），最多10个 |
| 标签 | 课程标签，便于筛选（如"热门"、"推荐"、"新课"） |

### 1.2.2 课程编辑（教师）

- 修改未上架课程的全部信息
- 已上架课程仅可修改描述、附件，不可改价格和人数限制
- 课程变更记录留存

### 1.2.3 课程审核（机构管理员）

| 审核状态 | 说明 |
|----------|------|
| 草稿 | 教师创建但未提交 |
| 待审核 | 教师提交，等待管理员审核 |
| 审核通过 | 管理员通过，可上架 |
| 审核驳回 | 管理员驳回，附驳回原因 |
| 已上架 | 对学生可见，可被选课 |
| 已下架 | 不再接受选课，已选课不受影响 |

### 1.2.4 课程上下架（机构管理员）

- 审核通过的课程方可上架
- 上架后学生端可见可报名
- 下架后停止新报名，已有选课记录保留
- 可设置上架/下架时间（定时上下架）

### 1.2.5 课程分类管理（机构管理员）

- 支持多级分类（如：学科培训 → 数学 → 奥数）
- 分类CRUD
- 分类排序

### 1.2.6 课程列表（多角色）

| 角色 | 视角 |
|------|------|
| 教师 | 我创建的课程，支持按状态筛选 |
| 机构管理员 | 本机构所有课程，支持按教师/分类/状态筛选 |
| 平台管理员 | 全平台课程，支持按机构/分类/状态筛选 |
| 学生/家长 | 仅已上架课程 |

## 1.3 业务流程

```
教师创建课程(草稿)
      │
      ▼
教师提交审核 ──── 修改 ──→ 返回草稿
      │
      ▼
机构管理员审核
      │
  ┌───┴───┐
  │       │
通过     驳回(附原因) ──→ 教师修改重新提交
  │
  ▼
上架(手动/定时) ──→ 学生可见可报名
  │
  ▼
下架(手动/定时) ──→ 停止报名，保留已有数据
```

## 1.4 角色权限矩阵

| 操作 | 教师 | 机构管理员 | 平台管理员 |
|------|:----:|:---------:|:---------:|
| 创建课程 | ✅ | ✅ | ✅ |
| 编辑自己课程 | ✅ | ✅ | ✅ |
| 编辑他人课程 | ❌ | ✅ | ✅ |
| 提交审核 | ✅ | — | — |
| 审核课程 | ❌ | ✅ | ✅ |
| 上下架 | ❌ | ✅ | ✅ |
| 管理分类 | ❌ | ✅ | ✅ |
| 删除课程 | ❌ | ✅(仅草稿) | ✅ |

## 1.5 数据库表设计

### Course（课程表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID，多租户隔离字段） |
| OrgId | bigint | 机构ID（冗余TenantId，便于关联查询） |
| CampusId | bigint FK | 所属校区ID（课程归属校区，可跨校区选课） |
| CourseCode | varchar(32) UNIQUE | 课程编码（自动生成） |
| Name | nvarchar(200) NOT NULL | 课程名称 |
| CategoryId | bigint FK | 分类ID |
| Description | nvarchar(max) | 富文本描述 |
| CoverImage | varchar(500) | 封面图片URL |
| TotalLessons | int NOT NULL DEFAULT 1 | 总课时数 |
| LessonDuration | int NOT NULL DEFAULT 45 | 单课时分钟数 |
| Difficulty | tinyint DEFAULT 0 | 级别：0入门 1进阶 2高级 |
| OriginalPrice | decimal(10,2) DEFAULT 0 | 原价 |
| DiscountPrice | decimal(10,2) DEFAULT 0 | 优惠价 |
| IsFree | bit DEFAULT 0 | 是否免费 |
| MaxStudents | int DEFAULT 30 | 最大人数 |
| MinStudents | int DEFAULT 1 | 最小开班人数 |
| EnrollmentDeadline | datetime2 | 报名截止时间 |
| Tags | varchar(500) | 标签，逗号分隔 |
| Status | tinyint NOT NULL DEFAULT 0 | 状态：0草稿 1待审核 2已通过 3已驳回 4已上架 5已下架 |
| TeacherId | bigint FK | 主讲教师ID |
| ReviewerId | bigint | 审核人ID |
| ReviewTime | datetime2 | 审核时间 |
| ReviewRemark | nvarchar(500) | 审核备注/驳回原因 |
| SortOrder | int DEFAULT 0 | 排序权重 |
| IsRecommend | bit DEFAULT 0 | 是否推荐 |
| ScheduledPublishTime | datetime2 | 定时上架时间 |
| ScheduledOfflineTime | datetime2 | 定时下架时间 |
| ViewCount | int DEFAULT 0 | 浏览量 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |
| IsDeleted | bit DEFAULT 0 | 软删除 |

### CourseCategory（课程分类表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| ParentId | bigint DEFAULT 0 | 父分类ID，0为顶级 |
| Name | nvarchar(50) NOT NULL | 分类名称 |
| Icon | varchar(200) | 分类图标 |
| SortOrder | int DEFAULT 0 | 排序 |
| IsDeleted | bit DEFAULT 0 | 软删除 |

### CourseAttachment（课程附件表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| CourseId | bigint FK NOT NULL | 课程ID |
| FileName | nvarchar(200) NOT NULL | 文件名 |
| FileUrl | varchar(500) NOT NULL | 文件URL |
| FileSize | bigint DEFAULT 0 | 文件大小(字节) |
| FileType | varchar(50) | 文件类型 |
| SortOrder | int DEFAULT 0 | 排序 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |

## 1.6 API接口规划

### 课程管理 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/course | 创建课程 | 教师/机构管理员 |
| PUT | /api/course/{id} | 编辑课程 | 教师(自己)/机构管理员 |
| DELETE | /api/course/{id} | 删除课程(仅草稿) | 机构管理员 |
| GET | /api/course/{id} | 课程详情 | 登录用户 |
| GET | /api/course/page | 课程分页列表 | 登录用户(按角色过滤) |
| POST | /api/course/{id}/submit-review | 提交审核 | 教师 |
| POST | /api/course/{id}/review | 审核通过/驳回 | 机构管理员 |
| POST | /api/course/{id}/publish | 上架 | 机构管理员 |
| POST | /api/course/{id}/offline | 下架 | 机构管理员 |

### 课程分类 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/course-category/tree | 分类树 | 登录用户 |
| POST | /api/course-category | 新增分类 | 机构管理员 |
| PUT | /api/course-category/{id} | 编辑分类 | 机构管理员 |
| DELETE | /api/course-category/{id} | 删除分类 | 机构管理员 |

### 课程附件 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/course/{id}/attachment | 上传附件 | 教师/机构管理员 |
| DELETE | /api/course/attachment/{id} | 删除附件 | 教师/机构管理员 |

## 1.7 页面规划

### PC端页面

| 页面 | 路由 | 说明 |
|------|------|------|
| 课程列表 | /course/list | 分页列表，支持筛选/搜索/批量操作 |
| 课程创建 | /course/create | 课程信息填写表单 |
| 课程编辑 | /course/edit/:id | 编辑已有课程 |
| 课程审核 | /course/review | 待审核列表+审核操作 |
| 课程分类 | /course/category | 分类树管理 |
| 课程详情 | /course/detail/:id | 查看课程完整信息 |

### 小程序端页面

| 页面 | 说明 |
|------|------|
| 课程首页 | 轮播推荐+分类入口+课程列表 |
| 课程分类页 | 按分类浏览课程 |
| 课程详情页 | 课程信息+教师信息+立即报名按钮 |
| 课程搜索页 | 关键词+分类+级别筛选 |

> ⚠️ **待确认**：课程是否需要版本管理？即课程信息修改后保留历史版本？当前设计为直接覆盖更新。
