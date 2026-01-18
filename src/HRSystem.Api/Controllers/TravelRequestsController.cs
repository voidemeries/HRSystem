using HRSystem.Application.Requests.TravelRequests.Commands.CreateTravelRequest;
using HRSystem.Application.Requests.TravelRequests.Queries.GetTravelRequestById;
using HRSystem.Application.Requests.TravelRequests.Queries.GetTravelRequests;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<TravelRequestDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetTravelRequestByIdQuery(id));
        return Ok(result);
    }

    [HttpGet]
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

    [HttpPost]
    public async Task<ActionResult<TravelRequestDto>> Create([FromBody] CreateTravelRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
```

### File: src/HRSystem.Api/Controllers/ExpenseRequestsController.cs
```csharp
using HRSystem.Application.Requests.Common;
using HRSystem.Application.Requests.ExpenseRequests.Commands.CreateExpenseRequest;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseRequestDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetExpenseRequestByIdQuery(id));
        return Ok(result);
    }

    [HttpGet]
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

    [HttpPost]
    public async Task<ActionResult<ExpenseRequestDto>> Create([FromBody] CreateExpenseRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}