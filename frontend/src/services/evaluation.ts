import { request } from '@umijs/max';

/** 评价分页列表 */
export async function getEvaluationPage(params: API.EvaluationPageParams) {
  return request<API.PagedResult<API.Evaluation>>('/api/evaluation/page', {
    method: 'GET',
    params,
  });
}

/** 评价详情 */
export async function getEvaluationDetail(id: number) {
  return request<API.Evaluation>(`/api/evaluation/${id}`, {
    method: 'GET',
  });
}
