import React from 'react';
import { ProTable } from '@ant-design/pro-components';
import { Tag } from 'antd';
import type { ProColumns } from '@ant-design/pro-components';
import * as courseApi from '@/services/course';

interface LessonItem {
  id: number;
  courseId: number;
  lessonNo: number;
  title: string;
  description?: string;
  sortOrder: number;
  status: number;
}

export default function LessonUnitManage() {
  const columns: ProColumns<LessonItem>[] = [
    { title: '课时号', dataIndex: 'lessonNo', width: 80 },
    { title: '课程ID', dataIndex: 'courseId', width: 80 },
    { title: '标题', dataIndex: 'title', width: 200 },
    { title: '描述', dataIndex: 'description', ellipsis: true, width: 200 },
    { title: '排序', dataIndex: 'sortOrder', width: 80 },
    { title: '状态', dataIndex: 'status', width: 80, render: (_, r) => r.status === 1 ? <Tag color="green">启用</Tag> : <Tag color="default">草稿</Tag> },
  ];

  return (
    <ProTable<LessonItem>
      headerTitle="课时管理"
      rowKey="id"
      columns={columns}
      search={{ labelWidth: 'auto' }}
      request={async (params) => {
        try {
          const res = await courseApi.getLessons({ page: params.current, pageSize: params.pageSize });
          return { data: res.data?.items || [], total: res.data?.total || 0, success: true };
        } catch { return { data: [], total: 0, success: false }; }
      }}
    />
  );
}
