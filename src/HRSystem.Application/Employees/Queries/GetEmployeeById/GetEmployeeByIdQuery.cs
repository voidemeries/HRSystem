using HRSystem.Application.Employees.Common;
using MediatR;

namespace HRSystem.Application.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeDto>;