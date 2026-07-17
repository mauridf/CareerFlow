using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Skills.Commands;
using CareerFlow.Application.Features.Skills.DTOs;
using CareerFlow.Application.Features.Skills.Queries;

namespace CareerFlow.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de habilidades do perfil.
/// </summary>
[ApiController]
[Route("api/v1/profile/skills")]
[Authorize]
public class SkillsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SkillsController> _logger;

    public SkillsController(IMediator mediator, ILogger<SkillsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as habilidades do perfil
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SkillResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkills()
    {
        var result = await _mediator.Send(new GetSkillsQuery());
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    /// <summary>
    /// Lista categorias de habilidades disponíveis
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<SkillCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkillCategories()
    {
        var result = await _mediator.Send(new GetSkillCategoriesQuery());
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    /// <summary>
    /// Adiciona uma nova habilidade
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SkillResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillRequest request)
    {
        var command = new CreateSkillCommand(
            request.Name,
            request.Category,
            request.ProficiencyLevel,
            request.IsPrimary,
            request.DisplayOrder);

        var result = await _mediator.Send(command);

        _logger.LogInformation("✅ Habilidade criada: {SkillName}", request.Name);

        return Created($"/api/v1/profile/skills/{result.Id}", new
        {
            success = true,
            data = result,
            meta = new { timestamp = DateTime.UtcNow }
        });
    }

    /// <summary>
    /// Atualiza uma habilidade existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SkillResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSkill(Guid id, [FromBody] UpdateSkillRequest request)
    {
        var command = new UpdateSkillCommand(
            id,
            request.Name,
            request.Category,
            request.ProficiencyLevel,
            request.IsPrimary,
            request.DisplayOrder);

        var result = await _mediator.Send(command);

        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    /// <summary>
    /// Remove uma habilidade
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSkill(Guid id)
    {
        await _mediator.Send(new DeleteSkillCommand(id));

        return NoContent();
    }

    /// <summary>
    /// Reordena as habilidades
    /// </summary>
    [HttpPost("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReorderSkills([FromBody] ReorderSkillsRequest request)
    {
        await _mediator.Send(new ReorderSkillsCommand(request.Skills));

        return Ok(new
        {
            success = true,
            data = new { message = "Habilidades reordenadas com sucesso" },
            meta = new { timestamp = DateTime.UtcNow }
        });
    }
}
