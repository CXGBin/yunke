import React from 'react';
import { ProTable } from '@ant-design/pro-components';
import { Tag, Space, Popconfirm, message } from 'antd';
import type { ProColumns } from '@ant-design/pro-components';
import * as evaluationApi from '@/services/evaluation';

interface TagItem {
  id: number;
  name: string;
  tagType: number;
  sortOrder: number;
  status: number;
  createdAt: string;
}

export default function EvaluationTagManage() {
  const columns: ProColumns<TagItem>[] = [
    { title: '标签名称', dataIndex: 'name', width: 150 },
    { title: '标签类型', dataIndex: 'tagType', width: 100, valueEnum: { 0: '课程评价', 1: '教师评价', 2: '课时评价' } },
    { title: '排序', dataIndex: 'sortOrder', width: 80 },
    { title: '状态', dataIndex: 'status', width: 80, render: (_, r) => r.status === 1 ? <Tag color="green">启用</Tag> : <Tag color="red">禁用</Tag> },
    { title: '创建时间', dataIndex: 'createdAt', width: 160, valueType: 'dateTime' },
    {
      title: '操作', width: 150, render: (_, r) => (
        <Space>
          <a onClick={async () => { message.info('编辑功能开发中'); }}>编辑</a>
          <Popconfirm title="确认删除?" onConfirm={async () => { await evaluationApi.deleteTag(r.id); message.success('删除成功'); window.location.reload(); }}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <ProTable<TagItem>
      headerTitle="评价标签管理"
      rowKey="id"
      columns={columns}
      search={{ labelWidth: 'auto' }}
      request={async (params) => {
        try {
          const res = await evaluationApi.getTags({ page: params.current, pageSize: params.pageSize });
          return { data: res.data?.items || [], total: res.data?.total || 0, success: true };
        } catch { return { data: [], total: 0, success: false }; }
      }}
    />
  );
}
