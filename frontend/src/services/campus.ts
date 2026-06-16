import { request } from '@umijs/max';

/** 校区分页列表 */
export async function getCampusPage(params: API.PageParams & { keyword?: string; orgId?: number; status?: number }) {
  return request<API.PagedResult<API.Campus>>('/api/campus/page', {
    method: 'GET',
    params,
  });
}

/** 校区详情 */
export async function getCampusDetail(id: number) {
  return request<API.Campus>(`/api/campus/${id}`, {
    method: 'GET',
  });
}

/** 创建校区 */
export async function createCampus(data: API.CampusParams) {
  return request('/api/campus', {
    method: 'POST',
    data,
  });
}

/** 更新校区 */
export async function updateCampus(id: number, data: Partial<API.CampusParams>) {
  return request(`/api/campus/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除校区 */
export async function deleteCampus(id: number) {
  return request(`/api/campus/${id}`, {
    method: 'DELETE',
  });
}

/** 更新校区状态 */
export async function updateCampusStatus(id: number, status: number) {
  return request(`/api/campus/${id}/status`, {
    method: 'PUT',
    data: { status },
  });
}
