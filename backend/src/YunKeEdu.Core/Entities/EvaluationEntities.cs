using SqlSugar;

namespace YunKeEdu.Core.Entities;

/// <summary>课程评价表</summary>
[SugarTable("CourseEvaluation")]
public class CourseEvaluation : BaseEntity
{
    public long OrgId { get; set; }
    public long? CampusId { get; set; }
    public long CourseId { get; set; }
    public long ScheduleId { get; set; }
    public long EvaluatorId { get; set; }
    public long TargetId { get; set; }
    public int EvalType { get; set; }
    public int? CourseRating { get; set; }
    public int? TeacherRating { get; set; }
    public int? LessonRating { get; set; }
    [SugarColumn(Length = -1, IsNullable = true, ColumnDataType = "nvarchar(max)")]
    public string? Content { get; set; }
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Tags { get; set; }
    [SugarColumn(Length = 1000, IsNullable = true)]
    public string? Images { get; set; }
    public bool IsAnonymous { get; set; }
    public int Status { get; set; }
    [SugarColumn(Length = -1, IsNullable = true, ColumnDataType = "nvarchar(max)")]
    public string? ReplyContent { get; set; }
    public long? ReplyBy { get; set; }
    public DateTime? ReplyAt { get; set; }
    public bool IsTop { get; set; }
}

/// <summary>追加评价/回复表</summary>
[SugarTable("EvaluationReply")]
public class EvaluationReply
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long EvaluationId { get; set; }
    [SugarColumn(Length = -1, IsNullable = false, ColumnDataType = "nvarchar(max)")]
    public string Content { get; set; } = string.Empty;
    [SugarColumn(Length = 1000, IsNullable = true)]
    public string? Images { get; set; }
    public int ReplyType { get; set; }
    public long ReplyById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
