import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { ProTable } from '@ant-design/pro-components';
import { Card, Tag, message } from 'antd';
import { getSchedulePage } from '@/services/schedule';

const ScheduleManage: React.FC = () => {
  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.Schedule>
          headerTitle="排课列表"
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getSchedulePage({
                pageIndex: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                orgId: params.orgId,
                startDate: params.startDate ? params.startDate[0] : undefined,
                endDate: params.startDate ? params.startDate[1] : undefined,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取排课列表失败');
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
              title: '排课日期',
              dataIndex: 'scheduleDate',
              width: 120,
              valueType: 'date',
            },
            {
              title: '时段',
              width: 150,
              search: false,
              render: (_, record) => `${record.startTime} - ${record.endTime}`,
            },
            {
              title: '日期范围',
              dataIndex: 'dateRange',
              hideInTable: true,
              valueType: 'dateRange',
              search: {
                transform: (value) => ({ startDate: value }),
              },
            },
            {
              title: '状态',
              dataIndex: 'status',
              width: 80,
              search: false,
              render: (_, record) => (
                <Tag color={record.status === 1 ? 'success' : 'default'}>
                  {record.status === 1 ? '正常' : '已取消'}
                </Tag>
              ),
            },
          ]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
        />
      </Card>
    </PageContainer>
  );
};

export default ScheduleManage;
