namespace HRSystem.Application.Requests.Common;

public class PendingApprovalDto
{
    public int Id { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public int RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public int ForEmployeeId { get; set; }
    public string ForEmployeeName { get; set; } = string.Empty;
    public DateTime SubmittedDate { get; set; }
    public string Summary { get; set; } = string.Empty;
}