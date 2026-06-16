import { request } from '@umijs/max';

/** 系统配置列表 */
export async function getConfigPage(params: API.PageParams & { configGroup?: string; keyword?: string }) {
  return request<API.PagedResult<API.SysConfig>>('/api/sys-config/page', {
    method: 'GET',
    params,
  });
}

/** 系统配置详情 */
export async function getConfigDetail(id: number) {
  return request<API.SysConfig>(`/api/sys-config/${id}`, {
    method: 'GET',
  });
}

/** 创建系统配置 */
export async function createConfig(data: API.SysConfigParams) {
  return request('/api/sys-config', {
    method: 'POST',
    data,
  });
}

/** 更新系统配置 */
export async function updateConfig(id: number, data: Partial<API.SysConfigParams>) {
  return request(`/api/sys-config/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除系统配置 */
export async function deleteConfig(id: number) {
  return request(`/api/sys-config/${id}`, {
    method: 'DELETE',
  });
}
