import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { ProTable } from '@ant-design/pro-components';
import { Card, Tag, Rate, message } from 'antd';
import { getEvaluationPage } from '@/services/evaluation';

const EvaluationManage: React.FC = () => {
  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.Evaluation>
          headerTitle="评价列表"
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getEvaluationPage({
                page: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                minScore: params.minScore,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取评价列表失败');
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
              title: '学生',
              dataIndex: 'studentName',
              width: 100,
              search: false,
            },
            {
              title: '教师',
              dataIndex: 'teacherName',
              width: 100,
              search: false,
            },
            {
              title: '评分',
              dataIndex: 'score',
              width: 120,
              search: false,
              render: (_, record) => <Rate disabled defaultValue={record.score} />,
            },
            {
              title: '评价内容',
              dataIndex: 'content',
              ellipsis: true,
              search: false,
              width: 200,
            },
            {
              title: '状态',
              dataIndex: 'status',
              width: 80,
              search: false,
              render: (_, record) => (
                <Tag color={record.status === 1 ? 'success' : record.status === 2 ? 'blue' : 'default'}>
                  {record.status === 1 ? '已回复' : record.status === 2 ? '已隐藏' : '待回复'}
                </Tag>
              ),
            },
            {
              title: '评价时间',
              dataIndex: 'createdAt',
              width: 170,
              search: false,
              valueType: 'dateTime',
            },
          ]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
          expandable={{
            expandedRowRender: (record) => (
              <div style={{ padding: '0 24px' }}>
                <p><strong>评价内容：</strong>{record.content || '无文字评价'}</p>
                {record.replyContent && (
                  <p><strong>教师回复：</strong>{record.replyContent}</p>
                )}
                {record.isAnonymous && <Tag color="purple">匿名评价</Tag>}
              </div>
            ),
          }}
        />
      </Card>
    </PageContainer>
  );
};

export default EvaluationManage;
