using HRSystem.Domain.Common;

namespace HRSystem.Domain.Entities;

public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int? ParentOrganizationId { get; set; }

    public Organization? ParentOrganization { get; set; }
    public ICollection<Organization> ChildOrganizations { get; set; } = new List<Organization>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}