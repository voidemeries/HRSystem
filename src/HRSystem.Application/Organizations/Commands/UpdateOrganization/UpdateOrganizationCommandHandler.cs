using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Organizations.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Organization with ID {request.Id} not found.");

        organization.Name = request.Name;
        organization.Code = request.Code;
        organization.ParentOrganizationId = request.ParentOrganizationId;

        await _context.SaveChangesAsync(cancellationToken);

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