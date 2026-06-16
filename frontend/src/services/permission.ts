import { request } from '@umijs/max';

/** 获取菜单树 */
export async function getMenuTree() {
  return request<API.MenuItem[]>('/api/menu/tree', { method: 'GET' });
}

/** 创建菜单 */
export async function createMenu(data: API.MenuParams) {
  return request<number>('/api/menu', { method: 'POST', data });
}

/** 更新菜单 */
export async function updateMenu(id: number, data: API.MenuParams) {
  return request(`/api/menu/${id}`, { method: 'PUT', data });
}

/** 删除菜单 */
export async function deleteMenu(id: number) {
  return request(`/api/menu/${id}`, { method: 'DELETE' });
}

/** 获取角色列表 */
export async function getRoleList(params: API.PageParams) {
  return request<API.PagedResult<API.RoleItem>>('/api/role/list', { method: 'GET', params });
}

/** 获取角色详情 */
export async function getRoleDetail(id: number) {
  return request<API.RoleItem & { menuIds: number[] }>(`/api/role/${id}`, { method: 'GET' });
}

/** 创建角色 */
export async function createRole(data: API.RoleParams) {
  return request<number>('/api/role', { method: 'POST', data });
}

/** 更新角色 */
export async function updateRole(id: number, data: API.RoleParams) {
  return request(`/api/role/${id}`, { method: 'PUT', data });
}

/** 删除角色 */
export async function deleteRole(id: number) {
  return request(`/api/role/${id}`, { method: 'DELETE' });
}
