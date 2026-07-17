using MediatR;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Admin.Commands;
using CareerFlow.Application.Features.Admin.DTOs;
using CareerFlow.Application.Features.Admin.Queries;

namespace CareerFlow.Api.Controllers.Admin;

public class AdminUsersController : AdminBaseController
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        var result = await _mediator.Send(new GetUsersListQuery(page, pageSize, search));
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetUserDetail(Guid id)
    {
        var result = await _mediator.Send(new GetUserDetailQuery(id));
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpPut("users/{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] AdminUpdateUserRequest request)
    {
        var result = await _mediator.Send(new AdminUpdateUserCommand(id, request.Name, request.Role));
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpPatch("users/{id:guid}/status")]
    public async Task<IActionResult> ToggleUserStatus(Guid id)
    {
        var result = await _mediator.Send(new AdminToggleUserStatusCommand(id));
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpPatch("users/{id:guid}/premium")]
    public async Task<IActionResult> ManagePremium(Guid id, [FromQuery] bool activate, [FromQuery] DateTime? until = null)
    {
        var result = await _mediator.Send(new AdminManagePremiumCommand(id, activate, until));
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _mediator.Send(new AdminDeleteUserCommand(id));
        return NoContent();
    }
}
