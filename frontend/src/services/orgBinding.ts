import { request } from '@umijs/max';

/** 获取用户绑定的机构列表 */
export async function getMyOrgs() {
  return request<any[]>('/api/org-binding/my-orgs', { method: 'GET' });
}

/** 获取机构详情 */
export async function getOrgDetail(orgId: number) {
  return request<any>(`/api/org-binding/detail/${orgId}`, { method: 'GET' });
}
