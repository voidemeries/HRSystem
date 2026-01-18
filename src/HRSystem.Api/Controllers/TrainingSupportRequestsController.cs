using HRSystem.Application.Requests.TrainingSupportRequests.Commands.CreateTrainingSupportRequest;
using HRSystem.Application.Requests.TrainingSupportRequests.Common;
using HRSystem.Application.Requests.TrainingSupportRequests.Queries.GetTrainingSupportRequestById;
using HRSystem.Application.Requests.TrainingSupportRequests.Queries.GetTrainingSupportRequests;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/requests/trainingsupport")]
public class TrainingSupportRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TrainingSupportRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get training support request by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TrainingSupportRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TrainingSupportRequestDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetTrainingSupportRequestByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get training support requests with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<TrainingSupportRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<TrainingSupportRequestDto>>> GetAll(
        [FromQuery] bool? mine,
        [FromQuery] int? forEmployeeId,
        [FromQuery] RequestStatus? status)
    {
        var query = new GetTrainingSupportRequestsQuery
        {
            Mine = mine,
            ForEmployeeId = forEmployeeId,
            Status = status
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new training support request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TrainingSupportRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TrainingSupportRequestDto>> Create([FromBody] CreateTrainingSupportRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}