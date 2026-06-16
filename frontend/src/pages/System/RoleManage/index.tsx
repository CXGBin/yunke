import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { Card, Tag, Empty, Space, Button, message } from 'antd';
import { SettingOutlined } from '@ant-design/icons';

const ROLE_LIST = [
  { code: 0, name: '无角色用户', desc: '刚登录尚未绑定机构的用户', fixed: true },
  { code: 1, name: '平台管理员', desc: '平台SaaS运营方，管理全平台数据', fixed: true },
  { code: 2, name: '机构管理员', desc: '机构管理方，管理本机构数据', fixed: true },
  { code: 3, name: '教师', desc: '授课教师，查看排课/签到/评价', fixed: true },
  { code: 4, name: '学生', desc: '学员，选课/签到/评价/请假', fixed: true },
  { code: 5, name: '家长', desc: '学员家长，关注孩子/代请假/代评价', fixed: true },
];

const RoleManage: React.FC = () => {
  return (
    <PageContainer>
      <Card
        title="角色管理"
        bordered={false}
        extra={
          <Button icon={<SettingOutlined />} onClick={() => message.info('权限配置功能待完善')}>
            权限配置
          </Button>
        }
      >
        <Empty description="暂无自定义角色">
          <p style={{ color: '#666', fontSize: 14, marginTop: 8 }}>
            系统预置以下角色，权限点配置功能即将上线
          </p>
        </Empty>
        <div style={{ marginTop: 24 }}>
          {ROLE_LIST.map((role) => (
            <div
              key={role.code}
              style={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                padding: '12px 16px',
                borderBottom: '1px solid #f0f0f0',
              }}
            >
              <Space>
                <span style={{ fontWeight: 500 }}>{role.name}</span>
                <Tag color="blue">编码: {role.code}</Tag>
              </Space>
              <span style={{ color: '#999' }}>{role.desc}</span>
            </div>
          ))}
        </div>
      </Card>
    </PageContainer>
  );
};

export default RoleManage;
