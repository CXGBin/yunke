import { request } from '@umijs/max';

/** 课程套餐分页列表 */
export async function getCoursePackagePage(params: API.PageParams & { keyword?: string; orgId?: number }) {
  return request<API.PagedResult<API.CoursePackage>>('/api/course-package/page', {
    method: 'GET',
    params,
  });
}

/** 课程套餐详情 */
export async function getCoursePackageDetail(id: number) {
  return request<API.CoursePackage>(`/api/course-package/${id}`, {
    method: 'GET',
  });
}
