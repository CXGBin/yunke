// src/app.tsx — Umi 运行时配置
import { history, RequestConfig } from '@umijs/max';
import { message } from 'antd';

const TOKEN_KEY = 'yunke_edu_token';

/** 请求拦截器：自动注入 Token */
const requestInterceptor = (url: string, options: any) => {
  const token = localStorage.getItem(TOKEN_KEY);
  if (token) {
    const headers = {
      ...options.headers,
      Authorization: `Bearer ${token}`,
    };
    return { url, options: { ...options, headers } };
  }
  return { url, options };
};

/** 响应拦截器：统一处理 401 */
const responseInterceptor = (response: any) => {
  const { data } = response;
  // 后端返回 code 非 200/0 时视为业务错误
  if (data && data.code !== undefined && data.code !== 200 && data.code !== 0) {
    if (data.code === 401 || data.code === 403) {
      localStorage.removeItem(TOKEN_KEY);
      message.error('登录已过期，请重新登录');
      history.push('/login');
    }
  }
  return response;
};

export const request: RequestConfig = {
  dataField: 'data',
  requestInterceptors: [requestInterceptor],
  responseInterceptors: [responseInterceptor],
  errorConfig: {
    errorHandler: (error: any) => {
      const { response } = error || {};
      if (response?.status === 401) {
        localStorage.removeItem(TOKEN_KEY);
        message.error('登录已过期，请重新登录');
        history.push('/login');
      } else if (response?.status === 403) {
        message.error('没有权限访问该资源');
      } else if (response?.status) {
        message.error(`请求失败 (${response.status})`);
      } else {
        message.error('网络错误，请稍后重试');
      }
    },
  },
};
