namespace HRSystem.Application.Requests.TravelReques.Common;

public class TravelRequestDto
{
    public int Id { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public int RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public int ForEmployeeId { get; set; }
    public string ForEmployeeName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ApproverPositionId { get; set; }
    public string ApproverPositionName { get; set; } = string.Empty;
    public int? ApproverId { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime SubmittedDate { get; set; }

    // Travel-specific fields
    public string Destination { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
}