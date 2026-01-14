using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Organizations.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Organizations.Queries.GetOrganizationById;

public class GetOrganizationByIdQueryHandler : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto>
{
    private readonly IApplicationDbContext _context;

    public GetOrganizationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizationDto> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Organization with ID {request.Id} not found.");

        return new OrganizationDto
        {
            Id = organization.Id,
            Name = organization.Name,
            Code = organization.Code,
            ParentOrganizationId = organization.ParentOrganizationId,
            CreatedAt = organization.CreatedAt,
            UpdatedAt = organization.UpdatedAt
        };
    }
}