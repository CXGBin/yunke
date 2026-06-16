import React, { useState } from 'react';
import { ProTable, ProForm, ModalForm, ProFormText, ProFormSelect, ProFormDigit, ProFormTextArea, ProFormDateTimePicker } from '@ant-design/pro-components';
import { Button, message, Tag, Space, Popconfirm } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import type { ProColumns } from '@ant-design/pro-components';
import * as teacherApi from '@/services/user';

interface TeacherItem {
  id: number;
  userCode?: string;
  userName: string;
  realName?: string;
  nickName?: string;
  avatar?: string;
  phone?: string;
  gender: number;
  role: number;
  orgId?: number;
  campusId?: number;
  status: number;
  createdAt: string;
}

export default function TeacherManage() {
  const [modalVisible, setModalVisible] = useState(false);
  const [editingRecord, setEditingRecord] = useState<TeacherItem | null>(null);

  const columns: ProColumns<TeacherItem>[] = [
    { title: '姓名', dataIndex: 'realName', width: 120 },
    { title: '用户名', dataIndex: 'userName', width: 120 },
    { title: '手机号', dataIndex: 'phone', width: 130 },
    { title: '性别', dataIndex: 'gender', width: 80, valueEnum: { 0: { text: '未知' }, 1: { text: '男' }, 2: { text: '女' } } },
    { title: '校区ID', dataIndex: 'campusId', width: 80 },
    { title: '角色', dataIndex: 'role', width: 80, valueEnum: { 2: { text: '教师' }, 3: { text: '助教' } } },
    { title: '状态', dataIndex: 'status', width: 80, render: (_, r) => r.status === 1 ? <Tag color="green">启用</Tag> : <Tag color="red">禁用</Tag> },
    { title: '创建时间', dataIndex: 'createdAt', width: 160, valueType: 'dateTime' },
    { title: '操作', width: 150, render: (_, r) => (
      <Space>
        <a onClick={() => { setEditingRecord(r); setModalVisible(true); }}>编辑</a>
        <Popconfirm title="确认删除?" onConfirm={async () => { await teacherApi.deleteUser(r.id); message.success('删除成功'); window.location.reload(); }}>
          <a style={{ color: '#ff4d4f' }}>删除</a>
        </Popconfirm>
      </Space>
    ),
    },
  ];

  return (
    <ProTable<TeacherItem>
      headerTitle="教师管理"
      rowKey="id"
      columns={columns}
      search={{ labelWidth: 'auto' }}
      request={async (params) => {
        try {
          const res = await teacherApi.getUsers({ page: params.current, pageSize: params.pageSize, keyword: params.realName, role: 2 });
          return { data: res.data?.items || [], total: res.data?.total || 0, success: true };
        } catch { return { data: [], total: 0, success: false }; }
      }}
      toolBarRender={() => [
        <Button key="add" type="primary" icon={<PlusOutlined />} onClick={() => { setEditingRecord(null); setModalVisible(true); }}>新增教师</Button>,
      ]}
    />
  );
}
