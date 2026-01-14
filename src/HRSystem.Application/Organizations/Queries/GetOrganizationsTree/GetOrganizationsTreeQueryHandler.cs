using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Organizations.Common;
using HRSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Organizations.Queries.GetOrganizationsTree;

public class GetOrganizationsTreeQueryHandler : IRequestHandler<GetOrganizationsTreeQuery, List<OrganizationTreeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrganizationsTreeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrganizationTreeDto>> Handle(GetOrganizationsTreeQuery request, CancellationToken cancellationToken)
    {
        var organizations = await _context.Organizations
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);

        return BuildTree(organizations, null);
    }

    private List<OrganizationTreeDto> BuildTree(List<Organization> allOrganizations, int? parentId)
    {
        return allOrganizations
            .Where(o => o.ParentOrganizationId == parentId)
            .Select(o => new OrganizationTreeDto
            {
                Id = o.Id,
                Name = o.Name,
                Code = o.Code,
                ParentOrganizationId = o.ParentOrganizationId,
                Children = BuildTree(allOrganizations, o.Id)
            })
            .ToList();
    }
}