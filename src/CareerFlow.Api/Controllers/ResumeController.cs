using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Resume.Commands;
using CareerFlow.Application.Features.Resume.DTOs;
using CareerFlow.Application.Features.Resume.Queries;
using CareerFlow.Api.Helpers;

namespace CareerFlow.Api.Controllers;

[ApiController]
[Route("api/v1/resume")]
[Authorize]
public class ResumeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ResumeController> _logger;

    public ResumeController(IMediator mediator, ILogger<ResumeController> logger)
    {
        _mediator = mediator; _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetResume()
    {
        var result = await _mediator.Send(new GetResumeQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        var result = await _mediator.Send(new GetResumeAnalyticsQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpPost("share")]
    public async Task<IActionResult> ShareResume()
    {
        var link = await _mediator.Send(new ShareResumeCommand());
        _logger.LogInformation("📤 Currículo compartilhado: {Link}", link);
        return ResponseHelper.OkResponse(new { shareLink = link }, HttpContext);
    }

    [HttpPut("publish")]
    public async Task<IActionResult> Publish()
    {
        await _mediator.Send(new PublishResumeCommand());
        return ResponseHelper.MessageResponse("Currículo publicado com sucesso", HttpContext);
    }

    [HttpPut("unpublish")]
    public async Task<IActionResult> Unpublish()
    {
        await _mediator.Send(new UnpublishResumeCommand());
        return ResponseHelper.MessageResponse("Currículo despublicado", HttpContext);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GeneratePdf()
    {
        var pdf = await _mediator.Send(new GenerateResumeCommand());
        return File(pdf, "application/pdf", "curriculo.pdf");
    }

    [HttpPost("generate-ats")]
    public async Task<IActionResult> GenerateAtsPdf()
    {
        var pdf = await _mediator.Send(new GenerateAtsResumeCommand());
        return File(pdf, "application/pdf", "curriculo-ats.pdf");
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze()
    {
        var result = await _mediator.Send(new AnalyzeResumeCommand());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions()
    {
        var result = await _mediator.Send(new GetResumeSuggestionsQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpPost("suggestions/generate")]
    public async Task<IActionResult> GenerateSuggestions()
    {
        var result = await _mediator.Send(new GenerateResumeSuggestionsCommand());
        return ResponseHelper.OkResponse(result, HttpContext);
    }
}
