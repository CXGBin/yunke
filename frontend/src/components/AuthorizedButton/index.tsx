// src/components/AuthorizedButton - 按钮级权限控制
import React from 'react';
import { hasPermission } from '@/utils/access';

interface Props {
  permission: string;
  children: React.ReactNode;
  fallback?: React.ReactNode;
}

const AuthorizedButton: React.FC<Props> = ({ permission, children, fallback = null }) => {
  if (!hasPermission(permission)) return <>{fallback}</>;
  return <>{children}</>;
};

export default AuthorizedButton;
