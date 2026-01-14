using HRSystem.Application.Permissions.Common;
using HRSystem.Application.Permissions.Queries.GetUserScreensTree;
using HRSystem.Application.Permissions.Queries.SearchUserScreens;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MeController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get authorized screens for current user in hierarchical tree structure
    /// </summary>
    [HttpGet("screens/tree")]
    [ProducesResponseType(typeof(List<UserScreenTreeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserScreenTreeDto>>> GetScreensTree()
    {
        var result = await _mediator.Send(new GetUserScreensTreeQuery());
        return Ok(result);
    }

    /// <summary>
    /// Search authorized screens for current user
    /// </summary>
    [HttpGet("screens/search")]
    [ProducesResponseType(typeof(List<UserScreenSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserScreenSearchResultDto>>> SearchScreens([FromQuery] string q = "")
    {
        var result = await _mediator.Send(new SearchUserScreensQuery { Query = q });
        return Ok(result);
    }
}