# 9. API 接口汇总

> 接口端：.NET 8 Web API  
> 风格：RESTful  
> 认证：JWT Token（Header: `Authorization: Bearer {token}`）  
> 数据格式：JSON  
> 统一响应结构：`{ code: 0, message: "success", data: T }`

## 全局约定

1. **分页请求**：`GET /api/xxx/page?page=1&pageSize=20&keyword=xxx&status=0`
2. **分页响应**：`{ code: 0, data: { items: [], total: 100, page: 1, pageSize: 20 } }`
3. **错误码**：0=成功，400=参数错误，401=未授权，403=无权限，404=不存在，500=服务器错误
4. **多租户过滤**：JwtMiddleware 解析 Token 中的 TenantId 注入 HttpContext，SqlSugar 全局过滤器自动追加查询条件；平台管理员（TenantId=null）自动移除过滤器实现跨租户查询

## 接口清单汇总

### 认证 & 用户（10个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/auth/login | PC端账号密码登录 | 公开 |
| POST | /api/auth/wx-login | 小程序微信登录 | 公开 |
| POST | /api/auth/bind-phone | 绑定手机号 | 登录用户 |
| POST | /api/auth/register-student | 学生注册 | 公开 |
| POST | /api/auth/register-parent | 家长注册 | 公开 |
| POST | /api/auth/change-password | 修改密码 | 登录用户 |
| GET | /api/auth/user-info | 获取当前用户信息 | 登录用户 |

### 机构管理（5个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/organization | 创建机构 | 平台管理员 |
| PUT | /api/organization/{id} | 编辑机构 | 平台管理员 |
| GET | /api/organization/{id} | 机构详情 | 平台管理员 |
| GET | /api/organization/page | 机构列表 | 平台管理员 |
| PUT | /api/organization/{id}/status | 更新机构状态 | 平台管理员 |

### 校区管理（6个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/campus | 创建校区 | 机构管理员 |
| PUT | /api/campus/{id} | 编辑校区 | 机构管理员 |
| GET | /api/campus/{id} | 校区详情 | 机构管理员 |
| GET | /api/campus/list | 校区列表 | 机构管理员/教师 |
| PUT | /api/campus/{id}/status | 更新校区状态 | 机构管理员 |
| GET | /api/campus/public-list | 校区公开列表 | 登录用户 |

### 教师管理（6个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/teacher | 创建教师 | 机构管理员 |
| PUT | /api/teacher/{id} | 编辑教师 | 机构管理员 |
| GET | /api/teacher/{id} | 教师详情 | 机构管理员 |
| GET | /api/teacher/page | 教师列表 | 机构管理员 |
| PUT | /api/teacher/{id}/status | 更新教师状态 | 机构管理员 |
| GET | /api/teacher/public-list | 教师公开列表 | 登录用户 |

### 学生管理（3个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/student/page | 学生列表 | 机构管理员 |
| GET | /api/student/{id} | 学生详情 | 机构管理员/教师 |
| POST | /api/student/import | 批量导入学生 | 机构管理员 |

### 家长 & 关联关系（7个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/parent/page | 家长列表 | 机构管理员 |
| GET | /api/parent/{id} | 家长详情 | 机构管理员 |
| POST | /api/parent/bind-student | 机构代绑定 | 机构管理员 |
| DELETE | /api/parent/unbind/{id} | 解除关联 | 机构管理员/家长 |
| POST | /api/parent/link-student | 家长请求关联 | 家长 |
| POST | /api/parent/confirm-link | 确认关联 | 学生/家长 |
| GET | /api/parent/my-children | 我关联的孩子 | 家长 |

### 课程管理（12个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/course | 创建课程 | 教师/机构管理员 |
| PUT | /api/course/{id} | 编辑课程 | 教师/机构管理员 |
| DELETE | /api/course/{id} | 删除课程 | 机构管理员 |
| GET | /api/course/{id} | 课程详情 | 登录用户 |
| GET | /api/course/page | 课程分页列表 | 登录用户 |
| POST | /api/course/{id}/submit-review | 提交审核 | 教师 |
| POST | /api/course/{id}/review | 审核课程 | 机构管理员 |
| POST | /api/course/{id}/publish | 上架 | 机构管理员 |
| POST | /api/course/{id}/offline | 下架 | 机构管理员 |
| GET | /api/course-category/tree | 分类树 | 登录用户 |
| POST | /api/course-category | 新增分类 | 机构管理员 |
| PUT | /api/course-category/{id} | 编辑分类 | 机构管理员 |

### 课程附件（2个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/course/{id}/attachment | 上传附件 | 教师/机构管理员 |
| DELETE | /api/course/attachment/{id} | 删除附件 | 教师/机构管理员 |

### 选课管理（11个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/enrollment | 选课报名 | 学生/家长 |
| DELETE | /api/enrollment/{id} | 退课 | 学生/家长 |
| GET | /api/enrollment/my-courses | 我的已选课程 | 学生/家长 |
| GET | /api/enrollment/my-schedule | 我的课表 | 学生/家长/教师 |
| GET | /api/enrollment/course-students | 选课学生列表 | 机构管理员/教师 |
| POST | /api/enrollment/manual-add | 手动添加选课 | 机构管理员 |
| DELETE | /api/enrollment/manual-remove/{id} | 手动移除选课 | 机构管理员 |
| POST | /api/waitlist/join | 加入候补 | 学生/家长 |
| DELETE | /api/waitlist/{id} | 取消候补 | 学生/家长 |
| GET | /api/waitlist/my-list | 我的候补列表 | 学生/家长 |
| GET | /api/enrollment/export | 导出选课名单 | 机构管理员 |

