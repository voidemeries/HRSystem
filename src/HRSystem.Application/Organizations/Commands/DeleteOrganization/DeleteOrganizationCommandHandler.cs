using HRSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Organizations.Commands.DeleteOrganization;

public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Organization with ID {request.Id} not found.");

        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}