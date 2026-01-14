using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Positions.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Positions.Commands.UpdatePosition;

public class UpdatePositionCommandHandler : IRequestHandler<UpdatePositionCommand, PositionDto>
{
    private readonly IApplicationDbContext _context;

    public UpdatePositionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PositionDto> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Position with ID {request.Id} not found.");

        position.Name = request.Name;
        position.Code = request.Code;
        position.ParentPositionId = request.ParentPositionId;

        await _context.SaveChangesAsync(cancellationToken);

        return new PositionDto
        {
            Id = position.Id,
            Name = position.Name,
            Code = position.Code,
            ParentPositionId = position.ParentPositionId,
            CreatedAt = position.CreatedAt,
            UpdatedAt = position.UpdatedAt
        };
    }
}