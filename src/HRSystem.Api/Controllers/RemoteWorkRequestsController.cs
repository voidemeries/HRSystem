using HRSystem.Application.Requests.RemoteWorkRequests.Commands.CreateRemoteWorkRequest;
using HRSystem.Application.Requests.RemoteWorkRequests.Common;
using HRSystem.Application.Requests.RemoteWorkRequests.Queries.GetRemoteWorkRequestById;
using HRSystem.Application.Requests.RemoteWorkRequests.Queries.GetRemoteWorkRequests;
using HRSystem.Application.Requests.RemoteWorkRequests.Commands.ApproveRemoteWorkRequest;
using HRSystem.Application.Requests.RemoteWorkRequests.Commands.RejectRemoteWorkRequest;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/requests/remotework")]
public class RemoteWorkRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RemoteWorkRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RemoteWorkRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RemoteWorkRequestDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetRemoteWorkRequestByIdQuery(id));
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RemoteWorkRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<RemoteWorkRequestDto>>> GetAll(
        [FromQuery] bool? mine,
        [FromQuery] int? forEmployeeId,
        [FromQuery] RequestStatus? status)
    {
        var query = new GetRemoteWorkRequestsQuery
        {
            Mine = mine,
            ForEmployeeId = forEmployeeId,
            Status = status
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RemoteWorkRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RemoteWorkRequestDto>> Create([FromBody] CreateRemoteWorkRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// Approve a remote work request
    /// </summary>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(RemoteWorkRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RemoteWorkRequestDto>> Approve(int id)
    {
        var result = await _mediator.Send(new ApproveRemoteWorkRequestCommand { Id = id });
        return Ok(result);
    }

    /// <summary>
    /// Reject a remote work request
    /// </summary>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(typeof(RemoteWorkRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RemoteWorkRequestDto>> Reject(int id, [FromBody] RejectRemoteWorkRequestCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}