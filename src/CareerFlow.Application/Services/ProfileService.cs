using AutoMapper;
using CareerFlow.Application.Common;
using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using CareerFlow.Domain.Common;
using CareerFlow.Domain.Entities;
using CareerFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Services;

public class ProfileService : ServiceBase, IProfileService
{
    public ProfileService(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<ProfileService> logger)
        : base(context, mapper, logger)
    {
    }

    public async Task<ProfessionalSummaryDto?> GetSummaryAsync(Guid userId)
    {
        var summary = await _context.ProfessionalSummaries
            .AsNoTracking()
            .FirstOrDefaultAsync(ps => ps.UserId == userId);

        return summary == null ? null : _mapper.Map<ProfessionalSummaryDto>(summary);
    }

    public async Task<ProfessionalSummaryDto> CreateOrUpdateSummaryAsync(Guid userId, CreateProfessionalSummaryDto dto)
    {
        var existingSummary = await _context.ProfessionalSummaries
            .FirstOrDefaultAsync(ps => ps.UserId == userId);

        if (existingSummary != null)
        {
            existingSummary.Summary = dto.Summary;
            _context.ProfessionalSummaries.Update(existingSummary);
        }
        else
        {
            var summary = _mapper.Map<ProfessionalSummary>(dto);
            summary.UserId = userId;
            _context.ProfessionalSummaries.Add(summary);
        }

        await _context.SaveChangesAsync();

        var result = await GetSummaryAsync(userId);
        return result!;
    }

