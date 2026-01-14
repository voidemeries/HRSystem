namespace HRSystem.Application.Permissions.Common;

public class PermissionAssignmentDto
{
    public int Id { get; set; }
    public int ScreenResourceId { get; set; }
    public string ScreenName { get; set; } = string.Empty;
    public string ScopeType { get; set; } = string.Empty;
    public int ScopeId { get; set; }
    public string ScopeName { get; set; } = string.Empty;
    public string PermissionType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}