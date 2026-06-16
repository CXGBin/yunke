import { request } from '@umijs/max';

/** 排课分页列表 */
export async function getSchedulePage(params: API.SchedulePageParams) {
  return request<API.PagedResult<API.Schedule>>('/api/schedule/page', {
    method: 'GET',
    params,
  });
}

/** 排课详情 */
export async function getScheduleDetail(id: number) {
  return request<API.Schedule>(`/api/schedule/${id}`, {
    method: 'GET',
  });
}
