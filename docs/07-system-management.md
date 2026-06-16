# 7. 系统管理模块

## 7.1 模块概述

系统管理模块是平台的基础设施模块，包含机构管理、校区管理、用户管理、角色权限管理、主题配置等功能。本模块是其他所有业务模块的前置依赖。

## 7.2 功能清单

### 7.2.1 机构管理（平台管理员）

| 功能点 | 说明 |
|--------|------|
| 机构CRUD | 机构名称、Logo、联系人、联系电话、地址、状态 |
| 机构状态 | 启用/停用/过期 |
| 机构配置 | 机构级别的参数配置（退课免费天数、签到方式、评价审核开关等） |
| 机构查询 | 按名称/状态/地区筛选，分页列表 |
| 机构概览 | 查看某机构的统计数据（学生数、课程数、教师数） |
| 开通机构 | 创建机构账号，生成管理员初始密码 |

### 7.2.2 校区管理（机构管理员）

| 功能点 | 说明 |
|--------|------|
| 校区CRUD | 校区名称、地址、联系电话、负责人、状态 |
| 校区状态 | 启用/停用 |
| 默认校区 | 创建机构时自动创建默认校区，不可删除 |
| 校区查询 | 按名称/状态筛选 |
| 校区概览 | 查看某校区的统计数据（教师数、学生数、教室数） |

### 7.2.3 教师管理（机构管理员）

| 功能点 | 说明 |
|--------|------|
| 教师CRUD | 姓名、手机号、头像、所属校区、工号、简介、状态 |
| 教师账号 | 机构管理员创建教师账号（用户名+初始密码），或教师自主注册+审核 |
| 教师跨校区 | 教师主校区固定，但可被其他校区的课程排课 |
| 教师状态 | 启用/停用 |
| 教师查询 | 按校区/姓名/状态筛选 |

### 7.2.4 学生管理（机构管理员）

| 功能点 | 说明 |
|--------|------|
| 学生列表 | 查看本机构所有校区学生，按校区/姓名/手机号筛选 |
| 学生详情 | 学生基本信息、所属校区、已选课程、出勤记录 |
| 学生导入 | 批量导入学生（Excel，预留） |
| 学号生成 | 注册时自动生成学号（机构编码+年份+序号） |

### 7.2.5 家长管理（机构管理员）

| 功能点 | 说明 |
|--------|------|
| 家长列表 | 查看本机构所有家长，按手机号/关联学生数筛选 |
| 家长详情 | 家长基本信息、关联的孩子列表 |
| 关联关系管理 | 查看/解绑家长-学生关联 |

### 7.2.6 家长关联学生（家长/机构管理员）

| 功能点 | 说明 |
|--------|------|
| 家长绑定孩子 | 家长在小程序输入孩子学号或扫码，孩子/家长确认后建立关联 |
| 机构代绑定 | 机构管理员在后台手动建立家长-学生关联 |
| 解除关联 | 家长或机构管理员可解除关联 |
| 关联限制 | 一个家长最多关联10个孩子；一个孩子最多关联5个家长 |

### 7.2.7 角色权限管理

| 功能点 | 说明 |
|--------|------|
| 角色定义 | 系统预置角色：平台管理员、机构管理员、教师、学生、家长 |
| 权限点 | 按模块+操作定义权限（如：course:create, course:review, schedule:manage） |
| 角色授权 | 角色绑定权限点集合 |
| 自定义角色（预留） | 机构可自定义角色并分配权限，第一期暂不实现 |

### 7.2.8 主题配置

| 功能点 | 说明 |
|--------|------|
| 平台主题 | 平台管理员配置全局默认主题 |
| 机构主题 | 机构管理员配置本机构小程序主题（主色、辅色、按钮色、背景色、Logo） |
| 主题切换 | 学生端可在可选主题中切换 |
| 主题预览 | 配置时实时预览效果（PC端） |

### 7.2.9 系统参数配置

| 功能点 | 说明 |
|--------|------|
| 全局参数 | 平台管理员配置（如：签到超时分钟数、评价审核开关、报名截止天数） |
| 机构参数 | 机构管理员配置（覆盖全局默认值） |

## 7.3 业务流程

### 7.3.1 机构开通流程

