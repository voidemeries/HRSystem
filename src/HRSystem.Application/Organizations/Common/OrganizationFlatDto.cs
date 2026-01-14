namespace HRSystem.Application.Organizations.Common;

public class OrganizationFlatDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int? ParentOrganizationId { get; set; }
}