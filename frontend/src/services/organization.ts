import { request } from '@umijs/max';

/** 机构分页列表 */
export async function getOrgPage(params: API.PageParams & { keyword?: string; status?: number }) {
  return request<API.PagedResult<API.Organization>>('/api/organization/page', {
    method: 'GET',
    params,
  });
}

/** 机构详情 */
export async function getOrgDetail(id: number) {
  return request<API.Organization>(`/api/organization/${id}`, {
    method: 'GET',
  });
}

/** 创建机构 */
export async function createOrg(data: API.OrganizationParams) {
  return request('/api/organization', {
    method: 'POST',
    data,
  });
}

/** 更新机构 */
export async function updateOrg(id: number, data: Partial<API.OrganizationParams>) {
  return request(`/api/organization/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除机构 */
export async function deleteOrg(id: number) {
  return request(`/api/organization/${id}`, {
    method: 'DELETE',
  });
}

/** 更新机构状态 */
export async function updateOrgStatus(id: number, status: number) {
  return request(`/api/organization/${id}/status`, {
    method: 'PUT',
    data: { status },
  });
}
