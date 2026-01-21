using AutoMapper;
using CareerFlow.Application.Common;
using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using CareerFlow.Domain.Common;
using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Services;

public class CertificateService : ServiceBase, ICertificateService
{
    public CertificateService(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<CertificateService> logger)
        : base(context, mapper, logger)
    {
    }

    public async Task<IEnumerable<CertificateDto>> GetUserCertificatesAsync(Guid userId)
    {
        var certificates = await _context.Certificates
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CertificateDto>>(certificates);
    }

    public async Task<CertificateDto> GetCertificateByIdAsync(Guid id)
    {
        var certificate = await _context.Certificates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (certificate == null)
            throw new KeyNotFoundException($"Certificado com ID {id} não encontrado.");

        return _mapper.Map<CertificateDto>(certificate);
    }

    public async Task<CertificateDto> CreateCertificateAsync(Guid userId, CreateCertificateDto dto)
    {
        var certificate = _mapper.Map<Certificate>(dto);
        certificate.UserId = userId;

        // TODO: Lidar com upload de certificado
        // if (dto.CertificateFile != null)
        // {
        //     certificate.CertificatePath = await SaveFileAsync(dto.CertificateFile);
        // }

        _context.Certificates.Add(certificate);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Certificado criado: {Name} para usuário {UserId}",
            certificate.Name, userId);

        return _mapper.Map<CertificateDto>(certificate);
    }

    public async Task<CertificateDto> UpdateCertificateAsync(Guid id, UpdateCertificateDto dto)
    {
        var certificate = await _context.Certificates.FindAsync(id);

        if (certificate == null)
            throw new KeyNotFoundException($"Certificado com ID {id} não encontrado.");

        // Atualizar propriedades
        if (!string.IsNullOrEmpty(dto.Name))
            certificate.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.Description))
            certificate.Description = dto.Description;

        if (dto.StartDate.HasValue)
            certificate.StartDate = dto.StartDate.Value;

        if (dto.EndDate.HasValue)
            certificate.EndDate = dto.EndDate.Value;

        // TODO: Lidar com upload de certificado
        // if (dto.CertificateFile != null)
        // {
        //     certificate.CertificatePath = await SaveFileAsync(dto.CertificateFile);
        // }

        _context.Certificates.Update(certificate);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Certificado atualizado: {CertificateId}", id);
        return _mapper.Map<CertificateDto>(certificate);
    }

    public async Task<bool> DeleteCertificateAsync(Guid id)
    {
        var certificate = await _context.Certificates.FindAsync(id);

        if (certificate == null)
            return false;

        _context.Certificates.Remove(certificate);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Certificado deletado: {CertificateId}", id);
        return true;
    }

    public async Task<IEnumerable<CertificateDto>> GetExpiringCertificatesAsync(Guid userId, int daysThreshold = 30)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

        var certificates = await _context.Certificates
            .AsNoTracking()
            .Where(c => c.UserId == userId &&
                       c.EndDate.HasValue &&
                       c.EndDate.Value <= thresholdDate &&
                       c.EndDate.Value >= DateTime.UtcNow)
            .OrderBy(c => c.EndDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CertificateDto>>(certificates);
    }
}