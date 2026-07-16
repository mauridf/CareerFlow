namespace CareerFlow.Core.Events;

/// <summary>
/// Evento disparado quando um currículo público é visualizado.
/// </summary>
public sealed class ResumeViewedEvent : DomainEvent
{
    public Guid PersonId { get; }
    public string? IpAddress { get; }
    public string? UserAgent { get; }
    public string? Source { get; }

    public ResumeViewedEvent(Guid personId, string? ipAddress, string? userAgent, string? source = "direct")
    {
        PersonId = personId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Source = source;
    }
}
