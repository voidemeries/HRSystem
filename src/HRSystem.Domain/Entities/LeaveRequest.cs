using HRSystem.Domain.Enums;

namespace HRSystem.Domain.Entities;

public class LeaveRequest : BaseRequest
{
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;

    public LeaveRequest()
    {
        RequestType = RequestType.Leave;
    }
}