using HRSystem.Application.Requests.LeaveRequests.Commands.ApproveLeaveRequest;
using HRSystem.Application.Requests.LeaveRequests.Commands.CreateLeaveRequest;
using HRSystem.Application.Requests.LeaveRequests.Commands.RejectLeaveRequest;
using HRSystem.Application.Requests.LeaveRequests.Common;
using HRSystem.Application.Requests.LeaveRequests.Queries.GetLeaveRequestById;
using HRSystem.Application.Requests.LeaveRequests.Queries.GetLeaveRequests;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/requests/leave")]
public class LeaveRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get leave request by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LeaveRequestDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetLeaveRequestByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get leave requests with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<LeaveRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetAll(
        [FromQuery] bool? mine,
        [FromQuery] int? forEmployeeId,
        [FromQuery] RequestStatus? status)
    {
        var query = new GetLeaveRequestsQuery
        {
            Mine = mine,
            ForEmployeeId = forEmployeeId,
            Status = status
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new leave request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LeaveRequestDto>> Create([FromBody] CreateLeaveRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Approve a leave request
    /// </summary>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LeaveRequestDto>> Approve(int id)
    {
        var result = await _mediator.Send(new ApproveLeaveRequestCommand { Id = id });
        return Ok(result);
    }

    /// <summary>
    /// Reject a leave request
    /// </summary>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LeaveRequestDto>> Reject(int id, [FromBody] RejectLeaveRequestCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}