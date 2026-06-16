import { request } from '@umijs/max';

/** 年费套餐分页列表 */
export async function getOrgPackagePage(params: API.PageParams & { keyword?: string; status?: number }) {
  return request<API.PagedResult<API.OrgPackage>>('/api/org-package/page', {
    method: 'GET',
    params,
  });
}

/** 套餐详情 */
export async function getOrgPackageDetail(id: number) {
  return request<API.OrgPackage>(`/api/org-package/${id}`, {
    method: 'GET',
  });
}

/** 创建套餐 */
export async function createOrgPackage(data: API.OrgPackageParams) {
  return request('/api/org-package', {
    method: 'POST',
    data,
  });
}

/** 更新套餐 */
export async function updateOrgPackage(id: number, data: Partial<API.OrgPackageParams>) {
  return request(`/api/org-package/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除套餐 */
export async function deleteOrgPackage(id: number) {
  return request(`/api/org-package/${id}`, {
    method: 'DELETE',
  });
}

/** 添加套餐功能 */
export async function addPackageFeature(packageId: number, data: { featureCode: string; featureName: string; featureGroup?: string; minPackageLevel: number }) {
  return request(`/api/org-package/${packageId}/feature`, {
    method: 'POST',
    data,
  });
}

/** 移除套餐功能 */
export async function removePackageFeature(packageId: number, featureCode: string) {
  return request(`/api/org-package/${packageId}/feature/${featureCode}`, {
    method: 'DELETE',
  });
}

/** 订阅历史记录 */
export async function getSubscriptionHistory(params: API.PageParams & { orgId?: number }) {
  return request<API.PagedResult<API.OrgSubscription>>('/api/org-subscription/history', {
    method: 'GET',
    params,
  });
}
