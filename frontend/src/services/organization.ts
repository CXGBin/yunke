import { request } from '@umijs/max';

/** 机构分页列表 (后端: GET /api/organization/page) */
export async function getOrgPage(params: API.PageParams & { keyword?: string; status?: number }) {
  return request<API.PagedResult<API.Organization>>('/api/organization/page', {
    method: 'GET',
    params,
  });
}

/** 机构详情 (后端: GET /api/organization/{id}) */
export async function getOrgDetail(id: number) {
  return request<API.Organization>(`/api/organization/${id}`, {
    method: 'GET',
  });
}

/** 创建机构 (后端: POST /api/organization) */
export async function createOrg(data: API.OrganizationParams) {
  return request('/api/organization', {
    method: 'POST',
    data,
  });
}

/** 更新机构 (后端: PUT /api/organization/{id}) */
export async function updateOrg(id: number, data: Partial<API.OrganizationParams>) {
  return request(`/api/organization/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 更新机构状态 (后端: PUT /api/organization/{id}/status) */
export async function updateOrgStatus(id: number, status: number) {
  return request(`/api/organization/${id}/status`, {
    method: 'PUT',
    data: { status },
  });
}
export async function deleteOrg(id: number) { return request(`/api/organization/${id}`, { method: "DELETE" }); }
