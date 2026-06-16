import { request } from '@umijs/max';

/** 获取结算规则 (后端: GET /api/settlement/rule/{courseId}) */
export async function getSettlementRule(courseId: number) {
  return request<API.SettlementRule>(`/api/settlement/rule/${courseId}`, {
    method: 'GET',
  });
}

/** 获取钱包信息 (后端: GET /api/settlement/wallet) */
export async function getWallet() {
  return request<API.Wallet>('/api/settlement/wallet', {
    method: 'GET',
  });
}

/** 钱包明细 (后端: GET /api/settlement/wallet/detail) */
export async function getWalletDetail(params: API.PageParams) {
  return request<API.PagedResult<API.WalletDetail>>('/api/settlement/wallet/detail', {
    method: 'GET',
    params,
  });
}

/** 结算记录 (后端: GET /api/settlement/records) */
export async function getSettlementRecords(params: API.PageParams) {
  return request<API.PagedResult<API.FeeSettlementRecord>>('/api/settlement/records', {
    method: 'GET',
    params,
  });
}

/** 手动触发结算 (后端: POST /api/settlement/manual-trigger) */
export async function manualTriggerSettlement(data: { scheduleId: number; remark?: string }) {
  return request('/api/settlement/manual-trigger', {
    method: 'POST',
    data,
  });
}

/** 结算汇总 (后端: GET /api/settlement/summary) */
export async function getSettlementSummary(month?: number) {
  return request('/api/settlement/summary', {
    method: 'GET',
    params: month !== undefined ? { month } : undefined,
  });
}

/** 导出结算 (后端: GET /api/settlement/export) */
export async function exportSettlement() {
  return request('/api/settlement/export', {
    method: 'GET',
  });
}

export const getSettlementPage = getSettlementRecords;

