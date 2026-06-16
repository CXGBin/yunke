namespace YunKeEdu.Core.Enums;

/// <summary>角色枚举</summary>
public enum RoleEnum { NoRole = 0, PlatformAdmin = 1, OrgAdmin = 2, Teacher = 3, Student = 4, Parent = 5 }

/// <summary>课程状态</summary>
public enum CourseStatus { Draft = 0, Published = 1, Offline = 2 }

/// <summary>通用状态</summary>
public enum CommonStatus { Disabled = 0, Enabled = 1 }

/// <summary>结算方式</summary>
public enum SettlementType { Fixed = 0, Percentage = 1 }

/// <summary>签到状态</summary>
public enum AttendanceStatus { NotSigned = 0, Present = 1, Late = 2, Absent = 3, Leave = 4 }

/// <summary>排课状态</summary>
public enum ScheduleStatus { Planned = 0, Published = 1, Cancelled = 2 }
