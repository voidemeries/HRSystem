using HRSystem.Domain.Enums;

namespace HRSystem.Domain.Entities;

public class TrainingSupportRequest : BaseRequest
{
    public string TrainingName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Cost { get; set; }
    public string Justification { get; set; } = string.Empty;

    public TrainingSupportRequest()
    {
        RequestType = RequestType.TrainingSupport;
    }
}