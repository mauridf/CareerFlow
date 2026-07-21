using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Dashboard.DTOs;
using CareerFlow.Application.Features.Dashboard.Queries;
using CareerFlow.Api.Helpers;

namespace CareerFlow.Api.Controllers;

/// <summary>
/// Controller para dashboard e estatísticas do usuário.
/// </summary>
[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Estatísticas gerais do perfil
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardStatsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var result = await _mediator.Send(new GetDashboardStatsQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    /// <summary>
    /// Insights e recomendações do currículo
    /// </summary>
    [HttpGet("insights")]
    [ProducesResponseType(typeof(ResumeInsightsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInsights()
    {
        var result = await _mediator.Send(new GetResumeInsightsQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    /// <summary>
    /// Atividades recentes do usuário
    /// </summary>
    [HttpGet("activity")]
    [ProducesResponseType(typeof(IReadOnlyList<ActivityResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentActivity([FromQuery] int limit = 10)
    {
        var result = await _mediator.Send(new GetRecentActivityQuery(limit));
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    /// <summary>
    /// Gráfico de visualizações por período
    /// </summary>
    [HttpGet("views-chart")]
    [ProducesResponseType(typeof(ViewsChartResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetViewsChart([FromQuery] int days = 30)
    {
        var result = await _mediator.Send(new GetViewsChartQuery(days));
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    /// <summary>
    /// Análise de gap de habilidades
    /// </summary>
    [HttpGet("skills-gap")]
    [ProducesResponseType(typeof(SkillsGapResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkillsGap()
    {
        var result = await _mediator.Send(new GetSkillsGapQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }
}
