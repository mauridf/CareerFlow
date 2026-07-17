namespace CareerFlow.Core.Events;

public sealed class SocialNetworkUpdatedEvent : DomainEvent
{
    public Guid SocialNetworkId { get; }
    public Guid PersonId { get; }

    public SocialNetworkUpdatedEvent(Guid socialNetworkId, Guid personId)
    {
        SocialNetworkId = socialNetworkId;
        PersonId = personId;
    }
}
