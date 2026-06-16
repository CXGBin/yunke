import React, { useState } from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { ProTable, ModalForm, ProFormDigit } from '@ant-design/pro-components';
import { Card, Tag, message } from 'antd';
import { getAttendanceBySchedule, getMyAttendanceRecords } from '@/services/attendance';
import { getSchedulePage } from '@/services/schedule';

const STATUS_MAP: Record<number, { text: string; color: string }> = {
  0: { text: '未签到', color: 'default' },
  1: { text: '已签到', color: 'success' },
  2: { text: '请假', color: 'warning' },
  3: { text: '迟到', color: 'orange' },
  4: { text: '缺勤', color: 'error' },
};

const AttendanceManage: React.FC = () => {
  const [scheduleId, setScheduleId] = useState<number | undefined>();
  const [records, setRecords] = useState<API.Attendance[]>([]);

  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<any>
          headerTitle="签到记录"
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getMyAttendanceRecords();
              const items = res || [];
              return {
                data: items,
                total: items.length,
                success: true,
              };
            } catch {
              message.error('获取签到记录失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '课程名称',
              dataIndex: 'courseName',
              ellipsis: true,
              width: 160,
            },
            {
              title: '学生姓名',
              dataIndex: 'studentName',
              width: 100,
            },
            {
              title: '签到状态',
              dataIndex: 'status',
              width: 100,
              valueType: 'select',
              fieldProps: {
                options: [
                  { label: '全部', value: undefined },
                  ...Object.entries(STATUS_MAP).map(([v, s]) => ({ label: s.text, value: Number(v) })),
                ],
              },
              render: (_, record) => {
                const s = STATUS_MAP[record.status] || { text: '未知', color: 'default' };
                return <Tag color={s.color}>{s.text}</Tag>;
              },
            },
            {
              title: '签到方式',
              dataIndex: 'signMethod',
              width: 100,
              search: false,
            },
            {
              title: '签到时间',
              dataIndex: 'signInTime',
              width: 170,
              search: false,
              valueType: 'dateTime',
            },
            {
              title: '备注',
              dataIndex: 'remark',
              ellipsis: true,
              width: 120,
              search: false,
            },
          ]}
          pagination={false}
        />
      </Card>
    </PageContainer>
  );
};

export default AttendanceManage;
