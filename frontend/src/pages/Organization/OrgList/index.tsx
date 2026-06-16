import React, { useRef } from 'react';
import { PageContainer } from '@ant-design/pro-components';
import {
  ProTable,
  ModalForm,
  ProFormText,
  ProFormTextArea,
  ProFormSelect,
} from '@ant-design/pro-components';
import { Button, Tag, Space, message, Popconfirm, Card, Image } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import {
  getOrgPage,
  createOrg,
  updateOrg,
  deleteOrg,
  updateOrgStatus,
} from '@/services/organization';
import type { ActionType } from '@ant-design/pro-components';

const STATUS_MAP: Record<number, { text: string; color: string }> = {
  0: { text: '停用', color: 'error' },
  1: { text: '启用', color: 'success' },
  2: { text: '过期', color: 'warning' },
};

const OrgList: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const modalRef = useRef<any>();

  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.Organization>
          headerTitle="机构列表"
          actionRef={actionRef}
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getOrgPage({
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
              message.error('获取机构列表失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '机构编码',
              dataIndex: 'orgCode',
              width: 100,
              search: false,
            },
            {
              title: '机构名称',
              dataIndex: 'name',
              ellipsis: true,
              width: 180,
              render: (_, record) => (
                <Space>
                  {record.logo && <Image src={record.logo} width={24} height={24} style={{ borderRadius: 4 }} preview={false} />}
                  {record.name}
                </Space>
              ),
            },
            {
              title: '联系人',
              dataIndex: 'contactPerson',
              width: 100,
              search: false,
            },
            {
              title: '联系电话',
              dataIndex: 'contactPhone',
              width: 130,
              search: false,
            },
            {
              title: '地区',
              width: 160,
              search: false,
              render: (_, record) => [record.province, record.city, record.district].filter(Boolean).join(' / '),
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
                  { label: '过期', value: 2 },
                ],
              },
              render: (_, record) => {
                const s = STATUS_MAP[record.status] || { text: '未知', color: 'default' };
                return <Tag color={s.color}>{s.text}</Tag>;
              },
            },
            {
              title: '到期时间',
              dataIndex: 'expiredAt',
              width: 170,
              search: false,
              valueType: 'dateTime',
            },
            {
              title: '操作',
              width: 200,
              search: false,
              render: (_, record) => (
                <Space>
                  <a
                    onClick={() => {
                      modalRef.current?.setFieldsValue(record);
                      modalRef.current?.open();
                    }}
                  >
                    编辑
                  </a>
                  <Popconfirm
                    title={record.status === 1 ? '确认停用？' : '确认启用？'}
                    onConfirm={async () => {
                      try {
                        await updateOrgStatus(record.id, record.status === 1 ? 0 : 1);
                        message.success('操作成功');
                        actionRef.current?.reload();
                      } catch {
                        message.error('操作失败');
                      }
                    }}
                  >
                    <a>{record.status === 1 ? '停用' : '启用'}</a>
                  </Popconfirm>
                  <Popconfirm
                    title="确认删除？"
                    onConfirm={async () => {
                      try {
                        await deleteOrg(record.id);
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
              新增机构
            </Button>,
          ]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
        />

        <ModalForm
          title="机构信息"
          modalProps={{ destroyOnClose: true }}
          width={600}
          formRef={modalRef}
          onFinish={async (values: any) => {
            try {
              if (values.id) {
                await updateOrg(values.id, values);
              } else {
                await createOrg(values);
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
            name="name"
            label="机构名称"
            rules={[{ required: true, message: '请输入机构名称' }]}
          />
          <ProFormText name="contactPerson" label="联系人" />
          <ProFormText name="contactPhone" label="联系电话" />
          <ProFormText name="province" label="省份" />
          <ProFormText name="city" label="城市" />
          <ProFormText name="district" label="区县" />
          <ProFormText name="address" label="详细地址" />
          <ProFormTextArea name="description" label="简介" />
        </ModalForm>
      </Card>
    </PageContainer>
  );
};

export default OrgList;
