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

  /** 后端 PagedResult: items/total/page/pageSize (CamelCase) */
  interface PagedResult<T = unknown> {
    items: T[];
    total: number;
    page: number;
    pageSize: number;
  }

  /** 前端分页请求参数，映射到后端 PageRequest.page / PageRequest.pageSize */
  interface PageParams {
    page?: number;
    pageSize?: number;
    keyword?: string;
  }

  // ============ 认证 ============
  /** 后端 UserInfoDto (CamelCase) */
  interface CurrentUser {
    userId: number;
    userName: string;
    phone: string;
    realName?: string;
    nickName?: string;
    avatar?: string;
    role: number;
    tenantId: number;
    orgId: number;
    bindings?: UserOrgInfo[];
  }

  interface UserOrgInfo {
    orgId: number;
    orgName: string;
    campusId: number;
    campusName: string;
    role: number;
    userCode?: string;
  }

  interface LoginParams {
    phone: string;
    password: string;
  }

  /** 后端 LoginResponse: token + userInfo + permissions */
  interface LoginResult {
    token: string;
    userInfo: CurrentUser;
    permissions?: UserPermission;
  }

  /** 用户权限信息 */
  interface UserPermission {
    roles: RoleItem[];
    menus: MenuItem[];
    permissions: string[];
  }

  interface RoleItem {
    id: number;
    tenantId: number;
    roleName: string;
    roleCode?: string;
    status: number;
  }

  /** 菜单树节点 */
  interface MenuItem {
    id: number;
    parentId: number;
    menuType: number; // 1=目录 2=菜单 3=按钮
    name: string;
    path?: string;
    component?: string;
    icon?: string;
    sortOrder: number;
    permission?: string;
    btnType?: string;
    visible: number;
    status: number;
    children: MenuItem[];
  }

  interface RoleParams {
    roleName: string;
    roleCode?: string;
    description?: string;
    sortOrder?: number;
    status?: number;
    dataScope?: number;
    menuIds?: number[];
  }

  interface MenuParams {
    parentId: number;
    menuType: number;
    name: string;
    path?: string;
    component?: string;
    icon?: string;
    sortOrder?: number;
    permission?: string;
    btnType?: string;
    visible?: number;
    status?: number;
    description?: string;
  }

  // ============ 机构 (后端 OrgDto) ============
  interface Organization {
    id: number;
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

  // ============ 校区 (后端 CampusDto) ============
  interface Campus {
    id: number;
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
  }

  interface CampusParams {
    name: string;
    contactPerson?: string;
    contactPhone?: string;
    address?: string;
    longitude?: number;
    latitude?: number;
    sortOrder?: number;
  }

  // ============ 用户 (后端 UserController DTOs) ============
  interface SysUser {
    id: number;
    userName: string;
    realName?: string;
    nickName?: string;
    phone?: string;
    avatar?: string;
    gender: number;
    role: number;
    orgId?: number;
    campusId?: number;
    status: number;
  }

  interface CreateUserParams {
    userName?: string;
    realName?: string;
    phone?: string;
    password?: string;
    role: number;
    orgId?: number;
    campusId?: number;
  }

  interface UserPageParams {
    page?: number;
    pageSize?: number;
    orgId?: number;
    role?: number;
    keyword?: string;
  }

  // ============ 系统配置 (后端 SysConfigDto) ============
  interface SysConfig {
    id: number;
    configKey: string;
    configValue: string;
    configGroup?: string;
    description?: string;
    updatedAt: string;
  }

  interface SysConfigParams {
    configKey: string;
    configValue: string;
    configGroup?: string;
    description?: string;
  }

  // ============ 年费套餐 (后端 PackageDto) ============
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
    features?: OrgPackageFeature[];
    createdAt: string;
  }

  interface OrgPackageParams {
    packageName: string;
    packageCode: string;
    packageLevel: number;
    price: number;
    description?: string;
    images?: string;
    maxCampusCount?: number;
    maxTeacherCount?: number;
    maxStudentCount?: number;
    maxNotificationTypes?: number;
    maxPushChannels?: number;
    analyticsDimensions?: string;
    sortOrder?: number;
  }

  interface OrgPackageFeature {
    id: number;
    packageId: number;
    featureCode: string;
    featureName: string;
    featureGroup?: string;
    minPackageLevel: number;
    sortOrder: number;
  }

  /** 后端 SubscriptionDto */
  interface OrgSubscription {
    id: number;
    orgId: number;
    packageId: number;
    packageName: string;
    packageLevel: number;
    startDate: string;
    endDate: string;
    amount: number;
    payStatus: number;
    payTime?: string;
    subscriptionType: number;
    preSubscriptionId?: number;
    remark?: string;
    remainingDays: number;
    createdAt: string;
  }

  // ============ 课程套餐 (后端 CoursePackageDto) ============
  interface CoursePackage {
    id: number;
    packageName: string;
    description?: string;
    coverImage?: string;
    totalPrice: number;
    courseCount: number;
    status: number;
    buyCount: number;
    sortOrder: number;
    isRecommend: boolean;
    orgId?: number;
    orgName?: string;
    items?: CoursePackageItem[];
    createdAt: string;
  }

  interface CoursePackageItem {
    id: number;
    courseId: number;
    courseName: string;
    unitPrice: number;
    sortOrder: number;
  }

  // ============ 课程 (后端 CourseDto) ============
  interface Course {
    id: number;
    courseCode?: string;
    name: string;
    categoryId?: number;
    categoryName?: string;
    description?: string;
    coverImage?: string;
    totalLessons: number;
    lessonDuration: number;
    difficulty: number;
    originalPrice: number;
    discountPrice: number;
    maxStudents: number;
    minStudents: number;
    enrollmentDeadline?: string;
    tags?: string;
    status: number;
    teacherId: number;
    teacherName?: string;
    campusId: number;
    campusName?: string;
    settlementType: number;
    fixedFeePerLesson: number;
    studentCountCommission: number;
    sortOrder: number;
    isRecommend: boolean;
    viewCount: number;
    createdBy: number;
    orgId?: number;
    orgName?: string;
    createdAt: string;
  }

  interface CoursePageParams extends PageParams {
    status?: number;
    orgId?: number;
    categoryId?: number;
  }

  // ============ 课程分类 (后端 CategoryTreeNode) ============
  interface CategoryTreeNode {
    id: number;
    name: string;
    icon?: string;
    sortOrder: number;
    parentId: number;
    children?: CategoryTreeNode[];
  }

  // ============ 课时 (后端 LessonUnitDto) ============
  interface LessonUnit {
    id: number;
    courseId: number;
    lessonNo: number;
    title: string;
    description?: string;
    sortOrder: number;
    status: number;
  }

  // ============ 报名 (后端 EnrollmentDto) ============
  interface Enrollment {
    id: number;
    courseId: number;
    courseName: string;
    studentId: number;
    studentName: string;
    status: number;
    enrolledAt: string;
    createdAt: string;
  }

  // ============ 排课 (后端 ScheduleDto) ============
  interface Schedule {
    id: number;
    courseId: number;
    courseName: string;
    campusId: number;
    campusName?: string;
    teacherId: number;
    teacherName?: string;
    lessonDate: string;
    startTime: string;
    endTime: string;
    lessonNo?: number;
    lessonTitle?: string;
    remark?: string;
    status: number;
    cancelReason?: string;
    isRescheduled: boolean;
    publishedAt?: string;
    createdAt: string;
  }

  interface SchedulePageParams extends PageParams {
    courseId?: number;
    campusId?: number;
    teacherId?: number;
    startDate?: string;
    endDate?: string;
  }

  // ============ 签到 (后端 AttendanceDto) ============
  interface Attendance {
    id: number;
    scheduleId: number;
    courseId: number;
    courseName: string;
    studentId: number;
    studentName: string;
    status: number;
    signInTime?: string;
    signMethod?: number;
    remark?: string;
    createdAt: string;
  }

  // ============ 评价 (后端 EvaluationDto) ============
  interface Evaluation {
    id: number;
    courseId: number;
    courseName: string;
    scheduleId: number;
    evaluatorId: number;
    evaluatorName: string;
    evaluatorAvatar?: string;
    targetId: number;
    targetName: string;
    evalType: number;
    courseRating?: number;
    teacherRating?: number;
    lessonRating?: number;
    content?: string;
    tags?: string;
    images?: string;
    isAnonymous: boolean;
    status: number;
    replyContent?: string;
    replyBy?: number;
    replyAt?: string;
    isTop: boolean;
    replies?: EvaluationReply[];
    createdAt: string;
  }

  interface EvaluationReply {
    id: number;
    evaluationId: number;
    content: string;
    images?: string;
    replyType: number;
    replyById: number;
    replyByName?: string;
    createdAt: string;
  }

  interface EvaluationPageParams extends PageParams {
    courseId?: number;
  }

  // ============ 评价标签 (后端 EvaluationTagDto) ============
  interface EvaluationTag {
    id: number;
    name: string;
    tagType: number;
    sortOrder: number;
    status: number;
    createdAt: string;
  }

  // ============ 通知模板 (后端 NotificationTemplateDto) ============
  interface NotificationTemplate {
    id: number;
    orgId?: number;
    templateName: string;
    templateCode: string;
    notifyType: number;
    channel: number;
    titleTemplate: string;
    contentTemplate: string;
    variables?: string;
    status: number;
    createdAt: string;
  }

  interface NotificationParams {
    templateName: string;
    templateCode: string;
    notifyType: number;
    channel: number;
    titleTemplate: string;
    contentTemplate: string;
    variables?: string;
  }

  // ============ 通知日志 (后端 NotificationLogDto) ============
  interface NotificationLog {
    id: number;
    recipientId: number;
    recipientName: string;
    notifyType: number;
    channel: number;
    title: string;
    content: string;
    isRead: boolean;
    readAt?: string;
    relateType?: string;
    relateId?: number;
    sendStatus: number;
    createdAt: string;
  }

  // ============ 请假 (后端 LeaveRequestDto) ============
  interface LeaveRequest {
    id: number;
    studentId: number;
    studentName: string;
    courseId: number;
    courseName: string;
    scheduleId?: number;
    leaveType: number;
    startDate: string;
    endDate: string;
    reason: string;
    status: number;
    applicantId: number;
    preReviewerId?: number;
    preReviewerName?: string;
    preReviewedAt?: string;
    preReviewRemark?: string;
    approverId?: number;
    approverName?: string;
    approvedAt?: string;
    approveRemark?: string;
    createdAt: string;
  }

  // ============ 学生 (后端 StudentDto) ============
  interface Student {
    id: number;
    userCode?: string;
    userName: string;
    realName?: string;
    avatar?: string;
    phone?: string;
    gender: number;
    grade?: string;
    status: number;
    orgId?: number;
    campusId?: number;
    orgName?: string;
    createdAt: string;
  }

  // ============ 家长 (后端 ParentDto) ============
  interface Parent {
    id: number;
    userName: string;
    realName?: string;
    avatar?: string;
    phone?: string;
    childrenCount: number;
    createdAt: string;
  }

  interface ChildInfo {
    relationId: number;
    studentId: number;
    studentName: string;
    studentAvatar?: string;
    grade?: string;
    relationType: number;
    isPrimary: boolean;
  }

  // ============ 结算 (后端 SettlementRuleDto) ============
  interface SettlementRule {
    courseId: number;
    courseName: string;
    settlementType: number;
    fixedAmount: number;
    originalPrice: number;
    totalLessons: number;
  }

  interface Wallet {
    teacherId: number;
    teacherName: string;
    balance: number;
    totalIncome: number;
    totalWithdrawn: number;
    lastSettlementAt?: string;
  }

  interface WalletDetail {
    id: number;
    courseId: number;
    courseName: string;
    scheduleId: number;
    settlementDate: string;
    amount: number;
    settlementType: number;
    studentCount: number;
    triggerType: number;
    status: number;
    remark?: string;
    createdAt: string;
  }

  interface FeeSettlementRecord {
    id: number;
    courseId: number;
    courseName: string;
    scheduleId: number;
    teacherId: number;
    teacherName: string;
    amount: number;
    settlementType: number;
    studentCount: number;
    settlementDate: string;
    settledAt: string;
    triggerType: number;
    status: number;
    remark?: string;
    createdAt: string;
  }

  interface Settlement {
    id: number;
    courseId: number;
    courseName: string;
    teacherId: number;
    teacherName: string;
    amount: number;
    settlementType: number;
    studentCount: number;
    settlementDate: string;
    triggerType: number;
    status: number;
    remark?: string;
    createdAt: string;
  }

  interface SettlementPageParams extends PageParams {
    courseId?: number;
    teacherId?: number;
    month?: number;
  }

  // ============ 统计 ============
  interface DashboardStats {
    totalStudents: number;
    totalTeachers: number;
    totalCourses: number;
    activeCourses: number;
    todayEnrollments: number;
    todayAttendanceRate: number;
    monthlyRevenue: number;
    pendingLeaves: number;
    todaySchedules: number;
  }

  interface OrgOverview {
    totalOrgs: number;
    totalStudents: number;
    totalTeachers: number;
    totalCourses: number;
    totalRevenue: number;
  }

  // ============ 主题 ============
  interface Theme {
    themeId: string;
    themeName: string;
    primaryColor?: string;
    secondaryColor?: string;
    buttonColor?: string;
    backgroundColor?: string;
  }

  // ============ 邀请 ============
  interface Invitation {
    id: number;
    inviteCode: string;
    invitedRole: number;
    invitedName?: string;
    invitedPhone?: string;
    status: number;
    expiresAt: string;
    usedBy?: number;
    usedAt?: string;
    remark?: string;
    createdAt: string;
  }

  interface ValidateInvitation {
    valid: boolean;
    inviteCode?: string;
    invitedRole: number;
    orgName?: string;
    campusName?: string;
    invitedName?: string;
  }
}
