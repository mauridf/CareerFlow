using AutoMapper;
using CareerFlow.Application.Common;
using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using CareerFlow.Domain.Common;
using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Services;

public class AcademicService : ServiceBase, IAcademicService
{
    public AcademicService(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<AcademicService> logger)
        : base(context, mapper, logger)
    {
    }

    public async Task<IEnumerable<AcademicBackgroundDto>> GetUserAcademicsAsync(Guid userId)
    {
        var academics = await _context.AcademicBackgrounds
            .AsNoTracking()
            .Where(ab => ab.UserId == userId)
            .OrderByDescending(ab => ab.StartDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<AcademicBackgroundDto>>(academics);
    }

    public async Task<AcademicBackgroundDto> GetAcademicByIdAsync(Guid id)
    {
        var academic = await _context.AcademicBackgrounds
            .AsNoTracking()
            .FirstOrDefaultAsync(ab => ab.Id == id);

        if (academic == null)
            throw new KeyNotFoundException($"Formação acadêmica com ID {id} não encontrada.");

        return _mapper.Map<AcademicBackgroundDto>(academic);
    }

    public async Task<AcademicBackgroundDto> CreateAcademicAsync(Guid userId, CreateAcademicBackgroundDto dto)
    {
        var academic = _mapper.Map<AcademicBackground>(dto);
        academic.UserId = userId;

        // TODO: Lidar com upload de diploma
        // if (dto.Diploma != null)
        // {
        //     academic.DiplomaPath = await SaveFileAsync(dto.Diploma);
        // }

        _context.AcademicBackgrounds.Add(academic);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Formação acadêmica criada: {Institution} para usuário {UserId}",
            academic.Institution, userId);

        return _mapper.Map<AcademicBackgroundDto>(academic);
    }

    public async Task<AcademicBackgroundDto> UpdateAcademicAsync(Guid id, UpdateAcademicBackgroundDto dto)
    {
        var academic = await _context.AcademicBackgrounds.FindAsync(id);

        if (academic == null)
            throw new KeyNotFoundException($"Formação acadêmica com ID {id} não encontrada.");

        // Atualizar propriedades
        if (!string.IsNullOrEmpty(dto.Institution))
            academic.Institution = dto.Institution;

        if (!string.IsNullOrEmpty(dto.CourseName))
            academic.CourseName = dto.CourseName;

        if (!string.IsNullOrEmpty(dto.Level))
        {
            var level = Domain.Enums.EducationLevel.TryFromName(dto.Level, out var educationLevel)
                ? educationLevel
                : academic.Level;
            academic.Level = level;
        }

        if (dto.StartDate.HasValue)
            academic.StartDate = dto.StartDate.Value;

        if (dto.EndDate.HasValue)
            academic.EndDate = dto.EndDate.Value;

        // TODO: Lidar com upload de diploma
        // if (dto.Diploma != null)
        // {
        //     academic.DiplomaPath = await SaveFileAsync(dto.Diploma);
        // }

        _context.AcademicBackgrounds.Update(academic);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Formação acadêmica atualizada: {AcademicId}", id);
        return _mapper.Map<AcademicBackgroundDto>(academic);
    }

    public async Task<bool> DeleteAcademicAsync(Guid id)
    {
        var academic = await _context.AcademicBackgrounds.FindAsync(id);

        if (academic == null)
            return false;

        _context.AcademicBackgrounds.Remove(academic);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Formação acadêmica deletada: {AcademicId}", id);
        return true;
    }
}