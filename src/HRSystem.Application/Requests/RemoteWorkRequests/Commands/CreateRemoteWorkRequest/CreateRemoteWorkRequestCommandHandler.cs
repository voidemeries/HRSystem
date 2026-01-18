using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.RemoteWorkRequests.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;

namespace HRSystem.Application.Requests.RemoteWorkRequests.Commands.CreateRemoteWorkRequest;

public class CreateRemoteWorkRequestCommandHandler : IRequestHandler<CreateRemoteWorkRequestCommand, RemoteWorkRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public CreateRemoteWorkRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<RemoteWorkRequestDto> Handle(CreateRemoteWorkRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var forEmployeeId = request.ForEmployeeId ?? currentEmployeeId;

        var approverPositionId = await _approverService.GetApproverPositionIdAsync(forEmployeeId, cancellationToken);

        if (!approverPositionId.HasValue)
        {
            throw new InvalidOperationException("Cannot determine approver. Employee may not have a manager.");
        }

        var remoteWorkRequest = new RemoteWorkRequest
        {
            RequesterId = currentEmployeeId,
            ForEmployeeId = forEmployeeId,
            Status = RequestStatus.Submitted,
            ApproverPositionId = approverPositionId.Value,
            SubmittedDate = DateTime.UtcNow,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Location = request.Location,
            Reason = request.Reason
        };

        _context.RemoteWorkRequests.Add(remoteWorkRequest);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(remoteWorkRequest).Reference(r => r.Requester).LoadAsync(cancellationToken);
        await _context.Entry(remoteWorkRequest).Reference(r => r.ForEmployee).LoadAsync(cancellationToken);
        await _context.Entry(remoteWorkRequest).Reference(r => r.ApproverPosition).LoadAsync(cancellationToken);

        return new RemoteWorkRequestDto
        {
            Id = remoteWorkRequest.Id,
            RequestType = remoteWorkRequest.RequestType.ToString(),
            RequesterId = remoteWorkRequest.RequesterId,
            RequesterName = $"{remoteWorkRequest.Requester.FirstName} {remoteWorkRequest.Requester.LastName}",
            ForEmployeeId = remoteWorkRequest.ForEmployeeId,
            ForEmployeeName = $"{remoteWorkRequest.ForEmployee.FirstName} {remoteWorkRequest.ForEmployee.LastName}",
            Status = remoteWorkRequest.Status.ToString(),
            ApproverPositionId = remoteWorkRequest.ApproverPositionId,
            ApproverPositionName = remoteWorkRequest.ApproverPosition.Name,
            SubmittedDate = remoteWorkRequest.SubmittedDate,
            StartDate = remoteWorkRequest.StartDate,
            EndDate = remoteWorkRequest.EndDate,
            Location = remoteWorkRequest.Location,
            Reason = remoteWorkRequest.Reason
        };
    }
}