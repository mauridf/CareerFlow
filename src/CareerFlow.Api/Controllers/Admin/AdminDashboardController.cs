using MediatR;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Admin.DTOs;
using CareerFlow.Application.Features.Admin.Queries;

namespace CareerFlow.Api.Controllers.Admin;

public class AdminDashboardController : AdminBaseController
{
    private readonly IMediator _mediator;

    public AdminDashboardController(IMediator mediator) => _mediator = mediator;

    [HttpGet("stats/overview")]
    public async Task<IActionResult> GetSystemStats()
    {
        var result = await _mediator.Send(new GetSystemStatsQuery());
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }
}
