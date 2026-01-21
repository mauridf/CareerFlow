using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ATSController : ApiControllerBase
{
    private readonly IATSResumeService _atsResumeService;
    private readonly ILogger<ATSController> _logger;

    public ATSController(IATSResumeService atsResumeService, ILogger<ATSController> logger)
    {
        _atsResumeService = atsResumeService;
        _logger = logger;
    }

    [HttpGet("resume")]
    public async Task<ActionResult<ATSResumeDto>> GetATSResume()
    {
        try
        {
            var userId = GetUserId();
            var atsResume = await _atsResumeService.GenerateATSResumeAsync(userId);
            return Ok(atsResume);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar currículo ATS");
            return StatusCode(500, new { message = "Erro interno ao gerar currículo ATS." });
        }
    }

    [HttpGet("resume/pdf")]
    public async Task<IActionResult> GetATSResumePdf()
    {
        try
        {
            var userId = GetUserId();
            var pdfBytes = await _atsResumeService.GenerateATSResumePdfAsync(userId);

            return File(pdfBytes, "application/pdf", $"Curriculo_ATS_{userId}_{DateTime.Now:yyyyMMdd}.txt");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar PDF do currículo ATS");
            return StatusCode(500, new { message = "Erro interno ao gerar PDF do currículo ATS." });
        }
    }

    [HttpGet("resume/json")]
    public async Task<IActionResult> GetATSResumeJson()
    {
        try
        {
            var userId = GetUserId();
            var jsonBytes = await _atsResumeService.GenerateATSResumeJsonAsync(userId);

            return File(jsonBytes, "application/json", $"Curriculo_ATS_{userId}_{DateTime.Now:yyyyMMdd}.json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar JSON do currículo ATS");
            return StatusCode(500, new { message = "Erro interno ao gerar JSON do currículo ATS." });
        }
    }

    [HttpGet("resume/text")]
    public async Task<ActionResult<string>> GetATSResumeText()
    {
        try
        {
            var userId = GetUserId();
            var resumeText = await _atsResumeService.GetATSResumeTextAsync(userId);
            return Ok(resumeText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar texto do currículo ATS");
            return StatusCode(500, new { message = "Erro interno ao gerar texto do currículo ATS." });
        }
    }

    [HttpGet("keywords")]
    public async Task<ActionResult<List<string>>> GetATSKeywords()
    {
        try
        {
            var userId = GetUserId();
            var keywords = await _atsResumeService.GetATSKeywordsAsync(userId);
            return Ok(keywords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter keywords ATS");
            return StatusCode(500, new { message = "Erro interno ao obter keywords ATS." });
        }
    }

    [HttpGet("score")]
    public async Task<ActionResult<int>> GetATSScore()
    {
        try
        {
            var userId = GetUserId();
            var score = await _atsResumeService.CalculateATSScoreAsync(userId);
            return Ok(score);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular score ATS");
            return StatusCode(500, new { message = "Erro interno ao calcular score ATS." });
        }
    }
}