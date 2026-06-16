import { request } from '@umijs/max';

/** 用户列表 (后端: GET /api/users/list) */
export async function getUserList(params: API.UserPageParams) {
  return request<API.PagedResult<API.SysUser>>('/api/users/list', {
    method: 'GET',
    params,
  });
}

/** 用户详情 (后端: GET /api/users/{id}) */
export async function getUserDetail(id: number) {
  return request<API.SysUser>(`/api/users/${id}`, {
    method: 'GET',
  });
}

/** 创建用户 (后端: POST /api/users) */
export async function createUser(data: API.CreateUserParams) {
  return request('/api/users', {
    method: 'POST',
    data,
  });
}

/** 更新用户 (后端: PUT /api/users/{id}) */
export async function updateUser(id: number, data: Partial<API.CreateUserParams>) {
  return request(`/api/users/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除用户 (后端: DELETE /api/users/{id}) */
export async function deleteUser(id: number) {
  return request(`/api/users/${id}`, {
    method: 'DELETE',
  });
}

/** 重置密码 (后端: POST /api/users/{id}/reset-password) */
export async function resetUserPassword(id: number, newPassword?: string) {
  return request(`/api/users/${id}/reset-password`, {
    method: 'POST',
    data: { newPassword },
  });
}

/** 别名兼容 */
export const getUserPage = getUserList;
export async function updateUserStatus(id: number, status: number) {
  return request(`/api/users/${id}/status`, { method: 'PUT', data: { status } });
}
export async function getParents(params: any) { return request<API.PagedResult<any>>("/api/parent/page", { method: "GET", params }); }
