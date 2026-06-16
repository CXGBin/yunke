import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { ProTable } from '@ant-design/pro-components';
import { Card, Tag, message } from 'antd';
import { getCourseStudents } from '@/services/enrollment';

const Enrollment: React.FC = () => {
  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.Enrollment>
          headerTitle="课程学员管理"
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const courseId = params.courseId as number | undefined;
              if (!courseId) {
                return { data: [], total: 0, success: true };
              }
              const res = await getCourseStudents(courseId, {
                page: params.current,
                pageSize: params.pageSize,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取报名列表失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '课程ID',
              dataIndex: 'courseId',
              width: 80,
              search: false,
            },
            {
              title: '学生姓名',
              dataIndex: 'studentName',
              width: 100,
            },
            {
              title: '状态',
              dataIndex: 'status',
              width: 80,
              valueType: 'select',
              fieldProps: {
                options: [
                  { label: '全部', value: undefined },
                  { label: '已确认', value: 1 },
                  { label: '待确认', value: 0 },
                  { label: '已退课', value: 2 },
                ],
              },
              render: (_, record) => {
                const map: Record<number, { text: string; color: string }> = {
                  0: { text: '待确认', color: 'warning' },
                  1: { text: '已确认', color: 'success' },
                  2: { text: '已退课', color: 'error' },
                };
                const s = map[record.status] || { text: '未知', color: 'default' };
                return <Tag color={s.color}>{s.text}</Tag>;
              },
            },
            {
              title: '报名时间',
              dataIndex: 'enrolledAt',
              width: 170,
              search: false,
              valueType: 'dateTime',
            },
          ]}
          search={{
            filterType: 'light',
          }}
          form={{ layout: 'horizontal' }}
          params={{ courseId: undefined }}
          onReset={() => {}}
        />
      </Card>
    </PageContainer>
  );
};

export default Enrollment;
