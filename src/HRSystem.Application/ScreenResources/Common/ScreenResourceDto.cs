namespace HRSystem.Application.ScreenResources.Common;

public class ScreenResourceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoutePath { get; set; } = string.Empty;
    public int? ParentScreenId { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public string? Icon { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}