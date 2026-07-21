using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Languages.Commands;
using CareerFlow.Application.Features.Languages.DTOs;
using CareerFlow.Application.Features.Languages.Queries;
using CareerFlow.Api.Helpers;

namespace CareerFlow.Api.Controllers;

[ApiController]
[Route("api/v1/profile/languages")]
[Authorize]
public class LanguagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LanguagesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetLanguages()
    {
        var result = await _mediator.Send(new GetLanguagesQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLanguage([FromBody] CreateLanguageRequest request)
    {
        var cmd = new CreateLanguageCommand(request.LanguageName, request.ProficiencyLevel, request.IsNative);
        var result = await _mediator.Send(cmd);
        return ResponseHelper.CreatedResponse($"/api/v1/profile/languages/{result.Id}", result, HttpContext);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLanguage(Guid id, [FromBody] UpdateLanguageRequest request)
    {
        var cmd = new UpdateLanguageCommand(id, request.LanguageName, request.ProficiencyLevel, request.IsNative);
        var result = await _mediator.Send(cmd);
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLanguage(Guid id)
    {
        await _mediator.Send(new DeleteLanguageCommand(id));
        return NoContent();
    }
}
