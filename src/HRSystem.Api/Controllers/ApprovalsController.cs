using HRSystem.Application.Requests.Common;
using HRSystem.Application.Requests.Queries.GetMyApprovals;
using HRSystem.Application.Requests.Queries.GetPendingApprovals;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApprovalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all pending approvals for the current user
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(List<PendingApprovalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PendingApprovalDto>>> GetPendingApprovals()
    {
        var result = await _mediator.Send(new GetPendingApprovalsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Get approval history for the current user
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(List<PendingApprovalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PendingApprovalDto>>> GetApprovalHistory(
        [FromQuery] RequestStatus? status)
    {
        var query = new GetMyApprovalsQuery { Status = status };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}