import { request } from '@umijs/max';

/** 学生分页列表 (后端: GET /api/student/page) */
export async function getStudentPage(params: { page?: number; pageSize?: number; keyword?: string }) {
  return request<API.PagedResult<API.Student>>('/api/student/page', {
    method: 'GET',
    params,
  });
}

/** 学生详情 (后端: GET /api/student/{id}) */
export async function getStudentDetail(id: number) {
  return request<API.Student>(`/api/student/${id}`, {
    method: 'GET',
  });
}

/** 导入学生 (后端: POST /api/student/import) */
export async function importStudent(data: any) {
  return request('/api/student/import', {
    method: 'POST',
    data,
  });
}
