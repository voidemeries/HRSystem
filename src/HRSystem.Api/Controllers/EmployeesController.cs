using HRSystem.Application.Employees.Commands.CreateEmployee;
using HRSystem.Application.Employees.Commands.DeleteEmployee;
using HRSystem.Application.Employees.Commands.UpdateEmployee;
using HRSystem.Application.Employees.Common;
using HRSystem.Application.Employees.Queries.GetEmployeeById;
using HRSystem.Application.Employees.Queries.GetEmployees;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetEmployeeByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get employees with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<EmployeeListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<EmployeeListDto>>> GetAll(
        [FromQuery] int? organizationId,
        [FromQuery] int? positionId,
        [FromQuery] bool? isActive,
        [FromQuery] bool? isAdmin,
        [FromQuery] string? searchTerm)
    {
        var query = new GetEmployeesQuery
        {
            OrganizationId = organizationId,
            PositionId = positionId,
            IsActive = isActive,
            IsAdmin = isAdmin,
            SearchTerm = searchTerm
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new employee (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(CreateEmployeeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CreateEmployeeResponseDto>> Create([FromBody] CreateEmployeeCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing employee (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EmployeeDto>> Update(int id, [FromBody] UpdateEmployeeCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deactivate an employee (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteEmployeeCommand(id));
        return NoContent();
    }
}