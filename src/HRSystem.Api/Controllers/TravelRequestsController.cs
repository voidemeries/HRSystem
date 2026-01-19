using HRSystem.Application.Requests.TravelRequests.Commands.CreateTravelRequest;
using HRSystem.Application.Requests.TravelRequests.Common;
using HRSystem.Application.Requests.TravelRequests.Queries.GetTravelRequestById;
using HRSystem.Application.Requests.TravelRequests.Queries.GetTravelRequests;
using HRSystem.Application.Requests.TravelRequests.Commands.ApproveTravelRequest;
using HRSystem.Application.Requests.TravelRequests.Commands.RejectTravelRequest;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/requests/travel")]
public class TravelRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TravelRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get travel request by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TravelRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TravelRequestDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetTravelRequestByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get travel requests with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<TravelRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<TravelRequestDto>>> GetAll(
        [FromQuery] bool? mine,
        [FromQuery] int? forEmployeeId,
        [FromQuery] RequestStatus? status)
    {
        var query = new GetTravelRequestsQuery
        {
            Mine = mine,
            ForEmployeeId = forEmployeeId,
            Status = status
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new travel request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TravelRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TravelRequestDto>> Create([FromBody] CreateTravelRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    /// <summary>
    /// Approve a travel request
    /// </summary>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(TravelRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TravelRequestDto>> Approve(int id)
    {
        var result = await _mediator.Send(new ApproveTravelRequestCommand { Id = id });
        return Ok(result);
    }

    /// <summary>
    /// Reject a travel request
    /// </summary>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(typeof(TravelRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TravelRequestDto>> Reject(int id, [FromBody] RejectTravelRequestCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}