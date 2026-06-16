import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { ProTable } from '@ant-design/pro-components';
import { Card, Tag, message } from 'antd';
import { getSettlementPage } from '@/services/settlement';

const STATUS_MAP: Record<number, { text: string; color: string }> = {
  0: { text: '待结算', color: 'warning' },
  1: { text: '已结算', color: 'success' },
  2: { text: '已取消', color: 'default' },
};

const CourseSettlement: React.FC = () => {
  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.Settlement>
          headerTitle="课程结算记录"
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getSettlementPage({
                page: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                status: params.status,
                settlementMonth: params.settlementMonth,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取结算列表失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '教师',
              dataIndex: 'teacherName',
              width: 100,
            },
            {
              title: '所属机构',
              dataIndex: 'orgName',
              ellipsis: true,
              width: 140,
              search: false,
            },
            {
              title: '课程名称',
              dataIndex: 'courseName',
              ellipsis: true,
              width: 160,
              search: false,
            },
            {
              title: '结算月份',
              dataIndex: 'settlementMonth',
              width: 100,
              valueType: 'month',
            },
            {
              title: '课时数',
              dataIndex: 'lessonCount',
              width: 80,
              search: false,
            },
            {
              title: '单价(元)',
              dataIndex: 'unitPrice',
              width: 90,
              search: false,
              render: (_, r) => `¥${r.unitPrice?.toFixed(2)}`,
            },
            {
              title: '总金额(元)',
              dataIndex: 'totalAmount',
              width: 100,
              search: false,
              render: (_, r) => <span style={{ fontWeight: 600 }}>¥{r.totalAmount?.toFixed(2)}</span>,
            },
            {
              title: '状态',
              dataIndex: 'status',
              width: 80,
              valueType: 'select',
              fieldProps: {
                options: [
                  { label: '全部', value: undefined },
                  { label: '待结算', value: 0 },
                  { label: '已结算', value: 1 },
                  { label: '已取消', value: 2 },
                ],
              },
              render: (_, record) => {
                const s = STATUS_MAP[record.status] || { text: '未知', color: 'default' };
                return <Tag color={s.color}>{s.text}</Tag>;
              },
            },
            {
              title: '结算时间',
              dataIndex: 'settledAt',
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

export default CourseSettlement;
