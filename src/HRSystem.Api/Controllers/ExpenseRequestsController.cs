using HRSystem.Application.Requests.ExpenseRequests.Commands.CreateExpenseRequest;
using HRSystem.Application.Requests.ExpenseRequests.Common;
using HRSystem.Application.Requests.ExpenseRequests.Queries.GetExpenseRequestById;
using HRSystem.Application.Requests.ExpenseRequests.Queries.GetExpenseRequests;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/requests/expense")]
public class ExpenseRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpenseRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get expense request by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExpenseRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ExpenseRequestDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetExpenseRequestByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get expense requests with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ExpenseRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ExpenseRequestDto>>> GetAll(
        [FromQuery] bool? mine,
        [FromQuery] int? forEmployeeId,
        [FromQuery] RequestStatus? status)
    {
        var query = new GetExpenseRequestsQuery
        {
            Mine = mine,
            ForEmployeeId = forEmployeeId,
            Status = status
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new expense request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ExpenseRequestDto>> Create([FromBody] CreateExpenseRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}