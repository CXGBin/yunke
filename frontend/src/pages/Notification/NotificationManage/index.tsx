import React, { useRef } from 'react';
import { PageContainer } from '@ant-design/pro-components';
import {
  ProTable,
  ModalForm,
  ProFormText,
  ProFormTextArea,
  ProFormDigit,
  ProFormSelect,
} from '@ant-design/pro-components';
import { Button, Tag, Space, message, Popconfirm, Card } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import {
  getNotificationTemplatePage,
  createNotificationTemplate,
  updateNotificationTemplate,
  deleteNotificationTemplate,
} from '@/services/notification';
import type { ActionType } from '@ant-design/pro-components';

const TEMPLATE_TYPE_OPTIONS = [
  { label: '上课提醒', value: 1 },
  { label: '签到提醒', value: 2 },
  { label: '评价提醒', value: 3 },
  { label: '请假通知', value: 4 },
  { label: '结算通知', value: 5 },
  { label: '续费提醒', value: 6 },
  { label: '系统通知', value: 99 },
];

const CHANNEL_OPTIONS = [
  { label: '站内消息', value: 1 },
  { label: '小程序订阅消息', value: 2 },
  { label: '短信', value: 3 },
];

const NotificationManage: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const modalRef = useRef<any>();

  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.NotificationTemplate>
          headerTitle="通知模板管理"
          actionRef={actionRef}
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getNotificationTemplatePage({
                page: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                notifyType: params.templateType,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取通知列表失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '模板名称',
              dataIndex: 'templateName',
              width: 160,
            },
            {
              title: '模板编码',
              dataIndex: 'templateCode',
              width: 140,
              search: false,
            },
            {
              title: '模板类型',
              dataIndex: 'notifyType',
              width: 110,
              valueType: 'select',
              fieldProps: { options: TEMPLATE_TYPE_OPTIONS },
              render: (_, record) => {
                const t = TEMPLATE_TYPE_OPTIONS.find((o) => o.value === record.notifyType);
                return t ? <Tag>{t.label}</Tag> : record.notifyType;
              },
            },
            {
              title: '推送渠道',
              dataIndex: 'channel',
              width: 120,
              search: false,
              render: (_, record) => {
                const c = CHANNEL_OPTIONS.find((o) => o.value === record.channel);
                return c ? <Tag color="blue">{c.label}</Tag> : record.channel;
              },
            },
            {
              title: '标题',
              dataIndex: 'title',
              ellipsis: true,
              width: 180,
              search: false,
            },
            {
              title: '状态',
              dataIndex: 'status',
              width: 80,
              search: false,
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
                      modalRef.current?.setFieldsValue(record);
                      modalRef.current?.open();
                    }}
                  >
                    <EditOutlined /> 编辑
                  </a>
                  <Popconfirm
                    title="确认删除？"
                    onConfirm={async () => {
                      try {
                        await deleteNotificationTemplate(record.id);
                        message.success('删除成功');
                        actionRef.current?.reload();
                      } catch {
                        message.error('删除失败');
                      }
                    }}
                  >
                    <a style={{ color: '#ff4d4f' }}>
                      <DeleteOutlined /> 删除
                    </a>
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
              新增模板
            </Button>,
          ]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
          expandable={{
            expandedRowRender: (record) => (
              <div style={{ padding: '0 16px' }}>
                <p><strong>模板内容：</strong>{record.contentTemplate || '暂无'}</p>
                <p><strong>变量：</strong>{record.variables || '无'}</p>
              </div>
            ),
          }}
        />

        <ModalForm
          title="通知模板"
          modalProps={{ destroyOnClose: true, width: 600 }}
          formRef={modalRef}
          onFinish={async (values: any) => {
            try {
              if (values.id) {
                await updateNotificationTemplate(values.id, values);
              } else {
                await createNotificationTemplate(values);
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
            name="templateName"
            label="模板名称"
            rules={[{ required: true, message: '请输入模板名称' }]}
          />
          <ProFormText
            name="templateCode"
            label="模板编码"
            rules={[{ required: true, message: '请输入模板编码' }]}
          />
          <ProFormSelect
            name="templateType"
            label="模板类型"
            rules={[{ required: true }]}
            options={TEMPLATE_TYPE_OPTIONS}
          />
          <ProFormSelect
            name="channel"
            label="推送渠道"
            rules={[{ required: true }]}
            options={CHANNEL_OPTIONS}
          />
          <ProFormText name="title" label="标题" />
          <ProFormTextArea name="contentTemplate" label="模板内容" />
          <ProFormText name="variables" label="变量列表" placeholder="如: {{courseName}}, {{teacherName}}" />
        </ModalForm>
      </Card>
    </PageContainer>
  );
};

export default NotificationManage;
