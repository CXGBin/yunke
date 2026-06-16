import { defineConfig } from '@umijs/max';

export default defineConfig({
  antd: {
    theme: {
      token: {
        colorPrimary: '#1677ff',
      },
    },
  },
  access: {},
  model: {},
  initialState: {},
  request: {
    dataField: 'data',
  },
  layout: {
    title: '云科智教',
    locale: false,
  },
  locale: false,
  routes: [
    {
      path: '/login',
      layout: false,
      component: './Login',
    },
    {
      path: '/',
      routes: [
        {
          path: '/dashboard',
          name: '工作台',
          icon: 'dashboard',
          component: './Dashboard',
        },
        {
          path: '/system',
          name: '系统管理',
          icon: 'setting',
          routes: [
            { path: '/system/users', name: '用户管理', component: './System/UserManage' },
            { path: '/system/roles', name: '角色管理', component: './System/RoleManage' },
            { path: '/system/config', name: '系统配置', component: './System/SysConfig' },
          ],
        },
        {
          path: '/organization',
          name: '机构管理',
          icon: 'bank',
          routes: [
            { path: '/organization/list', name: '机构列表', component: './Organization/OrgList' },
            { path: '/organization/campus', name: '校区管理', component: './Organization/CampusManage' },
          ],
        },
        {
          path: '/package',
          name: '套餐管理',
          icon: 'crown',
          routes: [
            { path: '/package/annual', name: '年费套餐', component: './Package/AnnualPackage' },
            { path: '/package/course', name: '课程套餐', component: './Package/CoursePackage' },
          ],
        },
        {
          path: '/course',
          name: '课程管理',
          icon: 'book',
          routes: [
            { path: '/course/publish', name: '课程发布', component: './Course/CoursePublish' },
            { path: '/course/enrollment', name: '报名管理', component: './Course/Enrollment' },
          ],
        },
        {
          path: '/schedule',
          name: '排课管理',
          icon: 'calendar',
          routes: [
            { path: '/schedule/manage', name: '排课列表', component: './Schedule/ScheduleManage' },
          ],
        },
        {
          path: '/attendance',
          name: '考勤管理',
          icon: 'check-circle',
          routes: [
            { path: '/attendance/manage', name: '签到管理', component: './Attendance/AttendanceManage' },
          ],
        },
        {
          path: '/evaluation',
          name: '评价管理',
          icon: 'star',
          routes: [
            { path: '/evaluation/manage', name: '评价列表', component: './Evaluation/EvaluationManage' },
          ],
        },
        {
          path: '/settlement',
          name: '费用结算',
          icon: 'money-collect',
          routes: [
            { path: '/settlement/course', name: '课程结算', component: './Settlement/CourseSettlement' },
          ],
        },
        {
          path: '/statistics',
          name: '数据分析',
          icon: 'bar-chart',
          routes: [
            { path: '/statistics/overview', name: '数据概览', component: './Statistics/Overview' },
          ],
        },
        {
          path: '/notification',
          name: '通知管理',
          icon: 'notification',
          routes: [
            { path: '/notification/manage', name: '通知列表', component: './Notification/NotificationManage' },
          ],
        },
      ],
    },
    {
      path: '*',
      layout: false,
      component: './404',
    },
  ],
  proxy: {
    '/api': {
      target: 'http://localhost:5000',
      changeOrigin: true,
    },
  },
  npmClient: 'pnpm',
});
