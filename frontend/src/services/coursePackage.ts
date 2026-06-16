import { request } from '@umijs/max';

/** 课程套餐分页列表 (后端: GET /api/course-package/page) */
export async function getCoursePackagePage(params: API.PageParams & { keyword?: string; orgId?: number }) {
  return request<API.PagedResult<API.CoursePackage>>('/api/course-package/page', {
    method: 'GET',
    params,
  });
}

/** 课程套餐详情 (后端: GET /api/course-package/{id}) */
export async function getCoursePackageDetail(id: number) {
  return request<API.CoursePackage>(`/api/course-package/${id}`, {
    method: 'GET',
  });
}

/** 创建课程套餐 (后端: POST /api/course-package) */
export async function createCoursePackage(data: any) {
  return request('/api/course-package', {
    method: 'POST',
    data,
  });
}

/** 更新课程套餐 (后端: PUT /api/course-package/{id}) */
export async function updateCoursePackage(id: number, data: any) {
  return request(`/api/course-package/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除课程套餐 (后端: DELETE /api/course-package/{id}) */
export async function deleteCoursePackage(id: number) {
  return request(`/api/course-package/${id}`, {
    method: 'DELETE',
  });
}

/** 发布课程套餐 (后端: POST /api/course-package/{id}/publish) */
export async function publishCoursePackage(id: number) {
  return request(`/api/course-package/${id}/publish`, {
    method: 'POST',
  });
}

/** 下线课程套餐 (后端: POST /api/course-package/{id}/offline) */
export async function offlineCoursePackage(id: number) {
  return request(`/api/course-package/${id}/offline`, {
    method: 'POST',
  });
}

/** 添加课程到套餐 (后端: POST /api/course-package/{id}/add-course) */
export async function addCourseToPackage(id: number, courseId: number) {
  return request(`/api/course-package/${id}/add-course?courseId=${courseId}`, {
    method: 'POST',
  });
}

/** 从套餐移除课程 (后端: DELETE /api/course-package/{id}/remove-course/{courseId}) */
export async function removeCourseFromPackage(id: number, courseId: number) {
  return request(`/api/course-package/${id}/remove-course/${courseId}`, {
    method: 'DELETE',
  });
}

/** 获取可用课程列表 (后端: GET /api/course-package/available-courses) */
export async function getAvailableCourses() {
  return request<API.Course[]>('/api/course-package/available-courses', {
    method: 'GET',
  });
}

/** 购买课程套餐 (后端: POST /api/course-package/{id}/purchase) */
export async function purchaseCoursePackage(id: number, data?: { payChannel?: string }) {
  return request(`/api/course-package/${id}/purchase`, {
    method: 'POST',
    data,
  });
}

/** 我的课程套餐列表 (后端: GET /api/course-package/my-packages) */
export async function getMyPackages() {
  return request('/api/course-package/my-packages', {
    method: 'GET',
  });
}

/** 购买详情 (后端: GET /api/course-package/purchase/{id}/detail) */
export async function getPurchaseDetail(id: number) {
  return request(`/api/course-package/purchase/${id}/detail`, {
    method: 'GET',
  });
}
