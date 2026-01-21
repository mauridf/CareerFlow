using CareerFlow.Application.DTOs;

namespace CareerFlow.Application.Interfaces;

public interface ICertificateService
{
    Task<IEnumerable<CertificateDto>> GetUserCertificatesAsync(Guid userId);
    Task<CertificateDto> GetCertificateByIdAsync(Guid id);
    Task<CertificateDto> CreateCertificateAsync(Guid userId, CreateCertificateDto dto);
    Task<CertificateDto> UpdateCertificateAsync(Guid id, UpdateCertificateDto dto);
    Task<bool> DeleteCertificateAsync(Guid id);
    Task<IEnumerable<CertificateDto>> GetExpiringCertificatesAsync(Guid userId, int daysThreshold = 30);
}