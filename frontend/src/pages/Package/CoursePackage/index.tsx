import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { ProTable } from '@ant-design/pro-components';
import { Card, Tag, message, Image } from 'antd';
import { getCoursePackagePage } from '@/services/coursePackage';

const CoursePackagePage: React.FC = () => {
  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.CoursePackage>
          headerTitle="课程套餐（机构创建）"
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getCoursePackagePage({
                pageIndex: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                orgId: params.orgId,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取课程套餐列表失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '套餐名称',
              dataIndex: 'packageName',
              ellipsis: true,
              width: 180,
            },
            {
              title: '所属机构',
              dataIndex: 'orgName',
              ellipsis: true,
              width: 160,
              search: false,
            },
            {
              title: '课程数量',
              dataIndex: 'courseCount',
              width: 90,
              search: false,
            },
            {
              title: '总价(元)',
              dataIndex: 'totalPrice',
              width: 100,
              search: false,
              render: (_, record) => <span>¥{record.totalPrice?.toFixed(2)}</span>,
            },
            {
              title: '状态',
              dataIndex: 'status',
              width: 80,
              search: false,
              render: (_, record) => (
                <Tag color={record.status === 1 ? 'success' : 'error'}>
                  {record.status === 1 ? '上架' : '下架'}
                </Tag>
              ),
            },
            {
              title: '创建时间',
              dataIndex: 'createdAt',
              width: 170,
              search: false,
              valueType: 'dateTime',
            },
          ]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
          search={false}
        />
      </Card>
    </PageContainer>
  );
};

export default CoursePackagePage;
