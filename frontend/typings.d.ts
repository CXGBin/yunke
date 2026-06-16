/// <reference types="umi" />

declare module '*.css';
declare module '*.less';
declare module '*.png';
declare module '*.jpg';
declare module '*.jpeg';
declare module '*.svg';
declare module '*.json';

declare namespace API {

  // ============ 通用类型 ============
  interface ApiResponse<T = unknown> {
    code: number;
    message: string;
    data: T;
  }

  interface PagedResult<T = unknown> {
    items: T[];
    total: number;
    pageIndex: number;
    pageSize: number;
  }

  interface PageParams {
    pageIndex?: number;
    pageSize?: number;
  }

  // ============ 认证 ============
  interface CurrentUser {
    userId: number;
    userName: string;
    role: number;
    tenantId: number;
    orgId: number;
    realName?: string;
    phone?: string;
    avatar?: string;
  }

  interface LoginParams {
    phone: string;
    password: string;
  }

  interface LoginResult {
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
  }

  // ============ 机构 ============
  interface Organization {
    id: number;
    tenantId: number;
    orgCode: string;
    name: string;
    logo?: string;
    contactPerson?: string;
    contactPhone?: string;
    address?: string;
    province?: string;
    city?: string;
    district?: string;
    status: number;
    expiredAt?: string;
    description?: string;
    currentPackageId?: number;
    createdAt: string;
    updatedAt: string;
  }

  interface OrganizationParams {
    name: string;
    logo?: string;
    contactPerson?: string;
    contactPhone?: string;
    address?: string;
    province?: string;
    city?: string;
    district?: string;
    description?: string;
  }

  // ============ 校区 ============
  interface Campus {
    id: number;
    tenantId: number;
    orgId: number;
    campusCode: string;
    name: string;
    isDefault: boolean;
    contactPerson?: string;
    contactPhone?: string;
    address?: string;
    longitude?: number;
    latitude?: number;
    status: number;
    sortOrder: number;
    createdAt: string;
    updatedAt: string;
    orgName?: string;
  }

  interface CampusParams {
    orgId: number;
    name: string;
    contactPerson?: string;
    contactPhone?: string;
    address?: string;
    longitude?: number;
    latitude?: number;
  }

  // ============ 用户 ============
  interface SysUser {
    id: number;
    tenantId: number;
    orgId?: number;
    campusId?: number;
    userCode?: string;
    userName: string;
    realName?: string;
    nickName?: string;
    avatar?: string;
    phone?: string;
    gender: number;
    role: number;
    status: number;
    lastLoginAt?: string;
    createdAt: string;
    updatedAt: string;
    orgName?: string;
  }

  interface UserPageParams extends PageParams {
    keyword?: string;
    role?: number;
    status?: number;
    orgId?: number;
  }

  // ============ 角色 ============
  interface SysRole {
    id: number;
    roleName: string;
    roleCode: string;
    description?: string;
    status: number;
    createdAt: string;
    updatedAt: string;
  }

  // ============ 系统配置 ============
  interface SysConfig {
    id: number;
    configKey: string;
    configValue: string;
    configGroup?: string;
    description?: string;
    createdAt: string;
    updatedAt: string;
  }

  interface SysConfigParams {
    configKey: string;
    configValue: string;
    configGroup?: string;
    description?: string;
  }

  // ============ 年费套餐 ============
  interface OrgPackage {
    id: number;
    packageName: string;
    packageCode: string;
    packageLevel: number;
    price: number;
    description?: string;
    images?: string;
    maxCampusCount: number;
    maxTeacherCount: number;
    maxStudentCount: number;
    maxNotificationTypes: number;
    maxPushChannels: number;
    analyticsDimensions?: string;
    sortOrder: number;
    status: number;
    createdAt: string;
    updatedAt: string;
  }

  interface OrgPackageParams {
    packageName: string;
    packageCode: string;
    packageLevel: number;
    price: number;
    description?: string;
    images?: string;
    maxCampusCount: number;
    maxTeacherCount: number;
    maxStudentCount: number;
    maxNotificationTypes?: number;
    maxPushChannels?: number;
    analyticsDimensions?: string;
    sortOrder?: number;
    status?: number;
  }

  interface OrgPackageFeature {
    id: number;
    packageId: number;
    featureCode: string;
    featureName: string;
    featureGroup?: string;
    minPackageLevel: number;
    sortOrder: number;
    createdAt: string;
  }

  interface OrgSubscription {
    id: number;
    tenantId: number;
    orgId: number;
    packageId: number;
    startDate: string;
    endDate: string;
    amount: number;
    payStatus: number;
    payTime?: string;
    payChannel?: string;
    subscriptionType: number;
    preSubscriptionId?: number;
    remark?: string;
    createdAt: string;
    updatedAt: string;
    orgName?: string;
    packageName?: string;
  }

