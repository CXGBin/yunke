import React, { useEffect, useState } from 'react';
import { Button, Modal, Form, Input, Select, InputNumber, Tag, Space, message, Tree, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { ProTable } from '@ant-design/pro-components';
import { getRoleList, getRoleDetail, createRole, updateRole, deleteRole, getMenuTree } from '@/services/permission';
import AuthorizedButton from '@/components/AuthorizedButton';

const RoleManage: React.FC = () => {
  const [modalOpen, setModalOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [allMenus, setAllMenus] = useState<API.MenuItem[]>([]);
  const [checkedKeys, setCheckedKeys] = useState<React.Key[]>([]);
  const [form] = Form.useForm();

  const fetchMenus = async () => {
    try { setAllMenus(await getMenuTree()); } catch { /* ignore */ }
  };

  useEffect(() => { fetchMenus(); }, []);

  const handleAdd = () => {
    setEditId(null);
    form.resetFields();
    form.setFieldsValue({ sortOrder: 0, status: 1, dataScope: 1 });
    setCheckedKeys([]);
    setModalOpen(true);
  };

  const handleEdit = async (id: number) => {
    setEditId(id);
    form.resetFields();
    try {
      const role = await getRoleDetail(id);
      form.setFieldsValue(role);
      setCheckedKeys(role.menuIds || []);
    } catch { message.error('获取角色详情失败'); }
    setModalOpen(true);
  };

  const handleDelete = async (id: number) => {
    try { await deleteRole(id); message.success('删除成功'); }
    catch (e: any) { message.error(e?.message || '删除失败'); }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      values.menuIds = checkedKeys as number[];
      if (editId) { await updateRole(editId, values); message.success('更新成功'); }
      else { await createRole(values); message.success('创建成功'); }
      setModalOpen(false);
    } catch { /* validation */ }
  };

  const menuTreeData = (items: API.MenuItem[]): any[] =>
    items.map((item) => ({
      key: item.id, title: item.name, children: menuTreeData(item.children || []),
    }));

  const columns = [
    { title: '角色名称', dataIndex: 'roleName', search: false },
    { title: '角色编码', dataIndex: 'roleCode', search: false },
    {
      title: '数据范围', dataIndex: 'dataScope', search: false,
      valueEnum: { 0: { text: '全部' }, 1: { text: '本机构' }, 2: { text: '本校区' }, 3: { text: '仅本人' } },
    },
    {
      title: '状态', dataIndex: 'status', search: false,
      valueEnum: { 1: { text: '启用', status: 'Success' }, 0: { text: '禁用', status: 'Error' } },
    },
    {
      title: '操作', valueType: 'option', search: false, width: 200,
      render: (_: any, record: any) => (
        <Space>
          <AuthorizedButton permission="sys:role:edit">
            <a onClick={() => handleEdit(record.id)}>编辑</a>
          </AuthorizedButton>
          <AuthorizedButton permission="sys:role:delete">
            <Popconfirm title="确定删除？" onConfirm={() => handleDelete(record.id)}>
              <a style={{ color: '#ff4d4f' }}>删除</a>
            </Popconfirm>
          </AuthorizedButton>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <ProTable<API.RoleItem>
        rowKey="id"
        headerTitle="角色管理"
        columns={columns}
        toolBarRender={() => [
          <AuthorizedButton permission="sys:role:add" key="add">
            <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>新增角色</Button>
          </AuthorizedButton>,
        ]}
        request={async (params) => {
          const res = await getRoleList({ page: params.current || 1, pageSize: params.pageSize || 20, keyword: params.roleName as string });
          return { data: res.items, total: res.total, success: true };
        }}
        pagination={{ defaultPageSize: 20 }}
        search={false}
      />
      <Modal title={editId ? '编辑角色' : '新增角色'} open={modalOpen} onOk={handleSubmit} onCancel={() => setModalOpen(false)} width={700} destroyOnClose>
        <Form form={form} layout="vertical">
          <Form.Item name="roleName" label="角色名称" rules={[{ required: true, message: '请输入角色名称' }]}><Input /></Form.Item>
          <Form.Item name="roleCode" label="角色编码"><Input /></Form.Item>
          <Form.Item name="description" label="描述"><Input.TextArea rows={2} /></Form.Item>
          <Form.Item name="sortOrder" label="排序"><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="dataScope" label="数据范围">
            <Select options={[{ label: '全部', value: 0 }, { label: '本机构', value: 1 }, { label: '本校区', value: 2 }, { label: '仅本人', value: 3 }]} />
          </Form.Item>
          <Form.Item label="菜单权限">
            <Tree
              checkable checkedKeys={checkedKeys}
              onCheck={(keys) => setCheckedKeys(keys as React.Key[])}
              treeData={menuTreeData(allMenus)} defaultExpandAll checkStrictly
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default RoleManage;
