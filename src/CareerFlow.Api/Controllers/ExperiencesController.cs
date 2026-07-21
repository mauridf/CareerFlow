using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Experiences.Commands;
using CareerFlow.Application.Features.Experiences.DTOs;
using CareerFlow.Application.Features.Experiences.Queries;
using CareerFlow.Api.Helpers;

namespace CareerFlow.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de experiências profissionais.
/// </summary>
[ApiController]
[Route("api/v1/profile/experiences")]
[Authorize]
public class ExperiencesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ExperiencesController> _logger;

    public ExperiencesController(IMediator mediator, ILogger<ExperiencesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as experiências profissionais do perfil
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExperienceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExperiences()
    {
        var result = await _mediator.Send(new GetExperiencesQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    /// <summary>
    /// Obtém detalhes de uma experiência específica
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExperienceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExperienceDetail(Guid id)
    {
        var result = await _mediator.Send(new GetExperienceDetailQuery(id));
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    /// <summary>
    /// Adiciona uma nova experiência profissional
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExperienceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateExperience([FromBody] CreateExperienceRequest request)
    {
        var command = new CreateExperienceCommand(
            request.CompanyName,
            request.Position,
            request.StartDate,
            request.EndDate,
            request.Description,
            request.SkillsUsed,
            request.City,
            request.State,
            request.Country,
            request.EmploymentType);

        var result = await _mediator.Send(command);

        _logger.LogInformation("✅ Experiência criada: {Company} - {Position}", request.CompanyName, request.Position);

        return ResponseHelper.CreatedResponse($"/api/v1/profile/experiences/{result.Id}", result, HttpContext);
    }

    /// <summary>
    /// Atualiza uma experiência existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ExperienceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExperience(Guid id, [FromBody] UpdateExperienceRequest request)
    {
        var command = new UpdateExperienceCommand(
            id,
            request.CompanyName,
            request.Position,
            request.StartDate,
            request.EndDate,
            request.Description,
            request.SkillsUsed,
            request.City,
            request.State,
            request.EmploymentType);

        var result = await _mediator.Send(command);

        return ResponseHelper.OkResponse(result, HttpContext);
    }

    /// <summary>
    /// Remove uma experiência profissional
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteExperience(Guid id)
    {
        await _mediator.Send(new DeleteExperienceCommand(id));
        return NoContent();
    }
}
