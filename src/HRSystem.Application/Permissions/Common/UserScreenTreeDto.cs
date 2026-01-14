namespace HRSystem.Application.Permissions.Common;

public class UserScreenTreeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoutePath { get; set; } = string.Empty;
    public int? ParentScreenId { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public List<string> Permissions { get; set; } = new();
    public List<UserScreenTreeDto> Children { get; set; } = new();
}