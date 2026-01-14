namespace HRSystem.Application.Permissions.Common;

public class UserScreenSearchResultDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoutePath { get; set; } = string.Empty;
    public int? ParentScreenId { get; set; }
    public string? ParentScreenName { get; set; }
    public string? Icon { get; set; }
    public List<string> Permissions { get; set; } = new();
}