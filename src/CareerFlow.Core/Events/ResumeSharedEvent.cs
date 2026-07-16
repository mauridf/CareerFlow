namespace CareerFlow.Core.Events;

/// <summary>
/// Evento disparado quando um currículo é compartilhado.
/// </summary>
public sealed class ResumeSharedEvent : DomainEvent
{
    public Guid PersonId { get; }
    public Guid UserId { get; }
    public string ShareMethod { get; }

    public ResumeSharedEvent(Guid personId, Guid userId, string shareMethod = "link")
    {
        PersonId = personId;
        UserId = userId;
        ShareMethod = shareMethod;
    }
}
