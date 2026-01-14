namespace HRSystem.Application.Positions.Common;

public class PositionFlatDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int? ParentPositionId { get; set; }
}