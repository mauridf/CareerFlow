using CareerFlow.Core.Interfaces;
using CareerFlow.Core.Interfaces.Settings;
using Microsoft.Extensions.Options;

namespace CareerFlow.Infrastructure.External.Storage;

public class LocalStorageService : IStorageService
{
    private readonly StorageSettings _settings;

    public LocalStorageService(IOptions<StorageSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<string> UploadAsync(
        string fileName,
        Stream content,
        string contentType,
        string? folder = null,
        CancellationToken cancellationToken = default)
    {
        var subDir = string.IsNullOrWhiteSpace(folder) ? "" : folder;
        var uploadDir = Path.Combine(_settings.LocalPath, subDir);
        Directory.CreateDirectory(uploadDir);

        var uniqueName = GenerateUniqueFileName(fileName);
        var filePath = Path.Combine(uploadDir, uniqueName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await content.CopyToAsync(stream, cancellationToken);
        }

        return $"/{_settings.LocalPath}/{subDir}/{uniqueName}".Replace("\\", "/");
    }

    public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath.TrimStart('/'));
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath.TrimStart('/'));
        return Task.FromResult(File.Exists(fullPath));
    }

    public Task<string> GetPublicUrlAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(filePath);
    }

    public string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        return $"{Guid.NewGuid()}{extension}";
    }
}
