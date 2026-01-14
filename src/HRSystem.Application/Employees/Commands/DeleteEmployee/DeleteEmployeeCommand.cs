using MediatR;

namespace HRSystem.Application.Employees.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(int Id) : IRequest<Unit>;