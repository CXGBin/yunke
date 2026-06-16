import { request } from '@umijs/max';

/** 通知模板分页列表 (后端: GET /api/notification/template/page) */
export async function getNotificationTemplatePage(params: API.PageParams & { keyword?: string; templateType?: number }) {
  return request<API.PagedResult<API.NotificationTemplate>>('/api/notification/template/page', {
    method: 'GET',
    params,
  });
}

/** 创建通知模板 (后端: POST /api/notification/template) */
export async function createNotificationTemplate(data: API.NotificationParams) {
  return request('/api/notification/template', {
    method: 'POST',
    data,
  });
}

/** 更新通知模板 (后端: PUT /api/notification/template/{id}) */
export async function updateNotificationTemplate(id: number, data: Partial<API.NotificationParams>) {
  return request(`/api/notification/template/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除通知模板 (后端: DELETE /api/notification/template/{id}) */
export async function deleteNotificationTemplate(id: number) {
  return request(`/api/notification/template/${id}`, {
    method: 'DELETE',
  });
}

/** 获取通知配置 (后端: GET /api/notification/config) */
export async function getNotificationConfig() {
  return request('/api/notification/config', {
    method: 'GET',
  });
}

/** 更新通知配置 (后端: PUT /api/notification/config) */
export async function updateNotificationConfig(data: any) {
  return request('/api/notification/config', {
    method: 'PUT',
    data,
  });
}

/** 我的通知列表 (后端: GET /api/notification/my-list) */
export async function getMyNotifications(params: API.PageParams) {
  return request<API.PagedResult<API.NotificationLog>>('/api/notification/my-list', {
    method: 'GET',
    params,
  });
}

/** 我的未读数量 (后端: GET /api/notification/my-unread-count) */
export async function getUnreadCount() {
  return request<number>('/api/notification/my-unread-count', {
    method: 'GET',
  });
}

/** 标记已读 (后端: PUT /api/notification/{id}/read) */
export async function markNotificationRead(id: number) {
  return request(`/api/notification/${id}/read`, {
    method: 'PUT',
  });
}

/** 全部已读 (后端: POST /api/notification/read-all) */
export async function markAllNotificationsRead() {
  return request('/api/notification/read-all', {
    method: 'POST',
  });
}

/** 发送通知 (后端: POST /api/notification/send) */
export async function sendNotification(data: {
  recipientId: number;
  notifyType: number;
  channel: number;
  title: string;
  content: string;
  relateType?: string;
  relateId?: number;
}) {
  return request('/api/notification/send', {
    method: 'POST',
    data,
  });
}

export const getNotificationPage = getMyNotifications;
export const createNotification = createNotificationTemplate;
export const updateNotification = updateNotificationTemplate;
export const deleteNotification = deleteNotificationTemplate;

