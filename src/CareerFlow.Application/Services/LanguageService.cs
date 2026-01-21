using AutoMapper;
using CareerFlow.Application.Common;
using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using CareerFlow.Domain.Common;
using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Services;

public class LanguageService : ServiceBase, ILanguageService
{
    public LanguageService(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<LanguageService> logger)
        : base(context, mapper, logger)
    {
    }

    public async Task<IEnumerable<LanguageDto>> GetUserLanguagesAsync(Guid userId)
    {
        var languages = await _context.Languages
            .AsNoTracking()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Level)
            .ThenBy(l => l.Name)
            .ToListAsync();

        return _mapper.Map<IEnumerable<LanguageDto>>(languages);
    }

    public async Task<LanguageDto> GetLanguageByIdAsync(Guid id)
    {
        var language = await _context.Languages
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (language == null)
            throw new KeyNotFoundException($"Idioma com ID {id} não encontrado.");

        return _mapper.Map<LanguageDto>(language);
    }

    public async Task<LanguageDto> CreateLanguageAsync(Guid userId, CreateLanguageDto dto)
    {
        var language = _mapper.Map<Language>(dto);
        language.UserId = userId;

        _context.Languages.Add(language);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Idioma criado: {Name} para usuário {UserId}",
            language.Name, userId);

        return _mapper.Map<LanguageDto>(language);
    }

    public async Task<LanguageDto> UpdateLanguageAsync(Guid id, UpdateLanguageDto dto)
    {
        var language = await _context.Languages.FindAsync(id);

        if (language == null)
            throw new KeyNotFoundException($"Idioma com ID {id} não encontrado.");

        // Atualizar propriedades
        if (!string.IsNullOrEmpty(dto.Name))
            language.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.Level))
        {
            var level = Domain.Enums.LanguageLevel.TryFromName(dto.Level, out var languageLevel)
                ? languageLevel
                : language.Level;
            language.Level = level;
        }

        _context.Languages.Update(language);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Idioma atualizado: {LanguageId}", id);
        return _mapper.Map<LanguageDto>(language);
    }

    public async Task<bool> DeleteLanguageAsync(Guid id)
    {
        var language = await _context.Languages.FindAsync(id);

        if (language == null)
            return false;

        _context.Languages.Remove(language);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Idioma deletado: {LanguageId}", id);
        return true;
    }
}