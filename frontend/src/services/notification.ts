import { request } from '@umijs/max';

/** 通知模板分页列表 */
export async function getNotificationPage(params: API.PageParams & { keyword?: string; templateType?: number }) {
  return request<API.PagedResult<API.NotificationTemplate>>('/api/notification/page', {
    method: 'GET',
    params,
  });
}

/** 通知模板详情 */
export async function getNotificationDetail(id: number) {
  return request<API.NotificationTemplate>(`/api/notification/${id}`, {
    method: 'GET',
  });
}

/** 创建通知模板 */
export async function createNotification(data: API.NotificationParams) {
  return request('/api/notification', {
    method: 'POST',
    data,
  });
}

/** 更新通知模板 */
export async function updateNotification(id: number, data: Partial<API.NotificationParams>) {
  return request(`/api/notification/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除通知模板 */
export async function deleteNotification(id: number) {
  return request(`/api/notification/${id}`, {
    method: 'DELETE',
  });
}
