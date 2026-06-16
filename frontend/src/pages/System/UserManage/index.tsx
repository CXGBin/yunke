import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import {
  ProTable,
  ModalForm,
  ProFormText,
  ProFormDigit,
  ProFormSelect,
  ProFormRadio,
} from '@ant-design/pro-components';
import { Button, Tag, Space, message, Popconfirm, Card } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { getUserList, updateUser, deleteUser, resetUserPassword } from '@/services/user';

const ROLE_OPTIONS = [
  { label: '平台管理员', value: 1 },
  { label: '机构管理员', value: 2 },
  { label: '教师', value: 3 },
  { label: '学生', value: 4 },
  { label: '家长', value: 5 },
];

const STATUS_MAP: Record<number, { text: string; color: string }> = {
  0: { text: '停用', color: 'error' },
  1: { text: '正常', color: 'success' },
  2: { text: '待审核', color: 'warning' },
};

const UserManage: React.FC = () => {
  return (
    <PageContainer>
      <Card bordered={false}>
        <ProTable<API.SysUser>
          headerTitle="用户管理"
          rowKey="id"
          search={{
            labelWidth: 'auto',
          }}
          request={async (params) => {
            try {
              const res = await getUserList({
                page: params.current,
                pageSize: params.pageSize,
                keyword: params.keyword,
                role: params.role,
                status: params.status,
              });
              return {
                data: res?.items || [],
                total: res?.total || 0,
                success: true,
              };
            } catch (err) {
              message.error('获取用户列表失败');
              return { data: [], total: 0, success: true };
            }
          }}
          columns={[
            {
              title: '用户名',
              dataIndex: 'userName',
              ellipsis: true,
              width: 120,
            },
            {
              title: '真实姓名',
              dataIndex: 'realName',
              ellipsis: true,
              width: 100,
              search: false,
            },
            {
              title: '手机号',
              dataIndex: 'phone',
              ellipsis: true,
              width: 130,
              search: false,
            },
            {
              title: '角色',
              dataIndex: 'role',
              width: 100,
              valueType: 'select',
              fieldProps: { options: ROLE_OPTIONS },
              render: (_, record) => {
                const r = ROLE_OPTIONS.find((o) => o.value === record.role);
                return r ? <Tag>{r.label}</Tag> : record.role;
              },
            },
            {
              title: '所属机构',
              dataIndex: 'orgName',
              ellipsis: true,
              width: 140,
              search: false,
            },
            {
              title: '状态',
              dataIndex: 'status',
              width: 80,
              valueType: 'select',
              fieldProps: {
                options: [
                  { label: '全部', value: undefined },
                  { label: '正常', value: 1 },
                  { label: '停用', value: 0 },
                  { label: '待审核', value: 2 },
                ],
              },
              render: (_, record) => {
                const s = STATUS_MAP[record.status] || { text: '未知', color: 'default' };
                return <Tag color={s.color}>{s.text}</Tag>;
              },
            },
            {
              title: '最后登录',
              dataIndex: 'lastLoginAt',
              width: 170,
              search: false,
              valueType: 'dateTime',
            },
            {
              title: '操作',
              width: 180,
              search: false,
              render: (_, record) => (
                <Space>
                  <Popconfirm
                    title={record.status === 1 ? '确认停用？' : '确认启用？'}
                    onConfirm={async () => {
                      try {
                        await updateUser(record.id, { status: record.status === 1 ? 0 : 1 });
                        message.success('操作成功');
                        return true;
                      } catch {
                        message.error('操作失败');
                        return false;
                      }
                    }}
                  >
                    <a>{record.status === 1 ? '停用' : '启用'}</a>
                  </Popconfirm>
                  <Popconfirm
                    title="确认重置密码？"
                    onConfirm={async () => {
                      try {
                        await resetUserPassword(record.id);
                        message.success('密码已重置');
                      } catch {
                        message.error('重置失败');
                      }
                    }}
                  >
                    <a>重置密码</a>
                  </Popconfirm>
                  <Popconfirm
                    title="确认删除？"
                    onConfirm={async () => {
                      try {
                        await deleteUser(record.id);
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
            <Button key="add" type="primary" icon={<PlusOutlined />}>
              新增用户
            </Button>,
          ]}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
        />
      </Card>
    </PageContainer>
  );
};

export default UserManage;
