using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Resume.Commands;
using CareerFlow.Application.Features.Resume.DTOs;
using CareerFlow.Application.Features.Resume.Queries;

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
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        var result = await _mediator.Send(new GetResumeAnalyticsQuery());
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpPost("share")]
    public async Task<IActionResult> ShareResume()
    {
        var link = await _mediator.Send(new ShareResumeCommand());
        _logger.LogInformation("📤 Currículo compartilhado: {Link}", link);
        return Ok(new { success = true, data = new { shareLink = link }, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpPut("publish")]
    public async Task<IActionResult> Publish()
    {
        await _mediator.Send(new PublishResumeCommand());
        return Ok(new { success = true, data = new { message = "Currículo publicado com sucesso" }, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpPut("unpublish")]
    public async Task<IActionResult> Unpublish()
    {
        await _mediator.Send(new UnpublishResumeCommand());
        return Ok(new { success = true, data = new { message = "Currículo despublicado" }, meta = new { timestamp = DateTime.UtcNow } });
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
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions()
    {
        var result = await _mediator.Send(new GetResumeSuggestionsQuery());
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpPost("suggestions/generate")]
    public async Task<IActionResult> GenerateSuggestions()
    {
        var result = await _mediator.Send(new GenerateResumeSuggestionsCommand());
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }
}
