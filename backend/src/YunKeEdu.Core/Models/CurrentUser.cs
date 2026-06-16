namespace YunKeEdu.Core.Models;

public class CurrentUser
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Role { get; set; }
    public long TenantId { get; set; }
    public long OrgId { get; set; }
}
