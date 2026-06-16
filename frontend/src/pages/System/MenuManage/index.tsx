import React, { useEffect, useState } from 'react';
import { Button, Modal, Form, Input, InputNumber, Select, Space, message, Tree, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { getMenuTree, createMenu, updateMenu, deleteMenu } from '@/services/permission';
import AuthorizedButton from '@/components/AuthorizedButton';

const MenuManage: React.FC = () => {
  const [menus, setMenus] = useState<API.MenuItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [editItem, setEditItem] = useState<API.MenuItem | null>(null);
  const [form] = Form.useForm();

  const fetchData = async () => {
    setLoading(true);
    try {
      const data = await getMenuTree();
      setMenus(data);
    } catch { message.error('获取菜单失败'); }
    setLoading(false);
  };

  useEffect(() => { fetchData(); }, []);

  const handleAdd = (parentId?: number) => {
    setEditItem(null);
    form.resetFields();
    form.setFieldsValue({ parentId: parentId || 0, menuType: 2, sortOrder: 0, visible: 1, status: 1 });
    setModalOpen(true);
  };

  const handleEdit = (record: API.MenuItem) => {
    setEditItem(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleDelete = async (id: number) => {
    try { await deleteMenu(id); message.success('删除成功'); fetchData(); }
    catch (e: any) { message.error(e?.message || '删除失败'); }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (editItem) { await updateMenu(editItem.id, values); message.success('更新成功'); }
      else { await createMenu(values); message.success('创建成功'); }
      setModalOpen(false);
      fetchData();
    } catch { /* validation */ }
  };

  const convertToTreeData = (items: API.MenuItem[]): any[] =>
    items.map((item) => ({
      key: item.id,
      title: (
        <Space>
          <span>{item.name}</span>
          <span style={{ color: '#999', fontSize: 12 }}>
            {item.menuType === 1 ? '目录' : item.menuType === 2 ? '菜单' : `按钮(${item.permission || ''})`}
          </span>
          <AuthorizedButton permission="sys:menu:add">
            <Button size="small" type="link" icon={<PlusOutlined />} onClick={() => handleAdd(item.id)} />
          </AuthorizedButton>
          <AuthorizedButton permission="sys:menu:edit">
            <Button size="small" type="link" icon={<EditOutlined />} onClick={() => handleEdit(item)} />
          </AuthorizedButton>
          <AuthorizedButton permission="sys:menu:delete">
            <Popconfirm title="确定删除？" onConfirm={() => handleDelete(item.id)}>
              <Button size="small" type="link" danger icon={<DeleteOutlined />} />
            </Popconfirm>
          </AuthorizedButton>
        </Space>
      ),
      children: convertToTreeData(item.children || []),
    }));

  return (
    <div>
      <div style={{ marginBottom: 16 }}>
        <AuthorizedButton permission="sys:menu:add">
          <Button type="primary" icon={<PlusOutlined />} onClick={() => handleAdd(0)}>新增顶级菜单</Button>
        </AuthorizedButton>
      </div>
      <Tree loading={loading} treeData={convertToTreeData(menus)} defaultExpandAll />
      <Modal title={editItem ? '编辑菜单' : '新增菜单'} open={modalOpen} onOk={handleSubmit} onCancel={() => setModalOpen(false)} width={600}>
        <Form form={form} layout="vertical">
          <Form.Item name="parentId" label="父菜单ID" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="menuType" label="类型" rules={[{ required: true }]}>
            <Select options={[{ label: '目录', value: 1 }, { label: '菜单', value: 2 }, { label: '按钮', value: 3 }]} />
          </Form.Item>
          <Form.Item name="name" label="名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="path" label="路由路径"><Input /></Form.Item>
          <Form.Item name="component" label="组件路径"><Input /></Form.Item>
          <Form.Item name="icon" label="图标"><Input /></Form.Item>
          <Form.Item name="sortOrder" label="排序"><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="permission" label="权限码"><Input /></Form.Item>
          <Form.Item name="visible" label="可见">
            <Select options={[{ label: '显示', value: 1 }, { label: '隐藏', value: 0 }]} />
          </Form.Item>
          <Form.Item name="status" label="状态">
            <Select options={[{ label: '启用', value: 1 }, { label: '禁用', value: 0 }]} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default MenuManage;
