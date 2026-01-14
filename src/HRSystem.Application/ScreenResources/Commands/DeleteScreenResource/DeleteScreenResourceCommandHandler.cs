using HRSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.ScreenResources.Commands.DeleteScreenResource;

public class DeleteScreenResourceCommandHandler : IRequestHandler<DeleteScreenResourceCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteScreenResourceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteScreenResourceCommand request, CancellationToken cancellationToken)
    {
        var screenResource = await _context.ScreenResources
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Screen resource with ID {request.Id} not found.");

        _context.ScreenResources.Remove(screenResource);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}