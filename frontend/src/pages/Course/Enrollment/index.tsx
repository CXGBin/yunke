import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { ProTable } from '@ant-design/pro-components';
import { Card, Tag, message } from 'antd';
import { getEnrollmentPage } from '@/services/enrollment';

const Enrollment: React.FC = () => {
  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.Enrollment>
          headerTitle="报名管理"
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getEnrollmentPage({
                pageIndex: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                status: params.status,
                courseId: params.courseId,
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
              title: '学生姓名',
              dataIndex: 'studentName',
              width: 100,
            },
            {
              title: '课程名称',
              dataIndex: 'courseName',
              ellipsis: true,
              width: 180,
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
              dataIndex: 'enrollTime',
              width: 170,
              search: false,
              valueType: 'dateTime',
            },
          ]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
        />
      </Card>
    </PageContainer>
  );
};

export default Enrollment;
