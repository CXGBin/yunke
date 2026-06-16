import React, { useRef } from 'react';
import { PageContainer } from '@ant-design/pro-components';
import {
  ProTable,
  ModalForm,
  ProFormText,
  ProFormDigit,
  ProFormSelect,
} from '@ant-design/pro-components';
import { Button, Tag, Space, message, Popconfirm, Card } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import {
  getCampusPage,
  createCampus,
  updateCampus,
  deleteCampus,
  updateCampusStatus,
} from '@/services/campus';
import { getOrgPage } from '@/services/organization';
import type { ActionType } from '@ant-design/pro-components';

const CampusManage: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const modalRef = useRef<any>();

  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.Campus>
          headerTitle="校区管理"
          actionRef={actionRef}
          rowKey="id"
          search={{ labelWidth: 'auto' }}
          request={async (params) => {
            try {
              const res = await getCampusPage({
                pageIndex: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                orgId: params.orgId,
                status: params.status,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch {
              message.error('获取校区列表失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '校区编码',
              dataIndex: 'campusCode',
              width: 120,
              search: false,
            },
            {
              title: '校区名称',
              dataIndex: 'name',
              ellipsis: true,
              width: 160,
            },
            {
              title: '所属机构',
              dataIndex: 'orgName',
              ellipsis: true,
              width: 160,
              search: false,
            },
            {
              title: '所属机构',
              dataIndex: 'orgId',
              hideInTable: true,
              valueType: 'select',
              request: async () => {
                try {
                  const res = await getOrgPage({ pageIndex: 1, pageSize: 200 });
                  return (res?.items || []).map((o) => ({ label: o.name, value: o.id }));
                } catch {
                  return [];
                }
              },
            },
            {
              title: '负责人',
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
              title: '地址',
              dataIndex: 'address',
              ellipsis: true,
              search: false,
            },
            {
              title: '默认校区',
              dataIndex: 'isDefault',
              width: 90,
              search: false,
              render: (_, record) =>
                record.isDefault ? <Tag color="blue">默认</Tag> : '-',
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
              width: 180,
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
                        await updateCampusStatus(record.id, record.status === 1 ? 0 : 1);
                        message.success('操作成功');
                        actionRef.current?.reload();
                      } catch {
                        message.error('操作失败');
                      }
                    }}
                  >
                    <a>{record.status === 1 ? '停用' : '启用'}</a>
                  </Popconfirm>
                  {record.isDefault ? (
                    <span style={{ color: '#ccc' }}>删除</span>
                  ) : (
                    <Popconfirm
                      title="确认删除？"
                      onConfirm={async () => {
                        try {
                          await deleteCampus(record.id);
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
                  )}
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
              新增校区
            </Button>,
          ]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
        />

        <ModalForm
          title="校区信息"
          modalProps={{ destroyOnClose: true }}
          width={550}
          formRef={modalRef}
          onFinish={async (values: any) => {
            try {
              if (values.id) {
                await updateCampus(values.id, values);
              } else {
                await createCampus(values);
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
          <ProFormSelect
            name="orgId"
            label="所属机构"
            rules={[{ required: true, message: '请选择所属机构' }]}
            request={async () => {
              try {
                const res = await getOrgPage({ pageIndex: 1, pageSize: 200 });
                return (res?.items || []).map((o) => ({ label: o.name, value: o.id }));
              } catch {
                return [];
              }
            }}
          />
          <ProFormText
            name="name"
            label="校区名称"
            rules={[{ required: true, message: '请输入校区名称' }]}
          />
          <ProFormText name="contactPerson" label="负责人" />
          <ProFormText name="contactPhone" label="联系电话" />
          <ProFormText name="address" label="地址" />
        </ModalForm>
      </Card>
    </PageContainer>
  );
};

export default CampusManage;
