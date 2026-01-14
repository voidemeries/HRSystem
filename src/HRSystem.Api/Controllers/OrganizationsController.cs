using HRSystem.Application.Organizations.Commands.CreateOrganization;
using HRSystem.Application.Organizations.Commands.DeleteOrganization;
using HRSystem.Application.Organizations.Commands.UpdateOrganization;
using HRSystem.Application.Organizations.Common;
using HRSystem.Application.Organizations.Queries.GetOrganizationById;
using HRSystem.Application.Organizations.Queries.GetOrganizationsFlat;
using HRSystem.Application.Organizations.Queries.GetOrganizationsTree;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get organization by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrganizationDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetOrganizationByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get organizations in hierarchical tree structure
    /// </summary>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(List<OrganizationTreeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<OrganizationTreeDto>>> GetTree()
    {
        var result = await _mediator.Send(new GetOrganizationsTreeQuery());
        return Ok(result);
    }

    /// <summary>
    /// Get organizations in flat list (for dropdowns/combobox)
    /// </summary>
    [HttpGet("flat")]
    [ProducesResponseType(typeof(List<OrganizationFlatDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<OrganizationFlatDto>>> GetFlat()
    {
        var result = await _mediator.Send(new GetOrganizationsFlatQuery());
        return Ok(result);
    }

    /// <summary>
    /// Create a new organization
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrganizationDto>> Create([FromBody] CreateOrganizationCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing organization
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrganizationDto>> Update(int id, [FromBody] UpdateOrganizationCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete an organization
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteOrganizationCommand(id));
        return NoContent();
    }
}