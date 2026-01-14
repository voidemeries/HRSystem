using HRSystem.Application.Organizations.Common;
using MediatR;

namespace HRSystem.Application.Organizations.Queries.GetOrganizationsFlat;

public record GetOrganizationsFlatQuery : IRequest<List<OrganizationFlatDto>>;