```
平台管理员在后台创建机构
      │
      ▼
填写机构信息（名称/Logo/联系人/联系电话/地址）
      │
      ▼
自动创建：
  ├─ 机构记录（Organization表）
  ├─ 默认校区（Campus表，名称=机构名称+"默认校区"）
  ├─ 机构管理员账号（SysUser表）
  └─ 机构基础配置（OrgConfig表）
      │
      ▼
平台管理员通知机构管理员初始账号密码
      │
      ▼
机构管理员首次登录 → 修改密码
      │
      ▼
创建更多校区 → 添加教师 → 运营开始
```

### 7.3.2 学生/家长注册流程

```
家长/学生打开小程序 → 微信授权登录
      │
      ▼
绑定手机号（微信getPhoneNumber）
      │
      ▼
选择机构（搜索/定位附近机构）
      │
      ▼
选择校区（机构下的校区列表）
      │
  ┌───┴───┐
  │       │
学生注册  家长注册
  │       │
  ▼       ▼
填写学生  无需填写
基本信息  （家长只绑定手机号即可）
(姓名/    │
年龄/年级) ▼
  │     家长首页提示"关联学生"
  │       │
  ▼       ▼
等待审核   输入孩子学号/扫码
（可选）    │
  │       ▼
  ▼     孩子确认/机构后台确认
注册完成   │
          ▼
        关联成功 → 查看孩子信息
```

## 7.4 角色权限矩阵

| 操作 | 平台管理员 | 机构管理员 | 教师 | 学生 | 家长 |
|------|:---------:|:---------:|:----:|:----:|:----:|
| 管理机构 | ✅ | ❌ | ❌ | ❌ | ❌ |
| 管理校区 | ✅(全平台) | ✅(本机构) | ❌ | ❌ | ❌ |
| 管理教师 | ✅ | ✅(本机构) | ❌ | ❌ | ❌ |
| 管理学生 | ✅ | ✅(本机构) | ❌ | ❌ | ❌ |
| 管理家长 | ✅ | ✅(本机构) | ❌ | ❌ | ❌ |
| 关联学生 | ✅ | ✅ | ❌ | ❌ | ✅ |
| 配置机构参数 | ❌ | ✅ | ❌ | ❌ | ❌ |
| 配置平台参数 | ✅ | ❌ | ❌ | ❌ | ❌ |
| 管理主题 | ✅(全局) | ✅(本机构) | ❌ | ❌ | ❌ |
| 切换主题 | ❌ | ❌ | ❌ | ✅ | ✅ |

## 7.5 数据库表设计

### Organization（机构表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID，多租户隔离字段） |
| OrgCode | varchar(32) UNIQUE NOT NULL | 机构编码（如YK001） |
| Name | nvarchar(200) NOT NULL | 机构名称 |
| Logo | varchar(500) | 机构Logo |
| ContactPerson | nvarchar(50) | 联系人 |
| ContactPhone | varchar(20) | 联系电话 |
| Address | nvarchar(300) | 地址 |
| Province | nvarchar(50) | 省份 |
| City | nvarchar(50) | 城市 |
| District | nvarchar(50) | 区县 |
| Status | tinyint NOT NULL DEFAULT 1 | 1启用 0停用 2过期 |
| ExpiredAt | datetime2 | 过期时间（SaaS到期） |
| Description | nvarchar(500) | 机构简介 |
| ThemeConfig | nvarchar(max) | 主题配置JSON |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |
| IsDeleted | bit DEFAULT 0 | 软删除 |

### Campus（校区表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint FK NOT NULL | 机构ID（冗余TenantId，便于校区查机构） |
| CampusCode | varchar(32) UNIQUE NOT NULL | 校区编码（如YK001-HD） |
| Name | nvarchar(200) NOT NULL | 校区名称 |
| IsDefault | bit DEFAULT 0 | 是否默认校区 |
| ContactPerson | nvarchar(50) | 负责人 |
| ContactPhone | varchar(20) | 联系电话 |
| Address | nvarchar(300) | 校区地址 |
| Longitude | decimal(10,6) | 经度（定位用） |
| Latitude | decimal(10,6) | 纬度（定位用） |
| Status | tinyint NOT NULL DEFAULT 1 | 1启用 0停用 |
| SortOrder | int DEFAULT 0 | 排序 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |
| IsDeleted | bit DEFAULT 0 | 软删除 |

