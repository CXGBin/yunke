# 5. 评价模块

## 5.1 模块概述

评价模块支持学生对课程进行评价，教师查看评价反馈，机构管理员管理评价内容和展示。评价体系包含课程评分、文字评价、教师评分等维度。

## 5.2 功能清单

### 5.2.1 学生评价

| 功能点 | 说明 |
|--------|------|
| 课程评分 | 1-5星评分 |
| 教师评分 | 1-5星评分 |
| 文字评价 | 文字内容（20-500字） |
| 匿名评价 | 可选匿名（默认实名） |
| 标签评价 | 预设标签快速评价（如"内容充实"、"互动性强"、"进度合理"） |
| 图片评价 | 上传评价图片（可选，最多3张） |
| 追加评价 | 课程结束后可追加评价（预留） |
| 评价条件 | 必须已签到至少1次才可评价（防刷评） |

### 5.2.2 评价展示

| 位置 | 说明 |
|------|------|
| 课程详情 | 综合评分 + 评价列表（分页，最新优先） |
| 教师主页 | 教师综合评分 + 评价摘要 |
| 机构首页 | 机构整体评分（加权平均） |

### 5.2.3 评价管理（机构管理员）

- 查看本机构所有评价
- 敏感词过滤/审核
- 评价回复（机构或教师回复）
- 置顶精选评价
- 隐藏不当评价（不删除，仅不展示）

### 5.2.4 教师评价反馈（教师）

- 查看学生对自己授课的评价
- 查看课程综合评分趋势
- 评价数据纳入教师考核（预留）

### 5.2.5 评价统计

| 维度 | 说明 |
|------|------|
| 课程评分 | 某课程的综合平均分 |
| 教师评分 | 某教师的综合平均分 |
| 机构评分 | 机构整体平均分 |
| 评价分布 | 5星/4星/3星/2星/1星分布 |
| 标签分布 | 各评价标签的数量统计 |
| 满意度 | 4星及以上占比为满意度 |

## 5.3 业务流程

```
课程完成 → 系统推送评价邀请
      │
      ▼
学生进入评价页
      │
      ▼
填写评分 + 标签 + 文字（可选图片）
      │
      ▼
提交评价
      │
      ▼
机构管理员审核（可选）
      │
  ┌───┴───┐
  │       │
  通过     隐藏
  │       │
  ▼       ▼
展示      不展示（记录保留）
  │
  ▼
机构/教师可回复评价
```

## 5.4 角色权限矩阵

| 操作 | 学生 | 家长 | 教师 | 机构管理员 | 平台管理员 |
|------|:----:|:----:|:----:|:---------:|:---------:|
| 提交评价 | ✅ | ✅(代孩子) | ❌ | ❌ | ❌ |
| 追加评价 | ✅ | ❌ | ❌ | ❌ | ❌ |
| 查看评价 | ✅ | ✅ | ✅ | ✅ | ✅ |
| 回复评价 | ❌ | ❌ | ✅ | ✅ | ✅ |
| 隐藏评价 | ❌ | ❌ | ❌ | ✅ | ✅ |
| 置顶评价 | ❌ | ❌ | ❌ | ✅ | ✅ |
| 查看评价统计 | ✅(已选课程) | ✅ | ✅(自己) | ✅ | ✅ |

## 5.5 数据库表设计

### CourseEvaluation（课程评价表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| CampusId | bigint | 校区ID |
| CourseId | bigint FK NOT NULL | 课程ID |
| ScheduleId | bigint FK | 课次ID（可关联到具体课次） |
| StudentId | bigint FK NOT NULL | 学生ID |
| CourseRating | tinyint NOT NULL | 课程评分 1-5 |
| TeacherRating | tinyint | 教师评分 1-5 |
| Content | nvarchar(max) | 评价内容 |
| Tags | varchar(200) | 评价标签，逗号分隔 |
| Images | varchar(1000) | 评价图片URL，逗号分隔 |
| IsAnonymous | bit DEFAULT 0 | 是否匿名 |
| Status | tinyint DEFAULT 1 | 0待审核 1已展示 2已隐藏 |
| ReplyContent | nvarchar(max) | 回复内容 |
| ReplyBy | bigint | 回复人ID |
| ReplyAt | datetime2 | 回复时间 |
| IsTop | bit DEFAULT 0 | 是否置顶 |
| LikeCount | int DEFAULT 0 | 点赞数 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |

**唯一约束**：(CourseId, StudentId) 保证同一学生同一课程只能评价一次（不含追加评价）

### EvaluationReply（追加评价/回复表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| EvaluationId | bigint FK NOT NULL | 关联评价ID |
| Content | nvarchar(max) NOT NULL | 追加/回复内容 |
| Images | varchar(1000) | 图片URL |
| ReplyType | tinyint NOT NULL | 0学生追加 1教师回复 2机构回复 |
| ReplyById | bigint NOT NULL | 回复人ID |
| CreatedAt | datetime2 NOT NULL | 创建时间 |

### EvaluationTag（评价标签配置表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| Name | nvarchar(20) NOT NULL | 标签名 |
| SortOrder | int DEFAULT 0 | 排序 |
| Status | tinyint DEFAULT 1 | 1启用 0停用 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |

## 5.6 API接口规划

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/evaluation | 提交评价 | 学生/家长 |
| GET | /api/evaluation/course/{courseId} | 课程评价列表 | 登录用户 |
| GET | /api/evaluation/my | 我的评价列表 | 学生/家长 |
| POST | /api/evaluation/{id}/reply | 回复评价 | 教师/机构管理员 |
| POST | /api/evaluation/{id}/supplement | 追加评价 | 学生 |
| PUT | /api/evaluation/{id}/hide | 隐藏评价 | 机构管理员 |
| PUT | /api/evaluation/{id}/top | 置顶评价 | 机构管理员 |
| GET | /api/evaluation/statistics/course/{courseId} | 课程评价统计 | 登录用户 |
| GET | /api/evaluation/statistics/teacher/{teacherId} | 教师评价统计 | 登录用户 |
| GET | /api/evaluation/tags | 评价标签列表 | 登录用户 |
| GET | /api/evaluation/page | 评价管理列表 | 机构管理员 |

## 5.7 页面规划

### 小程序端页面

| 页面 | 说明 |
|------|------|
| 评价填写页 | 评分+标签+文字+图片 |
| 评价列表页 | 课程评价列表 |
| 评价详情页 | 评价内容+回复 |
| 我的评价 | 我提交过的评价 |

### PC端页面

| 页面 | 路由 | 说明 |
|------|------|------|
| 评价管理 | /evaluation/list | 评价列表+筛选+操作 |
| 评价详情 | /evaluation/detail/:id | 评价详情+回复 |
| 评价统计 | /evaluation/statistics | 多维度评价统计 |

> ⚠️ **待确认**：评价是否需要审核后才能展示？还是提交后直接展示，事后管理？当前设计为直接展示，后台可事后隐藏。
> ⚠️ **待确认**：是否需要点赞功能？当前设计预留点赞字段，但小程序端暂不提供点赞入口。
