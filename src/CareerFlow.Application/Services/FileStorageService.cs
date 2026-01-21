using CareerFlow.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _basePath;
    private readonly long _maxFileSizeBytes;
    private readonly string[] _allowedExtensions;

    public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var fileStorageSettings = _configuration.GetSection("FileStorage");
        _basePath = fileStorageSettings["BasePath"] ?? "uploads";
        _maxFileSizeBytes = (fileStorageSettings.GetValue<int>("MaxFileSizeMB") ?? 5) * 1024 * 1024;
        _allowedExtensions = fileStorageSettings.GetSection("AllowedExtensions").Get<string[]>()
            ?? new[] { ".jpg", ".jpeg", ".png", ".pdf" };

        // Garantir que o diretório base existe
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subDirectory, Guid userId)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Arquivo inválido");

        if (!IsValidFile(file, _allowedExtensions, _maxFileSizeBytes))
            throw new InvalidOperationException($"Arquivo inválido. Extensões permitidas: {string.Join(", ", _allowedExtensions)}. Tamanho máximo: {_maxFileSizeBytes / (1024 * 1024)}MB");

        var userDirectory = Path.Combine(_basePath, userId.ToString(), subDirectory);
        if (!Directory.Exists(userDirectory))
        {
            Directory.CreateDirectory(userDirectory);
        }

        var fileName = GenerateUniqueFileName(file.FileName, userId);
        var filePath = Path.Combine(userDirectory, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        _logger.LogInformation("Arquivo salvo: {FilePath} para usuário {UserId}", filePath, userId);
        return filePath;
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return false;

        try
        {
            File.Delete(filePath);
            _logger.LogInformation("Arquivo deletado: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar arquivo: {FilePath}", filePath);
            return false;
        }
    }

    public async Task<byte[]> GetFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");

        return await File.ReadAllBytesAsync(filePath);
    }

    public string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }

    public bool IsValidFile(IFormFile file, string[] allowedExtensions, long maxSizeInBytes)
    {
        if (file == null || file.Length == 0)
            return false;

        if (file.Length > maxSizeInBytes)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }

    public string GenerateUniqueFileName(string originalFileName, Guid userId)
    {
        var extension = Path.GetExtension(originalFileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        var safeFileName = Path.GetInvalidFileNameChars()
            .Aggregate(fileNameWithoutExtension, (current, c) => current.Replace(c, '_'));

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        return $"{safeFileName}_{userId}_{timestamp}{extension}";
    }
}