using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Resume.Queries;

namespace CareerFlow.Api.Controllers;

[ApiController]
[Route("api/v1/resume/shared")]
[AllowAnonymous]
public class SharedResumeController : ControllerBase
{
    private readonly IMediator _mediator;

    public SharedResumeController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetPublicResume(string slug)
    {
        var result = await _mediator.Send(new GetPublicResumeQuery(slug));
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }
}