### SysUser（系统用户表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint | 所属机构ID（平台管理员为NULL） |
| CampusId | bigint | 所属校区ID（平台/机构管理员可为NULL） |
| UserCode | varchar(64) UNIQUE | 用户编码（学号/工号） |
| UserName | varchar(64) UNIQUE NOT NULL | 登录用户名/手机号 |
| Password | varchar(256) | 密码哈希（BCrypt） |
| RealName | nvarchar(50) | 真实姓名 |
| NickName | nvarchar(50) | 昵称 |
| Avatar | varchar(500) | 头像URL |
| Phone | varchar(20) | 手机号 |
| Gender | tinyint DEFAULT 0 | 0未知 1男 2女 |
| BirthDate | date | 出生日期 |
| Grade | nvarchar(20) | 年级（学生） |
| Role | tinyint NOT NULL | 角色：1平台管理员 2机构管理员 3教师 4学生 5家长 |
| OpenId | varchar(128) | 微信OpenId |
| UnionId | varchar(128) | 微信UnionId |
| Status | tinyint NOT NULL DEFAULT 1 | 1正常 0停用 2待审核 |
| LastLoginAt | datetime2 | 最后登录时间 |
| LastLoginIp | varchar(50) | 最后登录IP |
| PasswordChangedAt | datetime2 | 密码修改时间 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |
| IsDeleted | bit DEFAULT 0 | 软删除 |

### ParentStudentRelation（家长-学生关联表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint NOT NULL | 机构ID |
| ParentId | bigint FK NOT NULL | 家长用户ID |
| StudentId | bigint FK NOT NULL | 学生用户ID |
| RelationType | tinyint DEFAULT 0 | 关系类型：0父亲 1母亲 2爷爷 3奶奶 4其他 |
| IsPrimary | bit DEFAULT 0 | 是否主要联系人 |
| Status | tinyint DEFAULT 1 | 1正常 0已解绑 |
| ConfirmedBy | bigint | 确认人ID（学生本人或机构管理员） |
| ConfirmedAt | datetime2 | 确认时间 |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |

**唯一约束**：(ParentId, StudentId) 保证同一家长同一学生只能有一条关联

### OrgConfig（机构配置表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| TenantId | bigint NOT NULL | 租户ID（= 机构ID） |
| OrgId | bigint FK UNIQUE NOT NULL | 机构ID |
| FreeRefundDays | int DEFAULT 3 | 免费退课天数（开课前） |
| SignInMethods | varchar(100) DEFAULT "0,3" | 允许的签到方式，逗号分隔（0手动 1扫码 2定位 3一键全到） |
| AttendanceTimeout | int DEFAULT 15 | 签到超时分钟数（开课后多久标记迟到） |
| EnableEvaluationReview | bit DEFAULT 0 | 评价是否需要审核 |
| EnableLeaveApproval | bit DEFAULT 0 | 请假是否需要审批 |
| WaitlistExpireHours | int DEFAULT 24 | 候补通知过期小时数 |
| MaxStudentsPerParent | int DEFAULT 10 | 一个家长最多关联孩子数 |
| MaxParentsPerStudent | int DEFAULT 5 | 一个孩子最多关联家长数 |
| MaxCoursesPerStudent | int DEFAULT 20 | 一个学生最多选课数 |
| ThemeConfig | nvarchar(max) | 机构主题配置JSON |
| CreatedAt | datetime2 NOT NULL | 创建时间 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |

### SysConfig（系统全局配置表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint PK | 主键 |
| ConfigKey | varchar(100) UNIQUE NOT NULL | 配置键 |
| ConfigValue | nvarchar(max) NOT NULL | 配置值 |
| ConfigGroup | varchar(50) | 配置分组 |
| Description | nvarchar(200) | 说明 |
| UpdatedAt | datetime2 NOT NULL | 更新时间 |

## 7.6 API接口规划

### 机构管理 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/organization | 创建机构 | 平台管理员 |
| PUT | /api/organization/{id} | 编辑机构 | 平台管理员 |
| GET | /api/organization/{id} | 机构详情 | 平台管理员 |
| GET | /api/organization/page | 机构列表 | 平台管理员 |
| PUT | /api/organization/{id}/status | 更新机构状态 | 平台管理员 |

