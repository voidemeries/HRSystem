using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Organizations.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Organizations.Queries.GetOrganizationsFlat;

public class GetOrganizationsFlatQueryHandler : IRequestHandler<GetOrganizationsFlatQuery, List<OrganizationFlatDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrganizationsFlatQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrganizationFlatDto>> Handle(GetOrganizationsFlatQuery request, CancellationToken cancellationToken)
    {
        return await _context.Organizations
            .OrderBy(o => o.Name)
            .Select(o => new OrganizationFlatDto
            {
                Id = o.Id,
                Name = o.Name,
                Code = o.Code,
                ParentOrganizationId = o.ParentOrganizationId
            })
            .ToListAsync(cancellationToken);
    }
}