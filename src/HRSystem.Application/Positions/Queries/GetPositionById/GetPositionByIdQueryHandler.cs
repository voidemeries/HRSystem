using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Positions.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Positions.Queries.GetPositionById;

public class GetPositionByIdQueryHandler : IRequestHandler<GetPositionByIdQuery, PositionDto>
{
    private readonly IApplicationDbContext _context;

    public GetPositionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PositionDto> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
    {
        var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Position with ID {request.Id} not found.");

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