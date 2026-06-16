import React from 'react';
import { ProTable } from '@ant-design/pro-components';
import { Tag, Space, Popconfirm, message } from 'antd';
import type { ProColumns } from '@ant-design/pro-components';
import * as leaveApi from '@/services/attendance';

interface LeaveItem {
  id: number;
  studentId: number;
  courseId: number;
  leaveType: number;
  startDate: string;
  endDate: string;
  reason: string;
  status: number;
  createdAt: string;
}

export default function LeaveManage() {
  const columns: ProColumns<LeaveItem>[] = [
    { title: '学生ID', dataIndex: 'studentId', width: 80 },
    { title: '课程ID', dataIndex: 'courseId', width: 80 },
    { title: '请假类型', dataIndex: 'leaveType', width: 80, valueEnum: { 0: '病假', 1: '事假', 2: '其他' } },
    { title: '开始日期', dataIndex: 'startDate', width: 120, valueType: 'dateTime' },
    { title: '结束日期', dataIndex: 'endDate', width: 120, valueType: 'dateTime' },
    { title: '原因', dataIndex: 'reason', ellipsis: true, width: 150 },
    { title: '状态', dataIndex: 'status', width: 80, render: (_, r) => {
      const map: Record<number, { text: string; color: string }> = { 0: { text: '待审核', color: 'orange' }, 1: { text: '教师通过', color: 'blue' }, 2: { text: '已批准', color: 'green' }, 3: { text: '已拒绝', color: 'red' }, 4: { text: '已取消', color: 'default' } };
      const s = map[r.status] || { text: '未知', color: 'default' };
      return <Tag color={s.color}>{s.text}</Tag>;
    }},
    { title: '创建时间', dataIndex: 'createdAt', width: 160, valueType: 'dateTime' },
  ];

  return (
    <ProTable<LeaveItem>
      headerTitle="请假管理"
      rowKey="id"
      columns={columns}
      search={{ labelWidth: 'auto' }}
      request={async (params) => {
        try {
          const res = await leaveApi.getLeaves({ page: params.current, pageSize: params.pageSize });
          return { data: res.data?.items || [], total: res.data?.total || 0, success: true };
        } catch { return { data: [], total: 0, success: false }; }
      }}
    />
  );
}
