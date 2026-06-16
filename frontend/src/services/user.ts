import { request } from '@umijs/max';

/** 用户分页列表 */
export async function getUserPage(params: API.UserPageParams) {
  return request<API.PagedResult<API.SysUser>>('/api/user/page', {
    method: 'GET',
    params,
  });
}

/** 用户详情 */
export async function getUserDetail(id: number) {
  return request<API.SysUser>(`/api/user/${id}`, {
    method: 'GET',
  });
}

/** 更新用户 */
export async function updateUser(id: number, data: Partial<API.SysUser>) {
  return request(`/api/user/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除用户 */
export async function deleteUser(id: number) {
  return request(`/api/user/${id}`, {
    method: 'DELETE',
  });
}

/** 更新用户状态 */
export async function updateUserStatus(id: number, status: number) {
  return request(`/api/user/${id}/status`, {
    method: 'PUT',
    data: { status },
  });
}

/** 重置密码 */
export async function resetUserPassword(id: number) {
  return request(`/api/user/${id}/reset-password`, {
    method: 'POST',
  });
}
