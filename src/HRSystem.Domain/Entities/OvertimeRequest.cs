using HRSystem.Domain.Enums;

namespace HRSystem.Domain.Entities;

public class OvertimeRequest : BaseRequest
{
    public DateTime OvertimeDate { get; set; }
    public decimal Hours { get; set; }
    public string Reason { get; set; } = string.Empty;

    public OvertimeRequest()
    {
        RequestType = RequestType.Overtime;
    }
}