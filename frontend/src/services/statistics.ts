import { request } from '@umijs/max';

/** 机构仪表盘 (后端: GET /api/statistics/dashboard/org) */
export async function getOrgDashboard() {
  return request<API.DashboardStats>('/api/statistics/dashboard/org', {
    method: 'GET',
  });
}

/** 数据分析概览 (后端: GET /api/statistics/overview) */
export async function getStatisticsOverview() {
  return request<API.OrgOverview>('/api/statistics/overview', {
    method: 'GET',
  });
}

/** 平台仪表盘 (后端: GET /api/statistics/dashboard/platform) */
export async function getPlatformDashboard() {
  return request('/api/statistics/dashboard/platform', {
    method: 'GET',
  });
}

/** 出勤分析 (后端: GET /api/statistics/attendance) */
export async function getAttendanceStatistics(params?: { startDate?: string; endDate?: string }) {
  return request('/api/statistics/attendance', {
    method: 'GET',
    params,
  });
}

/** 报名分析 (后端: GET /api/statistics/enrollment) */
export async function getEnrollmentStatistics() {
  return request('/api/statistics/enrollment', {
    method: 'GET',
  });
}

/** 满意度分析 (后端: GET /api/statistics/satisfaction) */
export async function getSatisfactionStatistics() {
  return request('/api/statistics/satisfaction', {
    method: 'GET',
  });
}

/** 个人报告 (后端: GET /api/statistics/my-report) */
export async function getMyReport() {
  return request('/api/statistics/my-report', {
    method: 'GET',
  });
}

/** 教师报告 (后端: GET /api/statistics/teacher-report) */
export async function getTeacherReport() {
  return request('/api/statistics/teacher-report', {
    method: 'GET',
  });
}

/** 收入统计 (后端: GET /api/statistics/revenue) */
export async function getRevenueStatistics(year?: number) {
  return request('/api/statistics/revenue', {
    method: 'GET',
    params: year !== undefined ? { year } : undefined,
  });
}

/** 收支明细 (后端: GET /api/statistics/revenue/expense) */
export async function getRevenueExpense(year?: number) {
  return request('/api/statistics/revenue/expense', {
    method: 'GET',
    params: year !== undefined ? { year } : undefined,
  });
}

/** 收入汇总 (后端: GET /api/statistics/revenue/summary) */
export async function getRevenueSummary(year?: number) {
  return request('/api/statistics/revenue/summary', {
    method: 'GET',
    params: year !== undefined ? { year } : undefined,
  });
}

/** 课时消耗-学生 (后端: GET /api/statistics/lesson-consumption/student/{studentId}) */
export async function getLessonConsumptionStudent(studentId: number) {
  return request(`/api/statistics/lesson-consumption/student/${studentId}`, {
    method: 'GET',
  });
}

/** 课时消耗-课程 (后端: GET /api/statistics/lesson-consumption/course/{courseId}) */
export async function getLessonConsumptionCourse(courseId: number) {
  return request(`/api/statistics/lesson-consumption/course/${courseId}`, {
    method: 'GET',
  });
}

/** 课时消耗-机构 (后端: GET /api/statistics/lesson-consumption/org) */
export async function getLessonConsumptionOrg() {
  return request('/api/statistics/lesson-consumption/org', {
    method: 'GET',
  });
}

/** 导出报表 (后端: GET /api/statistics/export) */
export async function exportStatistics(type?: string, year?: number) {
  return request('/api/statistics/export', {
    method: 'GET',
    params: { type, year },
  });
}

export const getDashboardStats = getStatisticsOverview;

