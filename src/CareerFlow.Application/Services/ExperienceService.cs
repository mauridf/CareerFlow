using AutoMapper;
using CareerFlow.Application.Common;
using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using CareerFlow.Domain.Common;
using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Services;

public class ExperienceService : ServiceBase, IExperienceService
{
    public ExperienceService(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<ExperienceService> logger)
        : base(context, mapper, logger)
    {
    }

    public async Task<IEnumerable<ProfessionalExperienceDto>> GetUserExperiencesAsync(Guid userId)
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

    public async Task<ProfessionalExperienceDto> GetExperienceByIdAsync(Guid id)
    {
        var experience = await _context.ProfessionalExperiences
            .AsNoTracking()
            .Include(pe => pe.SkillExperiences)
                .ThenInclude(se => se.Skill)
            .FirstOrDefaultAsync(pe => pe.Id == id);

        if (experience == null)
            throw new KeyNotFoundException($"Experiência profissional com ID {id} não encontrada.");

        return _mapper.Map<ProfessionalExperienceDto>(experience);
    }

    public async Task<ProfessionalExperienceDto> CreateExperienceAsync(Guid userId, CreateProfessionalExperienceDto dto)
    {
        var experience = _mapper.Map<ProfessionalExperience>(dto);
        experience.UserId = userId;

        _context.ProfessionalExperiences.Add(experience);
        await _context.SaveChangesAsync();

        // Adicionar skills relacionados
        if (dto.SkillIds != null && dto.SkillIds.Any())
        {
            foreach (var skillId in dto.SkillIds)
            {
                var skill = await _context.Skills.FindAsync(skillId);
                if (skill != null && skill.UserId == userId)
                {
                    var skillExperience = new SkillExperience
                    {
                        SkillId = skillId,
                        ProfessionalExperienceId = experience.Id
                    };
                    _context.SkillExperiences.Add(skillExperience);
                }
            }
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Experiência profissional criada: {Company} para usuário {UserId}",
            experience.Company, userId);

        return await GetExperienceByIdAsync(experience.Id);
    }

    public async Task<ProfessionalExperienceDto> UpdateExperienceAsync(Guid id, UpdateProfessionalExperienceDto dto)
    {
        var experience = await _context.ProfessionalExperiences
            .Include(pe => pe.SkillExperiences)
            .FirstOrDefaultAsync(pe => pe.Id == id);

        if (experience == null)
            throw new KeyNotFoundException($"Experiência profissional com ID {id} não encontrada.");

        // Atualizar propriedades
        if (!string.IsNullOrEmpty(dto.Company))
            experience.Company = dto.Company;

        if (!string.IsNullOrEmpty(dto.Position))
            experience.Position = dto.Position;

        if (dto.StartDate.HasValue)
            experience.StartDate = dto.StartDate.Value;

        if (dto.EndDate.HasValue)
            experience.EndDate = dto.EndDate.Value;

        if (!string.IsNullOrEmpty(dto.Responsibilities))
            experience.Responsibilities = dto.Responsibilities;

        if (dto.IsPaid.HasValue)
            experience.IsPaid = dto.IsPaid.Value;

        // Atualizar skills se fornecidos
        if (dto.SkillIds != null)
        {
            // Remover skills existentes
            var existingSkillExperiences = experience.SkillExperiences.ToList();
            _context.SkillExperiences.RemoveRange(existingSkillExperiences);

            // Adicionar novos skills
            foreach (var skillId in dto.SkillIds)
            {
                var skill = await _context.Skills.FindAsync(skillId);
                if (skill != null && skill.UserId == experience.UserId)
                {
                    var skillExperience = new SkillExperience
                    {
                        SkillId = skillId,
                        ProfessionalExperienceId = experience.Id
                    };
                    _context.SkillExperiences.Add(skillExperience);
                }
            }
        }

        _context.ProfessionalExperiences.Update(experience);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Experiência profissional atualizada: {ExperienceId}", id);
        return await GetExperienceByIdAsync(experience.Id);
    }

    public async Task<bool> DeleteExperienceAsync(Guid id)
    {
        var experience = await _context.ProfessionalExperiences
            .Include(pe => pe.SkillExperiences)
            .FirstOrDefaultAsync(pe => pe.Id == id);

        if (experience == null)
            return false;

        _context.SkillExperiences.RemoveRange(experience.SkillExperiences);
        _context.ProfessionalExperiences.Remove(experience);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Experiência profissional deletada: {ExperienceId}", id);
        return true;
    }

    public async Task<bool> AddSkillToExperienceAsync(Guid experienceId, Guid skillId)
    {
        var experience = await _context.ProfessionalExperiences.FindAsync(experienceId);
        var skill = await _context.Skills.FindAsync(skillId);

        if (experience == null || skill == null || skill.UserId != experience.UserId)
            return false;

        // Verificar se já existe
        var existing = await _context.SkillExperiences
            .FirstOrDefaultAsync(se => se.SkillId == skillId && se.ProfessionalExperienceId == experienceId);

        if (existing != null)
            return true; // Já existe, considerar sucesso

        var skillExperience = new SkillExperience
        {
            SkillId = skillId,
            ProfessionalExperienceId = experienceId
        };

        _context.SkillExperiences.Add(skillExperience);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveSkillFromExperienceAsync(Guid experienceId, Guid skillId)
    {
        var skillExperience = await _context.SkillExperiences
            .FirstOrDefaultAsync(se => se.SkillId == skillId && se.ProfessionalExperienceId == experienceId);

        if (skillExperience == null)
            return false;

        _context.SkillExperiences.Remove(skillExperience);
        await _context.SaveChangesAsync();

        return true;
    }
}