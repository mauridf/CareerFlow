using CareerFlow.Application.DTOs;

namespace CareerFlow.Application.Interfaces;

public interface ILanguageService
{
    Task<IEnumerable<LanguageDto>> GetUserLanguagesAsync(Guid userId);
    Task<LanguageDto> GetLanguageByIdAsync(Guid id);
    Task<LanguageDto> CreateLanguageAsync(Guid userId, CreateLanguageDto dto);
    Task<LanguageDto> UpdateLanguageAsync(Guid id, UpdateLanguageDto dto);
    Task<bool> DeleteLanguageAsync(Guid id);
}