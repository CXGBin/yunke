# 6. 数据分析模块

## 6.1 模块概述

数据分析模块为机构管理员和平台管理员提供多维度的业务数据报表和可视化看板，辅助运营决策。学生端也可查看个人学习数据。

## 6.2 功能清单

### 6.2.1 机构数据看板（机构管理员）

| 看板项 | 说明 |
|--------|------|
| 概览卡片 | 在线课程数、总学生数、本月新增学生、本月营收（预留） |
| 选课趋势 | 近30天每日选课人数折线图 |
| 出勤率趋势 | 近30天每日出勤率折线图 |
| 课程热度TOP10 | 选课人数最多的课程排行 |
| 满意度分布 | 评价星级分布饼图 |
| 教师教学数据 | 各教师出勤率、评分、课次量对比 |

### 6.2.2 出勤分析

| 维度 | 说明 |
|------|------|
| 按课程 | 每门课程的平均出勤率 |
| 按教师 | 每位教师的授课出勤率 |
| 按时段 | 不同时段（上午/下午/晚上）的出勤率 |
| 按月 | 月度出勤率趋势 |
| 缺勤TOP学生 | 出勤率最低的学生列表 |

### 6.2.3 选课分析

| 维度 | 说明 |
|------|------|
| 选课率 | 每门课程的选课人数/最大人数 |
| 选课趋势 | 按日期/周/月的选课量趋势 |
| 分类热度 | 各课程分类的选课量占比 |
| 退课率 | 各课程的退课率 |
| 满课率 | 满员课程占比 |

### 6.2.4 满意度分析

| 维度 | 说明 |
|------|------|
| 综合评分 | 机构平均评分趋势 |
| 课程评分排行 | 评分最高/最低的课程 |
| 教师评分排行 | 评分最高/最低的教师 |
| 评价数量趋势 | 每月评价数量趋势 |
| 标签词云 | 高频评价标签词云图 |

### 6.2.5 营收分析（预留）

> ⚠️ **待确认**：是否需要营收分析？如果当前版本不涉及支付/缴费，此模块可预留接口不实现。

| 维度 | 说明 |
|------|------|
| 课程营收 | 各课程的报名收入 |
| 月度营收 | 月度营收趋势 |
| 教师课酬 | 教师课酬统计 |

### 6.2.6 学生学习报告（学生/家长）

| 内容 | 说明 |
|------|------|
| 选课概览 | 已选课程数、已完成课程数 |
| 出勤记录 | 出勤率、缺勤次数 |
| 学习时长 | 总学习课时数 |
| 评价记录 | 已评价/未评价课程 |

### 6.2.7 平台数据监控（平台管理员）

| 看板项 | 说明 |
|--------|------|
| 机构总览 | 机构数量、活跃机构数 |
| 学生总览 | 总学生数、本月新增 |
| 课程总览 | 总课程数、上架课程数 |
| 出勤总览 | 全平台平均出勤率 |
| 机构排行 | 各机构运营数据排行 |

### 6.2.8 报表导出

- 支持导出 Excel 格式
- 可选日期范围、筛选条件
- 导出课程报表、出勤报表、评价报表

## 6.3 数据刷新策略

| 数据类型 | 刷新频率 | 说明 |
|----------|---------|------|
| 看板概览 | 实时/5分钟缓存 | 关键指标 |
| 趋势图表 | 每小时缓存 | 折线图/柱状图 |
| 排行榜 | 每日刷新 | TOP10等 |
| 报表导出 | 实时查询 | 导出时实时计算 |

## 6.4 角色权限矩阵

| 操作 | 学生 | 家长 | 教师 | 机构管理员 | 平台管理员 |
|------|:----:|:----:|:----:|:---------:|:---------:|
| 个人学习报告 | ✅ | ✅ | ❌ | ❌ | ❌ |
| 个人教学数据 | ❌ | ❌ | ✅ | ❌ | ❌ |
| 机构看板 | ❌ | ❌ | ❌ | ✅ | ✅ |
| 平台看板 | ❌ | ❌ | ❌ | ❌ | ✅ |
| 导出报表 | ❌ | ❌ | ❌ | ✅ | ✅ |

## 6.5 数据库表设计

> 数据分析模块主要基于已有业务表聚合计算，不需要新建大量业务表。以下为统计快照表（提升查询性能）。

### StatisticsDailySnapshot（每日统计快照表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID，0=全平台汇总） |
| OrgId | bigint NOT NULL | 机构ID（0为全平台） |
| StatDate | date NOT NULL | 统计日期 |
| NewStudents | int DEFAULT 0 | 新增学生数 |
| ActiveCourses | int DEFAULT 0 | 在线课程数 |
| TotalEnrollments | int DEFAULT 0 | 总选课数 |
| TotalAttendanceRate | decimal(5,2) DEFAULT 0 | 出勤率(%) |
| TotalEvaluations | int DEFAULT 0 | 评价数 |
| AvgRating | decimal(3,2) DEFAULT 0 | 平均评分 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |

**唯一约束**：(OrgId, StatDate)

### StatisticsCourseSnapshot（课程统计快照表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| CourseId | bigint FK NOT NULL | 课程ID |
| StatMonth | date NOT NULL | 统计月份(每月1号) |
| EnrollmentCount | int DEFAULT 0 | 选课人数 |
| RefundCount | int DEFAULT 0 | 退课人数 |
| AttendanceRate | decimal(5,2) DEFAULT 0 | 出勤率(%) |
| AvgRating | decimal(3,2) DEFAULT 0 | 平均评分 |
| EvaluationCount | int DEFAULT 0 | 评价数 |
| CompletedLessons | int DEFAULT 0 | 已完成课次 |
| TotalLessons | int DEFAULT 0 | 总课次 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |

**唯一约束**：(CourseId, StatMonth)

## 6.6 API接口规划

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/statistics/dashboard/org | 机构看板数据 | 机构管理员 |
| GET | /api/statistics/dashboard/platform | 平台看板数据 | 平台管理员 |
| GET | /api/statistics/attendance | 出勤分析 | 机构管理员 |
| GET | /api/statistics/enrollment | 选课分析 | 机构管理员 |
| GET | /api/statistics/satisfaction | 满意度分析 | 机构管理员 |
| GET | /api/statistics/my-report | 个人学习报告 | 学生/家长 |
| GET | /api/statistics/teacher-report | 教师教学报告 | 教师 |
| GET | /api/statistics/export | 导出报表(Excel) | 机构管理员/平台管理员 |

## 6.7 页面规划

### PC端页面

| 页面 | 路由 | 说明 |
|------|------|------|
| 数据看板 | /statistics/dashboard | 概览卡片+趋势图+排行 |
| 出勤分析 | /statistics/attendance | 出勤多维度分析 |
| 选课分析 | /statistics/enrollment | 选课多维度分析 |
| 满意度分析 | /statistics/satisfaction | 评价满意度分析 |
| 教师教学报告 | /statistics/teacher | 教师数据对比（机构管理员） |
| 报表导出 | /statistics/export | 导出配置+下载 |

### 小程序端页面

| 页面 | 说明 |
|------|------|
| 我的学习 | 学习报告（选课数、出勤率、课时数） |
