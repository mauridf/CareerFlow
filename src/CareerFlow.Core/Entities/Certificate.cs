namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa certificados e certificações.
/// </summary>
public class Certificate : Entity<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Issuer { get; private set; } = string.Empty;
    public DateTime IssueDate { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public string? CertificateId { get; private set; }
    public string? CredentialId { get; private set; }
    public string? CredentialUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsVerified { get; private set; }
    public int DisplayOrder { get; private set; }

    private Certificate() { }

    public static Certificate Create(
        Guid personId,
        string title,
        string issuer,
        DateTime issueDate,
        DateTime? expirationDate = null,
        string? certificateId = null,
        string? credentialId = null,
        string? credentialUrl = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título é obrigatório", nameof(title));

        if (string.IsNullOrWhiteSpace(issuer))
            throw new ArgumentException("Emissor é obrigatório", nameof(issuer));

        if (expirationDate.HasValue && issueDate > expirationDate.Value)
            throw new ArgumentException("Data de emissão deve ser anterior à data de expiração");

        return new Certificate
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Title = title.Trim(),
            Issuer = issuer.Trim(),
            IssueDate = issueDate,
            ExpirationDate = expirationDate,
            CertificateId = certificateId,
            CredentialId = credentialId,
            CredentialUrl = credentialUrl,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string title,
        string issuer,
        DateTime issueDate,
        DateTime? expirationDate,
        string? certificateId = null,
        string? credentialUrl = null)
    {
        Title = title.Trim();
        Issuer = issuer.Trim();
        IssueDate = issueDate;
        ExpirationDate = expirationDate;
        CertificateId = certificateId;
        CredentialUrl = credentialUrl;
        MarkAsUpdated();
    }
}
