using CareerFlow.Application.DTOs;

namespace CareerFlow.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> GetByEmailAsync(string email);
    Task<UserDto> CreateAsync(CreateUserDto createUserDto);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto updateUserDto);
    Task DeleteAsync(Guid id);
    Task<AuthResponseDto> AuthenticateAsync(LoginDto loginDto);
    Task<bool> EmailExistsAsync(string email);
}

public interface IProfileService
{
    Task<ProfessionalSummaryDto?> GetSummaryAsync(Guid userId);
    Task<ProfessionalSummaryDto> CreateOrUpdateSummaryAsync(Guid userId, CreateProfessionalSummaryDto dto);
    Task<bool> DeleteSummaryAsync(Guid userId);

    Task<IEnumerable<SocialMediaDto>> GetSocialMediasAsync(Guid userId);
    Task<SocialMediaDto> AddSocialMediaAsync(Guid userId, CreateSocialMediaDto dto);
    Task<bool> RemoveSocialMediaAsync(Guid userId, Guid socialMediaId);

    Task<DashboardStatsDto> GetDashboardStatsAsync(Guid userId);
    Task<ResumeDataDto> GetResumeDataAsync(Guid userId);
}