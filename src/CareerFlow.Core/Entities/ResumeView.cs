using System.Net;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Registro de visualização do currículo público.
/// </summary>
public class ResumeView : Entity<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    public IPAddress? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Referrer { get; private set; }
    public string? ViewerCountry { get; private set; }
    public string? ViewerCity { get; private set; }
    public string? Source { get; private set; }
    public int? ViewDurationSeconds { get; private set; }
    public bool PdfDownloaded { get; private set; }

    private ResumeView() { }

    public static ResumeView Record(
        Guid personId,
        IPAddress? ipAddress,
        string? userAgent,
        string? referrer = null,
        string? source = "direct")
    {
        return new ResumeView
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Referrer = referrer,
            Source = source,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkPdfDownloaded()
    {
        PdfDownloaded = true;
    }
}
