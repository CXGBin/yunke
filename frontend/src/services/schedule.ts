import { request } from '@umijs/max';

/** 创建排课 (后端: POST /api/schedule) */
export async function createSchedule(data: Partial<API.Schedule>) {
  return request('/api/schedule', {
    method: 'POST',
    data,
  });
}

/** 创建重复排课 (后端: POST /api/schedule/recurrence) */
export async function createRecurrenceSchedule(data: Partial<API.Schedule>) {
  return request('/api/schedule/recurrence', {
    method: 'POST',
    data,
  });
}

/** 更新排课 (后端: PUT /api/schedule/{id}) */
export async function updateSchedule(id: number, data: Partial<API.Schedule>) {
  return request(`/api/schedule/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 取消排课 (后端: POST /api/schedule/{id}/cancel) */
export async function cancelSchedule(id: number, data?: { cancelReason?: string }) {
  return request(`/api/schedule/${id}/cancel`, {
    method: 'POST',
    data,
  });
}

/** 发布排课 (后端: POST /api/schedule/{id}/publish) */
export async function publishSchedule(id: number) {
  return request(`/api/schedule/${id}/publish`, {
    method: 'POST',
  });
}

/** 排课分页列表 (后端: GET /api/schedule/page) */
export async function getSchedulePage(params: API.SchedulePageParams) {
  return request<API.PagedResult<API.Schedule>>('/api/schedule/page', {
    method: 'GET',
    params,
  });
}

/** 日历视图 (后端: GET /api/schedule/calendar) */
export async function getScheduleCalendar(params: { startDate: string; endDate: string }) {
  return request('/api/schedule/calendar', {
    method: 'GET',
    params,
  });
}

/** 冲突检查 (后端: GET /api/schedule/check-conflict) */
export async function checkScheduleConflict(params: {
  campusId: number;
  lessonDate: string;
  startTime: string;
  endTime: string;
  excludeScheduleId?: number;
  excludeTeacherId?: number;
}) {
  return request('/api/schedule/check-conflict', {
    method: 'GET',
    params,
  });
}

/** 排课变更日志 (后端: GET /api/schedule/change-log/{scheduleId}) */
export async function getScheduleChangeLog(scheduleId: number) {
  return request(`/api/schedule/change-log/${scheduleId}`, {
    method: 'GET',
  });
}
