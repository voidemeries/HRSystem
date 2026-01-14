using HRSystem.Application.Organizations.Common;
using MediatR;

namespace HRSystem.Application.Organizations.Queries.GetOrganizationById;

public record GetOrganizationByIdQuery(int Id) : IRequest<OrganizationDto>;