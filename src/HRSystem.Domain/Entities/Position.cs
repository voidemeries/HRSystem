using HRSystem.Domain.Common;

namespace HRSystem.Domain.Entities;

public class Position : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int? ParentPositionId { get; set; }

    public Position? ParentPosition { get; set; }
    public ICollection<Position> ChildPositions { get; set; } = new List<Position>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}