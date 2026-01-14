using HRSystem.Application.Organizations.Common;
using MediatR;

namespace HRSystem.Application.Organizations.Commands.UpdateOrganization;

public record UpdateOrganizationCommand : IRequest<OrganizationDto>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public int? ParentOrganizationId { get; init; }
}