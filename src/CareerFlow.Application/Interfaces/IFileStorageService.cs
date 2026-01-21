using Microsoft.AspNetCore.Http;

namespace CareerFlow.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string subDirectory, Guid userId);
    Task<bool> DeleteFileAsync(string filePath);
    Task<byte[]> GetFileAsync(string filePath);
    string GetContentType(string fileName);
    bool IsValidFile(IFormFile file, string[] allowedExtensions, long maxSizeInBytes);
    string GenerateUniqueFileName(string originalFileName, Guid userId);
}