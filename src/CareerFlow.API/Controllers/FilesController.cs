using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FilesController : ApiControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileStorageService fileStorageService, ILogger<FilesController> logger)
    {
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<FileUploadResponseDto>> UploadFile(
        [FromForm] FileUploadDto fileUpload,
        [FromQuery] string subDirectory = "general")
    {
        try
        {
            var userId = GetUserId();

            var filePath = await _fileStorageService.SaveFileAsync(
                fileUpload.File,
                subDirectory,
                userId);

            var response = new FileUploadResponseDto
            {
                FilePath = filePath,
                FileName = Path.GetFileName(filePath),
                FileSize = fileUpload.File.Length,
                ContentType = _fileStorageService.GetContentType(fileUpload.File.FileName)
            };

            _logger.LogInformation("Arquivo enviado por usuário {UserId}: {FileName}", userId, response.FileName);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload de arquivo");
            return StatusCode(500, new { message = "Erro interno ao processar o arquivo." });
        }
    }

    [HttpDelete("{*filePath}")]
    public async Task<ActionResult> DeleteFile(string filePath)
    {
        try
        {
            var userId = GetUserId();

            // Verificar se o arquivo pertence ao usuário
            if (!filePath.Contains(userId.ToString()))
                return Forbid();

            var deleted = await _fileStorageService.DeleteFileAsync(filePath);

            return deleted ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar arquivo: {FilePath}", filePath);
            return StatusCode(500, new { message = "Erro interno ao deletar o arquivo." });
        }
    }

    [HttpGet("{*filePath}")]
    public async Task<IActionResult> GetFile(string filePath)
    {
        try
        {
            var userId = GetUserId();

            // Verificar se o arquivo pertence ao usuário
            if (!filePath.Contains(userId.ToString()))
                return Forbid();

            var fileBytes = await _fileStorageService.GetFileAsync(filePath);
            var contentType = _fileStorageService.GetContentType(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, contentType, fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao recuperar arquivo: {FilePath}", filePath);
            return StatusCode(500, new { message = "Erro interno ao recuperar o arquivo." });
        }
    }
}