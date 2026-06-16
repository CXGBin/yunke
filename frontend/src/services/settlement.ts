import { request } from '@umijs/max';

/** 结算记录分页列表 */
export async function getSettlementPage(params: API.SettlementPageParams) {
  return request<API.PagedResult<API.Settlement>>('/api/settlement/page', {
    method: 'GET',
    params,
  });
}

/** 结算详情 */
export async function getSettlementDetail(id: number) {
  return request<API.Settlement>(`/api/settlement/${id}`, {
    method: 'GET',
  });
}