    public async Task<bool> DeleteSummaryAsync(Guid userId)
    {
        var summary = await _context.ProfessionalSummaries
            .FirstOrDefaultAsync(ps => ps.UserId == userId);

        if (summary == null)
            return false;

        _context.ProfessionalSummaries.Remove(summary);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SocialMediaDto>> GetSocialMediasAsync(Guid userId)
    {
        var socialMedias = await _context.SocialMedias
            .AsNoTracking()
            .Where(sm => sm.UserId == userId)
            .OrderBy(sm => sm.Platform)
            .ToListAsync();

        return _mapper.Map<IEnumerable<SocialMediaDto>>(socialMedias);
    }

    public async Task<SocialMediaDto> AddSocialMediaAsync(Guid userId, CreateSocialMediaDto dto)
    {
        var socialMedia = _mapper.Map<SocialMedia>(dto);
        socialMedia.UserId = userId;

        _context.SocialMedias.Add(socialMedia);
        await _context.SaveChangesAsync();

        return _mapper.Map<SocialMediaDto>(socialMedia);
    }

    public async Task<bool> RemoveSocialMediaAsync(Guid userId, Guid socialMediaId)
    {
        var socialMedia = await _context.SocialMedias
            .FirstOrDefaultAsync(sm => sm.Id == socialMediaId && sm.UserId == userId);

        if (socialMedia == null)
            return false;

        _context.SocialMedias.Remove(socialMedia);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(Guid userId)
    {
        var stats = new DashboardStatsDto();

        // Contagens básicas
        stats.TotalSkills = await _context.Skills
            .CountAsync(s => s.UserId == userId);

        stats.TotalExperiences = await _context.ProfessionalExperiences
            .CountAsync(pe => pe.UserId == userId);

        stats.TotalCertificates = await _context.Certificates
            .CountAsync(c => c.UserId == userId);

        stats.TotalLanguages = await _context.Languages
            .CountAsync(l => l.UserId == userId);

        // Distribuição de skills
        var skillGroups = await _context.Skills
            .Where(s => s.UserId == userId)
            .GroupBy(s => s.Type.Name)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        var totalSkills = stats.TotalSkills > 0 ? stats.TotalSkills : 1;

        stats.SkillDistribution = skillGroups.Select(g => new SkillDistributionDto
        {
            Type = g.Type,
            Count = g.Count,
            Percentage = (int)Math.Round((g.Count * 100.0) / totalSkills)
        }).ToList();

        // Próximas expirações
        var upcomingCertificates = await _context.Certificates
            .Where(c => c.UserId == userId && c.EndDate.HasValue && c.EndDate > DateTime.UtcNow)
            .OrderBy(c => c.EndDate)
            .Take(5)
            .ToListAsync();

        stats.UpcomingExpirations = upcomingCertificates.Select(c => new UpcomingExpirationDto
        {
            Name = c.Name,
            Type = "certificate",
            ExpirationDate = c.EndDate,
            DaysUntilExpiration = c.EndDate.HasValue ? (c.EndDate.Value - DateTime.UtcNow).Days : 0
        }).ToList();

        // Calcular completude do perfil (simplificado)
        var profileCompleteness = CalculateProfileCompleteness(userId);
        stats.ProfileCompleteness = profileCompleteness;

        return stats;
    }

    private async Task<int> CalculateProfileCompleteness(Guid userId)
    {
        var completeness = 0;
        var totalFields = 10; // Número total de campos considerados

        // Verificar campos obrigatórios
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            if (!string.IsNullOrEmpty(user.Name)) completeness++;
            if (!string.IsNullOrEmpty(user.Email)) completeness++;
            if (!string.IsNullOrEmpty(user.City)) completeness++;
            if (!string.IsNullOrEmpty(user.State)) completeness++;
        }

        // Verificar se tem resumo
        var hasSummary = await _context.ProfessionalSummaries
            .AnyAsync(ps => ps.UserId == userId);
        if (hasSummary) completeness++;

        // Verificar se tem skills
        var hasSkills = await _context.Skills
            .AnyAsync(s => s.UserId == userId);
        if (hasSkills) completeness++;

        // Verificar se tem experiências
        var hasExperiences = await _context.ProfessionalExperiences
            .AnyAsync(pe => pe.UserId == userId);
        if (hasExperiences) completeness++;

        // Verificar se tem formação
        var hasAcademics = await _context.AcademicBackgrounds
            .AnyAsync(ab => ab.UserId == userId);
        if (hasAcademics) completeness++;

        // Verificar se tem idiomas
        var hasLanguages = await _context.Languages
            .AnyAsync(l => l.UserId == userId);
        if (hasLanguages) completeness++;

        return (int)Math.Round((completeness * 100.0) / totalFields);
    }

    public async Task<ResumeDataDto> GetResumeDataAsync(Guid userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException($"Usuário com ID {userId} não encontrado.");

        var resumeData = new ResumeDataDto
        {
            User = _mapper.Map<UserDto>(user),
            Summary = await GetSummaryAsync(userId),
            SocialMedias = (await GetSocialMediasAsync(userId)).ToList(),
            Skills = (await GetUserSkillsAsync(userId)).ToList(),
            Experiences = (await GetUserExperiencesAsync(userId)).ToList(),
            Academics = (await GetUserAcademicsAsync(userId)).ToList(),
            Certificates = (await GetUserCertificatesAsync(userId)).ToList(),
            Languages = (await GetUserLanguagesAsync(userId)).ToList()
        };

        return resumeData;
    }

    private async Task<IEnumerable<SkillDto>> GetUserSkillsAsync(Guid userId)
    {
        var skills = await _context.Skills
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.Level)
            .ThenBy(s => s.Name)
            .ToListAsync();

        return _mapper.Map<IEnumerable<SkillDto>>(skills);
    }

    private async Task<IEnumerable<ProfessionalExperienceDto>> GetUserExperiencesAsync(Guid userId)
    {
        var experiences = await _context.ProfessionalExperiences
            .AsNoTracking()
            .Include(pe => pe.SkillExperiences)
                .ThenInclude(se => se.Skill)
            .Where(pe => pe.UserId == userId)
            .OrderByDescending(pe => pe.StartDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProfessionalExperienceDto>>(experiences);
    }

    private async Task<IEnumerable<AcademicBackgroundDto>> GetUserAcademicsAsync(Guid userId)
    {
        var academics = await _context.AcademicBackgrounds
            .AsNoTracking()
            .Where(ab => ab.UserId == userId)
            .OrderByDescending(ab => ab.StartDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<AcademicBackgroundDto>>(academics);
    }

    private async Task<IEnumerable<CertificateDto>> GetUserCertificatesAsync(Guid userId)
    {
        var certificates = await _context.Certificates
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CertificateDto>>(certificates);
    }

    private async Task<IEnumerable<LanguageDto>> GetUserLanguagesAsync(Guid userId)
    {
        var languages = await _context.Languages
            .AsNoTracking()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Level)
            .ThenBy(l => l.Name)
            .ToListAsync();

        return _mapper.Map<IEnumerable<LanguageDto>>(languages);
    }
}