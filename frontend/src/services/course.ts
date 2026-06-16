import { request } from '@umijs/max';

/** 课程分页列表 (后端: GET /api/course/page) */
export async function getCoursePage(params: API.CoursePageParams) {
  return request<API.PagedResult<API.Course>>('/api/course/page', {
    method: 'GET',
    params,
  });
}

/** 课程详情 (后端: GET /api/course/{id}) */
export async function getCourseDetail(id: number) {
  return request<API.Course>(`/api/course/${id}`, {
    method: 'GET',
  });
}

/** 创建课程 (后端: POST /api/course) */
export async function createCourse(data: any) {
  return request('/api/course', {
    method: 'POST',
    data,
  });
}

/** 更新课程 (后端: PUT /api/course/{id}) */
export async function updateCourse(id: number, data: any) {
  return request(`/api/course/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除课程 (后端: DELETE /api/course/{id}) */
export async function deleteCourse(id: number) {
  return request(`/api/course/${id}`, {
    method: 'DELETE',
  });
}

/** 发布课程 (后端: POST /api/course/{id}/publish) */
export async function publishCourse(id: number) {
  return request(`/api/course/${id}/publish`, {
    method: 'POST',
  });
}

/** 下线课程 (后端: POST /api/course/{id}/offline) */
export async function offlineCourse(id: number) {
  return request(`/api/course/${id}/offline`, {
    method: 'POST',
  });
}

/** 课程分类树 (后端: GET /api/course-category/tree) */
export async function getCategoryTree() {
  return request<API.CategoryTreeNode[]>('/api/course-category/tree', {
    method: 'GET',
  });
}

/** 创建分类 (后端: POST /api/course-category) */
export async function createCategory(data: { name: string; parentId?: number; icon?: string; sortOrder?: number }) {
  return request('/api/course-category', {
    method: 'POST',
    data,
  });
}

/** 更新分类 (后端: PUT /api/course-category/{id}) */
export async function updateCategory(id: number, data: { name: string; parentId?: number; icon?: string; sortOrder?: number }) {
  return request(`/api/course-category/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除分类 (后端: DELETE /api/course-category/{id}) */
export async function deleteCategory(id: number) {
  return request(`/api/course-category/${id}`, {
    method: 'DELETE',
  });
}

/** 课时列表 (后端: GET /api/lesson-unit/course/{courseId}) */
export async function getLessons(courseId: number) {
  return request<API.LessonUnit[]>(`/api/lesson-unit/course/${courseId}`, {
    method: 'GET',
  });
}

/** 批量生成课时 (后端: POST /api/lesson-unit/batch-generate) */
export async function batchGenerateLessons(courseId: number, data: { count: number; titlePrefix?: string; startNo?: number }) {
  return request(`/api/lesson-unit/batch-generate?courseId=${courseId}`, {
    method: 'POST',
    data,
  });
}

/** 更新课时 (后端: PUT /api/lesson-unit/{id}) */
export async function updateLesson(id: number, data: { lessonNo?: number; title?: string; description?: string; sortOrder?: number }) {
  return request(`/api/lesson-unit/${id}`, {
    method: 'PUT',
    data,
  });
}

/** 删除课时 (后端: DELETE /api/lesson-unit/{id}) */
export async function deleteLesson(id: number) {
  return request(`/api/lesson-unit/${id}`, {
    method: 'DELETE',
  });
}
