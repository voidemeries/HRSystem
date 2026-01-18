using HRSystem.Application.Requests.OvertimeRequests.Common;
using MediatR;

namespace HRSystem.Application.Requests.OvertimeRequests.Queries.GetOvertimeRequestById;

public record GetOvertimeRequestByIdQuery(int Id) : IRequest<OvertimeRequestDto>;