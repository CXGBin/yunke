import { request } from '@umijs/max';

/** 生成邀请链接 */
export async function generateInvitation(data: { orgId: number; campusId: number; invitedRole: number; invitedName?: string; invitedPhone?: string; remark?: string }) {
  return request('/api/invitation/generate', { method: 'POST', data });
}

/** 邀请列表 */
export async function getInvitationPage(params: API.PageParams) {
  return request<API.PagedResult<any>>('/api/invitation/page', { method: 'GET', params });
}

/** 取消邀请 */
export async function cancelInvitation(id: number) {
  return request(`/api/invitation/${id}/cancel`, { method: 'PUT' });
}

/** 接受邀请 */
export async function acceptInvitation(data: { inviteCode: string }) {
  return request('/api/invitation/accept', { method: 'POST', data });
}

/** 验证邀请码 */
export async function validateInvitation(inviteCode: string) {
  return request(`/api/invitation/validate/${inviteCode}`, { method: 'GET' });
}
