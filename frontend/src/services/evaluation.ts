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

/** 评价标签列表 */
export async function getTags(params: any) {
  return request<API.PagedResult<any>>('/api/evaluation-tag/page', {
    method: 'GET',
    params,
  });
}

/** 删除评价标签 */
export async function deleteTag(id: number) {
  return request(`/api/evaluation-tag/${id}`, {
    method: 'DELETE',
  });
}
