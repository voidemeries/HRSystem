namespace HRSystem.Application.ScreenResources.Common;

public class ScreenResourceSearchResultDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoutePath { get; set; } = string.Empty;
    public int? ParentScreenId { get; set; }
    public string? ParentScreenName { get; set; }
    public bool IsActive { get; set; }
    public string? Icon { get; set; }
}