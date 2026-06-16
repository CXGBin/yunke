import { request } from '@umijs/max';

/** 仪表盘统计 */
export async function getDashboardStats() {
  return request<API.DashboardStats>('/api/statistics/dashboard', {
    method: 'GET',
  });
}

/** 数据分析概览 */
export async function getStatisticsOverview() {
  return request<API.StatisticsData>('/api/statistics/overview', {
    method: 'GET',
  });
}
