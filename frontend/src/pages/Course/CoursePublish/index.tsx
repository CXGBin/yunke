import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { ProTable } from '@ant-design/pro-components';
import { Card, Tag, Space, message, Image } from 'antd';
import { getCoursePage } from '@/services/course';

const CoursePublish: React.FC = () => {
  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.Course>
          headerTitle="课程发布（全平台）"
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getCoursePage({
                page: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                status: params.status,
                category: params.category,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取课程列表失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '课程名称',
              dataIndex: 'courseName',
              ellipsis: true,
              width: 180,
              render: (_, record) => (
                <Space>
                  {record.coverImage && (
                    <Image
                      src={record.coverImage}
                      width={36}
                      height={36}
                      style={{ borderRadius: 4, objectFit: 'cover' }}
                      preview={false}
                    />
                  )}
                  {record.courseName}
                </Space>
              ),
            },
            {
              title: '所属机构',
              dataIndex: 'orgName',
              ellipsis: true,
              width: 140,
              search: false,
            },
            {
              title: '教师',
              dataIndex: 'teacherName',
              width: 100,
              search: false,
            },
            {
              title: '校区',
              dataIndex: 'campusName',
              width: 120,
              search: false,
            },
            {
              title: '分类',
              dataIndex: 'category',
              width: 90,
            },
            {
              title: '报名/上限',
              width: 100,
              search: false,
              render: (_, record) => `${record.currentStudents}/${record.maxStudents}`,
            },
            {
              title: '结算方式',
              dataIndex: 'settlementType',
              width: 100,
              search: false,
              render: (_, record) => ['固定课时费', '按学生分成'][record.settlementType] || '-',
            },
            {
              title: '状态',
              dataIndex: 'status',
              width: 80,
              valueType: 'select',
              fieldProps: {
                options: [
                  { label: '全部', value: undefined },
                  { label: '已上架', value: 1 },
                  { label: '已下架', value: 0 },
                ],
              },
              render: (_, record) => (
                <Tag color={record.status === 1 ? 'success' : 'default'}>
                  {record.status === 1 ? '已上架' : '已下架'}
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
        />
      </Card>
    </PageContainer>
  );
};

export default CoursePublish;
