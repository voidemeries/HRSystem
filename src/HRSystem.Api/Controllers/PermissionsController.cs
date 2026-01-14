using HRSystem.Application.Permissions.Commands.CreatePermissionAssignment;
using HRSystem.Application.Permissions.Commands.DeletePermissionAssignment;
using HRSystem.Application.Permissions.Commands.UpdatePermissionAssignment;
using HRSystem.Application.Permissions.Common;
using HRSystem.Application.Permissions.Queries.GetPermissionAssignmentById;
using HRSystem.Application.Permissions.Queries.GetPermissionAssignments;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get permission assignment by ID (Admin only)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PermissionAssignmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PermissionAssignmentDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetPermissionAssignmentByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get permission assignments with optional filters (Admin only)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PermissionAssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<PermissionAssignmentDto>>> GetAll(
        [FromQuery] int? screenResourceId,
        [FromQuery] ScopeType? scopeType,
        [FromQuery] int? scopeId,
        [FromQuery] PermissionType? permissionType)
    {
        var query = new GetPermissionAssignmentsQuery
        {
            ScreenResourceId = screenResourceId,
            ScopeType = scopeType,
            ScopeId = scopeId,
            PermissionType = permissionType
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new permission assignment (Admin only)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PermissionAssignmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PermissionAssignmentDto>> Create([FromBody] CreatePermissionAssignmentCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing permission assignment (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PermissionAssignmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PermissionAssignmentDto>> Update(int id, [FromBody] UpdatePermissionAssignmentCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a permission assignment (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeletePermissionAssignmentCommand(id));
        return NoContent();
    }
}