import React, { useRef } from 'react';
import { PageContainer } from '@ant-design/pro-components';
import {
  ProTable,
  ModalForm,
  ProFormText,
  ProFormTextArea,
} from '@ant-design/pro-components';
import { Button, Space, message, Popconfirm, Card, Tag } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { getConfigPage, createConfig, updateConfig, deleteConfig } from '@/services/sysConfig';
import type { ActionType } from '@ant-design/pro-components';

const CONFIG_GROUP_OPTIONS = [
  { label: '全局', value: 'global' },
  { label: '签到', value: 'attendance' },
  { label: '报名', value: 'enrollment' },
  { label: '通知', value: 'notification' },
  { label: '结算', value: 'settlement' },
  { label: '其他', value: 'other' },
];

const SysConfig: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const modalRef = useRef<any>();

  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.SysConfig>
          headerTitle="系统配置"
          actionRef={actionRef}
          rowKey="id"
          search={{
            labelWidth: 'auto',
          }}
          request={async (params) => {
            try {
              const res = await getConfigPage({
                pageIndex: params.current,
                pageSize: params.pageSize,
                configGroup: params.configGroup,
                keyword: params.keyword,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取配置列表失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '配置键',
              dataIndex: 'configKey',
              width: 200,
              ellipsis: true,
            },
            {
              title: '配置值',
              dataIndex: 'configValue',
              ellipsis: true,
              search: false,
            },
            {
              title: '分组',
              dataIndex: 'configGroup',
              width: 100,
              valueType: 'select',
              fieldProps: { options: CONFIG_GROUP_OPTIONS },
              render: (_, record) => {
                const g = CONFIG_GROUP_OPTIONS.find((o) => o.value === record.configGroup);
                return g ? <Tag>{g.label}</Tag> : record.configGroup;
              },
            },
            {
              title: '说明',
              dataIndex: 'description',
              ellipsis: true,
              search: false,
            },
            {
              title: '更新时间',
              dataIndex: 'updatedAt',
              width: 170,
              search: false,
              valueType: 'dateTime',
            },
            {
              title: '操作',
              width: 140,
              search: false,
              render: (_, record) => (
                <Space>
                  <a
                    onClick={() => {
                      modalRef.current?.setFieldsValue({
                        id: record.id,
                        configKey: record.configKey,
                        configValue: record.configValue,
                        configGroup: record.configGroup,
                        description: record.description,
                      });
                      modalRef.current?.open();
                    }}
                  >
                    <EditOutlined /> 编辑
                  </a>
                  <Popconfirm
                    title="确认删除？"
                    onConfirm={async () => {
                      try {
                        await deleteConfig(record.id);
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
              新增配置
            </Button>,
          ]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
        />

        <ModalForm
          title="编辑系统配置"
          modalProps={{ destroyOnClose: true }}
          width={500}
          formRef={modalRef}
          onFinish={async (values: any) => {
            try {
              if (values.id) {
                await updateConfig(values.id, values);
              } else {
                await createConfig(values);
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
            name="configKey"
            label="配置键"
            rules={[{ required: true, message: '请输入配置键' }]}
            disabled={(form) => !!form.getFieldValue('id')}
          />
          <ProFormText
            name="configValue"
            label="配置值"
            rules={[{ required: true, message: '请输入配置值' }]}
          />
          <ProFormText name="configGroup" label="分组" placeholder="如: global, attendance" />
          <ProFormTextArea name="description" label="说明" />
        </ModalForm>
      </Card>
    </PageContainer>
  );
};

export default SysConfig;
