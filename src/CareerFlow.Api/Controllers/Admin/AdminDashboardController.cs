using MediatR;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Admin.DTOs;
using CareerFlow.Application.Features.Admin.Queries;
using CareerFlow.Api.Helpers;

namespace CareerFlow.Api.Controllers.Admin;

public class AdminDashboardController : AdminBaseController
{
    private readonly IMediator _mediator;

    public AdminDashboardController(IMediator mediator) => _mediator = mediator;

    [HttpGet("stats/overview")]
    public async Task<IActionResult> GetSystemStats()
    {
        var result = await _mediator.Send(new GetSystemStatsQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpGet("stats/users")]
    public async Task<IActionResult> GetUsersStats()
    {
        var result = await _mediator.Send(new GetAdminUsersStatsQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpGet("stats/resumes")]
    public async Task<IActionResult> GetResumesStats()
    {
        var result = await _mediator.Send(new GetAdminResumesStatsQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpGet("stats/views")]
    public async Task<IActionResult> GetViewsStats()
    {
        var result = await _mediator.Send(new GetAdminViewsStatsQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpGet("stats/ats-scores")]
    public async Task<IActionResult> GetAtsScoresStats()
    {
        var result = await _mediator.Send(new GetAdminAtsScoresStatsQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }
}
