using HRSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Positions.Commands.DeletePosition;

public class DeletePositionCommandHandler : IRequestHandler<DeletePositionCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeletePositionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Position with ID {request.Id} not found.");

        _context.Positions.Remove(position);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}