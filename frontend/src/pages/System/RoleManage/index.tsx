import React, { useEffect, useState } from 'react';
import { Button, Modal, Form, Input, Select, Table, Tag, Space, message, Tree, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { getRoleList, getRoleDetail, createRole, updateRole, deleteRole, getMenuTree } from '@/services/permission';
import AuthorizedButton from '@/components/AuthorizedButton';

const RoleManage: React.FC = () => {
  const [data, setData] = useState<API.PagedResult<API.RoleItem>>({ items: [], total: 0, page: 1, pageSize: 20 });
  const [loading, setLoading] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [allMenus, setAllMenus] = useState<API.MenuItem[]>([]);
  const [checkedKeys, setCheckedKeys] = useState<React.Key[]>([]);
  const [form] = Form.useForm();

  const fetchData = async (page = 1, pageSize = 20) => {
    setLoading(true);
    try {
      const res = await getRoleList({ page, pageSize });
      setData(res);
    } catch { message.error('获取角色列表失败'); }
    setLoading(false);
  };

  const fetchMenus = async () => {
    const tree = await getMenuTree();
    setAllMenus(tree);
  };

  useEffect(() => { fetchData(); fetchMenus(); }, []);

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
    try { await deleteRole(id); message.success('删除成功'); fetchData(data.page, data.pageSize); }
    catch (e: any) { message.error(e?.message || '删除失败'); }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      values.menuIds = checkedKeys as number[];
      if (editId) { await updateRole(editId, values); message.success('更新成功'); }
      else { await createRole(values); message.success('创建成功'); }
      setModalOpen(false);
      fetchData(data.page, data.pageSize);
    } catch { /* validation */ }
  };

  const menuTreeData = (items: API.MenuItem[]): any[] =>
    items.map((item) => ({
      key: item.id,
      title: item.name,
      children: menuTreeData(item.children || []),
    }));

  const columns = [
    { title: '角色名称', dataIndex: 'roleName' },
    { title: '角色编码', dataIndex: 'roleCode' },
    { title: '数据范围', dataIndex: 'dataScope', render: (v: number) => ['全部', '本机构', '本校区', '仅本人'][v] || v },
    { title: '状态', dataIndex: 'status', render: (v: number) => v === 1 ? <Tag color="green">启用</Tag> : <Tag color="red">禁用</Tag> },
    {
      title: '操作', width: 200,
      render: (_: any, record: API.RoleItem) => (
        <Space>
          <AuthorizedButton permission="sys:role:edit">
            <Button size="small" type="link" icon={<EditOutlined />} onClick={() => handleEdit(record.id)}>编辑</Button>
          </AuthorizedButton>
          <AuthorizedButton permission="sys:role:delete">
            <Popconfirm title="确定删除？" onConfirm={() => handleDelete(record.id)}>
              <Button size="small" type="link" danger icon={<DeleteOutlined />}>删除</Button>
            </Popconfirm>
          </AuthorizedButton>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 16 }}>
        <AuthorizedButton permission="sys:role:add">
          <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>新增角色</Button>
        </AuthorizedButton>
      </div>
      <Table
        rowKey="id" columns={columns} dataSource={data.items} loading={loading}
        pagination={{ current: data.page, pageSize: data.pageSize, total: data.total,
          onChange: (p, ps) => fetchData(p, ps) }}
      />
      <Modal title={editId ? '编辑角色' : '新增角色'} open={modalOpen} onOk={handleSubmit} onCancel={() => setModalOpen(false)} width={700}>
        <Form form={form} layout="vertical">
          <Form.Item name="roleName" label="角色名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="roleCode" label="角色编码"><Input /></Form.Item>
          <Form.Item name="description" label="描述"><Input.TextArea rows={2} /></Form.Item>
          <Form.Item name="sortOrder" label="排序"><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="dataScope" label="数据范围">
            <Select options={[{ label: '全部', value: 0 }, { label: '本机构', value: 1 }, { label: '本校区', value: 2 }, { label: '仅本人', value: 3 }]} />
          </Form.Item>
          <Form.Item label="菜单权限">
            <Tree
              checkable checkedKeys={checkedKeys} onCheck={(keys) => setCheckedKeys(keys as React.Key[])}
              treeData={menuTreeData(allMenus)} defaultExpandAll checkStrictly
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default RoleManage;
