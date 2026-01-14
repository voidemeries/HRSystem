using HRSystem.Application.Organizations.Common;
using MediatR;

namespace HRSystem.Application.Organizations.Commands.CreateOrganization;

public record CreateOrganizationCommand : IRequest<OrganizationDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public int? ParentOrganizationId { get; init; }
}