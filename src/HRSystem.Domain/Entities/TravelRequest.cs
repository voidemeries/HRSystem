using HRSystem.Domain.Enums;

namespace HRSystem.Domain.Entities;

public class TravelRequest : BaseRequest
{
    public string Destination { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }

    public TravelRequest()
    {
        RequestType = RequestType.Travel;
    }
}