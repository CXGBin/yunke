import React, { useRef } from 'react';
import { PageContainer } from '@ant-design/pro-components';
import {
  ProTable,
  ModalForm,
  ProFormText,
  ProFormDigit,
  ProFormTextArea,
  ProFormSelect,
  ProFormSlider,
} from '@ant-design/pro-components';
import { Button, Tag, Space, message, Popconfirm, Card, Divider, Tabs } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import {
  getOrgPackagePage,
  createOrgPackage,
  updateOrgPackage,
  deleteOrgPackage,
  getSubscriptionHistory,
} from '@/services/orgPackage';
import type { ActionType } from '@ant-design/pro-components';

const PACKAGE_LEVEL_MAP: Record<number, string> = {
  0: 'Plus',
  1: 'Pro',
  2: 'Ultra',
  3: 'Ultimate',
};

const AnnualPackage: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const modalRef = useRef<any>();
  const subActionRef = useRef<ActionType>();

  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.OrgPackage>
          headerTitle="年费套餐"
          actionRef={actionRef}
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getOrgPackagePage({
                page: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                status: params.status,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取套餐列表失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '套餐名称',
              dataIndex: 'packageName',
              width: 120,
            },
            {
              title: '编码',
              dataIndex: 'packageCode',
              width: 100,
              search: false,
            },
            {
              title: '等级',
              dataIndex: 'packageLevel',
              width: 90,
              search: false,
              render: (_, record) => (
                <Tag color={['green', 'blue', 'purple', 'gold'][record.packageLevel]}>
                  {PACKAGE_LEVEL_MAP[record.packageLevel] || record.packageLevel}
                </Tag>
              ),
            },
            {
              title: '价格(元/年)',
              dataIndex: 'price',
              width: 110,
              search: false,
              render: (_, record) => <span>¥{record.price?.toFixed(2)}</span>,
            },
            {
              title: '校区上限',
              dataIndex: 'maxCampusCount',
              width: 90,
              search: false,
              render: (_, record) => (record.maxCampusCount === -1 ? '不限' : record.maxCampusCount),
            },
            {
              title: '教师上限',
              dataIndex: 'maxTeacherCount',
              width: 90,
              search: false,
              render: (_, record) => (record.maxTeacherCount === -1 ? '不限' : record.maxTeacherCount),
            },
            {
              title: '学生上限',
              dataIndex: 'maxStudentCount',
              width: 90,
              search: false,
              render: (_, record) => (record.maxStudentCount === -1 ? '不限' : record.maxStudentCount),
            },
            {
              title: '状态',
              dataIndex: 'status',
              width: 80,
              valueType: 'select',
              fieldProps: {
                options: [
                  { label: '全部', value: undefined },
                  { label: '启用', value: 1 },
                  { label: '停用', value: 0 },
                ],
              },
              render: (_, record) => (
                <Tag color={record.status === 1 ? 'success' : 'error'}>
                  {record.status === 1 ? '启用' : '停用'}
                </Tag>
              ),
            },
            {
              title: '操作',
              width: 160,
              search: false,
              render: (_, record) => (
                <Space>
                  <a
                    onClick={() => {
                      modalRef.current?.setFieldsValue({
                        ...record,
                        price: record.price,
                      });
                      modalRef.current?.open();
                    }}
                  >
                    编辑
                  </a>
                  <Popconfirm
                    title="确认删除？"
                    onConfirm={async () => {
                      try {
                        await deleteOrgPackage(record.id);
                        message.success('删除成功');
                        return true;
                      } catch {
                        message.error('删除失败');
                        return false;
                      }
                    }}
                  >
                    <a style={{ color: '#ff4d4f' }}>删除</a>
                  </Popconfirm>
                </Space>
              ),
            },
          ]}
          toolBarRender={() => [
            <Button
              key="add"
              type="primary"
              icon={<PlusOutlined />}
              onClick={() => {
                modalRef.current?.resetFields();
                modalRef.current?.open();
              }}
            >
              新增套餐
            </Button>,
          ]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
          expandable={{
            expandedRowRender: (record) => (
              <div style={{ padding: '0 16px' }}>
                <p><strong>功能介绍：</strong>{record.description || '暂无'}</p>
                <p>
                  <strong>数据分析维度：</strong>{record.analyticsDimensions || 'basic'}
                </p>
                <p>
                  <strong>通知类型上限：</strong>{record.maxNotificationTypes}　
                  <strong>推送渠道：</strong>{record.maxPushChannels}
                </p>
              </div>
            ),
          }}
        />

        <Divider />

        <ProTable<API.OrgSubscription>
          headerTitle="订阅记录"
          actionRef={subActionRef}
          rowKey="id"
          search={false}
          request={async (params) => {
            try {
              const res = await getSubscriptionHistory({
                page: params.current,
                pageSize: params.pageSize,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            { title: '机构', dataIndex: 'orgName', ellipsis: true },
            { title: '套餐', dataIndex: 'packageName', width: 100 },
            {
              title: '类型',
              dataIndex: 'subscriptionType',
              width: 80,
              render: (_, r) => ['新购', '续费', '升级'][r.subscriptionType] || '-',
            },
            { title: '金额', dataIndex: 'amount', width: 100, render: (_, r) => `¥${r.amount?.toFixed(2)}` },
            {
              title: '支付状态',
              dataIndex: 'payStatus',
              width: 90,
              render: (_, r) => (
                <Tag color={r.payStatus === 1 ? 'success' : r.payStatus === 2 ? 'error' : 'warning'}>
                  {['待支付', '已支付', '已退款'][r.payStatus] || '-'}
                </Tag>
              ),
            },
            {
              title: '有效期',
              width: 200,
              search: false,
              render: (_, r) => `${r.startDate} ~ ${r.endDate}`,
            },
          ]}
          pagination={{ defaultPageSize: 5, showSizeChanger: true }}
        />
      </Card>

      <ModalForm
        title="年费套餐"
        modalProps={{ destroyOnClose: true, width: 650 }}
        formRef={modalRef}
        onFinish={async (values: any) => {
          try {
            if (values.id) {
              await updateOrgPackage(values.id, values);
            } else {
              await createOrgPackage(values);
            }
            message.success('保存成功');
            actionRef.current?.reload();
            return true;
          } catch {
            message.error('保存失败');
            return false;
          }
        }}
      >
        <ProFormText name="id" hidden />
        <ProFormText
          name="packageName"
          label="套餐名称"
          rules={[{ required: true, message: '请输入套餐名称' }]}
        />
        <ProFormText
          name="packageCode"
          label="套餐编码"
          rules={[{ required: true, message: '请输入套餐编码' }]}
        />
        <ProFormSelect
          name="packageLevel"
          label="套餐等级"
          rules={[{ required: true }]}
          options={[
            { label: 'Plus (基础)', value: 0 },
            { label: 'Pro (标准)', value: 1 },
            { label: 'Ultra (高级)', value: 2 },
            { label: 'Ultimate (旗舰)', value: 3 },
          ]}
        />
        <ProFormDigit
          name="price"
          label="价格(元/年)"
          min={0}
          fieldProps={{ precision: 2, prefix: '¥' }}
          rules={[{ required: true, message: '请输入价格' }]}
        />
        <ProFormDigit name="maxCampusCount" label="校区数量上限(-1为不限)" min={-1} />
        <ProFormDigit name="maxTeacherCount" label="教师数量上限(-1为不限)" min={-1} />
        <ProFormDigit name="maxStudentCount" label="学生数量上限(-1为不限)" min={-1} />
        <ProFormDigit name="maxNotificationTypes" label="通知类型上限" min={0} />
        <ProFormDigit name="maxPushChannels" label="推送渠道数量" min={0} />
        <ProFormTextArea name="description" label="功能介绍" />
      </ModalForm>
    </PageContainer>
  );
};

export default AnnualPackage;
