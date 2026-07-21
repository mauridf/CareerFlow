using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Education.Commands;
using CareerFlow.Application.Features.Education.DTOs;
using CareerFlow.Application.Features.Education.Queries;
using CareerFlow.Api.Helpers;

namespace CareerFlow.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de formação acadêmica.
/// </summary>
[ApiController]
[Route("api/v1/profile/education")]
[Authorize]
public class EducationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EducationController> _logger;

    public EducationController(IMediator mediator, ILogger<EducationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as formações acadêmicas do perfil
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EducationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEducation()
    {
        var result = await _mediator.Send(new GetEducationQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    /// <summary>
    /// Adiciona uma nova formação acadêmica
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EducationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEducation([FromBody] CreateEducationRequest request)
    {
        var command = new CreateEducationCommand(
            request.Institution,
            request.Course,
            request.EducationLevel,
            request.StartDate,
            request.EndDate,
            request.Status,
            request.Description,
            request.Grade,
            request.ThesisTitle);

        var result = await _mediator.Send(command);

        _logger.LogInformation("✅ Formação criada: {Institution} - {Course}", request.Institution, request.Course);

        return ResponseHelper.CreatedResponse($"/api/v1/profile/education/{result.Id}", result, HttpContext);
    }

    /// <summary>
    /// Atualiza uma formação acadêmica
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EducationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEducation(Guid id, [FromBody] UpdateEducationRequest request)
    {
        var command = new UpdateEducationCommand(
            id,
            request.Institution,
            request.Course,
            request.EducationLevel,
            request.StartDate,
            request.EndDate,
            request.Status,
            request.Description,
            request.Grade,
            request.ThesisTitle);

        var result = await _mediator.Send(command);

        return ResponseHelper.OkResponse(result, HttpContext);
    }

    /// <summary>
    /// Remove uma formação acadêmica
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEducation(Guid id)
    {
        await _mediator.Send(new DeleteEducationCommand(id));
        return NoContent();
    }
}
