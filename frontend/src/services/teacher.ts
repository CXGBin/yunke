import { request } from '@umijs/max';

/** 教师分页列表 (后端: GET /api/teacher/page) */
export async function getTeacherPage(params: { page?: number; pageSize?: number; keyword?: string }) {
  return request<API.PagedResult<any>>('/api/teacher/page', {
    method: 'GET',
    params,
  });
}

/** 教师详情 (后端: GET /api/teacher/{id}) */
export async function getTeacherDetail(id: number) {
  return request(`/api/teacher/${id}`, {
    method: 'GET',
  });
}

/** 创建教师 (后端: POST /api/teacher) */
export async function createTeacher(data: any) {
  return request('/api/teacher', {
    method: 'POST',
    data,
  });
}

/** 更新教师 (后端: PUT /api/teacher/{id}) */
export async function updateTeacher(id: number, data: any) {
  return request(`/api/teacher/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 更新教师状态 (后端: PUT /api/teacher/{id}/status) */
export async function updateTeacherStatus(id: number, status: number) {
  return request(`/api/teacher/${id}/status`, {
    method: 'PUT',
    data: { status },
  });
}

/** 公开教师列表 (后端: GET /api/teacher/public-list) */
export async function getPublicTeachers() {
  return request('/api/teacher/public-list', {
    method: 'GET',
  });
}
