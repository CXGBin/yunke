import { request } from '@umijs/max';

/** 校区列表 (后端: GET /api/campus/list) */
export async function getCampusList() {
  return request<API.Campus[]>('/api/campus/list', {
    method: 'GET',
  });
}

/** 校区详情 (后端: GET /api/campus/{id}) */
export async function getCampusDetail(id: number) {
  return request<API.Campus>(`/api/campus/${id}`, {
    method: 'GET',
  });
}

/** 创建校区 (后端: POST /api/campus) */
export async function createCampus(data: API.CampusParams) {
  return request('/api/campus', {
    method: 'POST',
    data,
  });
}

/** 更新校区 (后端: PUT /api/campus/{id}) */
export async function updateCampus(id: number, data: Partial<API.CampusParams>) {
  return request(`/api/campus/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 更新校区状态 (后端: PUT /api/campus/{id}/status) */
export async function updateCampusStatus(id: number, status: number) {
  return request(`/api/campus/${id}/status`, {
    method: 'PUT',
    data: { status },
  });
}

/** 别名兼容 */
export const getCampusPage = getCampusList;
export async function deleteCampus(id: number) {
  return request(`/api/campus/${id}`, { method: 'DELETE' });
}
