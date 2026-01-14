using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Positions.Common;
using HRSystem.Domain.Entities;
using MediatR;

namespace HRSystem.Application.Positions.Commands.CreatePosition;

public class CreatePositionCommandHandler : IRequestHandler<CreatePositionCommand, PositionDto>
{
    private readonly IApplicationDbContext _context;

    public CreatePositionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PositionDto> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
    {
        var position = new Position
        {
            Name = request.Name,
            Code = request.Code,
            ParentPositionId = request.ParentPositionId
        };

        _context.Positions.Add(position);
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