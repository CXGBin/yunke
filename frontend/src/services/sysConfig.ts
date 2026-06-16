import { request } from '@umijs/max';

/** 获取机构配置 (后端: GET /api/config/org) */
export async function getOrgConfig() {
  return request('/api/config/org', {
    method: 'GET',
  });
}

/** 更新机构配置 (后端: PUT /api/config/org) */
export async function updateOrgConfig(data: any) {
  return request('/api/config/org', {
    method: 'PUT',
    data,
  });
}

/** 获取系统配置列表 (后端: GET /api/config/sys) */
export async function getSysConfigList(group?: string) {
  return request('/api/config/sys', {
    method: 'GET',
    params: group ? { group } : undefined,
  });
}

/** 更新系统配置 (后端: PUT /api/config/sys) */
export async function updateSysConfig(data: { configKey: string; configValue: string; configGroup?: string; description?: string }) {
  return request('/api/config/sys', {
    method: 'PUT',
    data,
  });
}
export const getConfigPage = getSysConfigs;
export const createConfig = createSysConfig;
export const updateConfig = updateSysConfig;
export async function deleteConfig(id: number) { return request(`/api/system/config/${id}`, { method: "DELETE" }); }
