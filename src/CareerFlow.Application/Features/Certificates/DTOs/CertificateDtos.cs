namespace CareerFlow.Application.Features.Certificates.DTOs;

public record CreateCertificateRequest(
    string Title,
    string Issuer,
    DateTime IssueDate,
    DateTime? ExpirationDate = null,
    string? CertificateId = null,
    string? CredentialId = null,
    string? CredentialUrl = null);

public record UpdateCertificateRequest(
    string Title,
    string Issuer,
    DateTime IssueDate,
    DateTime? ExpirationDate = null,
    string? CertificateId = null,
    string? CredentialUrl = null);

public class CertificateResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? CertificateId { get; set; }
    public string? CredentialId { get; set; }
    public string? CredentialUrl { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
