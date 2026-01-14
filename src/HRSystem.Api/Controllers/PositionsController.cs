using HRSystem.Application.Positions.Commands.CreatePosition;
using HRSystem.Application.Positions.Commands.DeletePosition;
using HRSystem.Application.Positions.Commands.UpdatePosition;
using HRSystem.Application.Positions.Common;
using HRSystem.Application.Positions.Queries.GetPositionById;
using HRSystem.Application.Positions.Queries.GetPositionsFlat;
using HRSystem.Application.Positions.Queries.GetPositionsTree;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PositionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get position by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PositionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PositionDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetPositionByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get positions in hierarchical tree structure
    /// </summary>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(List<PositionTreeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PositionTreeDto>>> GetTree()
    {
        var result = await _mediator.Send(new GetPositionsTreeQuery());
        return Ok(result);
    }

    /// <summary>
    /// Get positions in flat list (for dropdowns/combobox)
    /// </summary>
    [HttpGet("flat")]
    [ProducesResponseType(typeof(List<PositionFlatDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PositionFlatDto>>> GetFlat()
    {
        var result = await _mediator.Send(new GetPositionsFlatQuery());
        return Ok(result);
    }

    /// <summary>
    /// Create a new position
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PositionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PositionDto>> Create([FromBody] CreatePositionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing position
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PositionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PositionDto>> Update(int id, [FromBody] UpdatePositionCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a position
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeletePositionCommand(id));
        return NoContent();
    }
}