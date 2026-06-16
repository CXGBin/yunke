import React, { useRef, useState } from 'react';
import { PageContainer } from '@ant-design/pro-components';
import {
  ProTable,
  ModalForm,
  ProFormText,
  ProFormTextArea,
} from '@ant-design/pro-components';
import { Button, Space, message, Card, Tag } from 'antd';
import { EditOutlined } from '@ant-design/icons';
import { getSysConfigList, updateSysConfig } from '@/services/sysConfig';
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
              const group = params.configGroup as string | undefined;
              const res = await getSysConfigList(group);
              const items = (res as any[]) || [];
              return {
                data: items,
                total: items.length,
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
              width: 100,
              search: false,
              render: (_, record) => (
                <Space>
                  <a
                    onClick={() => {
                      modalRef.current?.setFieldsValue({
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
                </Space>
              ),
            },
          ]}
          pagination={false}
        />

        <ModalForm
          title="编辑系统配置"
          modalProps={{ destroyOnClose: true }}
          width={500}
          formRef={modalRef}
          onFinish={async (values: any) => {
            try {
              await updateSysConfig(values);
              message.success('保存成功');
              actionRef.current?.reload();
              return true;
            } catch {
              message.error('保存失败');
              return false;
            }
          }}
        >
          <ProFormText
            name="configKey"
            label="配置键"
            rules={[{ required: true, message: '请输入配置键' }]}
            disabled
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
