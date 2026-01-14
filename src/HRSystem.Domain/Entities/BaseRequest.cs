using HRSystem.Domain.Common;
using HRSystem.Domain.Enums;

namespace HRSystem.Domain.Entities;

public abstract class BaseRequest : BaseEntity
{
    public RequestType RequestType { get; set; }
    public int RequesterId { get; set; }
    public int ForEmployeeId { get; set; }
    public RequestStatus Status { get; set; }
    public int ApproverPositionId { get; set; }
    public int? ApproverId { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime SubmittedDate { get; set; }

    public Employee Requester { get; set; } = null!;
    public Employee ForEmployee { get; set; } = null!;
    public Position ApproverPosition { get; set; } = null!;
    public Employee? Approver { get; set; }
}