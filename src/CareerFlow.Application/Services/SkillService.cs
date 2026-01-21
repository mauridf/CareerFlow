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

public class SkillService : ServiceBase, ISkillService
{
    public SkillService(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<SkillService> logger)
        : base(context, mapper, logger)
    {
    }

    public async Task<IEnumerable<SkillDto>> GetUserSkillsAsync(Guid userId, SkillFilterDto? filter = null)
    {
        var query = _context.Skills
            .AsNoTracking()
            .Where(s => s.UserId == userId);

        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.Type))
                query = query.Where(s => s.Type.Name == filter.Type);

            if (!string.IsNullOrEmpty(filter.Level))
                query = query.Where(s => s.Level.Name == filter.Level);
        }

        var skills = await query
            .OrderByDescending(s => s.Level)
            .ThenBy(s => s.Name)
            .ToListAsync();

        return _mapper.Map<IEnumerable<SkillDto>>(skills);
    }

    public async Task<SkillDto> GetSkillByIdAsync(Guid id)
    {
        var skill = await _context.Skills
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (skill == null)
            throw new KeyNotFoundException($"Habilidade com ID {id} não encontrada.");

        return _mapper.Map<SkillDto>(skill);
    }

    public async Task<SkillDto> CreateSkillAsync(Guid userId, CreateSkillDto createSkillDto)
    {
        var skill = _mapper.Map<Skill>(createSkillDto);
        skill.UserId = userId;

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Habilidade criada: {SkillName} para usuário {UserId}", skill.Name, userId);
        return _mapper.Map<SkillDto>(skill);
    }

    public async Task<SkillDto> UpdateSkillAsync(Guid id, UpdateSkillDto updateSkillDto)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill == null)
            throw new KeyNotFoundException($"Habilidade com ID {id} não encontrada.");

        // Atualizar apenas propriedades fornecidas
        if (!string.IsNullOrEmpty(updateSkillDto.Name))
            skill.Name = updateSkillDto.Name;

        if (!string.IsNullOrEmpty(updateSkillDto.Type))
            skill.Type = SkillType.TryFromName(updateSkillDto.Type, out var type) ? type : skill.Type;

        if (!string.IsNullOrEmpty(updateSkillDto.Level))
            skill.Level = SkillLevel.TryFromName(updateSkillDto.Level, out var level) ? level : skill.Level;

        _context.Skills.Update(skill);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Habilidade atualizada: {SkillId}", id);
        return _mapper.Map<SkillDto>(skill);
    }

    public async Task<bool> DeleteSkillAsync(Guid id)
    {
        var skill = await _context.Skills
            .Include(s => s.SkillExperiences)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (skill == null)
            return false;

        // Remover relacionamentos com experiências primeiro
        _context.SkillExperiences.RemoveRange(skill.SkillExperiences);

        // Remover skill
        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Habilidade deletada: {SkillId}", id);
        return true;
    }

    public async Task<IEnumerable<SkillDistributionDto>> GetSkillDistributionAsync(Guid userId)
    {
        var distribution = await _context.Skills
            .Where(s => s.UserId == userId)
            .GroupBy(s => s.Type.Name)
            .Select(g => new SkillDistributionDto
            {
                Type = g.Key,
                Count = g.Count(),
                Percentage = 0 // Será calculado depois
            })
            .OrderByDescending(sd => sd.Count)
            .ToListAsync();

        var total = distribution.Sum(sd => sd.Count);
        if (total > 0)
        {
            foreach (var item in distribution)
            {
                item.Percentage = (int)Math.Round((item.Count * 100.0) / total);
            }
        }

        return distribution;
    }
}