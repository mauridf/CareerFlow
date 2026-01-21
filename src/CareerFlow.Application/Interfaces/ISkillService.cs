using CareerFlow.Application.DTOs;

namespace CareerFlow.Application.Interfaces;

public interface ISkillService
{
    Task<IEnumerable<SkillDto>> GetUserSkillsAsync(Guid userId, SkillFilterDto? filter = null);
    Task<SkillDto> GetSkillByIdAsync(Guid id);
    Task<SkillDto> CreateSkillAsync(Guid userId, CreateSkillDto createSkillDto);
    Task<SkillDto> UpdateSkillAsync(Guid id, UpdateSkillDto updateSkillDto);
    Task<bool> DeleteSkillAsync(Guid id);
    Task<IEnumerable<SkillDistributionDto>> GetSkillDistributionAsync(Guid userId);
}