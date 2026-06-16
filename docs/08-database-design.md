# 8. 数据库设计汇总

> 数据库：MSSQL 2019  
> 连接串：Server=146.56.242.129;Database=YunKeEdu;User Id=sa;Password=YunKe1…ue;  
> 命名规范：PascalCase，表名无前缀（业务表），系统表用 Sys 前缀

## 全局约定

1. **主键**：所有表使用 `bigint` 自增主键 `Id`
2. **软删除**：`IsDeleted bit DEFAULT 0`，查询时需过滤 `IsDeleted = 0`
3. **时间审计**：`CreatedAt datetime2 NOT NULL`，`UpdatedAt datetime2 NOT NULL`
4. **多租户隔离**：所有业务表均包含 `TenantId bigint NOT NULL`（= 机构ID），实现共享库+租户字段方案
5. **ORM全局过滤器**：SqlSugar 在查询层自动追加 `WHERE TenantId = @CurrentTenantId`，平台管理员跨租户时移除过滤器
6. **外键**：逻辑外键（不启用数据库级 FK 约束，通过代码层保证一致性）
7. **TenantId与OrgId关系**：TenantId 即为机构ID，OrgId 字段在部分表中保留以便于显式关联查询，两者指向同一机构

## ER 关系图

```
Organization(=Tenant) 1──N Campus
Organization 1──N SysUser
Organization 1──1 OrgConfig
Campus 1──N SysUser (教师/学生所属校区)
Campus 1──N Classroom
Campus 1──N CourseSchedule

SysUser (教师) 1──N Course
SysUser (学生) N──N Course (通过 CourseEnrollment)
SysUser (家长) N──N SysUser (学生) (通过 ParentStudentRelation)

Course 1──N CourseCategory
Course 1──N CourseAttachment
Course 1──N CourseEnrollment
Course 1──N CourseSchedule
Course 1──N CourseEvaluation

CourseSchedule 1──N Attendance
CourseSchedule 1──N LeaveRequest

SysUser 1──N Attendance
SysUser 1──N CourseEvaluation
SysUser 1──N WaitList
SysUser 1──N LeaveRequest

注：Organization即租户(Tenant)，TenantId = Organization.Id
所有业务表均包含 TenantId 字段指向 Organization.Id
```

## 表清单

### 系统管理模块（7张表）

| 序号 | 表名 | 说明 | 所属文档 |
|------|------|------|---------|
| 1 | Organization | 机构表 | 07-系统管理 |
| 2 | Campus | 校区表 | 07-系统管理 |
| 3 | SysUser | 系统用户表（统一存储所有角色） | 07-系统管理 |
| 4 | ParentStudentRelation | 家长-学生关联表 | 07-系统管理 |
| 5 | OrgConfig | 机构配置表 | 07-系统管理 |
| 6 | SysConfig | 系统全局配置表 | 07-系统管理 |
| 7 | EvaluationTag | 评价标签配置表 | 05-评价 |

### 课程发布模块（3张表）

| 序号 | 表名 | 说明 | 所属文档 |
|------|------|------|---------|
| 8 | Course | 课程表 | 01-课程发布 |
| 9 | CourseCategory | 课程分类表 | 01-课程发布 |
| 10 | CourseAttachment | 课程附件表 | 01-课程发布 |

### 选课模块（2张表）

| 序号 | 表名 | 说明 | 所属文档 |
|------|------|------|---------|
| 11 | CourseEnrollment | 选课/报名表 | 02-选课 |
| 12 | WaitList | 候补表 | 02-选课 |

### 排课模块（4张表）

| 序号 | 表名 | 说明 | 所属文档 |
|------|------|------|---------|
| 13 | Classroom | 教室表 | 03-排课 |
| 14 | CourseSchedule | 排课计划表 | 03-排课 |
| 15 | ScheduleRecurrence | 循环排课规则表 | 03-排课 |
| 16 | ScheduleChangeLog | 排课变更记录表 | 03-排课 |

### 签到模块（3张表）

| 序号 | 表名 | 说明 | 所属文档 |
|------|------|------|---------|
| 17 | Attendance | 签到记录表 | 04-签到 |
| 18 | LeaveRequest | 请假记录表 | 04-签到 |
| 19 | SignInQRCode | 签到二维码表 | 04-签到 |

### 评价模块（2张表）

| 序号 | 表名 | 说明 | 所属文档 |
|------|------|------|---------|
| 20 | CourseEvaluation | 课程评价表 | 05-评价 |
| 21 | EvaluationReply | 追加评价/回复表 | 05-评价 |

### 数据分析模块（2张表）

| 序号 | 表名 | 说明 | 所属文档 |
|------|------|------|---------|
| 22 | StatisticsDailySnapshot | 每日统计快照表 | 06-数据分析 |
| 23 | StatisticsCourseSnapshot | 课程统计快照表 | 06-数据分析 |

**共计 23 张表**

## 索引规划

### 关键索引

| 表名 | 索引字段 | 索引类型 | 说明 |
|------|---------|---------|------|
| SysUser | UserName | UNIQUE | 登录用户名唯一 |
| SysUser | OpenId | INDEX | 微信OpenId查询 |
| SysUser | OrgId + CampusId + Role | INDEX | 按机构+校区+角色查询 |
| Course | OrgId + Status | INDEX | 机构已上架课程查询 |
| Course | OrgId + TeacherId | INDEX | 教师课程列表 |
| CourseEnrollment | CourseId + Status | INDEX | 课程选课名单 |
| CourseEnrollment | StudentId + Status | INDEX | 学生已选课程 |
| CourseSchedule | OrgId + CampusId + LessonDate | INDEX | 校区日排课查询 |
| CourseSchedule | TeacherId + LessonDate | INDEX | 教师日排课查询 |
| Attendance | ScheduleId | INDEX | 课次签到列表 |
| Attendance | StudentId | INDEX | 学生签到记录 |
| CourseEvaluation | CourseId | INDEX | 课程评价列表 |
| ParentStudentRelation | ParentId | INDEX | 家长的孩子列表 |
| ParentStudentRelation | StudentId | INDEX | 孩子的家长列表 |
| StatisticsDailySnapshot | OrgId + StatDate | UNIQUE INDEX | 每日统计唯一 |
| StatisticsCourseSnapshot | CourseId + StatMonth | UNIQUE INDEX | 课程月统计唯一 |

## 建库脚本（简要）

```sql
-- 建库（如需）
-- CREATE DATABASE YunKeEdu;
-- GO
-- USE YunKeEdu;
-- GO

-- 各表 CREATE TABLE 语句详见各模块文档中的表结构定义
-- 建议按模块顺序执行建表
```

> ⚠️ **待确认**：完整的建表 SQL 脚本将在需求确认后输出。
