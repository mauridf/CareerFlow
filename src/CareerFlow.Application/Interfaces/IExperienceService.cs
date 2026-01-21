using CareerFlow.Application.DTOs;

namespace CareerFlow.Application.Interfaces;

public interface IExperienceService
{
    Task<IEnumerable<ProfessionalExperienceDto>> GetUserExperiencesAsync(Guid userId);
    Task<ProfessionalExperienceDto> GetExperienceByIdAsync(Guid id);
    Task<ProfessionalExperienceDto> CreateExperienceAsync(Guid userId, CreateProfessionalExperienceDto dto);
    Task<ProfessionalExperienceDto> UpdateExperienceAsync(Guid id, UpdateProfessionalExperienceDto dto);
    Task<bool> DeleteExperienceAsync(Guid id);
    Task<bool> AddSkillToExperienceAsync(Guid experienceId, Guid skillId);
    Task<bool> RemoveSkillFromExperienceAsync(Guid experienceId, Guid skillId);
}