namespace CareerFlow.Core.Events;

public sealed class CertificateCreatedEvent : DomainEvent
{
    public Guid CertificateId { get; }
    public Guid PersonId { get; }
    public string Title { get; }

    public CertificateCreatedEvent(Guid certificateId, Guid personId, string title)
    {
        CertificateId = certificateId;
        PersonId = personId;
        Title = title;
    }
}
