using HRSystem.Application.Organizations.Common;
using MediatR;

namespace HRSystem.Application.Organizations.Queries.GetOrganizationsTree;

public record GetOrganizationsTreeQuery : IRequest<List<OrganizationTreeDto>>;