### 校区管理 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/campus | 创建校区 | 机构管理员 |
| PUT | /api/campus/{id} | 编辑校区 | 机构管理员 |
| GET | /api/campus/{id} | 校区详情 | 机构管理员 |
| GET | /api/campus/list | 校区列表（本机构） | 机构管理员/教师/注册选择 |
| PUT | /api/campus/{id}/status | 更新校区状态 | 机构管理员 |
| GET | /api/campus/public-list | 校区公开列表（小程序注册用） | 公开（登录后） |

### 教师管理 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/teacher | 创建教师 | 机构管理员 |
| PUT | /api/teacher/{id} | 编辑教师 | 机构管理员 |
| GET | /api/teacher/{id} | 教师详情 | 机构管理员 |
| GET | /api/teacher/page | 教师列表 | 机构管理员 |
| PUT | /api/teacher/{id}/status | 更新教师状态 | 机构管理员 |
| GET | /api/teacher/public-list | 教师公开列表（小程序展示） | 登录用户 |

### 学生管理 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/student/page | 学生列表 | 机构管理员 |
| GET | /api/student/{id} | 学生详情 | 机构管理员/教师 |
| POST | /api/student/import | 批量导入学生 | 机构管理员 |

### 家长管理 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/parent/page | 家长列表 | 机构管理员 |
| GET | /api/parent/{id} | 家长详情 | 机构管理员 |
| POST | /api/parent/bind-student | 机构代绑定家长-学生 | 机构管理员 |
| DELETE | /api/parent/unbind/{id} | 解除关联 | 机构管理员/家长 |

### 关联关系 API（家长端）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/parent/link-student | 家长请求关联学生 | 家长 |
| POST | /api/parent/confirm-link | 确认关联 | 学生/家长 |
| GET | /api/parent/my-children | 我关联的孩子列表 | 家长 |
| GET | /api/parent/my-parents | 我的家长列表 | 学生 |

### 配置 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/config/org | 获取机构配置 | 机构管理员 |
| PUT | /api/config/org | 更新机构配置 | 机构管理员 |
| GET | /api/config/sys | 获取全局配置 | 平台管理员 |
| PUT | /api/config/sys | 更新全局配置 | 平台管理员 |

### 主题 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/theme/current | 获取当前主题 | 登录用户 |
| GET | /api/theme/list | 可选主题列表 | 登录用户 |
| PUT | /api/theme/org | 更新机构主题 | 机构管理员 |
| POST | /api/theme/switch | 切换主题 | 学生/家长 |

### 登录/注册 API

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/auth/login | PC端账号密码登录 | 公开 |
| POST | /api/auth/wx-login | 小程序微信登录 | 公开 |
| POST | /api/auth/bind-phone | 绑定手机号 | 登录用户 |
| POST | /api/auth/register-student | 学生注册 | 公开 |
| POST | /api/auth/register-parent | 家长注册 | 公开 |
| POST | /api/auth/change-password | 修改密码 | 登录用户 |
| GET | /api/auth/user-info | 获取当前用户信息 | 登录用户 |

## 7.7 页面规划

### PC端页面

| 页面 | 路由 | 说明 |
|------|------|------|
| 机构列表 | /organization/list | 机构管理（平台管理员） |
| 机构详情 | /organization/detail/:id | 机构信息+统计数据 |
| 校区管理 | /campus/list | 校区列表+CRUD |
| 教师管理 | /teacher/list | 教师列表+CRUD |
| 学生管理 | /student/list | 学生列表+详情 |
| 家长管理 | /parent/list | 家长列表+关联管理 |
| 主题配置 | /theme/config | 机构主题配置 |
| 系统参数 | /config/system | 全局参数（平台管理员） |
| 机构参数 | /config/org | 机构参数 |

### 小程序端页面

| 页面 | 说明 |
|------|------|
| 登录/授权 | 微信一键登录 |
| 手机号绑定 | 获取手机号 |
| 选择机构 | 搜索/定位选择机构 |
| 选择校区 | 机构下校区列表 |
| 注册信息 | 学生填写基本信息/家长跳过 |
| 关联孩子 | 家长输入孩子学号或扫码 |
| 个人中心 | 个人信息、切换主题、关联管理 |
