import { request } from '@umijs/max';

/** 报名记录分页列表 */
export async function getEnrollmentPage(params: API.EnrollmentPageParams) {
  return request<API.PagedResult<API.Enrollment>>('/api/enrollment/page', {
    method: 'GET',
    params,
  });
}

/** 报名详情 */
export async function getEnrollmentDetail(id: number) {
  return request<API.Enrollment>(`/api/enrollment/${id}`, {
    method: 'GET',
  });
}
