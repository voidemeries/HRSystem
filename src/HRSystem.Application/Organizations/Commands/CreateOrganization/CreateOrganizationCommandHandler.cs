using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Organizations.Common;
using HRSystem.Domain.Entities;
using MediatR;

namespace HRSystem.Application.Organizations.Commands.CreateOrganization;

public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
{
    private readonly IApplicationDbContext _context;

    public CreateOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Name = request.Name,
            Code = request.Code,
            ParentOrganizationId = request.ParentOrganizationId
        };

        _context.Organizations.Add(organization);
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