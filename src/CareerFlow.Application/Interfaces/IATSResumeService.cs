using CareerFlow.Application.DTOs;

namespace CareerFlow.Application.Interfaces;

public interface IATSResumeService
{
    Task<ATSResumeDto> GenerateATSResumeAsync(Guid userId);
    Task<byte[]> GenerateATSResumePdfAsync(Guid userId);
    Task<byte[]> GenerateATSResumeJsonAsync(Guid userId);
    Task<string> GetATSResumeTextAsync(Guid userId);
    Task<List<string>> GetATSKeywordsAsync(Guid userId);
    Task<int> CalculateATSScoreAsync(Guid userId);
}