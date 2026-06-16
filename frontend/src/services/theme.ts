import { request } from '@umijs/max';

/** 获取当前主题 */
export async function getCurrentTheme() {
  return request('/api/theme/current', { method: 'GET' });
}

/** 获取主题列表 */
export async function getThemeList() {
  return request<any[]>('/api/theme/list', { method: 'GET' });
}

/** 更新机构主题 */
export async function updateOrgTheme(data: Partial<API.ThemeConfig>) {
  return request('/api/theme/update-org', { method: 'PUT', data });
}

/** 切换主题 */
export async function switchTheme(data: { themeId: number }) {
  return request('/api/theme/switch', { method: 'POST', data });
}