### 排课管理（10个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/schedule | 创建单次排课 | 教师/机构管理员 |
| POST | /api/schedule/recurrence | 创建循环排课 | 教师/机构管理员 |
| PUT | /api/schedule/{id} | 修改排课 | 教师/机构管理员 |
| POST | /api/schedule/{id}/cancel | 取消课次 | 教师/机构管理员 |
| POST | /api/schedule/{id}/publish | 发布课表 | 机构管理员 |
| GET | /api/schedule/page | 排课分页列表 | 教师/机构管理员 |
| GET | /api/schedule/calendar | 排课日历数据 | 登录用户 |
| GET | /api/schedule/check-conflict | 冲突检测 | 教师/机构管理员 |
| GET | /api/schedule/change-log/{id} | 排课变更记录 | 教师/机构管理员 |

### 教室管理（4个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/classroom/list | 教室列表 | 教师/机构管理员 |
| POST | /api/classroom | 新增教室 | 机构管理员 |
| PUT | /api/classroom/{id} | 编辑教室 | 机构管理员 |
| DELETE | /api/classroom/{id} | 删除教室 | 机构管理员 |

### 签到管理（12个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/attendance/sign-in | 手动签到 | 教师/机构管理员 |
| POST | /api/attendance/sign-all | 一键全到 | 教师 |
| POST | /api/attendance/scan-sign | 扫码签到 | 学生 |
| GET | /api/attendance/schedule/{id} | 课次签到列表 | 教师/机构管理员 |
| GET | /api/attendance/my-records | 我的签到记录 | 学生/家长 |
| GET | /api/attendance/statistics/student | 学生出勤统计 | 学生/家长 |
| GET | /api/attendance/statistics/course/{id} | 课程出勤统计 | 教师/机构管理员 |
| POST | /api/attendance/qrcode/generate | 生成签到二维码 | 教师 |
| GET | /api/attendance/qrcode/validate/{token} | 验证二维码 | 学生 |
| POST | /api/leave | 提交请假 | 学生/家长 |
| GET | /api/leave/my-list | 我的请假记录 | 学生/家长 |
| PUT | /api/leave/{id}/approve | 审批请假 | 机构管理员 |

### 评价管理（11个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /api/evaluation | 提交评价 | 学生/家长 |
| GET | /api/evaluation/course/{id} | 课程评价列表 | 登录用户 |
| GET | /api/evaluation/my | 我的评价列表 | 学生/家长 |
| POST | /api/evaluation/{id}/reply | 回复评价 | 教师/机构管理员 |
| POST | /api/evaluation/{id}/supplement | 追加评价 | 学生 |
| PUT | /api/evaluation/{id}/hide | 隐藏评价 | 机构管理员 |
| PUT | /api/evaluation/{id}/top | 置顶评价 | 机构管理员 |
| GET | /api/evaluation/statistics/course/{id} | 课程评价统计 | 登录用户 |
| GET | /api/evaluation/statistics/teacher/{id} | 教师评价统计 | 登录用户 |
| GET | /api/evaluation/tags | 评价标签列表 | 登录用户 |
| GET | /api/evaluation/page | 评价管理列表 | 机构管理员 |

### 数据统计（7个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/statistics/dashboard/org | 机构看板 | 机构管理员 |
| GET | /api/statistics/dashboard/platform | 平台看板 | 平台管理员 |
| GET | /api/statistics/attendance | 出勤分析 | 机构管理员 |
| GET | /api/statistics/enrollment | 选课分析 | 机构管理员 |
| GET | /api/statistics/satisfaction | 满意度分析 | 机构管理员 |
| GET | /api/statistics/my-report | 个人学习报告 | 学生/家长 |
| GET | /api/statistics/export | 导出报表 | 机构管理员/平台管理员 |

### 配置 & 主题（8个接口）

| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/config/org | 获取机构配置 | 机构管理员 |
| PUT | /api/config/org | 更新机构配置 | 机构管理员 |
| GET | /api/config/sys | 获取全局配置 | 平台管理员 |
| PUT | /api/config/sys | 更新全局配置 | 平台管理员 |
| GET | /api/theme/current | 获取当前主题 | 登录用户 |
| GET | /api/theme/list | 可选主题列表 | 登录用户 |
| PUT | /api/theme/org | 更新机构主题 | 机构管理员 |
| POST | /api/theme/switch | 切换主题 | 学生/家长 |

## 接口统计

| 模块 | 接口数 |
|------|--------|
| 认证 & 用户 | 7 |
| 机构管理 | 5 |
| 校区管理 | 6 |
| 教师管理 | 6 |
| 学生管理 | 3 |
| 家长 & 关联 | 7 |
| 课程管理 | 12 |
| 课程附件 | 2 |
| 选课管理 | 11 |
| 排课管理 | 10 |
| 教室管理 | 4 |
| 签到管理 | 12 |
| 评价管理 | 11 |
| 数据统计 | 7 |
| 配置 & 主题 | 8 |
| **合计** | **111** |

## API 分层设计

```
Controllers/                  ← API入口层
├── AuthController
├── OrganizationController
├── CampusController
├── TeacherController
├── StudentController
├── ParentController
├── CourseController
├── CourseCategoryController
├── CourseAttachmentController
├── EnrollmentController
├── WaitListController
├── ScheduleController
├── ClassroomController
├── AttendanceController
├── LeaveController
├── EvaluationController
├── StatisticsController
├── ConfigController
└── ThemeController
```
