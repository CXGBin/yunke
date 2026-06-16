import { request } from '@umijs/max';

/** 报名 (后端: POST /api/enrollment) */
export async function createEnrollment(data: { courseId: number; parentId?: number; remark?: string }) {
  return request('/api/enrollment', {
    method: 'POST',
    data,
  });
}

/** 我的课程列表 (后端: GET /api/enrollment/my-courses) */
export async function getMyCourses() {
  return request<API.Course[]>('/api/enrollment/my-courses', {
    method: 'GET',
  });
}

/** 我的课程表 (后端: GET /api/enrollment/my-schedule) */
export async function getMySchedule(params?: { startDate?: string; endDate?: string }) {
  return request('/api/enrollment/my-schedule', {
    method: 'GET',
    params,
  });
}

/** 课程学员列表 (后端: GET /api/enrollment/course-students) */
export async function getCourseStudents(courseId: number, params?: API.PageParams) {
  return request<API.PagedResult<API.Enrollment>>('/api/enrollment/course-students', {
    method: 'GET',
    params: { ...params, courseId },
  });
}

/** 手动添加报名 (后端: POST /api/enrollment/manual-add) */
export async function manualAddEnrollment(data: { courseId: number; studentId: number; remark?: string }) {
  return request('/api/enrollment/manual-add', {
    method: 'POST',
    data,
  });
}

/** 手动移除报名 (后端: DELETE /api/enrollment/manual-remove/{id}) */
export async function manualRemoveEnrollment(id: number) {
  return request(`/api/enrollment/manual-remove/${id}`, {
    method: 'DELETE',
  });
}

/** 加入候补 (后端: POST /api/waitlist/join) */
export async function joinWaitlist(courseId: number) {
  return request('/api/waitlist/join', {
    method: 'POST',
    params: { courseId },
  });
}

/** 取消候补 (后端: DELETE /api/waitlist/{id}) */
export async function cancelWaitlist(id: number) {
  return request(`/api/waitlist/${id}`, {
    method: 'DELETE',
  });
}

/** 我的候补列表 (后端: GET /api/waitlist/my-list) */
export async function getMyWaitlist() {
  return request('/api/waitlist/my-list', {
    method: 'GET',
  });
}

export async function getEnrollmentPage(params: API.PageParams & { courseId?: number }) {
  return request<API.PagedResult<any>>("/api/enrollment/page", { method: "GET", params });
}
