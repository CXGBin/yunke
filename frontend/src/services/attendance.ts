import { request } from '@umijs/max';

/** 签到 (后端: POST /api/attendance/sign-in) */
export async function signIn(data: { scheduleId: number; studentId: number; status: number; signMethod?: number; remark?: string }) {
  return request('/api/attendance/sign-in', {
    method: 'POST',
    data,
  });
}

/** 全员签到 (后端: POST /api/attendance/sign-all) */
export async function signAll(data: { scheduleId: number; status?: number; remark?: string }) {
  return request('/api/attendance/sign-all', {
    method: 'POST',
    data,
  });
}

/** 按排课获取签到记录 (后端: GET /api/attendance/schedule/{scheduleId}) */
export async function getAttendanceBySchedule(scheduleId: number) {
  return request<API.Attendance[]>(`/api/attendance/schedule/${scheduleId}`, {
    method: 'GET',
  });
}

/** 我的签到记录 (后端: GET /api/attendance/my-records) */
export async function getMyAttendanceRecords(limit?: number) {
  return request<API.Attendance[]>('/api/attendance/my-records', {
    method: 'GET',
    params: limit !== undefined ? { limit } : undefined,
  });
}

/** 学生出勤统计 (后端: GET /api/attendance/statistics/student) */
export async function getStudentAttendanceStatistics() {
  return request('/api/attendance/statistics/student', {
    method: 'GET',
  });
}

/** 课程出勤统计 (后端: GET /api/attendance/statistics/course/{courseId}) */
export async function getCourseAttendanceStatistics(courseId: number) {
  return request(`/api/attendance/statistics/course/${courseId}`, {
    method: 'GET',
  });
}

/** 请假记录分页 (后端: GET /api/leave/page) */
export async function getLeavePage(params: API.PageParams) {
  return request<API.PagedResult<API.LeaveRequest>>('/api/leave/page', {
    method: 'GET',
    params,
  });
}

/** 创建请假 (后端: POST /api/leave) */
export async function createLeave(data: any) {
  return request('/api/leave', {
    method: 'POST',
    data,
  });
}

/** 我的请假列表 (后端: GET /api/leave/my-list) */
export async function getMyLeaves() {
  return request<API.LeaveRequest[]>('/api/leave/my-list', {
    method: 'GET',
  });
}

/** 预审请假 (后端: PUT /api/leave/{id}/pre-review) */
export async function preReviewLeave(id: number, data: { approve: boolean; remark?: string }) {
  return request(`/api/leave/${id}/pre-review`, {
    method: 'PUT',
    data,
  });
}

/** 审批请假 (后端: PUT /api/leave/{id}/approve) */
export async function approveLeave(id: number, data: { approve: boolean; remark?: string }) {
  return request(`/api/leave/${id}/approve`, {
    method: 'PUT',
    data,
  });
}
export async function getAttendancePage(params: any) {
  return request<API.PagedResult<any>>('/api/attendance/page', { method: 'GET', params });
}
