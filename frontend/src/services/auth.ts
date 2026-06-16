import { request } from '@umijs/max';

/** 登录 */
export async function login(params: API.LoginParams) {
  return request<API.LoginResult>('/api/auth/login', {
    method: 'POST',
    data: params,
  });
}

/** 获取当前用户信息 */
export async function getCurrentUser() {
  return request<API.CurrentUser>('/api/auth/user-info', {
    method: 'GET',
  });
}
