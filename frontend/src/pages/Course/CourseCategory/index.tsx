import React from 'react';
import { ProTable } from '@ant-design/pro-components';
import { Tag, Space, Popconfirm, message } from 'antd';
import type { ProColumns } from '@ant-design/pro-components';
import * as courseApi from '@/services/course';

interface CategoryItem {
  id: number;
  parentId: number;
  name: string;
  icon?: string;
  sortOrder: number;
}

export default function CourseCategory() {
  const columns: ProColumns<CategoryItem>[] = [
    { title: 'ID', dataIndex: 'id', width: 80 },
    { title: '分类名称', dataIndex: 'name', width: 200 },
    { title: '父级ID', dataIndex: 'parentId', width: 80 },
    { title: '图标', dataIndex: 'icon', width: 100, ellipsis: true },
    { title: '排序', dataIndex: 'sortOrder', width: 80 },
    {
      title: '操作', width: 150, render: (_, r) => (
        <Space>
          <a onClick={async () => { message.info('编辑功能开发中'); }}>编辑</a>
          <Popconfirm title="确认删除?" onConfirm={async () => { await courseApi.deleteCategory(r.id); message.success('删除成功'); window.location.reload(); }}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <ProTable<CategoryItem>
      headerTitle="课程分类"
      rowKey="id"
      columns={columns}
      request={async (params) => {
        try {
          const res = await courseApi.getCategoryTree();
          const flat: CategoryItem[] = [];
          const flatten = (items: any[], depth = 0) => items.forEach(item => { flat.push({ ...item, name: depth > 0 ? '  '.repeat(depth) + item.name : item.name }); if (item.children) flatten(item.children, depth + 1); });
          flatten(res || []);
          return { data: flat, total: flat.length, success: true };
        } catch { return { data: [], total: 0, success: false }; }
      }}
    />
  );
}
