using HRSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Permissions.Commands.DeletePermissionAssignment;

public class DeletePermissionAssignmentCommandHandler : IRequestHandler<DeletePermissionAssignmentCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeletePermissionAssignmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeletePermissionAssignmentCommand request, CancellationToken cancellationToken)
    {
        var permission = await _context.PermissionAssignments
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Permission assignment with ID {request.Id} not found.");

        _context.PermissionAssignments.Remove(permission);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}