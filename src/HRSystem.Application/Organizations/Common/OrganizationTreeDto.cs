namespace HRSystem.Application.Organizations.Common;

public class OrganizationTreeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int? ParentOrganizationId { get; set; }
    public List<OrganizationTreeDto> Children { get; set; } = new();
}