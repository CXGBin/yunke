import { request } from '@umijs/max';

/** 家长分页列表 (后端: GET /api/parent/page) */
export async function getParentPage(params: { page?: number; pageSize?: number; keyword?: string }) {
  return request<API.PagedResult<API.Parent>>('/api/parent/page', {
    method: 'GET',
    params,
  });
}

/** 家长详情 (后端: GET /api/parent/{id}) */
export async function getParentDetail(id: number) {
  return request(`/api/parent/${id}`, {
    method: 'GET',
  });
}

/** 绑定学生 (后端: POST /api/parent/bind-student) */
export async function bindStudent(data: { parentId: number; studentId: number; relationType?: number; isPrimary?: boolean }) {
  return request('/api/parent/bind-student', {
    method: 'POST',
    data,
  });
}

/** 解绑学生 (后端: DELETE /api/parent/unbind/{id}) */
export async function unbindStudent(id: number) {
  return request(`/api/parent/unbind/${id}`, {
    method: 'DELETE',
  });
}

/** 关联学生 (后端: POST /api/parent/link-student) */
export async function linkStudent(data: { studentUserCode: string }) {
  return request('/api/parent/link-student', {
    method: 'POST',
    data,
  });
}

/** 确认关联 (后端: POST /api/parent/confirm-link) */
export async function confirmLink(data: { relationId: number; accept: boolean }) {
  return request('/api/parent/confirm-link', {
    method: 'POST',
    data,
  });
}

/** 我的孩子列表 (后端: GET /api/parent/my-children) */
export async function getMyChildren() {
  return request<API.ChildInfo[]>('/api/parent/my-children', {
    method: 'GET',
  });
}

/** 我的家长列表 (后端: GET /api/parent/my-parents) */
export async function getMyParents() {
  return request<API.Parent[]>('/api/parent/my-parents', {
    method: 'GET',
  });
}
