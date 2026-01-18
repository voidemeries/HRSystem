using HRSystem.Application.Requests.OvertimeRequests.Commands.CreateOvertimeRequest;
using HRSystem.Application.Requests.OvertimeRequests.Common;
using HRSystem.Application.Requests.OvertimeRequests.Queries.GetOvertimeRequestById;
using HRSystem.Application.Requests.OvertimeRequests.Queries.GetOvertimeRequests;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/requests/overtime")]
public class OvertimeRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OvertimeRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get overtime request by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OvertimeRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OvertimeRequestDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetOvertimeRequestByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get overtime requests with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OvertimeRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<OvertimeRequestDto>>> GetAll(
        [FromQuery] bool? mine,
        [FromQuery] int? forEmployeeId,
        [FromQuery] RequestStatus? status)
    {
        var query = new GetOvertimeRequestsQuery
        {
            Mine = mine,
            ForEmployeeId = forEmployeeId,
            Status = status
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new overtime request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OvertimeRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OvertimeRequestDto>> Create([FromBody] CreateOvertimeRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}