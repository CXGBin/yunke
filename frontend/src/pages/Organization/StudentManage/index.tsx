import React, { useState } from 'react';
import { ProTable } from '@ant-design/pro-components';
import { Tag, Space } from 'antd';
import type { ProColumns } from '@ant-design/pro-components';
import * as studentApi from '@/services/student';

interface StudentItem {
  id: number;
  userCode?: string;
  userName: string;
  realName?: string;
  avatar?: string;
  phone?: string;
  gender: number;
  grade?: string;
  status: number;
  orgId?: number;
  campusId?: number;
  orgName?: string;
  createdAt: string;
}

export default function StudentManage() {
  const columns: ProColumns<StudentItem>[] = [
    { title: '姓名', dataIndex: 'realName', width: 120 },
    { title: '用户名', dataIndex: 'userName', width: 120 },
    { title: '手机号', dataIndex: 'phone', width: 130 },
    { title: '年级', dataIndex: 'grade', width: 80 },
    { title: '性别', dataIndex: 'gender', width: 80, valueEnum: { 0: '未知', 1: '男', 2: '女' } },
    { title: '所属机构', dataIndex: 'orgName', width: 150 },
    { title: '校区ID', dataIndex: 'campusId', width: 80 },
    { title: '状态', dataIndex: 'status', width: 80, render: (_, r) => r.status === 1 ? <Tag color="green">启用</Tag> : <Tag color="red">禁用</Tag> },
    { title: '创建时间', dataIndex: 'createdAt', width: 160, valueType: 'dateTime' },
  ];

  return (
    <ProTable<StudentItem>
      headerTitle="学生管理"
      rowKey="id"
      columns={columns}
      search={{ labelWidth: 'auto' }}
      request={async (params) => {
        try {
          const res = await studentApi.getStudentPage({ page: params.current, pageSize: params.pageSize, keyword: params.realName });
          return { data: res.data?.items || [], total: res.data?.total || 0, success: true };
        } catch { return { data: [], total: 0, success: false }; }
      }}
    />
  );
}
