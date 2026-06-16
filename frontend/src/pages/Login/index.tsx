import React from 'react';
import { history, useModel } from '@umijs/max';
import { LoginForm, ProFormText } from '@ant-design/pro-components';
import { LockOutlined, MobileOutlined } from '@ant-design/icons';
import { message } from 'antd';
import { login } from '@/services/auth';

const TOKEN_KEY = 'yunke_edu_token';

const Login: React.FC = () => {
  const { refresh } = useModel('@@initialState');

  const handleSubmit = async (values: API.LoginParams) => {
    try {
      const res = await login(values);
      if (res) {
        localStorage.setItem(TOKEN_KEY, res.accessToken);
        message.success('登录成功');
        await refresh();
        history.push('/dashboard');
      }
    } catch (err: any) {
      message.error(err?.message || '登录失败，请检查账号密码');
    }
  };

  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
      }}
    >
      <div
        style={{
          padding: '40px',
          background: '#fff',
          borderRadius: '12px',
          boxShadow: '0 20px 60px rgba(0,0,0,0.15)',
          minWidth: 400,
        }}
      >
        <LoginForm
          title="云科智教"
          subTitle="平台管理端"
          onFinish={handleSubmit}
          actions={undefined}
        >
          <ProFormText
            name="phone"
            fieldProps={{
              size: 'large',
              prefix: <MobileOutlined />,
            }}
            placeholder="请输入手机号"
            rules={[
              { required: true, message: '请输入手机号' },
              { pattern: /^1[3-9]\d{9}$/, message: '手机号格式不正确' },
            ]}
          />
          <ProFormText.Password
            name="password"
            fieldProps={{
              size: 'large',
              prefix: <LockOutlined />,
            }}
            placeholder="请输入密码"
            rules={[{ required: true, message: '请输入密码' }]}
          />
          <div style={{ marginBottom: 16 }}>
            <a style={{ float: 'right', color: '#1677ff' }} onClick={() => message.info('请联系平台管理员重置密码')}>
              忘记密码？
            </a>
          </div>
        </LoginForm>
      </div>
    </div>
  );
};

export default Login;