  // ============ 课程套餐 ============
  interface CoursePackage {
    id: number;
    tenantId: number;
    orgId: number;
    packageName: string;
    description?: string;
    coverImage?: string;
    totalPrice: number;
    courseCount: number;
    status: number;
    createdAt: string;
    updatedAt: string;
    orgName?: string;
  }

  // ============ 课程 ============
  interface Course {
    id: number;
    tenantId: number;
    orgId: number;
    campusId: number;
    courseName: string;
    courseCode?: string;
    coverImage?: string;
    description?: string;
    category?: string;
    maxStudents: number;
    currentStudents: number;
    teacherId: number;
    teacherName?: string;
    campusName?: string;
    settlementType: number;
    fixedFeePerLesson?: number;
    studentCountCommission?: number;
    status: number;
    createdAt: string;
    updatedAt: string;
    orgName?: string;
  }

  interface CoursePageParams extends PageParams {
    keyword?: string;
    status?: number;
    orgId?: number;
    category?: string;
  }

  // ============ 报名 ============
  interface Enrollment {
    id: number;
    tenantId: number;
    orgId: number;
    courseId: number;
    courseName?: string;
    studentId: number;
    studentName?: string;
    campusId: number;
    campusName?: string;
    enrollTime: string;
    status: number;
    createdAt: string;
    orgName?: string;
  }

  interface EnrollmentPageParams extends PageParams {
    keyword?: string;
    status?: number;
    orgId?: number;
    courseId?: number;
  }

  // ============ 排课 ============
  interface Schedule {
    id: number;
    tenantId: number;
    orgId: number;
    campusId: number;
    courseId: number;
    courseName?: string;
    teacherId: number;
    teacherName?: string;
    campusName?: string;
    scheduleDate: string;
    startTime: string;
    endTime: string;
    status: number;
    createdAt: string;
    orgName?: string;
  }

  interface SchedulePageParams extends PageParams {
    keyword?: string;
    orgId?: number;
    campusId?: number;
    courseId?: number;
    startDate?: string;
    endDate?: string;
  }

  // ============ 签到 ============
  interface Attendance {
    id: number;
    tenantId: number;
    orgId: number;
    scheduleId: number;
    courseId: number;
    courseName?: string;
    studentId: number;
    studentName?: string;
    teacherId: number;
    teacherName?: string;
    campusName?: string;
    attendanceStatus: number;
    signTime?: string;
    remark?: string;
    createdAt: string;
    orgName?: string;
  }

  interface AttendancePageParams extends PageParams {
    keyword?: string;
    orgId?: number;
    attendanceStatus?: number;
    courseId?: number;
    scheduleDate?: string;
  }

  // ============ 评价 ============
  interface Evaluation {
    id: number;
    tenantId: number;
    orgId: number;
    courseId: number;
    courseName?: string;
    scheduleId: number;
    studentId: number;
    studentName?: string;
    teacherId: number;
    teacherName?: string;
    score: number;
    content?: string;
    isAnonymous: boolean;
    replyContent?: string;
    replyTime?: string;
    status: number;
    createdAt: string;
    orgName?: string;
  }

  interface EvaluationPageParams extends PageParams {
    keyword?: string;
    orgId?: number;
    courseId?: number;
    minScore?: number;
    maxScore?: number;
  }

  // ============ 结算 ============
  interface Settlement {
    id: number;
    tenantId: number;
    orgId: number;
    courseId: number;
    courseName?: string;
    teacherId: number;
    teacherName?: string;
    settlementMonth: string;
    lessonCount: number;
    unitPrice: number;
    totalAmount: number;
    status: number;
    settledAt?: string;
    createdAt: string;
    orgName?: string;
  }

  interface SettlementPageParams extends PageParams {
    keyword?: string;
    orgId?: number;
    teacherId?: number;
    settlementMonth?: string;
    status?: number;
  }

  // ============ 统计 ============
  interface DashboardStats {
    orgCount: number;
    studentCount: number;
    teacherCount: number;
    courseCount: number;
    todayAttendanceCount: number;
    monthRevenue: number;
  }

  interface StatisticsData {
    attendanceRate?: number;
    enrollmentRate?: number;
    satisfactionScore?: number;
    revenueList?: Array<{ date: string; amount: number }>;
    orgGrowthList?: Array<{ date: string; count: number }>;
  }

  // ============ 通知 ============
  interface NotificationTemplate {
    id: number;
    templateName: string;
    templateCode: string;
    templateType: number;
    channel: number;
    title?: string;
    content?: string;
    variables?: string;
    status: number;
    createdAt: string;
    updatedAt: string;
  }

  interface NotificationParams {
    templateName: string;
    templateCode: string;
    templateType: number;
    channel: number;
    title?: string;
    content?: string;
    variables?: string;
    status?: number;
  }
}
