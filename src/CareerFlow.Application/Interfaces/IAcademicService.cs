using CareerFlow.Application.DTOs;

namespace CareerFlow.Application.Interfaces;

public interface IAcademicService
{
    Task<IEnumerable<AcademicBackgroundDto>> GetUserAcademicsAsync(Guid userId);
    Task<AcademicBackgroundDto> GetAcademicByIdAsync(Guid id);
    Task<AcademicBackgroundDto> CreateAcademicAsync(Guid userId, CreateAcademicBackgroundDto dto);
    Task<AcademicBackgroundDto> UpdateAcademicAsync(Guid id, UpdateAcademicBackgroundDto dto);
    Task<bool> DeleteAcademicAsync(Guid id);
}