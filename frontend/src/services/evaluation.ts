import { request } from '@umijs/max';

/** 创建评价 (后端: POST /api/evaluation) */
export async function createEvaluation(data: Partial<API.Evaluation>) {
  return request('/api/evaluation', {
    method: 'POST',
    data,
  });
}

/** 收到的评价 (后端: GET /api/evaluation/received) */
export async function getReceivedEvaluations(params: API.PageParams & { targetType?: number }) {
  return request<API.PagedResult<API.Evaluation>>('/api/evaluation/received', {
    method: 'GET',
    params,
  });
}

/** 课程评价列表 (后端: GET /api/evaluation/course/{courseId}) */
export async function getCourseEvaluations(courseId: number, params: API.PageParams) {
  return request<API.PagedResult<API.Evaluation>>(`/api/evaluation/course/${courseId}`, {
    method: 'GET',
    params,
  });
}

/** 我的评价 (后端: GET /api/evaluation/my) */
export async function getMyEvaluations(params: API.PageParams) {
  return request<API.PagedResult<API.Evaluation>>('/api/evaluation/my', {
    method: 'GET',
    params,
  });
}

/** 评价分页列表 (后端: GET /api/evaluation/page) */
export async function getEvaluationPage(params: API.PageParams) {
  return request<API.PagedResult<API.Evaluation>>('/api/evaluation/page', {
    method: 'GET',
    params,
  });
}

/** 回复评价 (后端: POST /api/evaluation/{id}/reply) */
export async function replyEvaluation(id: number, data: { content: string; images?: string }) {
  return request(`/api/evaluation/${id}/reply`, {
    method: 'POST',
    data,
  });
}

/** 置顶评价 (后端: PUT /api/evaluation/{id}/top) */
export async function topEvaluation(id: number) {
  return request(`/api/evaluation/${id}/top`, {
    method: 'PUT',
  });
}

/** 隐藏评价 (后端: PUT /api/evaluation/{id}/hide) */
export async function hideEvaluation(id: number) {
  return request(`/api/evaluation/${id}/hide`, {
    method: 'PUT',
  });
}

/** 课程评价统计 (后端: GET /api/evaluation/statistics/course/{courseId}) */
export async function getCourseEvaluationStatistics(courseId: number) {
  return request(`/api/evaluation/statistics/course/${courseId}`, {
    method: 'GET',
  });
}

/** 评价标签列表 (后端: GET /api/evaluation-tag/list) */
export async function getEvaluationTags() {
  return request<API.EvaluationTag[]>('/api/evaluation-tag/list', {
    method: 'GET',
  });
}

/** 创建评价标签 (后端: POST /api/evaluation-tag) */
export async function createEvaluationTag(data: { name: string; tagType?: number; sortOrder?: number }) {
  return request('/api/evaluation-tag', {
    method: 'POST',
    data,
  });
}

/** 删除评价标签 (后端: DELETE /api/evaluation-tag/{id}) */
export async function deleteEvaluationTag(id: number) {
  return request(`/api/evaluation-tag/${id}`, {
    method: 'DELETE',
  });
}
