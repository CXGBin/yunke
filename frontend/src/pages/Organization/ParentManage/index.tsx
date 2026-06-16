import React from 'react';
import { ProTable } from '@ant-design/pro-components';
import { Tag } from 'antd';
import type { ProColumns } from '@ant-design/pro-components';
import * as parentApi from '@/services/parent';

interface ParentItem {
  id: number;
  userName: string;
  realName?: string;
  avatar?: string;
  phone?: string;
  childrenCount: number;
  createdAt: string;
}

export default function ParentManage() {
  const columns: ProColumns<ParentItem>[] = [
    { title: '姓名', dataIndex: 'realName', width: 120 },
    { title: '用户名', dataIndex: 'userName', width: 120 },
    { title: '手机号', dataIndex: 'phone', width: 130 },
    { title: '绑定孩子数', dataIndex: 'childrenCount', width: 100 },
    { title: '创建时间', dataIndex: 'createdAt', width: 160, valueType: 'dateTime' },
  ];

  return (
    <ProTable<ParentItem>
      headerTitle="家长管理"
      rowKey="id"
      columns={columns}
      search={{ labelWidth: 'auto' }}
      request={async (params) => {
        try {
          const res = await parentApi.getParentPage({ page: params.current, pageSize: params.pageSize, keyword: params.realName });
          return { data: res?.items || [], total: res?.total || 0, success: true };
        } catch { return { data: [], total: 0, success: false }; }
      }}
    />
  );
}
