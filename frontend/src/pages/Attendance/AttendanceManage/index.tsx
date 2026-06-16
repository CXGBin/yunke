import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { ProTable } from '@ant-design/pro-components';
import { Card, Tag, message } from 'antd';
import { getAttendancePage } from '@/services/attendance';

const AttendanceManage: React.FC = () => {
  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.Attendance>
          headerTitle="签到记录"
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getAttendancePage({
                page: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                attendanceStatus: params.attendanceStatus,
                scheduleDate: params.scheduleDate,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取签到记录失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '学生姓名',
              dataIndex: 'studentName',
              width: 100,
            },
            {
              title: '课程名称',
              dataIndex: 'courseName',
              ellipsis: true,
              width: 160,
              search: false,
            },
            {
              title: '所属机构',
              dataIndex: 'orgName',
              ellipsis: true,
              width: 140,
              search: false,
            },
            {
              title: '校区',
              dataIndex: 'campusName',
              width: 120,
              search: false,
            },
            {
              title: '教师',
              dataIndex: 'teacherName',
              width: 100,
              search: false,
            },
            {
              title: '签到状态',
              dataIndex: 'attendanceStatus',
              width: 100,
              valueType: 'select',
              fieldProps: {
                options: [
                  { label: '全部', value: undefined },
                  { label: '已签到', value: 1 },
                  { label: '未签到', value: 0 },
                  { label: '请假', value: 2 },
                  { label: '迟到', value: 3 },
                ],
              },
              render: (_, record) => {
                const map: Record<number, { text: string; color: string }> = {
                  0: { text: '未签到', color: 'default' },
                  1: { text: '已签到', color: 'success' },
                  2: { text: '请假', color: 'warning' },
                  3: { text: '迟到', color: 'orange' },
                };
                const s = map[record.attendanceStatus] || { text: '未知', color: 'default' };
                return <Tag color={s.color}>{s.text}</Tag>;
              },
            },
            {
              title: '签到时间',
              dataIndex: 'signTime',
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
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
        />
      </Card>
    </PageContainer>
  );
};

export default AttendanceManage;
