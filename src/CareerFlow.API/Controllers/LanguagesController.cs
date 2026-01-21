using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LanguagesController : ApiControllerBase
{
    private readonly ILanguageService _languageService;

    public LanguagesController(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LanguageDto>>> GetLanguages()
    {
        var userId = GetUserId();
        var languages = await _languageService.GetUserLanguagesAsync(userId);
        return Ok(languages);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LanguageDto>> GetLanguage(Guid id)
    {
        var language = await _languageService.GetLanguageByIdAsync(id);

        if (language.UserId != GetUserId())
            return Forbid();

        return Ok(language);
    }

    [HttpPost]
    public async Task<ActionResult<LanguageDto>> CreateLanguage(CreateLanguageDto dto)
    {
        var userId = GetUserId();
        var language = await _languageService.CreateLanguageAsync(userId, dto);
        return CreatedAtAction(nameof(GetLanguage), new { id = language.Id }, language);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LanguageDto>> UpdateLanguage(Guid id, UpdateLanguageDto dto)
    {
        var language = await _languageService.GetLanguageByIdAsync(id);

        if (language.UserId != GetUserId())
            return Forbid();

        var updatedLanguage = await _languageService.UpdateLanguageAsync(id, dto);
        return Ok(updatedLanguage);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteLanguage(Guid id)
    {
        var language = await _languageService.GetLanguageByIdAsync(id);

        if (language.UserId != GetUserId())
            return Forbid();

        var deleted = await _languageService.DeleteLanguageAsync(id);

        return deleted ? NoContent() : NotFound();
    }
}