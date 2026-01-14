using HRSystem.Domain.Enums;

namespace HRSystem.Domain.Entities;

public class RemoteWorkRequest : BaseRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;

    public RemoteWorkRequest()
    {
        RequestType = RequestType.RemoteWork;
    }
}