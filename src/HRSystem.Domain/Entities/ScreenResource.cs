using HRSystem.Domain.Common;

namespace HRSystem.Domain.Entities;

public class ScreenResource : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string RoutePath { get; set; } = string.Empty;
    public int? ParentScreenId { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public string? Icon { get; set; }

    public ScreenResource? ParentScreen { get; set; }
    public ICollection<ScreenResource> ChildScreens { get; set; } = new List<ScreenResource>();
}