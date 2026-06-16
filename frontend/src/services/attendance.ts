import { request } from '@umijs/max';

/** 签到记录分页列表 */
export async function getAttendancePage(params: API.AttendancePageParams) {
  return request<API.PagedResult<API.Attendance>>('/api/attendance/page', {
    method: 'GET',
    params,
  });
}

/** 签到详情 */
export async function getAttendanceDetail(id: number) {
  return request<API.Attendance>(`/api/attendance/${id}`, {
    method: 'GET',
  });
}
