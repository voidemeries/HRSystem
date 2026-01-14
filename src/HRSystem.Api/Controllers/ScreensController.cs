using HRSystem.Application.ScreenResources.Commands.CreateScreenResource;
using HRSystem.Application.ScreenResources.Commands.DeleteScreenResource;
using HRSystem.Application.ScreenResources.Commands.UpdateScreenResource;
using HRSystem.Application.ScreenResources.Common;
using HRSystem.Application.ScreenResources.Queries.GetScreenResourceById;
using HRSystem.Application.ScreenResources.Queries.GetScreenResourcesTree;
using HRSystem.Application.ScreenResources.Queries.SearchScreenResources;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScreensController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScreensController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get screen resource by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ScreenResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ScreenResourceDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetScreenResourceByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get screen resources in hierarchical tree structure
    /// </summary>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(List<ScreenResourceTreeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ScreenResourceTreeDto>>> GetTree()
    {
        var result = await _mediator.Send(new GetScreenResourcesTreeQuery());
        return Ok(result);
    }

    /// <summary>
    /// Search screen resources by name or route path
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<ScreenResourceSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ScreenResourceSearchResultDto>>> Search([FromQuery] string q = "")
    {
        var result = await _mediator.Send(new SearchScreenResourcesQuery { Query = q });
        return Ok(result);
    }

    /// <summary>
    /// Create a new screen resource (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ScreenResourceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ScreenResourceDto>> Create([FromBody] CreateScreenResourceCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing screen resource (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ScreenResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ScreenResourceDto>> Update(int id, [FromBody] UpdateScreenResourceCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a screen resource (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteScreenResourceCommand(id));
        return NoContent();
    }
}