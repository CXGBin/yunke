import { request } from '@umijs/max';

/** 课程分页列表 */
export async function getCoursePage(params: API.CoursePageParams) {
  return request<API.PagedResult<API.Course>>('/api/course/page', {
    method: 'GET',
    params,
  });
}

/** 课程详情 */
export async function getCourseDetail(id: number) {
  return request<API.Course>(`/api/course/${id}`, {
    method: 'GET',
  });
}

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

/** 课程分类树 */
export async function getCategoryTree() {
  return request<any[]>('/api/course-category/tree', {
    method: 'GET',
  });
}

/** 删除分类 */
export async function deleteCategory(id: number) {
  return request(`/api/course-category/${id}`, {
    method: 'DELETE',
  });
}

/** 课时列表 */
export async function getLessons(params: any) {
  return request<API.PagedResult<any>>('/api/lesson-unit/page', {
    method: 'GET',
    params,
  });
}
