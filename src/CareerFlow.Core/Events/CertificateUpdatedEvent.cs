namespace CareerFlow.Core.Events;

public sealed class CertificateUpdatedEvent : DomainEvent
{
    public Guid CertificateId { get; }
    public Guid PersonId { get; }

    public CertificateUpdatedEvent(Guid certificateId, Guid personId)
    {
        CertificateId = certificateId;
        PersonId = personId;
    }
}
