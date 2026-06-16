/** 权限工具函数 - 基于登录返回的权限码做按钮级控制 */

const permissionKey = 'yunke_permissions';

/** 缓存权限码列表 */
export function setPermissions(codes: string[]) {
  localStorage.setItem(permissionKey, JSON.stringify(codes));
}

/** 获取权限码列表 */
export function getPermissions(): string[] {
  try {
    const raw = localStorage.getItem(permissionKey);
    return raw ? JSON.parse(raw) : [];
  } catch {
    return [];
  }
}

/** 判断是否有指定权限码 */
export function hasPermission(code: string): boolean {
  return getPermissions().includes(code);
}

/** 判断是否有任一权限码 */
export function hasAnyPermission(...codes: string[]): boolean {
  const perms = getPermissions();
  return codes.some((c) => perms.includes(c));
}
